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
    public partial class UpdProfileP: Form
    {
        public UpdProfileP()
        {
            InitializeComponent();
            this.Load += new EventHandler(UpdProfile_Load); // Ensure Load event is attached

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

        private void UpdProfile_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName;  // Display username
            lbl_UserName.Refresh();

            // Fetch user details directly from the database
            string query = "SELECT Name, Designation, Email, Address, Gender FROM [User] WHERE username = @userName";

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userName", UserSession.UserName); // Use username to fetch details

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) // If user data exists
                {
                    txb_name.Text = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : "Enter Name";
                    txb_name.ForeColor = Color.Gray;

                    txb_desig.Text = reader["Designation"] != DBNull.Value ? reader["Designation"].ToString() : "Enter Designation";
                    txb_desig.ForeColor = Color.Gray;

                    txb_email.Text = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "Enter Email";
                    txb_email.ForeColor = Color.Gray;

                    txb_address.Text = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : "Enter Address";
                    txb_address.ForeColor = Color.Gray;

                    if (reader["Gender"] != DBNull.Value)
                    {
                        gender_select.SelectedItem = reader["Gender"].ToString();
                    }
                }
                reader.Close();
            }

            // Username should be readonly
            txb_username.Text = UserSession.UserName;
            txb_username.ForeColor = Color.Gray;
            txb_username.ReadOnly = true;
        }

        private void txb_name_TextChanged(object sender, EventArgs e)
        {

        }

        private void txb_desig_TextChanged(object sender, EventArgs e)
        {

        }

        private void txb_email_TextChanged(object sender, EventArgs e)
        {

        }

        private void txb_address_TextChanged(object sender, EventArgs e)
        {

        }

        private void gender_select_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txb_username_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_update_Click(object sender, EventArgs e)
        {

            string updateQuery = "UPDATE [User] SET Name = @FullName, designation = @Designation, email = @Email, " +
                                 "address = @Address, Gender = @Gender WHERE username = @Username";

            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                {
                    updateCmd.Parameters.AddWithValue("@FullName", txb_name.Text);
                    updateCmd.Parameters.AddWithValue("@Designation", txb_desig.Text);
                    updateCmd.Parameters.AddWithValue("@Email", txb_email.Text);
                    updateCmd.Parameters.AddWithValue("@Address", txb_address.Text);
                    updateCmd.Parameters.AddWithValue("@Gender", gender_select.SelectedItem?.ToString() ?? "");
                    updateCmd.Parameters.AddWithValue("@Username", txb_username.Text); // Ensure Username is used for identification

                    con.Open();  // Open connection before executing the query
                    int rowsAffected = updateCmd.ExecuteNonQuery();
                    con.Close(); // Close connection after execution

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Profile Updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Profile Not Updated. Try Again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void btn_change_pass_Click(object sender, EventArgs e)
        {
            //this.Hide();

            ChangePassP changepassForm = new ChangePassP();

            changepassForm.ShowDialog();

            this.Show();
        }

        private void panelMenu_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dsh_btn_Click_1(object sender, EventArgs e)
        {
            //this.Hide();

            PublicDash dashboard = new PublicDash();

            dashboard.ShowDialog();

            this.Close();
        }

        private void btn_report_Click(object sender, EventArgs e)
        {
            //this.Hide();

            SubmitReport submitreportform = new SubmitReport();
            submitreportform.ShowDialog();

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
