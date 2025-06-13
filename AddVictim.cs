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
    public partial class AddVictim : Form
    {
        public string SelectedCaseId { get; set; }
        private string attachedVictimFilePath = null;
        public AddVictim()
        {
            InitializeComponent();
            this.Load += new EventHandler(AddVictim_Load);

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

        private void AddVictim_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txb_age_Keypress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is not a digit (0-9) or a control key (like backspace)
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                // If it's not a digit or backspace, prevent the character from being entered
                e.Handled = true;
            }
        }

        private void btn_attach_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SelectedCaseId))
            {
                MessageBox.Show("No Case ID selected. Please select a case first.", "Missing Case", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int victimId = GetNextVictimId();  // This ensures folder is unique

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            ofd.Title = "Select suspect attachment";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string sourcePath = ofd.FileName;
                string fileName = Path.GetFileName(sourcePath);

                int caseId = int.Parse(SelectedCaseId);

                string rootFolder = @"D:\IntelCrimeNexus\Cases";
                string victimFolder = Path.Combine(rootFolder, $"Case_{caseId}", "Victims", $"Victim_{victimId}");

                Directory.CreateDirectory(victimFolder);

                string destinationPath = Path.Combine(victimFolder, fileName);

                if (!File.Exists(destinationPath))
                {
                    File.Copy(sourcePath, destinationPath);
                    attachedVictimFilePath = destinationPath;  // 🔥 store it
                    MessageBox.Show("Victim file attached successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("A file with the same name already exists for this victim.", "Duplicate File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private int GetNextVictimId()
        {
            int nextVictimId = 1;
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                string query = "SELECT MAX(victim_id) FROM Victims";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        nextVictimId = Convert.ToInt32(result) + 1;
                    }
                    con.Close();
                }
            }

            return nextVictimId;
        }


        private void btn_save_Click(object sender, EventArgs e)
        {
            // Validate that required combo boxes have valid selections
            if (select_gender.SelectedIndex == 0 || select_condition.SelectedIndex == 0)
            {
                MessageBox.Show("Please select valid options from the dropdowns.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Retrieve the UserSession for officer details
            string officerUsername = UserSession.UserName;
            int officerId = GetOfficerId(officerUsername);

            // Get the highest suspect_id and increment it
            int victimId = GetNextVictimId();

            // Get selected case ID from the form (make sure to set this value before saving)
            string selectedCaseId = SelectedCaseId;

            // Assuming a valid connection string
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                try
                {
                    // SQL query to insert the suspect information with the additional fields
                    string query = "INSERT INTO Victims (victim_id, name, age, Victim_Condition, gender, contact_info, " +
               "statement, case_id, file_path) " +
               "VALUES (@victim_id, @name, @age, @Victim_Condition, @gender, @contact_info, @statement ," +
               "@case_id, @file_path)";


                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Adding parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@victim_id", victimId);
                        cmd.Parameters.AddWithValue("@name", txb_name.Text);
                        cmd.Parameters.AddWithValue("@age", txb_age.Text);
                        cmd.Parameters.AddWithValue("@Victim_Condition", select_condition.SelectedItem.ToString());  // Ensure valid selection
                        cmd.Parameters.AddWithValue("@gender", select_gender.SelectedItem.ToString());  // Ensure valid selection
                        cmd.Parameters.AddWithValue("@contact_info", txb_contact.Text);
                        cmd.Parameters.AddWithValue("@statement", txb_statement.Text);
                        cmd.Parameters.AddWithValue("@case_id", selectedCaseId);
                        cmd.Parameters.AddWithValue("@file_path", string.IsNullOrWhiteSpace(attachedVictimFilePath) ? (object)DBNull.Value : attachedVictimFilePath);

                        // Open connection and execute the query
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        // Show confirmation message
                        MessageBox.Show("Victim information saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    // Show error message if there's an issue with the database operation
                    MessageBox.Show("Error saving victim data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
