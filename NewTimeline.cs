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
    public partial class NewTimeline : Form
    {
        private string caseId;
        public NewTimeline(string caseId)
        {
            InitializeComponent();
            this.caseId = caseId;

            this.Load += new EventHandler(NewTimeline_Load);

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

        private void NewTimeline_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update


            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = "SELECT case_no FROM Cases WHERE case_id = @caseId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@caseId", caseId);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    txb_caseno.Text = result.ToString();
                    txb_caseno.ReadOnly = true;
                }
                else
                {
                    MessageBox.Show("Case not found.");
                    this.Close();
                }
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            // 1. Validate Action Type
            if (select_action.SelectedItem == null || select_action.SelectedItem.ToString() == "Select Action")
            {
                MessageBox.Show("Please select a valid action type.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Validate time combo boxes (not placeholder values)
            if (select_hour.SelectedItem == null || select_min.SelectedItem == null || select_sec.SelectedItem == null ||
                select_hour.SelectedItem.ToString() == "Hour" ||
                select_min.SelectedItem.ToString() == "Minute" ||
                select_sec.SelectedItem.ToString() == "Second")
            {
                MessageBox.Show("Please select a valid time (Hour, Minute, Second).", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Combine Date + Time from dropdowns
            DateTime selectedDate = dtp_date.Value.Date;

            int hour = int.Parse(select_hour.SelectedItem.ToString());
            int minute = int.Parse(select_min.SelectedItem.ToString());
            int second = int.Parse(select_sec.SelectedItem.ToString());

            DateTime timestamp = selectedDate
                .AddHours(hour)
                .AddMinutes(minute)
                .AddSeconds(second);

            string actionType = select_action.SelectedItem.ToString();
            string description = txb_desc.Text.Trim();

            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Please enter a description.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();

                // 4. Get officer_id using current session
                string officerIdQuery = @"SELECT officer_id FROM Officer WHERE username = @username";
                SqlCommand cmdOfficer = new SqlCommand(officerIdQuery, conn);
                cmdOfficer.Parameters.AddWithValue("@username", UserSession.UserName);
                object officerResult = cmdOfficer.ExecuteScalar();

                if (officerResult == null)
                {
                    MessageBox.Show("Current officer not found.");
                    return;
                }

                int officerId = Convert.ToInt32(officerResult);

                // 5. Get new timeline_id
                string idQuery = "SELECT ISNULL(MAX(timeline_id), 0) + 1 FROM Case_Timelines";
                SqlCommand cmdId = new SqlCommand(idQuery, conn);
                int newTimelineId = Convert.ToInt32(cmdId.ExecuteScalar());

                // 6. Insert into Case_Timelines
                string insertQuery = @"
            INSERT INTO Case_Timelines 
                (timeline_id, case_id, action_type, action_description, timestamp, updated_by)
            VALUES 
                (@timeline_id, @case_id, @action_type, @description, @timestamp, @updated_by)";

                SqlCommand cmdInsert = new SqlCommand(insertQuery, conn);
                cmdInsert.Parameters.AddWithValue("@timeline_id", newTimelineId);
                cmdInsert.Parameters.AddWithValue("@case_id", caseId);
                cmdInsert.Parameters.AddWithValue("@action_type", actionType);
                cmdInsert.Parameters.AddWithValue("@description", description);
                cmdInsert.Parameters.AddWithValue("@timestamp", timestamp);
                cmdInsert.Parameters.AddWithValue("@updated_by", officerId);

                cmdInsert.ExecuteNonQuery();

                MessageBox.Show("Timeline added in the case progess successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();

              
            }
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

        private void txb_caseno_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
