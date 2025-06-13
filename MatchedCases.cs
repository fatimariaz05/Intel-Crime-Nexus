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
    public partial class MatchedCases : Form
    {
        string caseId, option1, option2;
        public MatchedCases(string caseId, string option1, string option2)
        {
            InitializeComponent();

            this.caseId = caseId;
            this.option1 = option1;
            this.option2 = option2;

            this.Load += new EventHandler(MatchedCases_Load);

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

        private void MatchedCases_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            const1.Text = option1;
            const2.Text = option2;

            LoadMatchedCases();
        }

        private void LoadMatchedCases()
        {
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();

                // FLOW 1 PANEL
                if (option1 != null)
                {
                    string query1 = BuildCompleteQuery(option1);
                    FillFlowPanel(flow1, query1, option1);
                }

                // FLOW 2 PANEL
                if (option2 != null)
                {
                    string query2 = BuildCompleteQuery(option2);
                    FillFlowPanel(flow2, query2, option2);
                }
            }
        }

        private string BuildCompleteQuery(string option)
        {
            // Start of the complete query
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
                c.detailed_description
            FROM Cases c
            WHERE c.case_id <> @caseId ";

            // Append the condition based on the option
            switch (option)
            {
                case "Crime Type":
                    query += @"
                    AND c.crime_category_id = (
                        SELECT crime_category_id 
                        FROM Cases 
                        WHERE case_id = @caseId
                    )";
                    break;

                case "Time Duration":
                    query += @"
                    AND ABS(DATEDIFF(DAY, c.crime_time, (SELECT crime_time FROM Cases WHERE case_id = @caseId))) <= 7";
                    break;

                case "Location":
                    query += @"
                    AND EXISTS (
                        SELECT 1 
                        FROM Scope_Cities sc
                        WHERE c.crime_location LIKE '%' + sc.City + '%' 
                        AND (SELECT crime_location FROM Cases WHERE case_id = @caseId) LIKE '%' + sc.City + '%'
                    )";
                    break;

                case "Victim Count":
                    query += @"
                    AND c.victim_count = (SELECT victim_count FROM Cases WHERE case_id = @caseId)";
                    break;

                case "Description Keywords":
                    query += @"
                    AND c.detailed_description IS NOT NULL
                    AND (SELECT detailed_description FROM Cases WHERE case_id = @caseId) IS NOT NULL
                    AND (
                        SELECT COUNT(*) 
                        FROM STRING_SPLIT(LOWER(CAST(c.detailed_description AS VARCHAR(MAX))), ' ') AS descWord
                        WHERE LEN(descWord.value) > 2
                          AND descWord.value NOT IN (
                              'this', 'that', 'what', 'in', 'of', 'and', 'is', 'are', 'on', 'the',
                              'with', 'a', 'an', 'to', 'for', 'at', 'by', 'as'
                          )
                          AND EXISTS (
                              SELECT 1 
                              FROM STRING_SPLIT(LOWER(CAST((SELECT detailed_description FROM Cases WHERE case_id = @caseId) AS VARCHAR(MAX))), ' ') AS word
                              WHERE LEN(word.value) > 2
                                AND word.value NOT IN (
                                    'this', 'that', 'what', 'in', 'of', 'and', 'is', 'are', 'on', 'the',
                                    'with', 'a', 'an', 'to', 'for', 'at', 'by', 'as'
                                )
                                AND word.value = descWord.value
                          )
                    ) >= (
                        SELECT CEILING(COUNT(*) / 3.0)  -- Keep the one-third threshold for matching
                        FROM STRING_SPLIT(LOWER(CAST((SELECT detailed_description FROM Cases WHERE case_id = @caseId) AS VARCHAR(MAX))), ' ') AS word
                        WHERE LEN(word.value) > 2
                          AND word.value NOT IN (
                              'this', 'that', 'what', 'in', 'of', 'and', 'is', 'are', 'on', 'the',
                              'with', 'a', 'an', 'to', 'for', 'at', 'by', 'as'
                          )
                    )";
                    break;






                default:
                    query += "AND 1=0"; // fallback to false if unknown option
                    break;
            }

            return query;
        }

        private void FillFlowPanel(FlowLayoutPanel panel, string query, string option)
        {
            panel.Controls.Clear();

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@caseId", caseId);
                SqlDataReader reader = cmd.ExecuteReader();

                // Check if any rows are returned
                if (!reader.HasRows)
                {
                    // No rows found, display "No Similar Cases Found" in the middle of the panel
                
                }
                else
                {
                    while (reader.Read())
                    {
                        Panel card = new Panel
                        {
                            Width = panel.ClientSize.Width - 25,
                            BorderStyle = BorderStyle.FixedSingle,
                            Margin = new Padding(5),
                            AutoScroll = true,
                            AutoSize = false,
                            Height = 10,
                            BackColor = Color.White
                        };

                        int yOffset = 10;

                        AddField(card, "Case No:", reader["case_no"]?.ToString() ?? "", ref yOffset, 1);
                        AddField(card, "Crime Type:", reader["crime_type"]?.ToString() ?? "", ref yOffset, 0);
                        AddField(card, "Crime Location:", reader["crime_location"]?.ToString() ?? "", ref yOffset, 0);
                        string time = reader["crime_time"] != DBNull.Value ? Convert.ToDateTime(reader["crime_time"]).ToString("yyyy-MM-dd HH:mm") : "";
                        AddField(card, "Crime Time:", time, ref yOffset, 0);
                        AddField(card, "Victim Count:", reader["victim_count"]?.ToString() ?? "", ref yOffset, 0);
                        AddField(card, "Status:", reader["status"]?.ToString() ?? "", ref yOffset, 0);
                        AddField(card, "Created At:", reader["created_at"]?.ToString() ?? "", ref yOffset, 0);
                        AddField(card, "Details:", reader["detailed_description"]?.ToString() ?? "", ref yOffset, 0);

                        card.Height = yOffset + 10;
                        panel.Controls.Add(card);
                    }
                }
            }
        }


        private void AddField(Control parent, string label, string value, ref int yOffset, int flag)
        {
            // Determine the font style based on the flag value
            FontStyle fontStyle = (flag == 1) ? FontStyle.Bold : FontStyle.Regular;

            // Create the label for the field
            Label lbl = new Label
            {
                Text = $"{label} {value}",
                Location = new Point(10, yOffset),
                AutoSize = false,  // Ensure we manually control the size
                Width = parent.Width - 20, // Set the width to match the parent panel width
                MaximumSize = new Size(parent.Width - 20, 0),  // Max width to prevent overflow
                TextAlign = ContentAlignment.TopLeft,
                AutoEllipsis = true,  // Optional, use to show ellipsis for very long text
                Font = new Font("Arial", 10, fontStyle) // Set font to Arial 10, Bold or Regular
            };

            lbl.AutoSize = true; // Allow label to resize to the text
            lbl.MaximumSize = new Size(parent.Width - 20, 0); // This is what will trigger the text to wrap

            parent.Controls.Add(lbl);
            yOffset += lbl.Height + 8;  // Add some space after the label for better separation
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
