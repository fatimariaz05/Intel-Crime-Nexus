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
    public partial class EditCase : Form
    {
        public string SelectedCaseId { get; set; }
        public EditCase()
        {
            InitializeComponent();
            this.Load += new EventHandler(EditCase_Load);

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

        private void EditCase_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = "SELECT case_no, case_title, public_description, detailed_description, crime_type FROM Cases WHERE case_id = @caseId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@caseId", SelectedCaseId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txb_caseno.Text = reader["case_no"].ToString();
                    txb_title.Text = reader["case_title"].ToString();
                    txb_pb_desc.Text = reader["public_description"].ToString();
                    txb_det_desc.Text = reader["detailed_description"].ToString();
                    txb_crimetype.Text = reader["crime_type"].ToString();

                    txb_caseno.ReadOnly = true;
                    txb_title.ReadOnly = false;
                    txb_pb_desc.ReadOnly = false;
                    txb_det_desc.ReadOnly = false;
                    txb_crimetype.ReadOnly = false;
                }
                else
                {
                    MessageBox.Show("Case not found.");
                    this.Close();
                }
            }

        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string updateQuery = @"UPDATE Cases 
                               SET case_title = @title,
                                   public_description = @publicDesc,
                                   detailed_description = @detailedDesc,
                                    crime_type = @crimetype
                               WHERE case_id = @caseId";

                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@title", txb_title.Text.Trim());
                cmd.Parameters.AddWithValue("@publicDesc", txb_pb_desc.Text.Trim());
                cmd.Parameters.AddWithValue("@detailedDesc", txb_det_desc.Text.Trim());
                cmd.Parameters.AddWithValue("@crimetype", txb_crimetype.Text.Trim());
                cmd.Parameters.AddWithValue("@caseId", SelectedCaseId); // assuming SelectedCaseId is accessible

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Case Information Saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Update failed. Please try again.", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }

        private void btn_updateProfile_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_searchcase_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_newreports_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_fir_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void siticoneButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
