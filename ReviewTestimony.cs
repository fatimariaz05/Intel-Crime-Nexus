using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CISystem
{
    public partial class ReviewTestimony : Form
    {
        public string SelectedCaseId { get; set; }
        public ReviewTestimony()
        {
            InitializeComponent();
            this.Load += new EventHandler(ReviewTestimony_Load);

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

        private void ReviewTestimony_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            LoadWitnesses();
        }

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void LoadWitnesses()
        {
            flp_witness.Controls.Clear();

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Witnesses WHERE case_id = @caseId and (approved_by = NULL or status = 'Pending' )";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@caseId", SelectedCaseId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    Panel card = new Panel
                    {
                        Size = new Size(flp_witness.Width - 50, 300),
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(10)
                    };

                    Font headingFont = new Font("Arial", 10, FontStyle.Bold);
                    Font textFont = new Font("Arial", 10);

                    string name = row["name"].ToString();
                    string type = row["witness_type"].ToString();
                    string date = Convert.ToDateTime(row["submitted_date"]).ToString("yyyy-MM-dd");
                    string statement = row["statement"].ToString();
                    string filePath = row["file_path"].ToString();
                    int witnessId = Convert.ToInt32(row["witness_id"]);

                    Label lbl_info = new Label
                    {
                        AutoSize = true,
                        Font = headingFont,
                        Text = "Witness Name: ",
                        Location = new Point(10, 10)
                    };
                    Label val_info = new Label
                    {
                        AutoSize = true,
                        Font = textFont,
                        Text = name,
                        Location = new Point(lbl_info.Right + 20, 10)
                    };

                    Label lbl_type = new Label
                    {
                        AutoSize = true,
                        Font = headingFont,
                        Text = "Witness Type: ",
                        Location = new Point(val_info.Right + 80, 10)
                    };
                    Label val_type = new Label
                    {
                        AutoSize = true,
                        Font = textFont,
                        Text = type,
                        Location = new Point(lbl_type.Right + 20, 10)
                    };

                    Label lbl_date = new Label
                    {
                        AutoSize = true,
                        Font = headingFont,
                        Text = "Submitted Date: ",
                        Location = new Point(10, lbl_info.Bottom + 10)
                    };
                    Label val_date = new Label
                    {
                        AutoSize = true,
                        Font = textFont,
                        Text = date,
                        Location = new Point(lbl_date.Right + 20, lbl_info.Bottom + 10)
                    };

                    Label lbl_statement = new Label
                    {
                        AutoSize = true,
                        Font = headingFont,
                        Text = "Statement:",
                        Location = new Point(10, lbl_date.Bottom + 10)
                    };
                    Label val_statement = new Label
                    {
                        AutoSize = false,
                        Font = textFont,
                        Text = statement,
                        Location = new Point(10, lbl_statement.Bottom + 5),
                        Size = new Size(550, 40)
                    };
                    val_statement.TextAlign = ContentAlignment.TopLeft;

                    Button btn_view = new Button
                    {
                        Text = "View Attached File",
                        BackColor = Color.MediumPurple,
                        ForeColor = Color.White,
                        Font = new Font("Arial", 9, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(150, 30),
                        Location = new Point(10, val_statement.Bottom + 10)
                    };
                    btn_view.FlatAppearance.BorderSize = 0;

                    btn_view.Click += (s, e) =>
                    {
                        if (string.IsNullOrEmpty(filePath))
                        {
                            MessageBox.Show("No File Attached", "File Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        Panel viewerPanel = new Panel
                        {
                            Location = new Point(292, 118),
                            Size = new Size(878, 538),
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.White,
                            Name = "viewerPanel"
                        };
                        this.Controls.Add(viewerPanel);
                        viewerPanel.BringToFront();

                        // 🔺 Cross (close) button
                        PictureBox btn_close = new PictureBox
                        {
                            Image = Image.FromFile(@"C:\Users\HP\OneDrive\Desktop\Crime investigation\Iteration 1\remove1.png"),
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Size = new Size(20, 20),
                            Location = new Point(viewerPanel.Width - 30, 5),
                            Cursor = Cursors.Hand,
                            BackColor = Color.Transparent
                        };
                        btn_close.Click += (csender, ce) =>
                        {
                            this.Controls.Remove(viewerPanel);
                            viewerPanel.Dispose();
                        };
                        viewerPanel.Controls.Add(btn_close);
                        btn_close.BringToFront();

                        string extension = Path.GetExtension(filePath).ToLower();

                        try
                        {
                            if (extension == ".jpg" || extension == ".png" || extension == ".jpeg" || extension == ".bmp")
                            {
                                PictureBox pic = new PictureBox
                                {
                                    Image = Image.FromFile(filePath),
                                    SizeMode = PictureBoxSizeMode.Zoom,
                                    Dock = DockStyle.Fill
                                };
                                viewerPanel.Controls.Add(pic);
                                pic.SendToBack(); // keep close button visible
                            }
                            else if (extension == ".mp4" || extension == ".avi" || extension == ".mov")
                            {
                                var player = new AxWMPLib.AxWindowsMediaPlayer();
                                ((System.ComponentModel.ISupportInitialize)(player)).BeginInit();

                                player.Dock = DockStyle.Fill;
                                player.Enabled = true;
                                viewerPanel.Controls.Add(player);

                                ((System.ComponentModel.ISupportInitialize)(player)).EndInit();

                                player.uiMode = "full";
                                player.URL = filePath;
                                player.Ctlcontrols.play();  // Start playback
                                player.SendToBack(); // So close button stays on top
                            }
                            else if (extension == ".mp3" || extension == ".wav")
                            {
                                var audioPlayer = new AxWMPLib.AxWindowsMediaPlayer();
                                ((System.ComponentModel.ISupportInitialize)(audioPlayer)).BeginInit();

                                audioPlayer.Dock = DockStyle.Fill;
                                audioPlayer.Enabled = true;
                                viewerPanel.Controls.Add(audioPlayer);

                                ((System.ComponentModel.ISupportInitialize)(audioPlayer)).EndInit();

                                audioPlayer.uiMode = "mini"; // Compact control for audio
                                audioPlayer.URL = filePath;
                                audioPlayer.Ctlcontrols.play();
                                audioPlayer.SendToBack(); // So close button or overlay stays visible
                            }

                            else if (extension == ".pdf" || extension == ".docx")
                            {
                                WebBrowser browser = new WebBrowser
                                {
                                    Dock = DockStyle.None
                                };

                                Panel scrollWrapper = new Panel
                                {
                                    Dock = DockStyle.Fill,
                                    AutoScroll = true,
                                    BackColor = Color.White
                                };

                                // Set browser size slightly smaller than the viewer panel, and add margin
                                browser.Size = new Size(viewerPanel.Width - 40, viewerPanel.Height - 40);
                                browser.Location = new Point(20, 20);

                                scrollWrapper.Controls.Add(browser);
                                viewerPanel.Controls.Add(scrollWrapper);
                                browser.Navigate(filePath);
                            }

                            else
                            {
                                MessageBox.Show("Unsupported File Type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to play media. Error: " + ex.Message, "Media Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };

                    Button btn_approve = new Button
                    {
                        Text = "Approve",
                        BackColor = Color.SeaGreen,
                        ForeColor = Color.White,
                        Font = new Font("Arial", 9, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(100, 30),
                        Location = new Point(card.Width - 220, val_statement.Bottom + 10)
                    };
                    btn_approve.FlatAppearance.BorderSize = 0;

                    btn_approve.Click += (s, e) =>
                    {
                        ApproveWitness(witnessId, "Approved");
                        DialogResult result = MessageBox.Show("Witness has been approved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (result == DialogResult.OK)
                        {
                            flp_witness.Controls.Remove(card);
                        }
                    };

                    Button btn_reject = new Button
                    {
                        Text = "Reject",
                        BackColor = Color.Firebrick,
                        ForeColor = Color.White,
                        Font = new Font("Arial", 9, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(100, 30),
                        Location = new Point(card.Width - 110, val_statement.Bottom + 10)
                    };
                    btn_reject.FlatAppearance.BorderSize = 0;

                    btn_reject.Click += (s, e) =>
                    {
                        ApproveWitness(witnessId, "Rejected");
                        DialogResult result = MessageBox.Show("Witness has been rejected.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (result == DialogResult.OK)
                        {
                            flp_witness.Controls.Remove(card);
                        }
                    };

                    card.Controls.AddRange(new Control[]
                    {
                lbl_info, val_info, lbl_type, val_type, lbl_date, val_date,
                lbl_statement, val_statement, btn_view, btn_approve, btn_reject
                    });

                    flp_witness.Controls.Add(card);
                }

                // No Data Case
                if (flp_witness.Controls.Count == 0)
                {
                    Label noWitnessLabel = new Label
                    {
                        Text = "No Witness Records Found",
                        Font = new Font("Arial", 14, FontStyle.Bold),
                        ForeColor = Color.Gray,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill
                    };

                    Panel emptyPanel = new Panel
                    {
                        Width = flp_witness.Width,
                        Height = flp_witness.Height
                    };
                    emptyPanel.Controls.Add(noWitnessLabel);
                    noWitnessLabel.Location = new Point(
                        (emptyPanel.Width - noWitnessLabel.PreferredWidth) / 2,
                        (emptyPanel.Height - noWitnessLabel.PreferredHeight) / 2
                    );

                    flp_witness.Controls.Add(emptyPanel);
                }
                if (flp_witness.Controls.Count == 0)
                {
                    Label noLeadsLabel = new Label();
                    noLeadsLabel.Text = "No Submitted Testimonies Found";
                    noLeadsLabel.Font = new Font("Arial", 14, FontStyle.Bold);
                    noLeadsLabel.ForeColor = Color.Gray;
                    noLeadsLabel.TextAlign = ContentAlignment.MiddleCenter;
                    noLeadsLabel.Dock = DockStyle.Fill;

                    Panel containerPanel = new Panel();
                    containerPanel.Width = flp_witness.Width;
                    containerPanel.Height = flp_witness.Height;
                    containerPanel.Controls.Add(noLeadsLabel);
                    noLeadsLabel.Location = new Point(
                        (containerPanel.Width - noLeadsLabel.PreferredWidth) / 2,
                        (containerPanel.Height - noLeadsLabel.PreferredHeight) / 2
                    );

                    flp_witness.Controls.Add(containerPanel);
                }
            }
        }


        void ApproveWitness(int witnessId, string status)
        {
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();

                string officerQuery = "SELECT officer_id FROM Officer WHERE username = @username";
                SqlCommand getOfficer = new SqlCommand(officerQuery, conn);
                getOfficer.Parameters.AddWithValue("@username", UserSession.UserName);
                object officerId = getOfficer.ExecuteScalar();

                if (officerId != null)
                {
                    string updateQuery = "UPDATE Witnesses SET status = @status, approved_by = @officerId WHERE witness_id = @id";
                    SqlCommand update = new SqlCommand(updateQuery, conn);
                    update.Parameters.AddWithValue("@status", status);
                    update.Parameters.AddWithValue("@officerId", officerId);
                    update.Parameters.AddWithValue("@id", witnessId);
                    update.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("Current officer not found in database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



    }
}
