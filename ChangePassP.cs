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
    public partial class ChangePassP: Form
    {
        private string connectionString = @"Data Source=PC-MAHNUR\SQLEXPRESS;Initial Catalog=CI;Integrated Security=True;Encrypt=False";
        public ChangePassP()
        {
            InitializeComponent();
            this.Load += new EventHandler(ChangePass_Load);

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

        private void ChangePass_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update
        }

        private void old_pass_Click(object sender, EventArgs e)
        {

        }

        private void txb_oldpass_TextChanged(object sender, EventArgs e)
        {

        }

        private void new_pass_Click(object sender, EventArgs e)
        {

        }

        private void txb_newpass_TextChanged(object sender, EventArgs e)
        {

        }

        private void re_enter_newpass_Click(object sender, EventArgs e)
        {

        }

        private void txb_re_enter_newpass_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_change_pass_Click(object sender, EventArgs e)
        {
            string oldPassword = txb_oldpass.Text.Trim();
            string newPassword = txb_newpass.Text.Trim();
            string reEnteredPassword = txb_re_enter_newpass.Text.Trim();

            // Step 1: Verify old password
            if (!VerifyOldPassword(oldPassword))
            {
                MessageBox.Show("Old password is incorrect!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Step 2: Check if new password and re-entered password match
            if (newPassword != reEnteredPassword)
            {
                MessageBox.Show("New password and re-entered password do not match!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Step 3: Update the password in the database
            if (UpdatePassword(newPassword))
            {
                MessageBox.Show("Password changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Close the change password form
            }
            else
            {
                MessageBox.Show("Failed to update password. Try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool VerifyOldPassword(string oldPassword)
        {
            bool isValid = false;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM [User] WHERE username = @Username AND password = @OldPassword";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", UserSession.UserName);  // Get username from session
                    cmd.Parameters.AddWithValue("@OldPassword", oldPassword);

                    con.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    isValid = (count == 1);
                    con.Close();
                }
            }
            return isValid;
        }

        private bool UpdatePassword(string newPassword)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE [User] SET password = @NewPassword WHERE username = @Username";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@NewPassword", newPassword);
                    cmd.Parameters.AddWithValue("@Username", UserSession.UserName); // Get username from session

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    con.Close();

                    return rowsAffected > 0;
                }
            }
        }

        private void dsh_btn_Click_1(object sender, EventArgs e)
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

        private void btn_update_Click(object sender, EventArgs e)
        {
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
    }
}
