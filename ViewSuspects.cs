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
    public partial class ViewSuspects : Form
    {
        public string SelectedCaseId { get; set; }
        public ViewSuspects()
        {
            InitializeComponent();
            this.Load += new EventHandler(ViewSuspects_Load);

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

        private void ViewSuspects_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            DisplaySuspects();
        }

        private void DisplaySuspects()
        {
            flp_suspects.Controls.Clear();

            string caseId = SelectedCaseId;
            string query = "SELECT * FROM Suspects WHERE case_id = @case_id";

            using (SqlConnection con = new SqlConnection(DB.connectionString))
            {
                try
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@case_id", caseId);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        Panel suspectCard = new Panel
                        {
                            Width = flp_suspects.Width - 50,
                            Height = flp_suspects.Height - 120,
                            AutoSize = true,
                            BackColor = Color.White,
                            Padding = new Padding(10),
                            Margin = new Padding(10),
                            BorderStyle = BorderStyle.FixedSingle,
                            Location = new Point(flp_suspects.Location.X + 20, flp_suspects.Location.Y + 20),
                        };

                        string filePath = row["file_path"].ToString();

                        // Labels
                        Label lbl_idHead = new Label { Text = "Suspect ID: ", Font = new Font("Arial", 10, FontStyle.Bold) };
                        Label lbl_id = new Label { Text = row["suspect_id"].ToString(), AutoSize = true, Font = new Font("Arial", 10) };

                        Label lbl_nameHead = new Label { Text = "Name: ", Font = new Font("Arial", 10, FontStyle.Bold) };
                        Label lbl_name = new Label { Text = row["name"].ToString(), AutoSize = true, Font = new Font("Arial", 10) };

                        Label lbl_ageHead = new Label { Text = "Age: ", Font = new Font("Arial", 10, FontStyle.Bold) };
                        Label lbl_age = new Label { Text = row["age"].ToString(), AutoSize = true, Font = new Font("Arial", 10) };

                        Label lbl_genderHead = new Label { Text = "Gender: ", Font = new Font("Arial", 10, FontStyle.Bold) };
                        Label lbl_gender = new Label { Text = row["gender"].ToString(), AutoSize = true, Font = new Font("Arial", 10) };

                        Label lbl_statusHead = new Label { Text = "Status: ", Font = new Font("Arial", 10, FontStyle.Bold) };
                        Label lbl_status = new Label { Text = row["status"].ToString(), AutoSize = true, Font = new Font("Arial", 10) };

                        Label lbl_locationHead = new Label { Text = "Last Known Location: ", Font = new Font("Arial", 10, FontStyle.Bold), AutoSize = true };
                        Label lbl_location = new Label { Text = row["last_known_location"].ToString(), AutoSize = true, Font = new Font("Arial", 10), MaximumSize = new Size(suspectCard.Width - 215, 0) };

                        Label lbl_historyHead = new Label { Text = "Criminal History: ", Font = new Font("Arial", 10, FontStyle.Bold), AutoSize = true };
                        Label lbl_history = new Label { Text = row["criminal_history"].ToString(), AutoSize = true, Font = new Font("Arial", 10),
                            MaximumSize = new Size(suspectCard.Width - 215, 0)
                        };

                        Label lbl_descHead = new Label { Text = "Description: ", Font = new Font("Arial", 10, FontStyle.Bold) };
                        Label lbl_desc = new Label
                        {
                            Text = row["description"].ToString(),
                            MaximumSize = new Size(suspectCard.Width - 150, 0),
                            AutoSize = true,
                            Font = new Font("Arial", 10)
                        };

                        // Media Viewer Panel
                        Panel viewerPanel = new Panel
                        {
                            Location = new Point(470, 8),
                            Size = new Size(suspectCard.Width / 2 - 50, suspectCard.Height / 2),
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.Gainsboro,
                            Name = "viewerPanel"
                        };

                        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                        {
                            Label noMediaLabel = new Label
                            {
                                Text = "No Media Attached",
                                Dock = DockStyle.Fill,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Font = new Font("Arial", 11),
                                ForeColor = Color.Black,
                                BackColor = Color.Transparent
                            };
                            viewerPanel.Controls.Add(noMediaLabel);
                        }
                        else
                        {
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
                                }
                                else if (extension == ".mp4" || extension == ".avi" || extension == ".mov")
                                {
                                    var player = new AxWMPLib.AxWindowsMediaPlayer();
                                    ((System.ComponentModel.ISupportInitialize)(player)).BeginInit();
                                    player.Dock = DockStyle.Fill;
                                    viewerPanel.Controls.Add(player);
                                    ((System.ComponentModel.ISupportInitialize)(player)).EndInit();

                                    player.uiMode = "full";
                                    player.URL = filePath;
                                    player.Ctlcontrols.play();
                                }
                                else if (extension == ".mp3" || extension == ".wav")
                                {
                                    var audioPlayer = new AxWMPLib.AxWindowsMediaPlayer();
                                    ((System.ComponentModel.ISupportInitialize)(audioPlayer)).BeginInit();
                                    audioPlayer.Dock = DockStyle.Fill;
                                    viewerPanel.Controls.Add(audioPlayer);
                                    ((System.ComponentModel.ISupportInitialize)(audioPlayer)).EndInit();

                                    audioPlayer.uiMode = "mini";
                                    audioPlayer.URL = filePath;
                                    audioPlayer.Ctlcontrols.play();
                                }
                                else if (extension == ".pdf" || extension == ".docx")
                                {
                                    WebBrowser browser = new WebBrowser
                                    {
                                        Dock = DockStyle.Fill
                                    };
                                    viewerPanel.Controls.Add(browser);
                                    browser.Navigate(filePath);
                                }
                                else
                                {
                                    Label unsupportedLabel = new Label
                                    {
                                        Text = "Unsupported File Type",
                                        Dock = DockStyle.Fill,
                                        TextAlign = ContentAlignment.MiddleCenter,
                                        Font = new Font("Segoe UI", 12, FontStyle.Bold),
                                        ForeColor = Color.Red
                                    };
                                    viewerPanel.Controls.Add(unsupportedLabel);
                                }
                            }
                            catch (Exception err)
                            {
                                MessageBox.Show("Error displaying file: " + err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        // Label positioning
                        int left = 10;
                        lbl_idHead.Location = new Point(left, 10);
                        lbl_id.Location = new Point(lbl_idHead.Right + 3, 10);

                        lbl_nameHead.Location = new Point(left, 35);
                        lbl_name.Location = new Point(lbl_nameHead.Right + 3, 35);

                        lbl_ageHead.Location = new Point(left, 60);
                        lbl_age.Location = new Point(lbl_ageHead.Right + 3, 60);

                        lbl_genderHead.Location = new Point(left, 85);
                        lbl_gender.Location = new Point(lbl_genderHead.Right + 3, 85);

                        lbl_statusHead.Location = new Point(left, 110);
                        lbl_status.Location = new Point(lbl_statusHead.Right + 3, 110);

                        lbl_locationHead.Location = new Point(left, 235);
                        lbl_location.Location = new Point(lbl_locationHead.Right + 100, 235);

                        lbl_historyHead.Location = new Point(left, lbl_location.Bottom + 50);
                        lbl_history.Location = new Point(lbl_historyHead.Right + 100, lbl_location.Bottom + 50);

                        lbl_descHead.Location = new Point(left, lbl_history.Bottom + 50);
                        lbl_desc.Location = new Point(lbl_descHead.Right + 10, lbl_history.Bottom + 50);


                        // Final assembly
                        suspectCard.Controls.AddRange(new Control[]
                        {
                    lbl_idHead, lbl_id,
                        lbl_nameHead, lbl_name,
                        lbl_ageHead, lbl_age,
                        lbl_genderHead, lbl_gender,
                        lbl_statusHead, lbl_status,
                        lbl_locationHead, lbl_location,
                        lbl_historyHead, lbl_history,
                        lbl_descHead, lbl_desc,
                        viewerPanel
                        });



                        flp_suspects.Controls.Add(suspectCard);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching suspects: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }




    }
}
