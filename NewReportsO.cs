using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CISystem
{
    public partial class NewReportsO : Form
    {
        public NewReportsO()
        {
            InitializeComponent();

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

        private void NewReportsO_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            LoadPendingReports();
        }

        private void LoadPendingReports()
        {
            try
            {
                flp_Reports.Controls.Clear(); // Clear previous reports
                bool hasReports = false; // Flag to check if there are pending reports

                using (SqlConnection conn = new SqlConnection(DB.connectionString))
                {
                    conn.Open();

                    string query = @"
                    SELECT r.* 
                    FROM Reports r
                    WHERE r.status = 'Pending'
                      AND EXISTS (
                          SELECT 1 
                          FROM Officer o
                          JOIN Scope_Cities sc ON o.police_station LIKE '%' + sc.City + '%'
                          WHERE o.username = @username and r.flag_check <> 2
                            AND r.Incident_Location LIKE '%' + sc.City + '%'
                      )";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", UserSession.UserName);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read()) // Loop through all reports
                    {
                        hasReports = true; // Set flag to true if reports are found

                        // Step 1: Create a Panel (Report Card)
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
                        Label lbl_id = new Label { Text = reader["report_id"].ToString() , AutoSize = true};
                        Label lbl_status = new Label { Text = reader["Status"].ToString(), AutoSize = true };
                        Label lbl_reporter = new Label { Text = reader["reporter"].ToString(), AutoSize = true };
                        Label lbl_reportnature = new Label { Text = reader["Report_Nature"].ToString(), AutoSize = true };
                        Label lbl_inclocation = new Label { Text = reader["Incident_Location"].ToString(), AutoSize = true };
                        Label lbl_incdate = new Label { Text = reader["Incident_Date"].ToString(), AutoSize = true };
                        Label lbl_viccount = new Label { Text = reader["Victim_Count"].ToString(), AutoSize = true };
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

                        lbl_detailsHeading.Location = new Point(10, 100);
                        lbl_details.Location = new Point(lbl_detailsHeading.Right + 10, 100);

                        // Step 5: Create Approve and Remove buttons
                        Button btnApprove = new Button
                        {
                            Text = "Approve",
                            BackColor = Color.LightGreen,
                            Location = new Point(10, 260),
                            Width = 100,
                            Height = 30,
                            Font = new Font("Arial", 9),  // Set the font to Segoe UI with size 9
                            FlatStyle = FlatStyle.Flat,       // Set the button style to flat
                            FlatAppearance = { BorderSize = 0 } // Optional: Remove the border for a cleaner look
                        };

                        Button btnRemove = new Button
                        {
                            Text = "Remove",
                            BackColor = Color.IndianRed,
                            Location = new Point(120, 260),
                            Width = 100,
                            Height = 30,
                            Font = new Font("Arial", 9),  // Set the font to Segoe UI with size 9
                            FlatStyle = FlatStyle.Flat,       // Set the button style to flat
                            FlatAppearance = { BorderSize = 0 } // Optional: Remove the border for a cleaner look
                        };

                        // Step 6: Assign Click Events
                        btnApprove.Click += (sender, e) => UpdateReportStatus(lbl_id.Text, "Approved");
                        btnRemove.Click += (sender, e) => UpdateReportStatus(lbl_id.Text, "Removed");

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
                        reportCard.Controls.Add(btnApprove);
                        reportCard.Controls.Add(btnRemove);

                        // Step 7: Add Panel to FlowLayoutPanel
                        flp_Reports.Controls.Add(reportCard);
                    }

                    reader.Close();
                }

                // If no reports were found, display "No pending reports" label
                if (!hasReports)
                {
                    Label lbl_noReports = new Label
                    {
                        Text = "No Pending Reports",
                        Font = new Font("Arial", 12, FontStyle.Regular),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    // Wrap it in a panel to center it
                    Panel wrapperPanel = new Panel
                    {
                        Width = flp_Reports.ClientSize.Width -50,
                        Height = flp_Reports.ClientSize.Height, // Adjust as needed
                        Anchor = AnchorStyles.None
                    };

                    lbl_noReports.Location = new Point((wrapperPanel.Width - lbl_noReports.Width) / 2,
                                                       (wrapperPanel.Height - lbl_noReports.Height) / 2);

                    wrapperPanel.Controls.Add(lbl_noReports);
                    flp_Reports.Controls.Add(wrapperPanel);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void UpdateReportStatus(string reportId, string newStatus)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DB.connectionString))
                {
                    conn.Open();

                    // Step 1: Get the current username from session
                    string currentUsername = UserSession.UserName;

                    // Step 2: Get the officer_id from the Officer table
                    int officerId = -1;
                    string getOfficerQuery = "SELECT officer_id FROM Officer WHERE username = @Username";
                    using (SqlCommand getOfficerCmd = new SqlCommand(getOfficerQuery, conn))
                    {
                        getOfficerCmd.Parameters.AddWithValue("@Username", currentUsername);
                        object result = getOfficerCmd.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int id))
                        {
                            officerId = id;
                        }
                        else
                        {
                            MessageBox.Show("Could not find officer linked to the current session username.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Step 3: Update report status and submitted_by
                    string updateQuery = "UPDATE Reports SET Status = @Status, submitted_by = @SubmittedBy, reviewed_at = @reviewtime WHERE report_id = @ReportID";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.Parameters.AddWithValue("@SubmittedBy", officerId);
                        cmd.Parameters.AddWithValue("@ReportID", reportId);
                        cmd.Parameters.AddWithValue("@reviewtime", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"Report {reportId} marked as {newStatus}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Optionally reload report grid or UI here
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void btnLoadReports_Click(object sender, EventArgs e)
        {
            LoadPendingReports();
        }


        private void flp_Reports_Paint(object sender, PaintEventArgs e)
        {

        }


        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerDash dashboard = new OfficerDash();
            dashboard.ShowDialog();
            this.Close();
        }

        private void btn_updateProfile_Click(object sender, EventArgs e)
        {
            this.Hide();

            UpdProfileO updateProfileForm = new UpdProfileO();

            updateProfileForm.ShowDialog();

            this.Close();
        }

        private void btn_searchcase_Click(object sender, EventArgs e)
        {
            this.Hide();

            SearchCaseO CaseSearchForm = new SearchCaseO();

            CaseSearchForm.ShowDialog();

            this.Close();
        }

        private void btn_fir_Click(object sender, EventArgs e)
        {
            this.Hide();
            selectRep ShowReports = new selectRep();
            ShowReports.ShowDialog();
            this.Close();
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerCases AssignedCases = new OfficerCases();
            AssignedCases.ShowDialog();
            this.Close();
        }
    }
}
