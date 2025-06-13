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
    public partial class SubmitTestimony : Form
    {
        private int caseId;
        private string caseTitle;
        private string location;
        private DateTime? crimeTime;
        private string crimetype;
        public SubmitTestimony(int caseId, string caseTitle, string location, DateTime? crimeTime, string crimetype)
        {
            InitializeComponent();
            this.Load += new EventHandler(SubmitTestimony_Load);

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
        private void SubmitTestimony_Load(object sender, EventArgs e)
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
        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            string witnessName = txb_name.Text.Trim();
            string witnessType = select_type.SelectedItem?.ToString();
            string statementText = statement.Text.Trim();

            if (string.IsNullOrWhiteSpace(witnessName) || string.IsNullOrWhiteSpace(witnessType) ||
                witnessType == "Select Witness Type" || string.IsNullOrWhiteSpace(statementText))
            {
                MessageBox.Show("Please fill all fields and select a valid witness type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!authenticate.Checked)
            {
                MessageBox.Show("You must confirm the accuracy and seriousness of this submission.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int newWitnessId = 1;
            int submittedByUserId = -1;
            string filePath = null;

            string rootFolder = @"D:\IntelCrimeNexus\Cases";
            string caseFolder = Path.Combine(rootFolder, $"Case_{caseId}");
            string testimonyFolder = Path.Combine(caseFolder, "Witness_Testimony");

            using (SqlConnection conn = new SqlConnection(@"Data Source=PC-MAHNUR\SQLEXPRESS;Initial Catalog=CI;Integrated Security=True;Encrypt=False"))
            {
                conn.Open();

                // Generate new witness_id
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(witness_id), 0) + 1 FROM Witnesses", conn))
                {
                    newWitnessId = (int)cmd.ExecuteScalar();
                }

                // Get user_id
                using (SqlCommand cmd = new SqlCommand("SELECT user_id FROM [User] WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", UserSession.UserName);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                        submittedByUserId = (int)result;
                    else
                    {
                        MessageBox.Show("User not found in the system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Get file path (first file in the folder)
                if (Directory.Exists(testimonyFolder))
                {
                    string[] files = Directory.GetFiles(testimonyFolder);
                    if (files.Length > 0)
                        filePath = files[0];
                }

                // Insert witness record
                using (SqlCommand insertCmd = new SqlCommand(@"
            INSERT INTO Witnesses (witness_id, case_id, name, witness_type, statement, submitted_date, submitted_by, file_path, status)
            VALUES (@witness_id, @case_id, @name, @witness_type, @statement, @submitted_date, @submitted_by, @file_path, @status)", conn))
                {
                    insertCmd.Parameters.AddWithValue("@witness_id", newWitnessId);
                    insertCmd.Parameters.AddWithValue("@case_id", caseId);
                    insertCmd.Parameters.AddWithValue("@name", witnessName);
                    insertCmd.Parameters.AddWithValue("@witness_type", witnessType);
                    insertCmd.Parameters.AddWithValue("@statement", statementText);
                    insertCmd.Parameters.AddWithValue("@submitted_date", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("@submitted_by", submittedByUserId);
                    insertCmd.Parameters.AddWithValue("@file_path", (object)filePath ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("@status", "Pending");

                    insertCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Witness testimony submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void btn_attach_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            ofd.Title = "Select an attachment";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string sourcePath = ofd.FileName;
                string fileName = Path.GetFileName(sourcePath);

                int caseId = this.caseId; // passed in constructor

                // Folder structure: Cases/Case_<id>/Witness_Testimony/
                string rootFolder = @"D:\IntelCrimeNexus\Cases";
                string caseFolder = Path.Combine(rootFolder, $"Case_{caseId}");
                string targetFolder = Path.Combine(caseFolder, "Witness_Testimony");

                // Create folder if it doesn't exist
                Directory.CreateDirectory(targetFolder);

                string destinationPath = Path.Combine(targetFolder, fileName);

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

        private void SubmitTestimony_Load_1(object sender, EventArgs e)
        {

        }
    }
}
