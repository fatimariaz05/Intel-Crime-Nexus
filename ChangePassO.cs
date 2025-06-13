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
    public partial class ChangePassO: Form
    {
       
        public ChangePassO()
        {
            InitializeComponent();
            this.Load += new EventHandler(ChangePassO_Load);

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

        private void ChangePassO_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update
        }

        private void txb_oldpass_TextChanged(object sender, EventArgs e)
        {

        }

        private void txb_newpass_TextChanged(object sender, EventArgs e)
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
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                string query = "SELECT COUNT(1) FROM [Officer] WHERE username = @Username AND password = @OldPassword";
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
            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                string query = "UPDATE [Officer] SET password = @NewPassword WHERE username = @Username";
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


        private void btn_searchcase_Click_1(object sender, EventArgs e)
        {
            this.Hide();

            SearchCaseO CaseSearchForm = new SearchCaseO();

            CaseSearchForm.ShowDialog();

            this.Close();
        }

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerDash dashboard = new OfficerDash();
            dashboard.ShowDialog();
            this.Close();
        }

        private void btn_updateProfile_Click_1(object sender, EventArgs e)
        {
            this.Hide();

            UpdProfileO updateProfileForm = new UpdProfileO();

            updateProfileForm.ShowDialog();

            this.Close();
        }

        private void btn_newreports_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            NewReportsO newreports = new NewReportsO();
            newreports.ShowDialog();
            this.Close();
        }

        private void btn_cases_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerCases AssignedCases = new OfficerCases();
            AssignedCases.ShowDialog();
            this.Close();
        }

        private void btn_fir_Click(object sender, EventArgs e)
        {
            this.Hide();
            selectRep ShowReports = new selectRep();
            ShowReports.ShowDialog();
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
    }
}
