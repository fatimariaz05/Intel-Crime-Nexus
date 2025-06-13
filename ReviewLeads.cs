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
    public partial class ReviewLeads : Form
    {
        public string SelectedCaseId { get; set; }
        public ReviewLeads()
        {
            InitializeComponent();
            this.Load += new EventHandler(ReviewLeads_Load);

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
        private void ReviewLeads_Load(object sender, EventArgs e)
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
        SELECT lead_id, submitted_by, statement, submitted_at
        FROM Leads
        WHERE case_id = @caseId AND status = 'Pending'";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@caseId", SelectedCaseId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    int leadId = Convert.ToInt32(reader["lead_id"]);
                    int currentLeadId = leadId;  // Scoped copy for lambda

                    string submittedBy = reader["submitted_by"].ToString();
                    string statement = reader["statement"].ToString();
                    string submittedAt = Convert.ToDateTime(reader["submitted_at"]).ToString("f");

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
                    lbl_desc.Location = new Point(10, val3.Bottom + 25);
                    lbl_desc.AutoSize = true;

                    // === TextBox ===
                    var txb_officerDesc = new Siticone.Desktop.UI.WinForms.SiticoneTextBox();
                    txb_officerDesc.PlaceholderText = "Write something for reference...";
                    txb_officerDesc.Font = new Font("Arial", 10, FontStyle.Regular);
                    txb_officerDesc.ForeColor = Color.Black;
                    txb_officerDesc.Width = card.Width - 20;
                    txb_officerDesc.Location = new Point(10, lbl_desc.Bottom + 5);

                    // === Buttons ===
                    Button btn_verify = new Button();
                    btn_verify.Text = "Verify";
                    btn_verify.BackColor = Color.LightGreen;
                    btn_verify.ForeColor = Color.Black;
                    btn_verify.Font = new Font("Arial", 9, FontStyle.Regular);
                    btn_verify.FlatStyle = FlatStyle.Flat;
                    btn_verify.FlatAppearance.BorderSize = 0;
                    btn_verify.Size = new Size(80, 30);
                    btn_verify.Location = new Point(card.Width - 180, card.Height - 50);

                    btn_verify.Click += (s, e) =>
                    {
                        string officerDesc = txb_officerDesc.Text.Trim();
                        string officerId = GetOfficerIdByUsername(UserSession.UserName);

                        if (officerId != null)
                            UpdateLeadStatus(currentLeadId, "Verified", officerDesc, officerId);
                        else
                            MessageBox.Show("Officer not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LoadLeads();  // Refresh the list
                    };

                    Button btn_remove = new Button();
                    btn_remove.Text = "Remove";
                    btn_remove.BackColor = Color.LightCoral;
                    btn_remove.ForeColor = Color.Black;
                    btn_remove.Font = new Font("Arial", 9, FontStyle.Regular);
                    btn_remove.FlatStyle = FlatStyle.Flat;
                    btn_remove.FlatAppearance.BorderSize = 0;
                    btn_remove.Size = new Size(80, 30);
                    btn_remove.Location = new Point(card.Width - 90, card.Height - 50);

                    btn_remove.Click += (s, e) =>
                    {
                        string officerDesc = txb_officerDesc.Text.Trim();
                        string officerId = GetOfficerIdByUsername(UserSession.UserName);

                        if (officerId != null)
                            UpdateLeadStatus(currentLeadId, "Not Verified", officerDesc, officerId);
                        else
                            MessageBox.Show("Officer not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LoadLeads();  // Refresh the list
                    };

                    // === Add to Card ===
                    card.Controls.Add(lbl1);
                    card.Controls.Add(val1);
                    card.Controls.Add(lbl2);
                    card.Controls.Add(val2);
                    card.Controls.Add(lbl3);
                    card.Controls.Add(val3);
                    card.Controls.Add(lbl_desc);
                    card.Controls.Add(txb_officerDesc);
                    card.Controls.Add(btn_verify);
                    card.Controls.Add(btn_remove);

                    flp_leads.Controls.Add(card);
                }

                reader.Close();

                if (flp_leads.Controls.Count == 0)
                {
                    Label noLeadsLabel = new Label();
                    noLeadsLabel.Text = "No Submitted Leads Found";
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

        private string GetOfficerIdByUsername(string username)
        {
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = "SELECT officer_id FROM Officer WHERE username = @username";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();  // returns null if not found
                }
            }
        }

        private void UpdateLeadStatus(int leadId, string status, string description, string officerId)
        {
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = @"
        UPDATE Leads
        SET status = @status,
            description = @description,
            verified_by = @officerId
        WHERE lead_id = @leadId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(description) ? DBNull.Value : (object)description);
                    cmd.Parameters.AddWithValue("@officerId", officerId);
                    cmd.Parameters.AddWithValue("@leadId", leadId);

                    cmd.ExecuteNonQuery();
                }
            }

            // Show success message
            if (status == "Verified")
            {
                MessageBox.Show("Lead is Verified", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (status == "Not Verified")
            {
                MessageBox.Show("Lead is declared as Not Verified", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_loadleads_Click(object sender, EventArgs e)
        {
            LoadLeads();
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void siticoneButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_updateProfile_Click(object sender, EventArgs e)
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
    }
}
