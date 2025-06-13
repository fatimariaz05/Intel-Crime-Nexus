using CISystem;
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

    public partial class AssignOfficer: Form
    {
        private int selectedCaseId = 0; // Add this at the class level

        public AssignOfficer()
        {
            InitializeComponent();
            this.Load += new EventHandler(AssignOfficer_Load);

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

        private void AssignOfficer_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update


            LoadCases();
        }


        private void LoadCases()
        {
            flp_cases.Controls.Clear(); // Clear existing cards

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();

                string query = @"
                SELECT 
                    c.case_id,
                    c.case_no, 
                    c.crime_type, 
                    c.crime_location, 
                    c.crime_time, 
                    c.victim_count, 
                    c.status, 
                    c.created_at, 
                    r.incident_details,
                    c.detailed_description,
                    f.fir_details, 
                    f.filed_by,
                    o.name AS officer_name
                FROM Cases c
                LEFT JOIN Reports r ON r.report_id = c.report_id
                LEFT JOIN FIR f ON f.fir_id = c.fir_id
                LEFT JOIN Officer o ON o.officer_id = f.filed_by
                WHERE 
                    c.status = 'Under Investigation'
                    AND EXISTS (
                        SELECT 1 
                        FROM Scope_Cities sc
                        WHERE 
                            (SELECT police_station 
                             FROM Officer 
                             WHERE username = @username) LIKE '%' + sc.City + '%'
                            AND c.crime_location LIKE '%' + sc.City + '%'
                    )
                ";


                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", UserSession.UserName);

                SqlDataReader reader = cmd.ExecuteReader();
                bool hasCases = false;

                while (reader.Read())
                {
                    hasCases = true;

                    Panel card = new Panel
                    {
                        Width = flp_cases.ClientSize.Width - 25,
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(5),
                        AutoScroll = true,
                        AutoSize = false,
                        Height = 10,
                        BackColor = Color.White
                    };

                    int yOffset = 10;

                    AddField(card, "Case No:", reader["case_no"]?.ToString() ?? "", ref yOffset);
                    AddField(card, "Crime Type:", reader["crime_type"]?.ToString() ?? "", ref yOffset);
                    AddField(card, "Crime Location:", reader["crime_location"]?.ToString() ?? "", ref yOffset);

                    string crimeTime = reader["crime_time"] != DBNull.Value
                        ? Convert.ToDateTime(reader["crime_time"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                    AddField(card, "Crime Time:", crimeTime, ref yOffset);

                    AddField(card, "Victim Count:", reader["victim_count"]?.ToString() ?? "", ref yOffset);
                    AddField(card, "Status:", reader["status"]?.ToString() ?? "", ref yOffset);

                    string createdAt = reader["created_at"] != DBNull.Value
                        ? Convert.ToDateTime(reader["created_at"]).ToString("yyyy-MM-dd HH:mm:ss") : "";
                    AddField(card, "Created At:", createdAt, ref yOffset);

                    AddField(card, "FIR Details:", reader["fir_details"]?.ToString() ?? "", ref yOffset);
                    string incidentDetails = reader["incident_details"]?.ToString();
                    string caseDescription = reader["detailed_description"]?.ToString();

                    string heading = !string.IsNullOrWhiteSpace(incidentDetails) ? "Incident Details:" : "Case Description:";
                    string data = !string.IsNullOrWhiteSpace(incidentDetails) ? incidentDetails : caseDescription ?? "";

                    AddField(card, heading, data, ref yOffset);


                    AddField(card, "FIR Filed by:", reader["officer_name"]?.ToString() ?? "", ref yOffset);

                    // Fetch case_id (integer)
                    int caseId = Convert.ToInt32(reader["case_id"]);

                    Button btnSelect = new Button
                    {
                        Text = "Select Case",
                        Width = 100,
                        Height = 30,
                        Left = card.Width - 115,
                        Top = yOffset,
                        Font = new Font("Arial", 9F, FontStyle.Bold),
                        BackColor = Color.FromArgb(164, 145, 60),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Tag = caseId  // Storing case_id here
                    };
                    btnSelect.FlatAppearance.BorderSize = 0;

                    btnSelect.Click += (s, e) =>
                    {
                        var existingPanel = this.Controls.Find("flp_officers", true).FirstOrDefault();
                        if (existingPanel != null)
                            this.Controls.Remove(existingPanel);

                        selectedCaseId = caseId; // Store the selected case_id

                        FlowLayoutPanel flp_officers = new FlowLayoutPanel
                        {
                            Name = "flp_officers",
                            Width = flp_cases.Width - 50,
                            Height = flp_cases.Height,
                            Left = flp_cases.Right + 10,
                            Top = flp_cases.Top,
                            Location = new Point(765, 92),
                            AutoScroll = true,
                            BackColor = Color.Purple,
                            FlowDirection = FlowDirection.TopDown,
                            WrapContents = false
                        };
                        this.Controls.Add(flp_officers);
                        flp_officers.BringToFront();

                        using (SqlConnection innerConn = new SqlConnection(DB.connectionString))
                        {
                            innerConn.Open();

                            string innerQuery = @"
                            SELECT 
                                o.officer_id, o.name, o.gender, o.designation, o.police_station, o.rank,
                                (SELECT COUNT(*) FROM Case_Assignment ca WHERE ca.officer_id = o.officer_id AND ca.status = 'Active') AS OngoingCases,
                                (SELECT COUNT(*) FROM Case_Assignment ca WHERE ca.officer_id = o.officer_id AND ca.status = 'Closed') AS SolvedCases,
                                (SELECT COUNT(*) FROM Case_Assignment ca WHERE ca.officer_id = o.officer_id AND ca.status = 'Removed') AS RemovedCases
                            FROM Officer o
                            WHERE EXISTS (
                                SELECT 1
                                FROM Scope_Cities sc
                                WHERE 
                                    (SELECT police_station 
                                     FROM Officer 
                                     WHERE username = @username) LIKE '%' + sc.City + '%'
                                    AND o.police_station LIKE '%' + sc.City + '%'
                            )";

                            SqlCommand innerCmd = new SqlCommand(innerQuery, innerConn);
                            innerCmd.Parameters.AddWithValue("@username", UserSession.UserName);

                            SqlDataReader innerReader = innerCmd.ExecuteReader();
                            bool hasOfficers = false;

                            while (innerReader.Read())
                            {
                                hasOfficers = true;
                                Panel officerCard = new Panel
                                {
                                    Width = flp_officers.Width - 30, // Fixed width with margin
                                    BorderStyle = BorderStyle.FixedSingle,
                                    Margin = new Padding(5),
                                    AutoSize = true,
                                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                                    BackColor = Color.White
                                };

                                int oy = 10;
                                AddField(officerCard, "Name:", innerReader["name"]?.ToString() ?? "", ref oy);
                                AddField(officerCard, "Gender:", innerReader["gender"]?.ToString() ?? "", ref oy);
                                AddField(officerCard, "Designation:", innerReader["designation"]?.ToString() ?? "", ref oy);
                                AddField(officerCard, "Police Station:", innerReader["police_station"]?.ToString() ?? "", ref oy);
                                AddField(officerCard, "Rank:", innerReader["rank"]?.ToString() ?? "", ref oy);

                                // Case Stats
                                AddField(officerCard, "Ongoing Cases:", innerReader["OngoingCases"].ToString(), ref oy);
                                AddField(officerCard, "Solved Cases:", innerReader["SolvedCases"].ToString(), ref oy);
                                AddField(officerCard, "Removed Cases:", innerReader["RemovedCases"].ToString(), ref oy);

                                int officerId = Convert.ToInt32(innerReader["officer_id"]);

                                Label lblRole = new Label
                                {
                                    Text = "Enter Officer Role:",
                                    Font = new Font("Arial", 9F, FontStyle.Bold),
                                    AutoSize = true,
                                    Top = oy + 10,
                                    Left = 10
                                };
                                officerCard.Controls.Add(lblRole);

                                Siticone.Desktop.UI.WinForms.SiticoneTextBox txtOfficerRole = new Siticone.Desktop.UI.WinForms.SiticoneTextBox
                                {
                                    Width = officerCard.Width - 40,
                                    Height = 30,
                                    Top = lblRole.Bottom + 5,
                                    Left = 10,
                                    BorderColor = Color.Black,
                                    BorderThickness = 1,
                                    PlaceholderText = "e.g. Lead Investigator",
                                    Name = "txtOfficerRole"  // Set the Name property
                                };

                                officerCard.Controls.Add(txtOfficerRole);

                                // Update oy to continue placing controls below
                                oy = txtOfficerRole.Bottom + 10;


                                Button btnAssign = new Button
                                {
                                    Text = "Assign",
                                    Width = 80,
                                    Height = 25,
                                    Font = new Font("Arial", 9F, FontStyle.Bold),
                                    BackColor = Color.FromArgb(153, 41, 161),
                                    ForeColor = Color.White,
                                    FlatStyle = FlatStyle.Flat
                                };
                                btnAssign.FlatAppearance.BorderSize = 0;

                                btnAssign.Click += (assignSender, assignArgs) =>
                                {
                                    // 1. Get role from the TextBox on the card
                                    var roleTextbox = officerCard.Controls.OfType<Siticone.Desktop.UI.WinForms.SiticoneTextBox>()
                                        .FirstOrDefault(tb => tb.Name == "txtOfficerRole");

                                    string officerRole = roleTextbox?.Text.Trim();

                                    if (string.IsNullOrWhiteSpace(officerRole))
                                    {
                                        MessageBox.Show("Please enter the officer's role before assigning.", "Missing Role", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        return;
                                    }

                                    using (SqlConnection conn1 = new SqlConnection(DB.connectionString))
                                    {
                                        conn1.Open();

                                        // 2. Check if the officer is already assigned to the case with 'ACTIVE' status
                                        SqlCommand checkAssignedCmd = new SqlCommand(@"
                                        SELECT COUNT(*)
                                        FROM Case_Assignment
                                        WHERE case_id = @case_id
                                          AND officer_id = @officer_id
                                          AND status = 'Active'", conn1);

                                        checkAssignedCmd.Parameters.AddWithValue("@case_id", selectedCaseId);
                                        checkAssignedCmd.Parameters.AddWithValue("@officer_id", officerId);

                                        int existingAssignments = (int)checkAssignedCmd.ExecuteScalar();

                                        if (existingAssignments > 0)
                                        {
                                            MessageBox.Show("This officer is already assigned to this case.", "Assignment Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            return;
                                        }

                                        // 3. Get new assignment_id
                                        SqlCommand getMaxIdCmd = new SqlCommand("SELECT ISNULL(MAX(assignment_id), 0) + 1 FROM Case_Assignment", conn1);
                                        int newAssignmentId = (int)getMaxIdCmd.ExecuteScalar();

                                        // 4. Get assigned_by id using session
                                        SqlCommand getAssignedByCmd = new SqlCommand("SELECT officer_id FROM Officer WHERE username = @username", conn1);
                                        getAssignedByCmd.Parameters.AddWithValue("@username", UserSession.UserName);
                                        int assignedById = (int?)getAssignedByCmd.ExecuteScalar() ?? -1;

                                        if (assignedById == -1)
                                        {
                                            MessageBox.Show("Logged-in officer not found!", "Session Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }

                                        // 5. Insert with officer role
                                        SqlCommand insertCmd = new SqlCommand(@"
                                        INSERT INTO Case_Assignment (
                                            assignment_id, case_id, officer_id, assigned_by, date_assigned, status, assigned_officer_role
                                        )
                                        VALUES (
                                            @assignment_id, @case_id, @officer_id, @assigned_by, @date_assigned, @status, @assigned_officer_role
                                        )", conn1);

                                        insertCmd.Parameters.AddWithValue("@assignment_id", newAssignmentId);
                                        insertCmd.Parameters.AddWithValue("@case_id", selectedCaseId);
                                        insertCmd.Parameters.AddWithValue("@officer_id", officerId);
                                        insertCmd.Parameters.AddWithValue("@assigned_by", assignedById);
                                        insertCmd.Parameters.AddWithValue("@date_assigned", DateTime.Now);
                                        insertCmd.Parameters.AddWithValue("@status", "Active");
                                        insertCmd.Parameters.AddWithValue("@assigned_officer_role", officerRole);

                                        insertCmd.ExecuteNonQuery();
                                        MessageBox.Show("Case successfully assigned.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                };


                                Button btnRemove = new Button
                                {
                                    Text = "Remove",
                                    Width = 80,
                                    Height = 25,
                                    Font = new Font("Arial", 9F, FontStyle.Bold),
                                    BackColor = Color.IndianRed,
                                    ForeColor = Color.White,
                                    FlatStyle = FlatStyle.Flat
                                };
                                btnRemove.FlatAppearance.BorderSize = 0;

                                btnRemove.Click += (removeSender, removeArgs) =>
                                {
                                    // Remove assignment logic
                                    using (SqlConnection conn1 = new SqlConnection(DB.connectionString))
                                    {
                                        conn1.Open();

                                        // Check if the officer is assigned to the case
                                        SqlCommand checkAssignedCmd = new SqlCommand(@"
                                        SELECT COUNT(*)
                                        FROM Case_Assignment
                                        WHERE case_id = @case_id
                                          AND officer_id = @officer_id
                                          AND status = 'Active'", conn1);

                                        checkAssignedCmd.Parameters.AddWithValue("@case_id", selectedCaseId); // Use selected case_id
                                        checkAssignedCmd.Parameters.AddWithValue("@officer_id", officerId);   // Use officer_id

                                        int isAssigned = (int)checkAssignedCmd.ExecuteScalar();

                                        if (isAssigned > 0)
                                        {
                                            // If the officer is assigned to the case, proceed with removal
                                            SqlCommand removeCmd = new SqlCommand(@"
                                            UPDATE Case_Assignment
                                            SET status = 'Removed'
                                            WHERE case_id = @case_id
                                              AND officer_id = @officer_id", conn1);

                                            removeCmd.Parameters.AddWithValue("@case_id", selectedCaseId); // Use selected case_id
                                            removeCmd.Parameters.AddWithValue("@officer_id", officerId);   // Use officer_id

                                            int rowsAffected = removeCmd.ExecuteNonQuery();
                                            if (rowsAffected > 0)
                                            {
                                                MessageBox.Show("Officer removed from the case successfully.", "Removal Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                            else
                                            {
                                                MessageBox.Show("Failed to remove the officer from the case.", "Removal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                        else
                                        {
                                            // If the officer is not assigned to the case, show a message
                                            MessageBox.Show("This officer is not assigned to the selected case.", "Assignment Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }
                                    }


                                };
                                officerCard.Controls.Add(btnAssign);
                                officerCard.Controls.Add(btnRemove);

                                officerCard.Height = oy + btnAssign.Height + 20;

                                // Set position for Assign button
                                btnAssign.Left = officerCard.Width - btnAssign.Width - btnRemove.Width - 20; // 10px gap between buttons
                                btnAssign.Top = officerCard.Height - btnAssign.Height - 10;

                                // Set position for Remove button (to the right of Assign)
                                btnRemove.Left = officerCard.Width - btnRemove.Width - 10;
                                btnRemove.Top = btnAssign.Top; // same vertical alignment


                                flp_officers.Controls.Add(officerCard);
                            }

                            if (!hasOfficers)
                            {
                                Label noOfficersLabel = new Label
                                {
                                    Text = "No Officers Found",
                                    ForeColor = Color.Black,
                                    Font = new Font("Arial", 12F, FontStyle.Bold),
                                    AutoSize = true
                                };
                                // Center the label in flp_officers
                                noOfficersLabel.Location = new Point(
                                    (flp_officers.Width - noOfficersLabel.Width) / 2,
                                    flp_officers.Height / 2
                                );

                                flp_officers.Controls.Add(noOfficersLabel);
                            }

                            innerReader.Close();
                        }
                    };

                    card.Controls.Add(btnSelect);
                    card.Height = yOffset + btnSelect.Height + 20;

                    flp_cases.Controls.Add(card);

                }
                if (!hasCases)
                {
                    Label noCasesLabel = new Label
                    {
                        Text = "No Under Investigation Cases Found",
                        ForeColor = Color.Black,
                        Font = new Font("Arial", 9F, FontStyle.Bold),
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    // Wrap in a Panel to center it in the FlowLayoutPanel
                    Panel wrapperPanel = new Panel
                    {
                        Width = flp_cases.Width - 10,
                        Height = flp_cases.Height - 50,
                        BackColor = flp_cases.BackColor
                    };

                    noCasesLabel.Location = new Point(
                        (wrapperPanel.Width - noCasesLabel.Width) / 2 -50,
                        (wrapperPanel.Height - noCasesLabel.Height) / 2
                    );

                    wrapperPanel.Controls.Add(noCasesLabel);
                    flp_cases.Controls.Add(wrapperPanel);
                }

                reader.Close();
            }
        }











        private void AddField(Panel card, string heading, string data, ref int yOffset)
        {
            Label lblHeading = CreateHeadingLabel(heading, yOffset, card.Width);
            card.Controls.Add(lblHeading);
            yOffset += lblHeading.PreferredHeight;

            Label lblData = CreateDataLabel(data, yOffset, card.Width);
            card.Controls.Add(lblData);
            yOffset += lblData.PreferredHeight + 5; // Space after each pair
        }

        private Label CreateHeadingLabel(string text, int top, int width)
        {
            return new Label
            {
                Text = text,
                Left = 10,
                Top = top,
                Width = width - 40,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = true,
                MaximumSize = new Size(width - 40, 0),
                TextAlign = ContentAlignment.TopLeft
            };
        }

        private Label CreateDataLabel(string text, int top, int width)
        {
            return new Label
            {
                Text = text,
                Left = 20,
                Top = top,
                Width = width - 60,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                AutoSize = true,
                MaximumSize = new Size(width - 60, 0),
                TextAlign = ContentAlignment.TopLeft
            };
        }





        private void flp_cases_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnLoadReports_Click(object sender, EventArgs e)
        {
            LoadCases();
        }

        private void btn_updateProfile_Click(object sender, EventArgs e)
        {
            this.Hide();

            UpdProfileO updateProfileForm = new UpdProfileO();

            updateProfileForm.ShowDialog();

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

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerDash dashboard = new OfficerDash();
            dashboard.ShowDialog();
            this.Close();
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerCases AssignedCases = new OfficerCases();
            AssignedCases.ShowDialog();
            this.Close();
        }
    }
}

