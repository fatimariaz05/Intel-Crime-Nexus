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
    public partial class SignUp: Form
    {
        public SignUp()
        {
            InitializeComponent();
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
        private void SignUp_Load(object sender, EventArgs e)
        {

        }


        private void backtologin_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txb_username_TextChanged_1(object sender, EventArgs e)
        {
            // Clear message if no text is entered
            if (string.IsNullOrWhiteSpace(txb_username.Text))
            {
                lblUsernameStatus.Text = "";  // Hide label if empty
                return;
            }


            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                try
                {
                    con.Open();
                    string checkQuery = "SELECT COUNT(*) FROM [User] WHERE username = @Username";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", txb_username.Text);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            lblUsernameStatus.Text = "❌ Username already exists!";
                            lblUsernameStatus.ForeColor = Color.Red;
                        }
                        else
                        {
                            lblUsernameStatus.Text = "";  // Hide label if username doesn't exist
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblUsernameStatus.Text = "Error checking username!";
                    lblUsernameStatus.ForeColor = Color.Red;
                }
            }
        }

        private void txb_pass_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void gender_select_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void txb_name_TextChanged_1(object sender, EventArgs e)
        {
            
        }

        private void txb_desig_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void txb_email_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void txb_address_TextChanged_1(object sender, EventArgs e)
        {

        }


        private void btn_sign_Click_1(object sender, EventArgs e)
        {
            // Ensure passwords match
            if (txb_pass.Text != txb_re_pass.Text)
            {
                MessageBox.Show("Passwords do not match! Please re-enter.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Stop execution if passwords don't match
            }

            if(string.IsNullOrWhiteSpace(txb_name.Text) || string.IsNullOrWhiteSpace(txb_desig.Text) || string.IsNullOrWhiteSpace(txb_email.Text) || string.IsNullOrWhiteSpace(txb_address.Text) || string.IsNullOrWhiteSpace(txb_username.Text.Trim()) || string.IsNullOrWhiteSpace(txb_pass.Text) || string.IsNullOrWhiteSpace(txb_re_pass.Text) || gender_select.SelectedItem.ToString() =="Select Gender")
            {
                MessageBox.Show("Please enter all required information.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                try
                {
                    con.Open();

                    // First, check if the username already exists
                    string checkQuery = "SELECT COUNT(*) FROM [User] WHERE username = @Username";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", txb_username.Text);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("Username already exists! Please choose another.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // Stop execution
                        }
                    }

                    // Get the highest user_id and increment it
                    string getMaxIdQuery = "SELECT ISNULL(MAX(user_id), 0) + 1 FROM [User]";
                    int newUserId;

                    using (SqlCommand maxIdCmd = new SqlCommand(getMaxIdQuery, con))
                    {
                        newUserId = Convert.ToInt32(maxIdCmd.ExecuteScalar()); // Get the next user_id
                    }

                    // Insert the new user with the calculated user_id
                    string insertQuery = "INSERT INTO [User] (user_id, Name, designation, email, address, Gender, username, password) " +
                                         "VALUES (@UserID, @FullName, @Designation, @Email, @Address, @Gender, @Username, @Password)";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                    {
                        insertCmd.Parameters.AddWithValue("@UserID", newUserId);
                        insertCmd.Parameters.AddWithValue("@FullName", txb_name.Text);
                        insertCmd.Parameters.AddWithValue("@Designation", txb_desig.Text);
                        insertCmd.Parameters.AddWithValue("@Email", txb_email.Text);
                        insertCmd.Parameters.AddWithValue("@Address", txb_address.Text);
                        insertCmd.Parameters.AddWithValue("@Gender", gender_select.SelectedItem?.ToString() ?? "");
                        insertCmd.Parameters.AddWithValue("@Username", txb_username.Text.Trim());
                        insertCmd.Parameters.AddWithValue("@Password", txb_pass.Text);  // Hash this in production

                        int rowsAffected = insertCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Sign Up Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();  // Closes the SignUp form after message is displayed
                        }
                        else
                        {
                            MessageBox.Show("Sign Up Failed. Try Again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void lblUsernameStatus_Click(object sender, EventArgs e)
        {

        }

        private void siticoneTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txb_re_pass_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
