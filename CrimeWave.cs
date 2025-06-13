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
    public partial class CrimeWave : Form
    {
        public CrimeWave()
        {
            InitializeComponent(); 
            this.Load += new EventHandler(CrimeWave_Load);

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
        private void CrimeWave_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update
            DetectWaves();
        }

        private void DetectWaves()
        {
            flp_waves.Controls.Clear();
            List<ReportData> reports = new List<ReportData>();

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = @"SELECT report_id, report_nature, incident_location, incident_date, victim_count, incident_details FROM Reports";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["incident_location"] == DBNull.Value || reader["incident_date"] == DBNull.Value)
                            continue;

                        reports.Add(new ReportData
                        {
                            Id = reader["report_id"] == DBNull.Value ? -1 : Convert.ToInt32(reader["report_id"]),
                            Nature = reader["report_nature"]?.ToString() ?? "",
                            Location = reader["incident_location"].ToString(),
                            Date = Convert.ToDateTime(reader["incident_date"]),
                            VictimCount = reader["victim_count"] == DBNull.Value ? "" : reader["victim_count"].ToString(),
                            Details = reader["incident_details"]?.ToString() ?? ""
                        });
                    }
                }
            }

            HashSet<int> used = new HashSet<int>();

            for (int i = 0; i < reports.Count; i++)
            {
                if (used.Contains(i)) continue;

                var baseReport = reports[i];
                List<ReportData> waveGroup = new List<ReportData> { baseReport };
                used.Add(i);

                for (int j = i + 1; j < reports.Count; j++)
                {
                    if (used.Contains(j)) continue;

                    var compareReport = reports[j];
                    if (baseReport.Nature != compareReport.Nature) continue;

                    if (Math.Abs((baseReport.Date - compareReport.Date).Days) <= 7 &&
                        IsLocationSimilar(baseReport.Location, compareReport.Location))
                    {
                        waveGroup.Add(compareReport);
                        used.Add(j);
                    }
                }

                if (waveGroup.Count > 1)
                    AddWaveCard(waveGroup);
            }
        }

        private bool IsLocationSimilar(string loc1, string loc2)
        {
            var words1 = loc1.ToLower().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            var words2 = loc2.ToLower().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            var common = words1.Intersect(words2).Count();
            return common >= Math.Min(words1.Length, words2.Length) / 2;
        }

        private void AddWaveCard(List<ReportData> group)
        {
            var panel = new Panel
            {
                Width = flp_waves.Width - 25,
                Height = 220,
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                Margin = new Padding(10)
            };

            var header = new Label
            {
                Text = $"No of Reports:   {group.Count}        Report_Nature:    {group[0].Nature}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            string ids = string.Join(" | ", group.Select(g => g.Id));
            string locations = string.Join(" | ", group.Select(g => g.Location));
            string counts = string.Join(" | ", group.Select(g => g.VictimCount));
            string dates = string.Join("   |   ", group.Select(g => g.Date.ToString("yyyy-MM-dd")));
            string details = string.Join("\r\n\r\n", group.Select(g => g.Details));

            var lblIds = new Label
            {
                Text = $"Report IDs:     {ids}",
                Location = new Point(10, 35),
                Size = new Size(panel.Width - 20, 20),
                AutoSize = false,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };

            var lblLoc = new Label
            {
                Text = $"Incident Locations:   {locations} \n",
                Location = new Point(10, 60),
                Size = new Size(panel.Width - 20, 30),
                AutoSize = true,
                MaximumSize = new Size(panel.Width - 150, 0),
                Font = new Font("Arial", 10, FontStyle.Regular)
            };

            var lblVictim = new Label
            {
                Text = $"Victim Count:     {counts}",
                Location = new Point(10, 90),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };

            var lblDate = new Label
            {
                Text = $"Incident Date:   {dates}",
                Location = new Point(10, 115),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };

            var lblDetail = new Label
            {
                Text = $"Incident Details (all combined):\r\n\n{details} \n\n",
                Location = new Point(10, 140),
                Size = new Size(panel.Width - 20, 70),
                MaximumSize = new Size (panel.Width - 150, 0),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };

            panel.Controls.Add(header);
            panel.Controls.Add(lblIds);
            panel.Controls.Add(lblLoc);
            panel.Controls.Add(lblVictim);
            panel.Controls.Add(lblDate);
            panel.Controls.Add(lblDetail);

            flp_waves.Controls.Add(panel);
        }

        class ReportData
        {
            public int Id;
            public string Nature;
            public string Location;
            public DateTime Date;
            public string VictimCount;
            public string Details;
        }

        private void flp_waves_Paint(object sender, PaintEventArgs e)
        {

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

        private void btn_fir_Click(object sender, EventArgs e)
        {
            this.Hide();
            selectRep ShowReports = new selectRep();
            ShowReports.ShowDialog();
            this.Close();
        }

        private void btn_newreports_Click(object sender, EventArgs e)
        {
            this.Hide();
            NewReportsO newreports = new NewReportsO();
            newreports.ShowDialog();
            this.Close();
        }

        private void btn_searchcase_Click(object sender, EventArgs e)
        {
            this.Hide();

            SearchCaseO CaseSearchForm = new SearchCaseO();

            CaseSearchForm.ShowDialog();

            this.Close();
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerCases AssignedCases = new OfficerCases();
            AssignedCases.ShowDialog();
            this.Close();
        }

        private void btn_updateProfile_Click(object sender, EventArgs e)
        {
            this.Hide();

            UpdProfileO updateProfileForm = new UpdProfileO();

            updateProfileForm.ShowDialog();

            this.Close();
        }

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerDash dashboard = new OfficerDash();
            dashboard.ShowDialog();
            this.Close();
        }
    }
}
