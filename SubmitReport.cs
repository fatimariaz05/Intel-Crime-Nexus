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
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace CISystem
{
    public partial class SubmitReport: Form
    {
        public SubmitReport()
        {
            InitializeComponent();

            LoadCrimeCategories();
            select_reportnature.DropDownHeight = 100;

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

        private void SubmitReport_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName;  // Display username
            lbl_UserName.Refresh();
        }

        private void LoadCrimeCategories()
        {
            string query = "SELECT DISTINCT crime_catg FROM Crime_Categories";

            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            //select_crime.Items.Clear(); // Ensure no previous data

                            while (reader.Read())
                            {
                                select_reportnature.Items.Add(reader["crime_catg"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading crime categories.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            select_reportnature.SelectedIndex = 0; // Set the placeholder as the default selected item
        }
        private void label6_Click(object sender, EventArgs e)
        {

        }

        

        private void selectPreference_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectPreference.SelectedItem.ToString() == "Anonymous – Hide My Identity")
            {
                reporter.Text = ""; // Clear any existing text
                reporter.Enabled = false; // Disable the textbox
            }
            else if (selectPreference.SelectedItem.ToString() == "Restricted – Only Visible to Police")
            {
                reporter.Enabled = true; // Enable the textbox
            }
        }

        private void reporter_TextChanged(object sender, EventArgs e)
        {

        }

        private void select_reportnature_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txb_victimcount_TextChanged(object sender, EventArgs e)
        {

        }

        private void txb_victimcount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow digits and control keys (Backspace, Delete, Arrow keys)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Block non-numeric input
            }
        }

        private void reportdate_picker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txb_crimelocation_TextChanged(object sender, EventArgs e)
        {

        }

        private void txb_incidentdetails_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            if (selectPreference.SelectedItem.ToString() == "Choose Preference")
            {
                MessageBox.Show("Please select valid anonymity preference.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectPreference.SelectedItem.ToString() == "Restricted – Only Visible to Police" && string.IsNullOrWhiteSpace(reporter.Text))
            {
                MessageBox.Show("Please enter your name, as this report is not anonymous.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (string.IsNullOrWhiteSpace(txb_crimelocation.Text) || string.IsNullOrWhiteSpace(txb_victimcount.Text) || (select_reportnature.SelectedItem.ToString() == "Choose Report Nature"))
            {
                MessageBox.Show("Please enter all required information.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? reportedBy = null;

            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                try
                {
                    con.Open();

                    // Get user_id if identity is shown
                    if (selectPreference.SelectedItem.ToString() != "Anonymous – Hide My Identity")
                    {
                        string getUserIdQuery = "SELECT user_id FROM [User] WHERE username = @username";
                        using (SqlCommand cmd = new SqlCommand(getUserIdQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@username", UserSession.UserName);
                            object result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                reportedBy = (int)result;
                            }
                        }
                    }

                    // Get the highest report_id
                    string getMaxIdQuery = "SELECT ISNULL(MAX(report_id), 0) + 1 FROM Reports";
                    int newReportId;
                    using (SqlCommand cmd = new SqlCommand(getMaxIdQuery, con))
                    {
                        newReportId = (int)cmd.ExecuteScalar();
                    }

                    // Insert new report
                    string insertQuery = @"
            INSERT INTO Reports (report_id, submitted_by, status, reporter, reported_by, Incident_details, Incident_date, Incident_Location, victim_count, Report_Nature, submitted_at)
            VALUES (@report_id, NULL, 'Pending', @reporter, @reported_by, @incident_details, @incident_date, @incident_location, @victim_count, @report_nature, @submitted_at)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@report_id", newReportId);
                        cmd.Parameters.AddWithValue("@reporter", reporter.Text);
                        cmd.Parameters.AddWithValue("@reported_by", reportedBy.HasValue ? (object)reportedBy.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@incident_details", txb_incidentdetails.Text);
                        cmd.Parameters.AddWithValue("@incident_date", reportdate_picker.Value);
                        cmd.Parameters.AddWithValue("@incident_location", txb_crimelocation.Text);
                        cmd.Parameters.AddWithValue("@victim_count", string.IsNullOrWhiteSpace(txb_victimcount.Text) ? (object)DBNull.Value : int.Parse(txb_victimcount.Text));
                        cmd.Parameters.AddWithValue("@report_nature", select_reportnature.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@submitted_at", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Report submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error submitting the report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            //this.Hide();

            PublicDash dashboard = new PublicDash();

            dashboard.ShowDialog();

            this.Close();
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            //this.Hide();

            UpdProfileP updateProfileForm = new UpdProfileP();

            updateProfileForm.ShowDialog();

            this.Close();
        }

        private void btn_seach_Click(object sender, EventArgs e)
        {
            //this.Hide();

            SearchCaseP CaseSearch = new SearchCaseP();
            CaseSearch.ShowDialog();

            this.Close();
        }

        private void btn_witness_Click(object sender, EventArgs e)
        {
            //this.Hide();

            SearchCaseP CaseSearch = new SearchCaseP();
            CaseSearch.ShowDialog();

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
