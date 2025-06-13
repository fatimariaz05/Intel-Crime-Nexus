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
    public partial class selectRep: Form
    {
        public selectRep()
        {
            InitializeComponent();
            this.Load += new EventHandler(selectRep_Load);

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

        private void selectRep_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            LoadApprovedReports();
        }

        private void LoadApprovedReports()
        {
            try
            {
                flp_Reports.Controls.Clear(); // Clear previous reports
                bool hasReports = false; // Flag to check if there are pending reports

                using (SqlConnection conn = new SqlConnection(DB.connectionString))
                {
                    conn.Open();
                    //SELECT * FROM Reports WHERE Status = 'Pending'
                    string query = @"
                    SELECT * 
                    FROM Reports r
                    WHERE 
                        r.status = 'Approved' 
                        AND NOT EXISTS (
                            SELECT 1 
                            FROM FIR f 
                            WHERE f.report_id = r.report_id
                        )
                        AND EXISTS (
                            SELECT 1 
                            FROM Officer o 
                            JOIN Scope_Cities sc ON o.police_station LIKE '%' + sc.City + '%'
                            WHERE o.username = @username 
                            AND r.Incident_Location LIKE '%' + sc.City + '%'
                        );";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", UserSession.UserName);  // ✅ Pass the parameter here
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
                        Label lbl_id = new Label { Text = reader["report_id"].ToString(), AutoSize = true };
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
                        Button btnfir = new Button
                        {
                            Text = "File FIR",
                            BackColor = Color.LightBlue,
                            Location = new Point(10, 260),
                            Width = 100,
                            Height = 30,
                            Font = new Font("Arial", 9),  // Set the font to Segoe UI with size 9
                            FlatStyle = FlatStyle.Flat,       // Set the button style to flat
                            FlatAppearance = { BorderSize = 0 } // Optional: Remove the border for a cleaner look
                        };

                        // Step 6: Assign Click Events
                        btnfir.Click += (sender, e) =>
                        {
                            string selected_report_id = lbl_id.Text;
                            this.Hide();
                            FileFIR firForm = new FileFIR(selected_report_id);
                            firForm.ShowDialog(); // or .Show() depending on your UX preference
                            this.Show();
                        };


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
                        reportCard.Controls.Add(btnfir);

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
                        Text = "No Approved Reports",
                        Font = new Font("Arial", 12, FontStyle.Regular),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    // Wrap it in a panel to center it
                    Panel wrapperPanel = new Panel
                    {
                        Width = flp_Reports.ClientSize.Width - 50,
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

        private void btnLoadReports_Click(object sender, EventArgs e)
        {
            LoadApprovedReports();
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

        private void btn_newreports_Click(object sender, EventArgs e)
        {
            this.Hide();
            NewReportsO newreports = new NewReportsO();
            newreports.ShowDialog();
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

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerCases AssignedCases = new OfficerCases();
            AssignedCases.ShowDialog();
            this.Close();
        }
    }
}
