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
    public partial class ViewLeads : Form
    {
        public string SelectedCaseId { get; set; }
        public ViewLeads()
        {
            InitializeComponent();
            this.Load += new EventHandler(ViewLeads_Load);

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

        private void ViewLeads_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update
            LoadLeads();
        }

        private void LoadLeads()
        {
            flp_leads.Controls.Clear();

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();

                string query = @"
        SELECT lead_id, submitted_by, statement, submitted_at, description
        FROM Leads
        WHERE case_id = @caseId AND status = 'Verified'";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@caseId", SelectedCaseId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    int leadId = Convert.ToInt32(reader["lead_id"]);
                    int currentLeadId = leadId;  // Scoped copy for lambda

                    string submittedBy = reader["submitted_by"].ToString();
                    string statement = reader["statement"] == DBNull.Value ? " " : reader["statement"].ToString();
                    string submittedAt = Convert.ToDateTime(reader["submitted_at"]).ToString("f");
                    string description = reader["description"] == DBNull.Value ? " " : reader["description"].ToString();


                    // === Card Panel ===
                    Panel card = new Panel();
                    card.BackColor = Color.White;
                    card.Width = flp_leads.Width - 40;
                    card.Height = 260;
                    card.Margin = new Padding(10);
                    card.BorderStyle = BorderStyle.FixedSingle;

                    // === Submitted By ===
                    Label lbl1 = new Label();
                    lbl1.Text = "Submitted By:";
                    lbl1.Font = new Font("Arial", 10, FontStyle.Bold);
                    lbl1.ForeColor = Color.Black;
                    lbl1.Location = new Point(10, 10);
                    lbl1.AutoSize = true;

                    Label val1 = new Label();
                    val1.Text = submittedBy;
                    val1.Font = new Font("Arial", 10, FontStyle.Regular);
                    val1.ForeColor = Color.Black;
                    val1.Location = new Point(120, 10);
                    val1.AutoSize = true;

                    // === Submitted At ===
                    Label lbl2 = new Label();
                    lbl2.Text = "Submitted At:";
                    lbl2.Font = new Font("Arial", 10, FontStyle.Bold);
                    lbl2.ForeColor = Color.Black;
                    lbl2.Location = new Point(10, 35);
                    lbl2.AutoSize = true;

                    Label val2 = new Label();
                    val2.Text = submittedAt;
                    val2.Font = new Font("Arial", 10, FontStyle.Regular);
                    val2.ForeColor = Color.Black;
                    val2.Location = new Point(120, 35);
                    val2.AutoSize = true;

                    // === Statement ===
                    Label lbl3 = new Label();
                    lbl3.Text = "Statement Of Lead:";
                    lbl3.Font = new Font("Arial", 10, FontStyle.Bold);
                    lbl3.ForeColor = Color.Black;
                    lbl3.Location = new Point(10, 60);
                    lbl3.AutoSize = true;

                    Label val3 = new Label();
                    val3.Text = statement;
                    val3.Font = new Font("Arial", 10, FontStyle.Regular);
                    val3.ForeColor = Color.Black;
                    val3.Location = new Point(30, 85);
                    val3.MaximumSize = new Size(card.Width - 20, 60);
                    val3.AutoSize = true;

                    // === Description Label (closer to statement) ===
                    Label lbl_desc = new Label();
                    lbl_desc.Text = "Enter Description:";
                    lbl_desc.Font = new Font("Arial", 10, FontStyle.Bold);
                    lbl_desc.ForeColor = Color.Black;
                    lbl_desc.Location = new Point(10, val3.Bottom + 10);
                    lbl_desc.AutoSize = true;

                    Label Desc = new Label();
                    Desc.Text = description;
                    Desc.Font = new Font("Arial", 10, FontStyle.Regular);
                    Desc.ForeColor = Color.Black;
                    Desc.Location = new Point(30, lbl_desc.Bottom + 5);
                    Desc.MaximumSize = new Size(card.Width - 20, 135);
                    Desc.AutoSize = true;

                    // === Add to Card ===
                    card.Controls.Add(lbl1);
                    card.Controls.Add(val1);
                    card.Controls.Add(lbl2);
                    card.Controls.Add(val2);
                    card.Controls.Add(lbl3);
                    card.Controls.Add(val3);
                    card.Controls.Add(lbl_desc);
                    card.Controls.Add(Desc);

                    flp_leads.Controls.Add(card);
                }

                reader.Close();

                if (flp_leads.Controls.Count == 0)
                {
                    Label noLeadsLabel = new Label();
                    noLeadsLabel.Text = "No Verified Leads Found";
                    noLeadsLabel.Font = new Font("Arial", 14, FontStyle.Bold);
                    noLeadsLabel.ForeColor = Color.Gray;
                    noLeadsLabel.TextAlign = ContentAlignment.MiddleCenter;
                    noLeadsLabel.Dock = DockStyle.Fill;

                    Panel containerPanel = new Panel();
                    containerPanel.Width = flp_leads.Width;
                    containerPanel.Height = flp_leads.Height;
                    containerPanel.Controls.Add(noLeadsLabel);
                    noLeadsLabel.Location = new Point(
                        (containerPanel.Width - noLeadsLabel.PreferredWidth) / 2,
                        (containerPanel.Height - noLeadsLabel.PreferredHeight) / 2
                    );

                    flp_leads.Controls.Add(containerPanel);
                }
            }
        }

        private void btn_loadevd_Click(object sender, EventArgs e)
        {
            LoadLeads();
        }
    }
}
