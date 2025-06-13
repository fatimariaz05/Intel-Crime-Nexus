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
    public partial class CaseTimeline : Form
    {
        public string SelectedCaseId { get; set; }
        public CaseTimeline()
        {
            InitializeComponent();
            this.Load += new EventHandler(CaseTimeline_Load);

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
        private void CaseTimeline_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            if (!string.IsNullOrEmpty(SelectedCaseId))
            {
                // You can use SelectedCaseId for queries here
                LoadTimeline(SelectedCaseId); // hypothetical method
            }
        }

        private void LoadTimeline(string caseId)
        {
            flp_timeline.Controls.Clear();
            flp_timeline.AutoScroll = true;
            flp_timeline.WrapContents = true;
            flp_timeline.FlowDirection = FlowDirection.LeftToRight;

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();

                string query = @"SELECT timeline_id, action_type, timestamp, action_description 
                         FROM Case_Timelines 
                         WHERE case_id = @caseId 
                         ORDER BY timestamp ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@caseId", caseId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string timelineId = reader["timeline_id"].ToString();

                            Panel card = new Panel
                            {
                                Size = new Size(220, 160),
                                BackColor = Color.White,
                                BorderStyle = BorderStyle.FixedSingle,
                                Margin = new Padding(10),
                                Padding = new Padding(10)
                            };

                            // Remove Button (Red Cross)
                            PictureBox removeButton = new PictureBox
                            {
                                Image = Image.FromFile(@"C:\Users\HP\OneDrive\Desktop\Crime investigation\Iteration 1\remove1.png"),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                                Size = new Size(16, 16),
                                Cursor = Cursors.Hand,
                                Location = new Point(card.Width - 25, 5),
                                Tag = timelineId // Store timeline_id here
                            };

                            removeButton.Click += (s, e) =>
                            {
                                DialogResult result = MessageBox.Show("Are you sure you want to delete this timeline entry?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                if (result == DialogResult.Yes)
                                {
                                    using (SqlConnection deleteConn = new SqlConnection(DB.connectionString))
                                    {
                                        deleteConn.Open();
                                        string deleteQuery = "DELETE FROM Case_Timelines WHERE timeline_id = @timelineId";
                                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, deleteConn))
                                        {
                                            deleteCmd.Parameters.AddWithValue("@timelineId", removeButton.Tag);
                                            deleteCmd.ExecuteNonQuery();
                                        }
                                    }
                                    LoadTimeline(caseId); // Refresh the view
                                }
                            };

                            card.Controls.Add(removeButton);

                            // Action Type
                            Label lblType = new Label
                            {
                                Text = reader["action_type"].ToString(),
                                Font = new Font("Arial", 9, FontStyle.Bold),
                                AutoSize = false,
                                Height = 20,
                                Dock = DockStyle.Top
                            };

                            // Spacing
                            Label spacing1 = new Label { Height = 5, Dock = DockStyle.Top };

                            // Timestamp
                            Label lblTime = new Label
                            {
                                Text = $"Time: {Convert.ToDateTime(reader["timestamp"]).ToString("yyyy-MM-dd HH:mm:ss")}",
                                Font = new Font("Arial", 9),
                                AutoSize = false,
                                Height = 20,
                                Dock = DockStyle.Top
                            };

                            Label spacing2 = new Label { Height = 5, Dock = DockStyle.Top };

                            // Description
                            Label lblDesc = new Label
                            {
                                Text = $"Description:\n{reader["action_description"]}",
                                Font = new Font("Arial", 9),
                                AutoSize = false,
                                Height = 50,
                                Dock = DockStyle.Top
                            };

                            // Order of Controls
                            card.Controls.Add(lblDesc);
                            card.Controls.Add(spacing2);
                            card.Controls.Add(lblTime);
                            card.Controls.Add(spacing1);
                            card.Controls.Add(lblType);

                            flp_timeline.Controls.Add(card);

                            // Arrow Between Cards
                            PictureBox arrow = new PictureBox
                            {
                                Image = Image.FromFile(@"C:\Users\HP\OneDrive\Desktop\Crime investigation\Iteration 1\arrow.png"),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                                Size = new Size(40, 40),
                                Margin = new Padding(5, 50, 5, 50)
                            };
                            flp_timeline.Controls.Add(arrow);
                        }
                    }
                }

                // Final Add (+) Button
                PictureBox btnAdd = new PictureBox
                {
                    Image = Image.FromFile(@"C:\Users\HP\OneDrive\Desktop\Crime investigation\Iteration 1\plus.png"),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Size = new Size(150, 150),
                    Cursor = Cursors.Hand,
                    Margin = new Padding(10)
                };


                btnAdd.Click += (s, e) =>
                {
                    NewTimeline form = new NewTimeline(caseId); // Pass caseId
                    form.ShowDialog();
                    LoadTimeline(caseId); // Reload after adding
                };

                flp_timeline.Controls.Add(btnAdd);
            }
        }

        private void btn_reviewleads_Click(object sender, EventArgs e)
        {
            ReviewLeads reviewForm = new ReviewLeads();
            reviewForm.SelectedCaseId = SelectedCaseId;
            // pass the caseId
            reviewForm.ShowDialog(); // open as modal
        }

        private void btn_reviewevd_Click(object sender, EventArgs e)
        {
            ReviewEvidence reviewForm = new ReviewEvidence();
            reviewForm.SelectedCaseId = SelectedCaseId;
            // pass the caseId
            reviewForm.ShowDialog(); // open as modal
        }

        private void btn_updateProfile_Click(object sender, EventArgs e)
        {
            this.Hide();

            UpdProfileO updateProfileForm = new UpdProfileO();

            updateProfileForm.ShowDialog();

            this.Close();
        }

        private void siticoneButton1_Click_1(object sender, EventArgs e)
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

        private void siticoneButton3_Click_1(object sender, EventArgs e)
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

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerDash dashboard = new OfficerDash();
            dashboard.ShowDialog();
            this.Close();
        }

        private void siticoneButton7_Click(object sender, EventArgs e)
        {
            ReviewTestimony reviewForm = new ReviewTestimony();
            reviewForm.SelectedCaseId = SelectedCaseId;
            // pass the caseId
            reviewForm.ShowDialog(); // open as modal
        }

        private void view_leads_Click(object sender, EventArgs e)
        {
            ViewLeads viewForm = new ViewLeads();
            viewForm.SelectedCaseId = SelectedCaseId;
            // pass the caseId
            viewForm.ShowDialog(); // open as modal
        }

        private void view_evd_Click(object sender, EventArgs e)
        {
            ViewEvidence viewForm = new ViewEvidence();
            viewForm.SelectedCaseId = SelectedCaseId;
            // pass the caseId
            viewForm.ShowDialog(); // open as modal
        }

        private void view_testim_Click(object sender, EventArgs e)
        {
            ViewTestimony viewForm = new ViewTestimony();
            viewForm.SelectedCaseId = SelectedCaseId;
            // pass the caseId
            viewForm.ShowDialog(); // open as modal
        }

        private void addsuspect_Click(object sender, EventArgs e)
        {
            AddSuspect viewForm = new AddSuspect();
            viewForm.SelectedCaseId = SelectedCaseId;
            // pass the caseId
            viewForm.ShowDialog(); // open as modal
        }

        private void viewsuspects_Click(object sender, EventArgs e)
        {
            ViewSuspects viewForm = new ViewSuspects();
            viewForm.SelectedCaseId = SelectedCaseId;
            // pass the caseId
            viewForm.ShowDialog(); // open as modal
        }

        private void addvictim_Click(object sender, EventArgs e)
        {
            AddVictim viewForm = new AddVictim();
            viewForm.SelectedCaseId = SelectedCaseId;
            // pass the caseId
            viewForm.ShowDialog(); // open as modal
        }

        private void viewvictims_Click(object sender, EventArgs e)
        {
            ViewVictims viewForm = new ViewVictims();
            viewForm.SelectedCaseId = SelectedCaseId;
            // pass the caseId
            viewForm.ShowDialog(); // open as modal
        }

        private void addscene_Click(object sender, EventArgs e)
        {
            AddScene viewForm = new AddScene();
            viewForm.SelectedCaseId = SelectedCaseId;
            // pass the caseId
            viewForm.ShowDialog(); // open as modal
        }

        private void viewscenes_Click(object sender, EventArgs e)
        {
            ViewScenes viewForm = new ViewScenes();
            viewForm.SelectedCaseId = SelectedCaseId;
            // pass the caseId
            viewForm.ShowDialog(); // open as modal
        }
    }
}
