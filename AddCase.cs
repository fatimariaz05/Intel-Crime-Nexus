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
    public partial class AddCase : Form
    {
        public AddCase()
        {
            InitializeComponent();
            this.Load += new EventHandler(AddCase_Load);

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

        private void AddCase_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            LoadCrimeCategories();
        }

        private void LoadCrimeCategories()
        {
            string query = "SELECT DISTINCT crime_catg FROM Crime_Categories";

            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            //select_crime.Items.Clear(); // Ensure no previous data

                            while (reader.Read())
                            {
                                select_type.Items.Add(reader["crime_catg"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading crime categories.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            select_type.SelectedIndex = 0; // Set the placeholder as the default selected item
        }

        private void txb_victimcount_keypress(object sender, KeyPressEventArgs e)
        {
            // Allow digits and control keys (Backspace, Delete, Arrow keys)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Block non-numeric input
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            // 1. Basic validation
            if (string.IsNullOrWhiteSpace(txb_title.Text) ||
                string.IsNullOrWhiteSpace(txb_victimcount.Text) ||
                select_status.SelectedIndex == 0 ||
                select_type.SelectedIndex == 0 ||
                string.IsNullOrWhiteSpace(txb_crimelocation.Text) ||
                string.IsNullOrWhiteSpace(txb_desc.Text))
            {
                MessageBox.Show("Please fill all fields and select valid options.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(DB.connectionString))
                {
                    con.Open();

                    // 2. Get next case_id
                    int nextCaseID = 1;
                    using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(case_id), 0) + 1 FROM cases", con))
                    {
                        nextCaseID = (int)cmd.ExecuteScalar();
                    }

                    // 3. Generate unique case_no in format 'CaseNO/Year'
                    Random rand = new Random();
                    string caseNo;
                    bool isUnique = false;
                    int year = DateTime.Now.Year;

                    do
                    {
                        int numPart = rand.Next(1, 100001); // Generate number part
                        caseNo = $"{numPart}/{year}";      // Format as '2345/2025'

                        using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM cases WHERE case_no = @caseNo", con))
                        {
                            cmd.Parameters.AddWithValue("@caseNo", caseNo);
                            isUnique = ((int)cmd.ExecuteScalar() == 0);
                        }

                    } while (!isUnique);


                    // 4. Get crime_category_ID from selected crime type
                    int crimeCategoryID = -1;
                    using (SqlCommand cmd = new SqlCommand("SELECT crime_catg_ID FROM Crime_categories WHERE crime_catg = @type", con))
                    {
                        cmd.Parameters.AddWithValue("@type", select_type.SelectedItem.ToString());
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            crimeCategoryID = Convert.ToInt32(result);
                        else
                            throw new Exception("Selected crime type is invalid.");
                    }

                    // 5. Combine date and time
                    DateTime crimeDate = date_picker.Value.Date;
                    int hour = int.Parse(select_hour.SelectedItem.ToString());
                    int minute = int.Parse(select_min.SelectedItem.ToString());
                    int second = int.Parse(select_sec.SelectedItem.ToString());
                    DateTime crimeDateTime = crimeDate.AddHours(hour).AddMinutes(minute).AddSeconds(second);

                    // 6. Insert into cases table
                    string query = @"INSERT INTO cases (case_id, case_no, case_title, victim_count, status, crime_type, 
                             crime_time, crime_location, detailed_description, crime_category_ID, created_at) 
                             VALUES (@id, @no, @title, @victims, @status, @type, @time, @location, 
                             @desc, @catID, @created)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", nextCaseID);
                        cmd.Parameters.AddWithValue("@no", caseNo);
                        cmd.Parameters.AddWithValue("@title", txb_title.Text.Trim());
                        cmd.Parameters.AddWithValue("@victims", int.Parse(txb_victimcount.Text));
                        cmd.Parameters.AddWithValue("@status", select_status.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@type", select_type.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@time", crimeDateTime);
                        cmd.Parameters.AddWithValue("@location", txb_crimelocation.Text.Trim());
                        cmd.Parameters.AddWithValue("@desc", txb_desc.Text.Trim());
                        cmd.Parameters.AddWithValue("@catID", crimeCategoryID);
                        cmd.Parameters.AddWithValue("@created", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Case initiated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
