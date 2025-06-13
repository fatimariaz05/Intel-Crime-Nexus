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
    public partial class SimilarCase : Form
    {
        public SimilarCase()
        {
            InitializeComponent();
            this.Load += new EventHandler(SimilarCase_Load);

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

        private void SimilarCase_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            // Link the check changed event to all checkboxes
            crimetype.CheckedChanged += Option_CheckedChanged;
            timeduration.CheckedChanged += Option_CheckedChanged;
            location.CheckedChanged += Option_CheckedChanged;
            victimcount.CheckedChanged += Option_CheckedChanged;
            description.CheckedChanged += Option_CheckedChanged;
        }

        private void btn_check_Click(object sender, EventArgs e)
        {
            string caseId = txb_ID.Text.Trim();
            if (string.IsNullOrEmpty(caseId))
            {
                MessageBox.Show("Please enter a Case ID.");
                return;
            }

            var selected = grp_options.Controls.OfType<CheckBox>()
                            .Where(cb => cb.Checked)
                            .Select(cb => cb.Name)
                            .ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show("Please select at least one option.");
                return;
            }

            if (selected.Count > 2)
            {
                MessageBox.Show("Please select only two options.");
                return;
            }

            string option1 = null, option2 = null;

            string MapNameToLabel(string name)
            {
                switch (name)
                {
                    case "crimetype": return "Crime Type";
                    case "timeduration": return "Time Duration";
                    case "location": return "Location";
                    case "victimcount": return "Victim Count";
                    case "description": return "Description Keywords";
                    default: return null;
                }
            }

            option1 = MapNameToLabel(selected[0]);
            if (selected.Count == 2)
                option2 = MapNameToLabel(selected[1]);

            // Send data to the next form
            this.Hide();
            MatchedCases matchedForm = new MatchedCases(caseId, option1, option2);
            matchedForm.ShowDialog();
            this.Close();
        }

        private void grp_options_Click(object sender, EventArgs e)
        {

        }
        private void Option_CheckedChanged(object sender, EventArgs e)
        {
            var checkedBoxes = grp_options.Controls.OfType<CheckBox>().Where(cb => cb.Checked).ToList();

            if (checkedBoxes.Count > 2)
            {
                ((CheckBox)sender).Checked = false;
                MessageBox.Show("You can only select up to two options.");
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerDash dashboard = new OfficerDash();
            dashboard.ShowDialog();
            this.Close();
        }

        private void btn_updateProfile_Click(object sender, EventArgs e)
        {
            this.Hide();

            UpdProfileO updateProfileForm = new UpdProfileO();

            updateProfileForm.ShowDialog();

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
            this.Hide();

            SearchCaseO CaseSearchForm = new SearchCaseO();

            CaseSearchForm.ShowDialog();

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
