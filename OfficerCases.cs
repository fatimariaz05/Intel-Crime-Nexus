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
    public partial class OfficerCases : Form
    {
        public OfficerCases()
        {
            InitializeComponent();
            this.Load += new EventHandler(OfficerCases_Load);

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

        private void OfficerCases_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            LoadAssignedCases();
        }

        private void LoadAssignedCases()
        {
            flp_cases.Controls.Clear();

            SqlConnection conn = new SqlConnection(
                @"Data Source=PC-MAHNUR\SQLEXPRESS;Initial Catalog=CI;Integrated Security=True;Encrypt=False");
            conn.Open();

            // 1) Get officer_id
            SqlCommand cmd1 = new SqlCommand(
                "SELECT officer_id FROM Officer WHERE username = @username", conn);
            cmd1.Parameters.AddWithValue("@username", UserSession.UserName);
            object officerResult = cmd1.ExecuteScalar();
            if (officerResult == null)
            {
                conn.Close();
                return;
            }
            int officerId = Convert.ToInt32(officerResult);

            // 2) Fetch active cases assigned to that officer
            SqlCommand cmd2 = new SqlCommand(@"
        SELECT c.case_id, c.case_no, c.case_title, c.crime_type, c.crime_location,
               c.crime_time, c.victim_count, c.public_description, c.detailed_description
        FROM Cases c
        JOIN Case_Assignment ca ON c.case_id = ca.case_id
        WHERE (ca.officer_id = @officer_id or ca.assigned_by = @officer_id) AND ca.status = 'Active'", conn);
            cmd2.Parameters.AddWithValue("@officer_id", officerId);

            SqlDataReader reader = cmd2.ExecuteReader();
            while (reader.Read())
            {
                // Extract
                string caseId = reader["case_id"].ToString();
                string caseNo = reader["case_no"].ToString();
                string title = reader["case_title"].ToString();
                string type = reader["crime_type"].ToString();
                string location = reader["crime_location"].ToString();
                string time = Convert.ToDateTime(reader["crime_time"])
                                      .ToString("dd-MMM-yyyy HH:mm");
                string victims = reader["victim_count"].ToString();
                string pubDesc = reader["public_description"].ToString();
                string detDesc = reader["detailed_description"].ToString();

                // Card panel
                Panel card = new Panel
                {
                    Width = flp_cases.Width - 50,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(10),
                    Margin = new Padding(10)
                };

                // Line 1: Title
                Label lblTitle = new Label
                {
                    Text = title,
                    Font = new Font("Arial", 11, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };
                card.Controls.Add(lblTitle);

                // Line 2
                int y2 = 40;
                Label lblCIDH = new Label
                {
                    Text = "Case ID:",
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    Location = new Point(10, y2),
                    AutoSize = true
                };
                Label lblCIDV = new Label
                {
                    Text = caseId,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    Location = new Point(lblCIDH.Right + 5, y2),
                    AutoSize = true
                };
                Label lblCNH = new Label
                {
                    Text = "Case No:",
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    Location = new Point(lblCIDV.Right + 10, y2),
                    AutoSize = true
                };
                Label lblCNV = new Label
                {
                    Text = caseNo,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    Location = new Point(lblCNH.Right + 5, y2),
                    AutoSize = true
                };
                Label lblCTH = new Label
                {
                    Text = "Crime Type:",
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    Location = new Point(lblCNV.Right + 20, y2),
                    AutoSize = true
                };
                Label lblCTV = new Label
                {
                    Text = type,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    Location = new Point(lblCTH.Right + 5, y2),
                    AutoSize = true
                };
                card.Controls.Add(lblCIDH);
                card.Controls.Add(lblCIDV);
                card.Controls.Add(lblCNH);
                card.Controls.Add(lblCNV);
                card.Controls.Add(lblCTH);
                card.Controls.Add(lblCTV);

                // Line 3
                int y3 = y2 + 30;
                Label lblVCH = new Label
                {
                    Text = "Victim Count:",
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    Location = new Point(10, y3),
                    AutoSize = true
                };
                Label lblVCV = new Label
                {
                    Text = victims,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    Location = new Point(lblVCH.Right + 5, y3),
                    AutoSize = true
                };
                Label lblCTH2 = new Label
                {
                    Text = "Crime Time:",
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    Location = new Point(lblVCV.Right + 10, y3),
                    AutoSize = true
                };
                Label lblCTV2 = new Label
                {
                    Text = time,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    Location = new Point(lblCTH2.Right + 5, y3),
                    AutoSize = true
                };
                Label lblCLH = new Label
                {
                    Text = "Crime Location:",
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    Location = new Point(lblCTV2.Right + 20, y3),
                    AutoSize = true
                };
                Label lblCLV = new Label
                {
                    Text = location,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    Location = new Point(lblCLH.Right + 5, y3),
                    AutoSize = true
                };
                card.Controls.Add(lblVCH);
                card.Controls.Add(lblVCV);
                card.Controls.Add(lblCTH2);
                card.Controls.Add(lblCTV2);
                card.Controls.Add(lblCLH);
                card.Controls.Add(lblCLV);

                // Line 4: Public Description
                int y4 = y3 + 30;
                Label lblPDH = new Label
                {
                    Text = "Public Description:",
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    Location = new Point(10, y4),
                    AutoSize = true
                };
                Label lblPDV = new Label
                {
                    Text = pubDesc,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    Location = new Point(20, lblPDH.Bottom + 5),
                    MaximumSize = new Size(card.Width - 40, 0),
                    AutoSize = true
                };
                card.Controls.Add(lblPDH);
                card.Controls.Add(lblPDV);

                // Line 5: Detailed Description
                Label lblDDH = new Label
                {
                    Text = "Detailed Description:",
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    Location = new Point(10, lblPDV.Bottom + 10),
                    AutoSize = true
                };
                Label lblDDV = new Label
                {
                    Text = detDesc,
                    Font = new Font("Arial", 9, FontStyle.Regular),
                    Location = new Point(20, lblDDH.Bottom + 5),
                    MaximumSize = new Size(card.Width - 40, 0),
                    AutoSize = true
                };
                card.Controls.Add(lblDDH);
                card.Controls.Add(lblDDV);

                // Buttons
                int yB = lblDDV.Bottom + 15;

                Button btnEdit = new Button
                {
                    Text = "Edit",
                    Size = new Size(100, 30),
                    Font = new Font("Arial", 9),
                    Location = new Point(card.Width - 220, yB),
                    BackColor = Color.LightSkyBlue,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    TextAlign = ContentAlignment.MiddleCenter
                };

                
                Button btnProgress = new Button
                {
                    Text = "Show Progress",
                    Size = new Size(110, 30),
                    Font = new Font("Arial", 9),
                    Location = new Point(card.Width - 110, yB),
                    BackColor = Color.LightGreen,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    TextAlign = ContentAlignment.MiddleCenter
                };

                string capturedCaseId = caseId;

                btnProgress.Click += (s, e) =>
                {
                    this.Hide();
                    CaseTimeline timelineForm = new CaseTimeline();
                    timelineForm.SelectedCaseId = capturedCaseId; // Pass case_id to form property
                    timelineForm.ShowDialog();
                    this.Close();
                };


                btnEdit.Click += (s, e) =>
                {
                    EditCase editForm = new EditCase();
                    editForm.SelectedCaseId = capturedCaseId; // Pass case_id to form property
                    editForm.ShowDialog();
                    LoadAssignedCases();
                    //next task
                };


                card.Controls.Add(btnEdit);
                card.Controls.Add(btnProgress);

                // Add to flow panel
                flp_cases.Controls.Add(card);
            }

            reader.Close();
            conn.Close();
        }

        private void btnLoadReports_Click(object sender, EventArgs e)
        {
            LoadAssignedCases();
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerCases AssignedCases = new OfficerCases();
            AssignedCases.ShowDialog();
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

        private void btn_fir_Click(object sender, EventArgs e)
        {
            this.Hide();
            selectRep ShowReports = new selectRep();
            ShowReports.ShowDialog();
            this.Close();
        }
    }
}
