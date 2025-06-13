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
    public partial class SearchCaseO: Form
    {
        public SearchCaseO()
        {
            InitializeComponent();
            this.Load += new EventHandler(SearchCaseO_Load);

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

        private void SearchCaseO_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            LoadCrimeCategories();
            select_crime.DropDownHeight = 100;
        }

        private void LoadCrimeCategories()
        {
            string connectionString = @"Data Source=PC-MAHNUR\SQLEXPRESS;Initial Catalog=CI;Integrated Security=True;Encrypt=False";
            string query = "SELECT DISTINCT crime_catg FROM Crime_Categories";

            using (SqlConnection con = new SqlConnection(connectionString))
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
                                select_crime.Items.Add(reader["crime_catg"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading crime categories.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            select_crime.SelectedIndex = 0; // Set the placeholder as the default selected item
        }


        private void btn_search_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(select_crime.Text) || select_crime.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a crime type before searching.", "Missing Crime Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string crime = select_crime.Text;
            string city = string.IsNullOrWhiteSpace(select_city.Text) || select_city.SelectedIndex == -1 ? null : select_city.Text;
            DateTime? weekStart = reportweek_picker.Checked ? reportweek_picker.Value.Date : (DateTime?)null;
            string location = string.IsNullOrWhiteSpace(txb_crimelocation.Text) ? null : txb_crimelocation.Text.Trim();
            int? victimCount = int.TryParse(txb_victimcount.Text, out int count) ? count : (int?)null;
            string status = string.IsNullOrWhiteSpace(select_status.Text) || select_status.SelectedIndex == -1 ? null : select_status.Text;
            string caseTitle = string.IsNullOrWhiteSpace(txb_casetitle.Text) ? null : txb_casetitle.Text.Trim();
            string caseNo = string.IsNullOrWhiteSpace(txb_caseno.Text) ? null : txb_caseno.Text.Trim();

            // Pass all values including crime and caseNo to the DisplayCasesP form
            DisplayCasesO displayForm = new DisplayCasesO(crime, city, weekStart, location, victimCount, status, caseTitle, caseNo);

            this.Hide();
            displayForm.ShowDialog();
            this.Close();
        }

        private void select_crime_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void select_city_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void reportweek_picker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txb_crimelocation_TextChanged(object sender, EventArgs e)
        {

        }

        private void txb_victimcount_TextChanged(object sender, EventArgs e)
        {

        }
        private void txb_victimcount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow digits and control keys (Backspace, Delete, Arrow keys)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Block non-numeric input
            }
        }

        private void select_status_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txb_caseno_TextChanged(object sender, EventArgs e)
        {

        }

        private void txb_casetitle_TextChanged(object sender, EventArgs e)
        {

        }

        private void dsh_btn_Click_1(object sender, EventArgs e)
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

        private void btn_newreports_Click(object sender, EventArgs e)
        {
            this.Hide();

            NewReportsO newreports = new NewReportsO();

            newreports.ShowDialog();

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
            using (SqlConnection conn = new SqlConnection("Data Source=PC-MAHNUR\\SQLEXPRESS;Initial Catalog=CI;Integrated Security=True;Encrypt=False"))
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

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerCases AssignedCases = new OfficerCases();
            AssignedCases.ShowDialog();
            this.Close();
        }

        private void btn_searchcase_Click(object sender, EventArgs e)
        {

        }
    }
}
