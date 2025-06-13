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
    public partial class DisplayCasesP : Form
    {
        private string crime;
        private string city;
        private DateTime? weekStart;
        private string location;
        private int? victimCount;
        private string status;
        private string caseTitle;
        public DisplayCasesP(string crime, string city, DateTime? weekStart, string location, int? victimCount, string status, string caseTitle)
        {
            InitializeComponent();
            this.Load += new EventHandler(DisplayCasesP_Load); // Ensure Load event is attached

            // Save values for use
            this.crime = crime;
            this.city = city;
            this.weekStart = weekStart;
            this.location = location;
            this.victimCount = victimCount;
            this.status = status;
            this.caseTitle = caseTitle;

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

        //Load Event: Display the Logged-in User's Name
        private void DisplayCasesP_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            LoadFilteredCases();
        }
        private void LoadFilteredCases()
        {
            flp_cases.Controls.Clear();

            using (SqlConnection con = new SqlConnection("Data Source=PC-MAHNUR\\SQLEXPRESS;Initial Catalog=CI;Integrated Security=True;Encrypt=False"))
            {
                con.Open();

                List<string> scoreParts = new List<string>();
                List<SqlParameter> parameters = new List<SqlParameter>();

                string baseQuery = "SELECT *, (";

                // === STEP 1: Mandatory Filter by crime_type ===
                string whereClause = "";
                if (!string.IsNullOrEmpty(crime))
                {
                    whereClause = " WHERE crime_category_id IN (SELECT crime_catg_id FROM Crime_Categories WHERE crime_catg = @crime)";
                    parameters.Add(new SqlParameter("@crime", crime));

                }
                else
                {
                    // if crime_type is null, return nothing (or skip filtering, depending on your logic)
                    return;
                }

                // === STEP 2: Build scoring logic for other optional fields ===
                if (!string.IsNullOrEmpty(city))
                {
                    scoreParts.Add("CASE WHEN crime_location LIKE @city THEN 1 ELSE 0 END");
                    parameters.Add(new SqlParameter("@city", "%" + city + "%"));
                }

                if (!string.IsNullOrEmpty(location))
                {
                    foreach (var word in location.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string paramName = "@loc_" + word;
                        scoreParts.Add($"CASE WHEN crime_location LIKE {paramName} THEN 1 ELSE 0 END");
                        parameters.Add(new SqlParameter(paramName, "%" + word + "%"));
                    }
                }

                if (weekStart != null)
                {
                    scoreParts.Add("CASE WHEN crime_time BETWEEN @start AND @end THEN 1 ELSE 0 END");
                    parameters.Add(new SqlParameter("@start", weekStart.Value));
                    parameters.Add(new SqlParameter("@end", weekStart.Value.AddDays(7)));
                }

                if (!string.IsNullOrEmpty(caseTitle))
                {
                    foreach (var word in caseTitle.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string paramName = "@title_" + word;
                        scoreParts.Add($"CASE WHEN case_title LIKE {paramName} THEN 1 ELSE 0 END");
                        parameters.Add(new SqlParameter(paramName, "%" + word + "%"));
                    }
                }

                if (victimCount != null)
                {
                    scoreParts.Add("CASE WHEN victim_count = @victim THEN 1 ELSE 0 END");
                    parameters.Add(new SqlParameter("@victim", victimCount));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    scoreParts.Add("CASE WHEN status = @status THEN 1 ELSE 0 END");
                    parameters.Add(new SqlParameter("@status", status));
                }

                // === STEP 3: Build final query ===
                string scoreClause = scoreParts.Count > 0 ? string.Join(" + ", scoreParts) : "0";
                baseQuery += scoreClause + ") AS Score FROM Cases" + whereClause;
                string finalQuery = baseQuery + " ORDER BY Score DESC, crime_time DESC";

                // === STEP 4: Execute ===
                SqlCommand cmd = new SqlCommand(finalQuery, con);
                cmd.Parameters.AddRange(parameters.ToArray());

                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    Label noResultsLabel = new Label
                    {
                        Text = "No Cases Found.",
                        Font = new Font("Arial", 12, FontStyle.Bold),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    // Wrap in a Panel to center it in the FlowLayoutPanel
                    Panel wrapperPanel = new Panel
                    {
                        Width = flp_cases.Width - 50,
                        Height = flp_cases.Height - 50,
                        BackColor = flp_cases.BackColor
                    };

                    noResultsLabel.Location = new Point(
                        (wrapperPanel.Width - noResultsLabel.Width) / 2,
                        (wrapperPanel.Height - noResultsLabel.Height) / 2
                    );

                    wrapperPanel.Controls.Add(noResultsLabel);
                    flp_cases.Controls.Add(wrapperPanel);

                    return;
                }


                while (reader.Read())
                {
                    Panel caseCard = new Panel
                    {
                        Width = flp_cases.Width - 35,
                        Height = 220,
                        BackColor = Color.White,
                        Padding = new Padding(10),
                        Margin = new Padding(10),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    Label lbl_title = new Label { Text = reader["case_title"].ToString(), AutoSize = true };
                    Label lbl_type = new Label { Text = reader["crime_type"].ToString(), AutoSize = true };
                    Label lbl_time = new Label
                    {
                        Text = reader["crime_time"] != DBNull.Value
                            ? Convert.ToDateTime(reader["crime_time"]).ToString("dd MMM yyyy")
                            : "N/A",
                        AutoSize = true
                    };
                    Label lbl_status = new Label { Text = reader["status"].ToString(), AutoSize = true };
                    Label lbl_location = new Label { Text = reader["crime_location"].ToString(), AutoSize = true };
                    Label lbl_victim = new Label { Text = reader["victim_count"].ToString(), AutoSize = true };
                    Label lbl_details = new Label
                    {
                        Text = reader["public_description"].ToString(),
                        MaximumSize = new Size(caseCard.Width - 150, 0),
                        AutoSize = true
                    };

                    Label lbl_titleHead = new Label { Text = "Case Title: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_typeHead = new Label { Text = "Crime Type: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_timeHead = new Label { Text = "Crime Time: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_statusHead = new Label { Text = "Case Status: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_locationHead = new Label { Text = "Crime Location: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_victimHead = new Label { Text = "Victim Count: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_detailsHead = new Label { Text = "Case Details: ", Font = new Font("Arial", 9, FontStyle.Bold) };

                    lbl_titleHead.Location = new Point(10, 10);
                    lbl_title.Location = new Point(lbl_titleHead.Right + 3, 10);

                    lbl_victimHead.Location = new Point(10, 40);
                    lbl_victim.Location = new Point(lbl_victimHead.Right + 3, 40);

                    lbl_timeHead.Location = new Point(lbl_victim.Right + 50, 40);
                    lbl_time.Location = new Point(lbl_timeHead.Right + 3, 40);

                    lbl_typeHead.Location = new Point(lbl_time.Right + 30, 40);
                    lbl_type.Location = new Point(lbl_typeHead.Right + 3, 40);

                    lbl_statusHead.Location = new Point(10, 70);
                    lbl_status.Location = new Point(lbl_statusHead.Right + 3, 70);

                    lbl_locationHead.Location = new Point(lbl_status.Right + 50, 70);
                    lbl_location.Location = new Point(lbl_locationHead.Right + 3, 70);

                    lbl_detailsHead.Location = new Point(10, 100);
                    lbl_details.Location = new Point(lbl_detailsHead.Right + 10, 100);

                    caseCard.Controls.Add(lbl_titleHead); caseCard.Controls.Add(lbl_title);
                    caseCard.Controls.Add(lbl_typeHead); caseCard.Controls.Add(lbl_type);
                    caseCard.Controls.Add(lbl_timeHead); caseCard.Controls.Add(lbl_time);
                    caseCard.Controls.Add(lbl_statusHead); caseCard.Controls.Add(lbl_status);
                    caseCard.Controls.Add(lbl_locationHead); caseCard.Controls.Add(lbl_location);
                    caseCard.Controls.Add(lbl_victimHead); caseCard.Controls.Add(lbl_victim);
                    caseCard.Controls.Add(lbl_detailsHead); caseCard.Controls.Add(lbl_details);

                    // Parse values for form passing
                    int caseId = reader["case_id"] != DBNull.Value ? Convert.ToInt32(reader["case_id"]) : 0;
                    string caseTitle = reader["case_title"] != DBNull.Value ? reader["case_title"].ToString() : "N/A";
                    string location = reader["crime_location"] != DBNull.Value ? reader["crime_location"].ToString() : "N/A";
                    DateTime? crimeTime = reader["crime_time"] != DBNull.Value ? Convert.ToDateTime(reader["crime_time"]) : (DateTime?)null;
                    string type = reader["crime_type"] != DBNull.Value ? reader["crime_type"].ToString() : "N/A";

                    // Create button for Submit Evidence
                    Button btnEvidence = new Button
                    {
                        Text = "Submit Evidence",
                        BackColor = Color.LightGreen,
                        Width = 120,
                        Height = 25,
                        Font = new Font("Arial", 9F, FontStyle.Regular),
                        ForeColor = Color.Black,
                        FlatStyle = FlatStyle.Flat,
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                        Location = new Point(caseCard.Width - 400, caseCard.Height - 40)
                    };

                    btnEvidence.FlatAppearance.BorderSize = 0;
                    btnEvidence.Click += (s, e) =>
                    {
                        SubmitEvidence form = new SubmitEvidence(caseId, caseTitle, location, crimeTime, type);
                        form.ShowDialog();
                    };

                    // Create button for Submit Lead
                    Button btnLead = new Button
                    {
                        Text = "Submit Lead",
                        BackColor = Color.SkyBlue,
                        Width = 100,
                        Height = 25,
                        Font = new Font("Arial", 9F, FontStyle.Regular),
                        ForeColor = Color.Black,
                        FlatStyle = FlatStyle.Flat,
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                        Location = new Point(caseCard.Width - 400 + 130, caseCard.Height - 40)
                    };

                    btnLead.FlatAppearance.BorderSize = 0;
                    btnLead.Click += (s, e) =>
                    {
                        SubmitLead form = new SubmitLead(caseId, caseTitle, location, crimeTime, type);
                        form.ShowDialog();
                    };

                    // Create button for Submit Testimony
                    Button btnTestimony = new Button
                    {
                        Text = "Submit Testimony",
                        BackColor = Color.Orange,
                        Width = 130,
                        Height = 25,
                        Font = new Font("Arial", 9F, FontStyle.Regular),
                        ForeColor = Color.Black,
                        FlatStyle = FlatStyle.Flat,
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                        Location = new Point(caseCard.Width - 400 + 130 +110, caseCard.Height - 40)
                    };
                    btnTestimony.FlatAppearance.BorderSize = 0;
                    btnTestimony.Click += (s, e) =>
                    {
                        SubmitTestimony form = new SubmitTestimony(caseId, caseTitle, location, crimeTime, type); // Assuming no args required
                        form.ShowDialog();
                    };

                    // Add buttons to panel
                    caseCard.Controls.Add(btnEvidence);
                    caseCard.Controls.Add(btnLead);
                    caseCard.Controls.Add(btnTestimony);


                    flp_cases.Controls.Add(caseCard);
                }

                reader.Close();
            }
        }

        private void flp_cases_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Hide();

            PublicDash dashboard = new PublicDash();

            dashboard.ShowDialog();

            this.Close();
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            this.Hide();

            UpdProfileP updateProfileForm = new UpdProfileP();

            updateProfileForm.ShowDialog();

            this.Close();
        }

        private void btn_report_Click(object sender, EventArgs e)
        {
            this.Hide();
            SubmitReport reportform = new SubmitReport();
            reportform.ShowDialog();
            this.Close();
        }

        private void btn_seach_Click(object sender, EventArgs e)
        {
            this.Hide();

            SearchCaseP CaseSearch = new SearchCaseP();
            CaseSearch.ShowDialog();

            this.Close();
        }

        private void btn_witness_Click(object sender, EventArgs e)
        {
            this.Hide();

            SearchCaseP CaseSearch = new SearchCaseP();
            CaseSearch.ShowDialog();

            this.Close();
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Hide();

            SearchCaseP CaseSearch = new SearchCaseP();
            CaseSearch.ShowDialog();

            this.Close();
        }
    }
}
