using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using CISystem;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CISystem
{
    public partial class LoginForm: Form
    {
        public LoginForm()
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
        private void LoginForm_Load(object sender, EventArgs e)
        {


        }

        private void lbl_UN_Click(object sender, EventArgs e)
        {

        }

        private void lbl_PS_Click(object sender, EventArgs e)
        {

        }


        private void User_choice_Enter(object sender, EventArgs e)
        {
           

        }

        private void rdb_public_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdb_officer_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void siticoneShapes2_Click(object sender, EventArgs e)
        {

        }

        private void siticoneTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void siticoneHtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void siticoneShapes1_Click(object sender, EventArgs e)
        {

        }

        private void rect2_Click(object sender, EventArgs e)
        {

        }

        private void siticoneTextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void tbx_password_TextChanged(object sender, EventArgs e)
        {

        }


        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            // Establish SQL connection
            SqlConnection con = new SqlConnection(DB.connectionString);

            try
            {
                con.Open();

                // Get input values
                string username = txb_username.Text;
                string password = txb_password.Text;
                string query = "";

                // Check if the public or officer radio button is selected
                if (rdb_public.Checked)
                {
                    query = "SELECT username FROM [User] WHERE username = @username AND password = @password";
                }
                else if (rdb_officer.Checked)
                {
                    query = "SELECT username FROM Officer WHERE username = @username AND password = @password";
                }
                else
                {
                    MessageBox.Show("Please select Public or Officer before logging in.");
                    return;
                }

                // Execute the query
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())  // If a matching record is found
                {
                    // Store the logged-in user's name
                    UserSession.UserName = reader["username"].ToString();

                    //MessageBox.Show("You are successfully logged in your account!");

                    //Redirect to the next form (Dashboard)

                    if (rdb_public.Checked)
                    {
                        this.Hide(); // Hide login form
                        PublicDash Pboard = new PublicDash();
                        Pboard.ShowDialog(); // Show dashboard
                        this.Close();
                    }
                    else if (rdb_officer.Checked)
                    {
                        this.Hide(); // Hide login form
                        OfficerDash Oboard = new OfficerDash();
                        Oboard.ShowDialog(); // Show dashboard
                        this.Close();
                    }
                    
                }
                else
                {
                    MessageBox.Show("Invalid username or password. Enter valid credentials!", "Error");
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }



        private void sign_option_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide login form
            SignUp signUpForm = new SignUp();
            signUpForm.ShowDialog(); // Show sign-up form as a modal dialog
            this.Show(); // Show login form again after sign-up form closes
        }
        
    }
}



