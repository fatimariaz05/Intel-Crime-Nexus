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
    public partial class PublicDash : Form
    {
        public PublicDash()
        {
            InitializeComponent();
            this.Load += new EventHandler(PublicDash_Load); // Ensure Load event is attached

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
        private void PublicDash_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text =  UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update
            //"Username: " +



            // Fetch user details directly from the database
            string query = "SELECT Name FROM [User] WHERE username = @userName";

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

            LoadUserStatistics();
        }
        private void LoadUserStatistics()
        {

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                try
                {
                    conn.Open();

                    // Step 1: Get user_id from User table based on username
                    string queryUserId = "SELECT user_id FROM [User] WHERE username = @userName";
                    SqlCommand cmdUserId = new SqlCommand(queryUserId, conn);
                    cmdUserId.Parameters.AddWithValue("@userName", UserSession.UserName);

                    object result = cmdUserId.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("User not found.");
                        return;
                    }

                    int userId = Convert.ToInt32(result);

                    // Step 2: Count occurrences in Reports, Evidence, and Witnesses tables
                    string queryCounts = @"
                    SELECT 
                        (SELECT COUNT(*) FROM Reports WHERE reported_by = @userId) AS ReportsCount,
                        (SELECT COUNT(*) FROM Evidence WHERE submitted_by = @userId) AS EvidenceCount,
                        (SELECT COUNT(*) FROM Witnesses WHERE submitted_by = @userId) AS WitnessesCount";

                    SqlCommand cmdCounts = new SqlCommand(queryCounts, conn);
                    cmdCounts.Parameters.AddWithValue("@userId", userId);

                    using (SqlDataReader reader = cmdCounts.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lbl_rep.Text = reader["ReportsCount"].ToString();
                            lbl_evd.Text = reader["EvidenceCount"].ToString();
                            lbl_witn.Text = reader["WitnessesCount"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void siticonePanel2_Paint(object sender, PaintEventArgs e)
        {

        }


        private void btn_update_Click(object sender, EventArgs e)
        { 
            //this.Hide();

            UpdProfileP updateProfileForm = new UpdProfileP();

            updateProfileForm.ShowDialog();

            this.Close();
        }

        private void dsh_btn_Click(object sender, EventArgs e)
        {

        }

        private void btn_report_Click(object sender, EventArgs e)
        {
            //this.Hide();
            SubmitReport reportform = new SubmitReport();
            reportform.ShowDialog();
            this.Close();
        }


        private void btn_witness_Click(object sender, EventArgs e)
        {
            //this.Hide();

            SearchCaseP CaseSearch = new SearchCaseP();
            CaseSearch.ShowDialog();

            this.Close();
        }


        private void PublicDash_Load_1(object sender, EventArgs e)
        {

        }

        private void btn_seach_Click(object sender, EventArgs e)
        {
            //this.Hide();

            SearchCaseP CaseSearch = new SearchCaseP();
            CaseSearch.ShowDialog();

            this.Close();
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            //this.Hide();

            SearchCaseP CaseSearch = new SearchCaseP();
            CaseSearch.ShowDialog();

            this.Close();
        }
    }
}
