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
    public partial class ViewScenes : Form
    {
        public string SelectedCaseId { get; set; }
        public ViewScenes()
        {
            InitializeComponent();
            this.Load += new EventHandler(ViewScenes_Load);

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
        private void ViewScenes_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            DisplayScenes();
        }

        private void DisplayScenes()
        {

            flp_scenes.Controls.Clear();

            string caseId = SelectedCaseId;
            string query = "SELECT * FROM Crime_Scenes WHERE case_id = @case_id";

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
                        Panel sceneCard = new Panel
                        {
                            Width = flp_scenes.Width - 50,
                            Height = flp_scenes.Height - 180,
                            AutoSize = true,
                            BackColor = Color.White,
                            Padding = new Padding(10),
                            Margin = new Padding(10),
                            BorderStyle = BorderStyle.FixedSingle,
                            Location = new Point(flp_scenes.Location.X + 20, flp_scenes.Location.Y + 20),
                        };

                        string filePath = row["file_path"].ToString();

                        // Labels
                        Label lbl_idHead = new Label { Text = "Crime Scene ID: ", Font = new Font("Arial", 10, FontStyle.Bold) };
                        Label lbl_id = new Label { Text = row["scene_id"].ToString(), AutoSize = true, Font = new Font("Arial", 10) };

                        Label lbl_typeHead = new Label { Text = "Scene Type: ", Font = new Font("Arial", 10, FontStyle.Bold) };
                        Label lbl_type = new Label { Text = row["scene_type"].ToString(), AutoSize = true, Font = new Font("Arial", 10) };

                        Label lbl_timehead = new Label { Text = "Added At: ", Font = new Font("Arial", 10, FontStyle.Bold) };
                        Label lbl_time = new Label { Text = row["added_at"].ToString(), AutoSize = true, Font = new Font("Arial", 10) };

                        Label lbl_locationHead = new Label { Text = "Location: ", Font = new Font("Arial", 10, FontStyle.Bold), AutoSize = true };
                        Label lbl_location = new Label { Text = row["scene_location"].ToString(), AutoSize = true, Font = new Font("Arial", 10),
                            MaximumSize = new Size(sceneCard.Width - 150, 0)
                        };

                        Label lbl_newline = new Label { Text = "\n"};

                        Label lbl_descHead = new Label { Text = "Description: ", Font = new Font("Arial", 10, FontStyle.Bold) };
                        Label lbl_desc = new Label
                        {
                            Text = row["scene_description"].ToString(),
                            MaximumSize = new Size(sceneCard.Width - 150, 0),
                            AutoSize = true,
                            Font = new Font("Arial", 10)
                        };

                        // Media Viewer Panel
                        Panel viewerPanel = new Panel
                        {
                            Location = new Point(470, 8),
                            Size = new Size(sceneCard.Width / 2 - 50, sceneCard.Height / 2),
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
                        lbl_id.Location = new Point(lbl_idHead.Right + 50, 10);

                        lbl_typeHead.Location = new Point(left, 40);
                        lbl_type.Location = new Point(lbl_typeHead.Right + 50, 40);

                        lbl_timehead.Location = new Point(left, 70);
                        lbl_time.Location = new Point(lbl_timehead.Right + 50, 70);

                        lbl_locationHead.Location = new Point(left, 240);
                        lbl_location.Location = new Point(lbl_locationHead.Right + 50, 240);

                        lbl_descHead.Location = new Point(left, 280);
                        lbl_desc.Location = new Point(lbl_descHead.Right + 50, 280);


                        // Final assembly
                        sceneCard.Controls.AddRange(new Control[]
                        {
                    lbl_idHead, lbl_id,
                        lbl_typeHead, lbl_type,
                        lbl_timehead, lbl_time,
                        lbl_locationHead, lbl_location,
                        lbl_descHead, lbl_desc,
                        viewerPanel
                        });



                        flp_scenes.Controls.Add(sceneCard);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching crime scenes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
