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
using AxWMPLib;
using WMPLib;

namespace CISystem
{
    public partial class ViewEvidence : Form
    {
        public string SelectedCaseId { get; set; }
        public ViewEvidence()
        {
            InitializeComponent();
            this.Load += new EventHandler(ViewEvidence_Load);

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

        private void ViewEvidence_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            LoadEvidence();
        }

        private void LoadEvidence()
        {
            flp_evidence.Controls.Clear();

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();

                string query = @"
        SELECT evidence_id, evidence_type, collected_date, current_location, description, status, file_path
        FROM Evidence
        WHERE case_id = @caseId AND status = 'Accepted'
        ";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@caseId", SelectedCaseId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int evidenceId = Convert.ToInt32(reader["evidence_id"]);
                    string type = reader["evidence_type"].ToString();
                    string status = reader["status"].ToString();
                    string date = Convert.ToDateTime(reader["collected_date"]).ToString("f");
                    string location = reader["current_location"].ToString();
                    string desc = reader["description"].ToString();
                    string filePath = reader["file_path"] == DBNull.Value ? null : reader["file_path"].ToString();

                    Panel card = new Panel
                    {
                        BackColor = Color.White,
                        Size = new Size (flp_evidence.Width- 20, 250),
                        AutoSize = false,
                        Margin = new Padding(10),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    Label lbl_type = new Label { Text = "Evidence Type:", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true };
                    Label val_type = new Label { Text = type, Font = new Font("Arial", 10), Location = new Point(140, 10), AutoSize = true };

                    Label lbl_status = new Label { Text = "Status:", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, 35), AutoSize = true };
                    Label val_status = new Label { Text = status, Font = new Font("Arial", 10), Location = new Point(140, 35), AutoSize = true };

                    Label lbl_date = new Label { Text = "Submitted Date:", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, 60), AutoSize = true };
                    Label val_date = new Label { Text = date, Font = new Font("Arial", 10), Location = new Point(140, 60), AutoSize = true };

                    Label lbl_location = new Label { Text = "Current Location:", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, 85), AutoSize = true };
                    Label val_location = new Label { Text = location, Font = new Font("Arial", 10), Location = new Point(140, 85), AutoSize = true };

                    Label lbl_desc = new Label { Text = "Description:", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, 110), AutoSize = true };
                    Label val_desc = new Label
                    {
                        Text = desc,
                        Font = new Font("Arial", 10),
                        Location = new Point(30, 135),
                        MaximumSize = new Size(card.Width - 40, 60),
                        AutoSize = true
                    };

                    // View Evidence Button
                    Button btn_view = new Button
                    {
                        Text = "View Evidence",
                        BackColor = Color.MediumPurple,
                        ForeColor = Color.White,
                        Font = new Font("Arial", 9, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(120, 30),
                        Location = new Point(10, val_desc.Bottom + 10)
                    };
                    btn_view.FlatAppearance.BorderSize = 0;

                    btn_view.Click += (s, e) =>
                    {
                        if (string.IsNullOrEmpty(filePath))
                        {
                            MessageBox.Show("No Evidence Attached", "File Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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


                    card.Controls.AddRange(new Control[] {
                        lbl_type, val_type,
                        lbl_status, val_status,
                        lbl_date, val_date,
                        lbl_location, val_location,
                        lbl_desc, val_desc,
                        btn_view
                    });

                    flp_evidence.Controls.Add(card);
                }


                reader.Close();

                if (flp_evidence.Controls.Count == 0)
                {
                    Label noLeadsLabel = new Label();
                    noLeadsLabel.Text = "No Accepted Evidence Found";
                    noLeadsLabel.Font = new Font("Arial", 14, FontStyle.Bold);
                    noLeadsLabel.ForeColor = Color.Gray;
                    noLeadsLabel.TextAlign = ContentAlignment.MiddleCenter;
                    noLeadsLabel.Dock = DockStyle.Fill;

                    Panel containerPanel = new Panel();
                    containerPanel.Width = flp_evidence.Width;
                    containerPanel.Height = flp_evidence.Height;
                    containerPanel.Controls.Add(noLeadsLabel);
                    noLeadsLabel.Location = new Point(
                        (containerPanel.Width - noLeadsLabel.PreferredWidth) / 2,
                        (containerPanel.Height - noLeadsLabel.PreferredHeight) / 2
                    );

                    flp_evidence.Controls.Add(containerPanel);
                }
            }
        }

        private void btn_loadevd_Click(object sender, EventArgs e)
        {
            LoadEvidence();
        }

        private void dsh_btn_Click(object sender, EventArgs e)
        {

        }
    }
}
