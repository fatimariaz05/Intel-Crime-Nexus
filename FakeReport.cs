using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CISystem
{
    public partial class FakeReport : Form
    {
        public FakeReport()
        {
            InitializeComponent();
            this.Load += new EventHandler(FakeReport_Load);

            // Set title bar color to black
            SetTitleBarColor(Color.Black);

            // Remove the top-left icon but keep minimize, maximize, and close buttons
            this.Text = "Intel Crime Nexus";  // Change this to your app name
            this.ShowIcon = false;      // Hide the icon
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private void SetTitleBarColor(Color color)
        {
            int colorValue = color.R | (color.G << 8) | (color.B << 16);
            DwmSetWindowAttribute(this.Handle, 35, ref colorValue, 4); // Works on Windows 10/11
        }
        private void FakeReport_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            LoadFlaggedReports();
        }

        private void flp_Reports_Paint(object sender, PaintEventArgs e)
        {

        }
        private bool CheckDuplicateReports(int reportId, string nature, DateTime date, string location, int victims, out List<int> matchingIds)
        {
            matchingIds = new List<int>();

            if (string.IsNullOrWhiteSpace(nature) || string.IsNullOrWhiteSpace(location) || victims < 0)
                return false;

            string q = @"SELECT report_id FROM Reports 
                 WHERE report_id != @id AND status = 'Pending' AND 
                       report_nature = @nature AND incident_date = @date AND 
                       incident_location = @location AND victim_count = @victims";

            using (SqlConnection con = new SqlConnection(DB.connectionString))
            using (SqlCommand cmd = new SqlCommand(q, con))
            {
                cmd.Parameters.AddWithValue("@id", reportId);
                cmd.Parameters.AddWithValue("@nature", nature);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@location", location);
                cmd.Parameters.AddWithValue("@victims", victims);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    matchingIds.Add(reader.GetInt32(0));
            }

            return matchingIds.Count > 0;
        }


        private string CheckReportsInNext48Hours(string userId, DateTime submittedAt)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            string query = @"
        SELECT COUNT(*) 
        FROM Reports 
        WHERE reported_by = @uid 
        AND status = 'Pending'
        AND submitted_at >= @start 
        AND submitted_at <= DATEADD(HOUR, 48, @start)";

            using (SqlConnection con = new SqlConnection(DB.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@start", submittedAt);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count >= 3 ? $"4. {count} reports by same user in 48 hours" : null;
            }
        }


        private bool IsMismatchCrime(string crime)
        {
            string[] criticalCrimes = new[]
            {
        "Homicide", "Hit & Run", "Sexual Offense", "Cyber Harassment", "Violence", "Missing Person"
    };

            return criticalCrimes.Any(c => crime.ToLower().Contains(c));
        }

        private bool IsUserCityMismatchFromDB(string userAddress, string incidentLocation)
        {
            if (string.IsNullOrWhiteSpace(userAddress) || string.IsNullOrWhiteSpace(incidentLocation))
                return false;

            string cityQuery = "SELECT City FROM Scope_Cities";
            List<string> cities = new List<string>();
            string matchedCity = null;

            using (SqlConnection con = new SqlConnection(DB.connectionString))
            using (SqlCommand cmd = new SqlCommand(cityQuery, con))
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        cities.Add(reader["City"].ToString());
                }
            }

            foreach (string city in cities)
            {
                if (userAddress.ToLower().Contains(city.ToLower()))
                {
                    matchedCity = city;
                    break;
                }
            }

            if (matchedCity == null) return false;

            return !incidentLocation.ToLower().Contains(matchedCity.ToLower());
        }

        private void ClearFlags(int reportId)
        {
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                con.Open();
                string query = "UPDATE Reports  SET flag_check = 1 WHERE report_id = @reportId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@reportId", reportId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Flags clearted for the Report ID " + reportId, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int GetOfficerIdByUsername(string username)
        {
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                con.Open();
                string query = "SELECT officer_id FROM officer WHERE username = @username";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", username);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }

        private void RemoveReport(int reportId, string flagReasons)
        {
            int officerId = GetOfficerIdByUsername(UserSession.UserName);
            if (officerId == -1)
            {
                MessageBox.Show("Officer not found.");
                return;
            }

            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                con.Open();
                string query = @"UPDATE Reports 
                         SET flag_check = 2, 
                             flag_reason = @flagReasons,
                             status = 'Removed',
                             submitted_by = @officerId 
                         WHERE report_id = @reportId";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@flagReasons", flagReasons);
                cmd.Parameters.AddWithValue("@officerId", officerId);
                cmd.Parameters.AddWithValue("@reportId", reportId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Report ID " + reportId + " has been removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void LoadFlaggedReports()
        {
            flp_Reports.Controls.Clear();
            string query = @"
    SELECT R.*, U.address 
    FROM Reports R 
    LEFT JOIN [User] U ON R.reported_by = U.user_id
    WHERE R.status = 'Pending' and flag_check <> 1";

            using (SqlConnection con = new SqlConnection(DB.connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.IsDBNull(reader.GetOrdinal("report_id")) ||
                        reader.IsDBNull(reader.GetOrdinal("report_nature")) ||
                        reader.IsDBNull(reader.GetOrdinal("incident_date")) ||
                        reader.IsDBNull(reader.GetOrdinal("incident_location")) ||
                        reader.IsDBNull(reader.GetOrdinal("victim_count")) ||
                        reader.IsDBNull(reader.GetOrdinal("submitted_at")) ||
                        reader.IsDBNull(reader.GetOrdinal("reported_by")))
                        continue;

                    List<string> flags = new List<string>();

                    int reportId = reader.GetInt32(reader.GetOrdinal("report_id"));
                    string reportNature = reader["report_nature"].ToString();
                    reportNature = reportNature.Replace("&", "&&"); // escape ampersand

                    DateTime incidentTime = Convert.ToDateTime(reader["incident_date"]);
                    DateTime submittedAt = Convert.ToDateTime(reader["submitted_at"]);
                    string incidentLocation = reader["incident_location"].ToString();
                    int victimCount = Convert.ToInt32(reader["victim_count"]);
                    string reportedBy = reader["reported_by"].ToString();
                    string userAddress = reader["address"]?.ToString() ?? "";

                    if (submittedAt < incidentTime)
                        flags.Add("1. Crime Reported Before the Crime Time");

                    if (CheckDuplicateReports(reportId, reportNature, incidentTime, incidentLocation, victimCount, out List<int> matchIds))
                        flags.Add($"2. Duplicate reports – matched with report ID {string.Join(", ", matchIds)}");

                    if (!string.IsNullOrWhiteSpace(userAddress) && IsUserCityMismatchFromDB(userAddress, incidentLocation))
                        flags.Add("3. User City is different from crime Location City");

                    string flag = CheckReportsInNext48Hours(reportedBy, submittedAt);
                    if (flag != null)
                        flags.Add(flag);

                    if (IsMismatchCrime(reportNature) && victimCount == 0)
                        flags.Add("5. Crime type & Victim Mismatch");

                    if (flags.Count == 0)
                        continue;

                    if (flags.Count > 0)
                    {
                        Panel reportCard = new Panel
                        {
                            Width = flp_Reports.Width - 35,
                            Height = 300,
                            BackColor = Color.White,
                            Padding = new Padding(10),
                            Margin = new Padding(10),
                            BorderStyle = BorderStyle.FixedSingle
                        };

                        // Create labels
                        Label lbl_id = new Label { Text = reader["report_id"].ToString(), AutoSize = true };
                        Label lbl_status = new Label { Text = reader["Status"].ToString(), AutoSize = true };
                        Label lbl_reporter = new Label { Text = reader["reporter"].ToString(), AutoSize = true };
                        Label lbl_reportnature = new Label { UseMnemonic = false,  Text = reader["Report_Nature"].ToString(), AutoSize = true };
                        Label lbl_inclocation = new Label { Text = reader["Incident_Location"].ToString(), AutoSize = true };
                        Label lbl_incdate = new Label { Text = reader["Incident_Date"].ToString(), AutoSize = true };
                        Label lbl_viccount = new Label { Text = reader["Victim_Count"].ToString(), AutoSize = true };
                        Label lbl_subdate = new Label { Text = reader["submitted_at"].ToString(), AutoSize = true };
                        Label lbl_details = new Label
                        {
                            Text = reader["Incident_Details"].ToString(),
                            MaximumSize = new Size(reportCard.Width - 150, 0),
                            AutoSize = true
                        };

                        // Create headings
                        Label lbl_idHeading = new Label { Text = "Report ID: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                        Label lbl_statusHeading = new Label { Text = "Status: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                        Label lbl_reporterHeading = new Label { Text = "Reporter: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                        Label lbl_reportnatureHeading = new Label { Text = "Report Nature: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                        Label lbl_inclocationHeading = new Label { Text = "Incident Location: ", Font = new Font("Arial", 9, FontStyle.Bold), AutoSize = true };
                        Label lbl_incdateHeading = new Label { Text = "Incident Date: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                        Label lbl_viccountHeading = new Label { Text = "Victim Count: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                        Label lbl_detailsHeading = new Label { Text = "Incident Details: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                        Label lbl_subdateHeading = new Label { Text = "Submitted Date: ", Font = new Font("Arial", 9, FontStyle.Bold) };

                        Label lbl_detectedHeading = new Label
                        {
                            Text = "Detected: ",
                            Font = new Font("Arial", 9, FontStyle.Bold),
                            ForeColor = Color.Red,
                            AutoSize = true
                        };

                        Label lbl_detectedText = new Label
                        {
                            Text = string.Join(" | ", flags.Select(f => f.Replace("1. ", "").Replace("2. ", "").Replace("3. ", "").Replace("4. ", "").Replace("5. ", ""))),
                            Font = new Font("Arial", 9, FontStyle.Regular),
                            ForeColor = Color.Red,
                            MaximumSize = new Size(reportCard.Width - 150, 0),
                            AutoSize = true
                        };

                        // Positioning labels
                        lbl_idHeading.Location = new Point(10, 10);
                        lbl_id.Location = new Point(lbl_idHeading.Right + 1, 10);

                        lbl_reporterHeading.Location = new Point(lbl_id.Right + 2, 10);
                        lbl_reporter.Location = new Point(lbl_reporterHeading.Right + 1, 10);

                        lbl_incdateHeading.Location = new Point(lbl_reporter.Right + 30, 10);
                        lbl_incdate.Location = new Point(lbl_incdateHeading.Right + 1, 10);

                        lbl_statusHeading.Location = new Point(10, 40);
                        lbl_status.Location = new Point(lbl_statusHeading.Right + 1, 40);

                        lbl_reportnatureHeading.Location = new Point(lbl_status.Right + 2, 40);
                        lbl_reportnature.Location = new Point(lbl_reportnatureHeading.Right + 1, 40);

                        lbl_viccountHeading.Location = new Point(lbl_reportnature.Right + 30, 40);
                        lbl_viccount.Location = new Point(lbl_viccountHeading.Right + 7, 40);

                        lbl_inclocationHeading.Location = new Point(10, 70);
                        lbl_inclocation.Location = new Point(lbl_inclocationHeading.Right + 10, 70);

                        lbl_subdateHeading.Location = new Point(lbl_inclocation.Right + 30, 70);
                        lbl_subdate.Location = new Point(lbl_subdateHeading.Right + 10, 70);

                        lbl_detailsHeading.Location = new Point(10, 100);
                        lbl_details.Location = new Point(lbl_detailsHeading.Right + 10, 100);

                        // Calculate the real size of the lbl_details text
                        Size textSize = TextRenderer.MeasureText(
                            lbl_details.Text,
                            lbl_details.Font,
                            new Size(lbl_details.Width, int.MaxValue), // use label width to allow wrapping
                            TextFormatFlags.WordBreak
                        );

                        // Set new height based on measured text
                        lbl_details.Height = textSize.Height;

                        // Position the detected heading and text below details
                        int belowDetailsY = lbl_details.Top + lbl_details.Height + 20;
                        lbl_detectedHeading.Location = new Point(10, belowDetailsY);
                        lbl_detectedText.Location = new Point(lbl_detectedHeading.Right + 10, belowDetailsY);


                        int buttonWidth = 120;
                        int buttonHeight = 30;
                        int buttonSpacing = 10;

                        int xRemove = reportCard.Width - buttonWidth - 10;
                        int xClear = xRemove - buttonWidth - buttonSpacing;
                        int yButtons = reportCard.Height - buttonHeight - 10;

                        Button btnClear = new Button
                        {
                            Text = "Clear Report",
                            Location = new Point(xClear, yButtons),
                            Size = new Size(buttonWidth, buttonHeight),
                            Tag = reportId,
                            BackColor = Color.LightGreen,
                            FlatStyle = FlatStyle.Flat
                        };
                        btnClear.FlatAppearance.BorderSize = 0;

                        btnClear.Click += (s, e) => ClearFlags(reportId);

                        Button btnRemove = new Button
                        {
                            Text = "Remove Report",
                            Location = new Point(xRemove, yButtons),
                            Size = new Size(buttonWidth, buttonHeight),
                            Tag = reportId,
                            BackColor = Color.IndianRed,
                            FlatStyle = FlatStyle.Flat
                        };
                        btnRemove.FlatAppearance.BorderSize = 0;
                        btnRemove.Click += (s, e) => RemoveReport(reportId, string.Join(" | ", flags));

                        // Add labels and buttons to the panel
                        reportCard.Controls.Add(lbl_idHeading);
                        reportCard.Controls.Add(lbl_id);
                        reportCard.Controls.Add(lbl_reporterHeading);
                        reportCard.Controls.Add(lbl_reporter);
                        reportCard.Controls.Add(lbl_inclocationHeading);
                        reportCard.Controls.Add(lbl_inclocation);
                        reportCard.Controls.Add(lbl_statusHeading);
                        reportCard.Controls.Add(lbl_status);
                        reportCard.Controls.Add(lbl_reportnatureHeading);
                        reportCard.Controls.Add(lbl_reportnature);
                        reportCard.Controls.Add(lbl_incdateHeading);
                        reportCard.Controls.Add(lbl_incdate);
                        reportCard.Controls.Add(lbl_viccountHeading);
                        reportCard.Controls.Add(lbl_viccount);
                        reportCard.Controls.Add(lbl_detailsHeading);
                        reportCard.Controls.Add(lbl_details);
                        reportCard.Controls.Add(lbl_detectedHeading);
                        reportCard.Controls.Add(lbl_detectedText);
                        reportCard.Controls.Add(btnClear);
                        reportCard.Controls.Add(btnRemove);


                        flp_Reports.Controls.Add(reportCard);
                    }
                    
                }
            }
        }

        private void btnLoadReports_Click(object sender, EventArgs e)
        {
            LoadFlaggedReports();
        }

        private void siticoneButton3_Click(object sender, EventArgs e)
        {
            string rank = "";

            // Fetch rank from Officer table using the current username
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = "SELECT Rank FROM Officer WHERE Username = @username";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", UserSession.UserName);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                        rank = result.ToString();
                }
            }

            this.Hide();

            // Check rank and open appropriate form
            if (rank == "Higher")
            {
                SpecialOp specialOperations = new SpecialOp();
                specialOperations.ShowDialog();
            }
            else
            {
                SpecialOpLow specialOperationsLow = new SpecialOpLow();
                specialOperationsLow.ShowDialog();
            }

            this.Close();
        }

        private void btn_fir_Click(object sender, EventArgs e)
        {
            this.Hide();
            selectRep ShowReports = new selectRep();
            ShowReports.ShowDialog();
            this.Close();
        }

        private void btn_newreports_Click(object sender, EventArgs e)
        {
            this.Hide();
            NewReportsO newreports = new NewReportsO();
            newreports.ShowDialog();
            this.Close();
        }

        private void btn_searchcase_Click(object sender, EventArgs e)
        {
            this.Hide();

            SearchCaseO CaseSearchForm = new SearchCaseO();

            CaseSearchForm.ShowDialog();

            this.Close();
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerCases AssignedCases = new OfficerCases();
            AssignedCases.ShowDialog();
            this.Close();
        }

        private void btn_updateProfile_Click(object sender, EventArgs e)
        {
            this.Hide();

            UpdProfileO updateProfileForm = new UpdProfileO();

            updateProfileForm.ShowDialog();

            this.Close();
        }

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerDash dashboard = new OfficerDash();
            dashboard.ShowDialog();
            this.Close();
        }

        private void panelMenu_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
