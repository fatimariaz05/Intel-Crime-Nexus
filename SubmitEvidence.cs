using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CISystem
{
    public partial class SubmitEvidence : Form
    {
        private int caseId;
        private string caseTitle;
        private string location;
        private DateTime? crimeTime;
        private string crimetype;

        public SubmitEvidence(int caseId, string caseTitle, string location, DateTime? crimeTime, string crimetype)
        {
            InitializeComponent();

            this.Load += new EventHandler(SubmitEvidence_Load);

            // Set title bar color to black
            SetTitleBarColor(Color.Black);

            // Remove the top-left icon but keep minimize, maximize, and close buttons
            this.Text = "Intel Crime Nexus";  // Change this to your app name
            this.ShowIcon = false;      // Hide the icon

            this.caseId = caseId;
            this.caseTitle = caseTitle;
            this.location = location;
            this.crimeTime = crimeTime;
            this.crimetype = crimetype;
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private void SetTitleBarColor(Color color)
        {
            int colorValue = color.R | (color.G << 8) | (color.B << 16);
            DwmSetWindowAttribute(this.Handle, 35, ref colorValue, 4); // Works on Windows 10/11
        }
        private void SubmitEvidence_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName;  // Display username
            lbl_UserName.Refresh();

            txb_title.Text = caseTitle;
            txb_title.ReadOnly = true;

            txb_type.Text = crimetype;
            txb_type.ReadOnly = true;

            txb_location.Text = location;
            txb_location.ReadOnly = true;

            txb_time.Text = crimeTime?.ToString("g");
            txb_time.ReadOnly = true;

            txb_location.TabStop = false;

            cmb_evidenceType.SelectedIndex = 0; // Ensure "Select Type" is default
        }

        private void btn_attach_Click(object sender, EventArgs e)
        {
            string evidenceType = cmb_evidenceType.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(evidenceType) || evidenceType == "Select Type")
            {
                MessageBox.Show("Please select a valid evidence type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            ofd.Title = "Select an attachment";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string sourcePath = ofd.FileName;
                string fileName = Path.GetFileName(sourcePath);

                int caseId = this.caseId; // passed in constructor

                // Correct folder structure
                string rootFolder = @"D:\IntelCrimeNexus\Cases";
                string caseFolder = Path.Combine(rootFolder, $"Case_{caseId}");
                string evidenceFolder = Path.Combine(caseFolder, "Evidence", evidenceType);

                // Create folders if they don’t exist
                Directory.CreateDirectory(evidenceFolder);  // this auto-creates full nested path

                string destinationPath = Path.Combine(evidenceFolder, fileName);

                if (!File.Exists(destinationPath))
                {
                    File.Copy(sourcePath, destinationPath);
                    MessageBox.Show("File attached and stored successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("A file with the same name already exists.", "Duplicate File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            string evidenceType = cmb_evidenceType.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(evidenceType) || evidenceType == "Select Type")
            {
                MessageBox.Show("Please select a valid evidence type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string currentLocation = txb_evidencelocation.Text.Trim();
            string description = txb_description.Text.Trim();

            if (string.IsNullOrWhiteSpace(currentLocation) || string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Please fill all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string filePath = null; // Optional
            string rootFolder = @"D:\IntelCrimeNexus\Cases";
            string caseFolder = Path.Combine(rootFolder, $"Case_{caseId}");
            string evidenceFolder = Path.Combine(caseFolder, "Evidence", evidenceType);

            // Find highest evidence_id
            int newEvidenceId = 1;
            int submittedByUserId = -1;

            // Validate checkbox
            if (!authenticate.Checked)
            {
                MessageBox.Show("You must confirm the accuracy and seriousness of this submission.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(@"Data Source=PC-MAHNUR\SQLEXPRESS;Initial Catalog=CI;Integrated Security=True;Encrypt=False"))
            {
                conn.Open();

                // Get max ID
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(evidence_id), 0) + 1 FROM Evidence", conn))
                {
                    newEvidenceId = (int)cmd.ExecuteScalar();
                }

                // Get user_id from username
                using (SqlCommand cmd = new SqlCommand("SELECT user_id FROM [User] WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", UserSession.UserName);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        submittedByUserId = (int)result;
                    else
                    {
                        MessageBox.Show("Could not find user in system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Check if file already exists in the folder (already handled during attach, just retrieve path if file was attached)
                string[] existingFiles = Directory.Exists(evidenceFolder) ? Directory.GetFiles(evidenceFolder) : null;
                if (existingFiles != null && existingFiles.Length > 0)
                {
                    filePath = existingFiles[0]; // Assume the first is the recent one
                }

                // Insert into DB
                using (SqlCommand insertCmd = new SqlCommand(@"INSERT INTO Evidence (evidence_id, case_id, evidence_type, collected_date, current_location, status, description, submitted_by, file_path)
                                                       VALUES (@evidence_id, @case_id, @evidence_type, @collected_date, @current_location, @status, @description, @submitted_by, @file_path)", conn))
                {
                    insertCmd.Parameters.AddWithValue("@evidence_id", newEvidenceId);
                    insertCmd.Parameters.AddWithValue("@case_id", caseId);
                    insertCmd.Parameters.AddWithValue("@evidence_type", evidenceType);
                    insertCmd.Parameters.AddWithValue("@collected_date", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("@current_location", currentLocation);
                    insertCmd.Parameters.AddWithValue("@status", "Submitted");
                    insertCmd.Parameters.AddWithValue("@description", description);
                    insertCmd.Parameters.AddWithValue("@submitted_by", submittedByUserId);
                    insertCmd.Parameters.AddWithValue("@file_path", (object)filePath ?? DBNull.Value);

                    insertCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Evidence submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
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

        private void siticoneButton1_Click_1(object sender, EventArgs e)
        {
            this.Hide();

            SearchCaseP CaseSearch = new SearchCaseP();
            CaseSearch.ShowDialog();

            this.Close();
        }
    }

}
