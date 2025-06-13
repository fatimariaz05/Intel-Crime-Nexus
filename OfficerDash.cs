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
    public partial class OfficerDash: Form
    {
        public OfficerDash()
        {
            InitializeComponent();
            this.Load += new EventHandler(OfficerDash_Load);

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

        private void OfficerDash_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            // Fetch user details directly from the database
            string query = "SELECT Name FROM [Officer] WHERE username = @userName";

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userName", UserSession.UserName); // Use username to fetch details

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) // If user data exists
                {
                    lbl_name.Text = reader["Name"] != DBNull.Value ? "Welcome, " + reader["Name"].ToString() : "Welcome, ";

                }
                reader.Close();
            }

            LoadOfficerStatistics();
        }

        private void LoadOfficerStatistics()
        {

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                try
                {
                    conn.Open();

                    // Step 1: Get user_id from User table based on username
                    string queryOfficerId = "SELECT officer_id FROM [Officer] WHERE username = @userName";
                    SqlCommand cmdOfficerId = new SqlCommand(queryOfficerId, conn);
                    cmdOfficerId.Parameters.AddWithValue("@userName", UserSession.UserName);

                    object result = cmdOfficerId.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("User not found.");
                        return;
                    }

                    int OfficerId = Convert.ToInt32(result);

                    // Step 2: Count occurrences in Reports, Evidence, and Witnesses tables
                    string queryCounts = @"
                    SELECT 
                        (SELECT COUNT(*) FROM Reports WHERE submitted_by = @OfficerId and status = 'Approved') AS ReportsCount,
                        (SELECT COUNT(*) FROM Case_Assignment WHERE status = 'Active' and officer_id = @OfficerId) AS CasesCount,
                        (SELECT COUNT(*) FROM Case_Assignment WHERE status = 'Closed' and officer_id = @OfficerId) AS ClosedCasesCount";

                    SqlCommand cmdCounts = new SqlCommand(queryCounts, conn);
                    cmdCounts.Parameters.AddWithValue("@OfficerId", OfficerId);

                    using (SqlDataReader reader = cmdCounts.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lbl_rep.Text = reader["ReportsCount"].ToString();
                            lbl_asc.Text = reader["CasesCount"].ToString();
                            lbl_solc.Text = reader["ClosedCasesCount"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void panelMenu_Paint(object sender, PaintEventArgs e)
        {

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

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void btn_fir_Click(object sender, EventArgs e)
        {
            this.Hide();
            selectRep ShowReports = new selectRep();
            ShowReports.ShowDialog();
            this.Close();
            
        }

        private void specialOP_Click(object sender, EventArgs e)
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
