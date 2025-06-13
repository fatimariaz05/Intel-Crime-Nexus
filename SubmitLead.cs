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
    public partial class SubmitLead : Form
    {
        private int caseId;
        private string caseTitle;
        private string location;
        private DateTime? crimeTime;
        private string crime_type;

        public SubmitLead(int caseId, string caseTitle, string location, DateTime? crimeTime, string crime_type)
        {
            InitializeComponent();
            this.Load += new EventHandler(SubmitLead_Load);

            // Set title bar color to black
            SetTitleBarColor(Color.Black);

            // Remove the top-left icon but keep minimize, maximize, and close buttons
            this.Text = "Intel Crime Nexus";  // Change this to your app name
            this.ShowIcon = false;      // Hide the icon

            this.caseId = caseId;
            this.caseTitle = caseTitle;
            this.location = location;
            this.crimeTime = crimeTime;
            this.crime_type = crime_type;


        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private void SetTitleBarColor(Color color)
        {
            int colorValue = color.R | (color.G << 8) | (color.B << 16);
            DwmSetWindowAttribute(this.Handle, 35, ref colorValue, 4); // Works on Windows 10/11
        }
        private void SubmitLead_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName;  // Display username
            lbl_UserName.Refresh();

            // Autofill
            txb_title.Text = caseTitle;
            txb_time.Text = crimeTime.HasValue ? crimeTime.Value.ToString("g") : "N/A";
            txb_location.Text = location;
            txb_type.Text = crime_type;

            // Disable editing
            txb_title.ReadOnly = true;
            txb_time.ReadOnly = true;
            txb_location.ReadOnly = true;
            txb_type.ReadOnly = true;

            txb_location.TabStop = false;
        }


        private void btn_submit_Click_1(object sender, EventArgs e)
        {
            // Validate identity
            string identity = select_identity.SelectedItem?.ToString();
            if (identity != "Public" && identity != "Officer" && identity != "Anonymous")
            {
                MessageBox.Show("Please select a valid identity (Public, Officer, or Anonymous).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate statement
            string statement = txb_statement.Text.Trim();
            if (string.IsNullOrWhiteSpace(statement))
            {
                MessageBox.Show("Please enter the information you want to submit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // Validate checkbox
            if (!authenticate.Checked)
            {
                MessageBox.Show("You must confirm the accuracy and seriousness of this submission.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Fetch new lead_id
            int newLeadId = 1;
            using (SqlConnection conn = new SqlConnection(@"Data Source=PC-MAHNUR\SQLEXPRESS;Initial Catalog=CI;Integrated Security=True;Encrypt=False"))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(lead_id), 0) + 1 FROM Leads", conn))
                {
                    newLeadId = (int)cmd.ExecuteScalar();
                }

                using (SqlCommand insertCmd = new SqlCommand("INSERT INTO Leads (lead_id, case_id, submitted_by, statement, status, submitted_at) VALUES (@lead_id, @case_id, @submitted_by, @statement, @status, @submitted_at)", conn))
                {
                    insertCmd.Parameters.AddWithValue("@lead_id", newLeadId);
                    insertCmd.Parameters.AddWithValue("@case_id", caseId);
                    insertCmd.Parameters.AddWithValue("@submitted_by", identity);
                    insertCmd.Parameters.AddWithValue("@statement", statement);
                    insertCmd.Parameters.AddWithValue("@status", "Pending");
                    insertCmd.Parameters.AddWithValue("@submitted_at", DateTime.Now);

                    insertCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Lead submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
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

        private void btn_update_Click(object sender, EventArgs e)
        {
            this.Hide();

            UpdProfileP updateProfileForm = new UpdProfileP();

            updateProfileForm.ShowDialog();

            this.Close();
        }

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Hide();

            PublicDash dashboard = new PublicDash();

            dashboard.ShowDialog();

            this.Close();
        }

        private void btn_report_Click(object sender, EventArgs e)
        {
            this.Hide();
            SubmitReport reportform = new SubmitReport();
            reportform.ShowDialog();
            this.Close();
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Hide();

            SearchCaseP CaseSearch = new SearchCaseP();
            CaseSearch.ShowDialog();

            this.Close();
        }

        private void siticoneButton1_Click_1(object sender, EventArgs e)
        {
            this.Hide();

            SearchCaseP CaseSearch = new SearchCaseP();
            CaseSearch.ShowDialog();

            this.Close();
        }
    }

}
