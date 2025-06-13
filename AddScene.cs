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
    public partial class AddScene : Form
    {
        public string SelectedCaseId { get; set; }
        private string attachedSceneFilePath = null;
        public AddScene()
        {
            InitializeComponent(); 
            this.Load += new EventHandler(AddScene_Load);

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
        private void AddScene_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update
        }

        private void btn_attach_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SelectedCaseId))
            {
                MessageBox.Show("No Case ID selected. Please select a case first.", "Missing Case", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int sceneId = GetNextSceneId();  // This ensures folder is unique

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            ofd.Title = "Select suspect attachment";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string sourcePath = ofd.FileName;
                string fileName = Path.GetFileName(sourcePath);

                int caseId = int.Parse(SelectedCaseId);

                string rootFolder = @"D:\IntelCrimeNexus\Cases";
                string sceneFolder = Path.Combine(rootFolder, $"Case_{caseId}", "CrimeScenes", $"Scene_{sceneId}");

                Directory.CreateDirectory(sceneFolder);

                string destinationPath = Path.Combine(sceneFolder, fileName);

                if (!File.Exists(destinationPath))
                {
                    File.Copy(sourcePath, destinationPath);
                    attachedSceneFilePath = destinationPath;  // 🔥 store it
                    MessageBox.Show("Crime Scene file attached successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("A file with the same name already exists for this scene.", "Duplicate File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private int GetOfficerId(string username)
        {
            int officerId = 0;
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                string query = "SELECT officer_id FROM Officer WHERE username = @username";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        officerId = Convert.ToInt32(result);
                    }
                    con.Close();
                }
            }

            return officerId;
        }

        private int GetNextSceneId()
        {
            int nextsceneId = 1;
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                string query = "SELECT MAX(scene_id) FROM Crime_Scenes";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        nextsceneId = Convert.ToInt32(result) + 1;
                    }
                    con.Close();
                }
            }

            return nextsceneId;
        }

        private void btn_save_Click_1(object sender, EventArgs e)
        {
            // Validate that required combo boxes have valid selections
            if (select_type.SelectedIndex == 0)
            {
                MessageBox.Show("Please select valid options from the dropdowns.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Retrieve the UserSession for officer details
            string officerUsername = UserSession.UserName;
            int officerId = GetOfficerId(officerUsername);

            // Get the highest suspect_id and increment it
            int sceneId = GetNextSceneId();

            // Get selected case ID from the form (make sure to set this value before saving)
            string selectedCaseId = SelectedCaseId;

            // Assuming a valid connection string
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                try
                {

                    // Get current date and time
                    DateTime addedAt = DateTime.Now;

                    // SQL query to insert the suspect information with the additional fields
                    string query = "INSERT INTO Crime_Scenes (scene_id, scene_type, scene_location, " +
               "scene_description, case_id, file_path, added_at, added_by) " +
               "VALUES (@scene_id, @scene_type, @scene_location, @scene_description, " +
               "@case_id, @file_path, @added_at, @added_by)";


                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Adding parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@scene_id", sceneId);
                        cmd.Parameters.AddWithValue("@scene_type", select_type.SelectedItem.ToString());  // Ensure valid selection
                        cmd.Parameters.AddWithValue("@scene_location", txb_location.Text);
                        cmd.Parameters.AddWithValue("@scene_description", txb_desc.Text);
                        cmd.Parameters.AddWithValue("@case_id", selectedCaseId);
                        cmd.Parameters.AddWithValue("@file_path", string.IsNullOrWhiteSpace(attachedSceneFilePath) ? (object)DBNull.Value : attachedSceneFilePath);
                        cmd.Parameters.AddWithValue("@added_at", addedAt);
                        cmd.Parameters.AddWithValue("@added_by", officerId);

                        // Open connection and execute the query
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        // Show confirmation message
                        MessageBox.Show("Crime Scene information saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    // Show error message if there's an issue with the database operation
                    MessageBox.Show("Error saving crime scene data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
