using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;

namespace CISystem
{
    public partial class DisplayCasesO : Form
    {
        private string crime;
        private string city;
        private DateTime? weekStart;
        private string location;
        private int? victimCount;
        private string status;
        private string caseTitle;
        private string caseno;
        public DisplayCasesO(string crime, string city, DateTime? weekStart, string location, int? victimCount, string status, string caseTitle, string caseno)
        {
            InitializeComponent();
            this.Load += new EventHandler(DisplayCasesO_Load); // Ensure Load event is attached

            // Save values for use
            this.crime = crime;
            this.city = city;
            this.weekStart = weekStart;
            this.location = location;
            this.victimCount = victimCount;
            this.status = status;
            this.caseTitle = caseTitle;
            this.caseno = caseno;

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

        //Load Event: Display the Logged-in User's Name

        private void DisplayCasesO_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update

            LoadFilteredCases();
        }


        private void LoadFilteredCases()
        {
            flp_cases.Controls.Clear();

            using (SqlConnection con = new SqlConnection("Data Source=PC-MAHNUR\\SQLEXPRESS;Initial Catalog=CI;Integrated Security=True;Encrypt=False"))
            {
                con.Open();

                List<string> scoreParts = new List<string>();
                List<SqlParameter> parameters = new List<SqlParameter>();

                string isCaseMatchClause = "0 AS IsCaseMatch"; // default if caseNo is empty
                string baseQuery;

                string whereClause = "";
                if (!string.IsNullOrEmpty(crime))
                {
                    whereClause = " WHERE crime_category_id IN (SELECT crime_catg_id FROM Crime_Categories WHERE crime_catg = @crime)";
                    parameters.Add(new SqlParameter("@crime", crime));
                }
                else
                {
                    return; // required filter
                }

                if (!string.IsNullOrEmpty(city))
                {
                    scoreParts.Add("CASE WHEN crime_location LIKE @city THEN 1 ELSE 0 END");
                    parameters.Add(new SqlParameter("@city", "%" + city + "%"));
                }

                if (!string.IsNullOrEmpty(location))
                {
                    foreach (var word in location.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string paramName = "@loc_" + word;
                        scoreParts.Add($"CASE WHEN crime_location LIKE {paramName} THEN 1 ELSE 0 END");
                        parameters.Add(new SqlParameter(paramName, "%" + word + "%"));
                    }
                }

                if (weekStart != null)
                {
                    scoreParts.Add("CASE WHEN crime_time BETWEEN @start AND @end THEN 1 ELSE 0 END");
                    parameters.Add(new SqlParameter("@start", weekStart.Value));
                    parameters.Add(new SqlParameter("@end", weekStart.Value.AddDays(7)));
                }

                if (!string.IsNullOrEmpty(caseTitle))
                {
                    foreach (var word in caseTitle.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string paramName = "@title_" + word;
                        scoreParts.Add($"CASE WHEN case_title LIKE {paramName} THEN 1 ELSE 0 END");
                        parameters.Add(new SqlParameter(paramName, "%" + word + "%"));
                    }
                }

                if (victimCount != null)
                {
                    scoreParts.Add("CASE WHEN victim_count = @victim THEN 1 ELSE 0 END");
                    parameters.Add(new SqlParameter("@victim", victimCount));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    scoreParts.Add("CASE WHEN status = @status THEN 1 ELSE 0 END");
                    parameters.Add(new SqlParameter("@status", status));
                }

                if (!string.IsNullOrEmpty(caseno))
                {
                    isCaseMatchClause = "CASE WHEN case_no = @caseNo THEN 1 ELSE 0 END AS IsCaseMatch";
                    parameters.Add(new SqlParameter("@caseNo", caseno));
                }

                string scoreClause = scoreParts.Count > 0 ? string.Join(" + ", scoreParts) : "0";
                baseQuery = $"SELECT *, {isCaseMatchClause}, ({scoreClause}) AS Score FROM Cases" + whereClause;
                string finalQuery = baseQuery + " ORDER BY IsCaseMatch DESC, Score DESC, crime_time DESC";

                SqlCommand cmd = new SqlCommand(finalQuery, con);
                cmd.Parameters.AddRange(parameters.ToArray());

                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    Label noResultsLabel = new Label
                    {
                        Text = "No Cases Found.",
                        Font = new Font("Arial", 12, FontStyle.Bold),
                        ForeColor = Color.Black,
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    Panel wrapperPanel = new Panel
                    {
                        Width = flp_cases.Width - 50,
                        Height = flp_cases.Height - 50,
                        BackColor = flp_cases.BackColor
                    };

                    noResultsLabel.Location = new Point(
                        (wrapperPanel.Width - noResultsLabel.Width) / 2,
                        (wrapperPanel.Height - noResultsLabel.Height) / 2
                    );

                    wrapperPanel.Controls.Add(noResultsLabel);
                    flp_cases.Controls.Add(wrapperPanel);
                    return;
                }

                while (reader.Read())
                {
                    bool isCaseMatch = Convert.ToInt32(reader["IsCaseMatch"]) == 1;
                    string title = reader["case_title"].ToString();
                    string caseNo = reader["case_no"].ToString();

                    Panel caseCard = new Panel
                    {
                        Width = flp_cases.Width - 35,
                        Height = 270,
                        BackColor = Color.White,
                        Padding = new Padding(10),
                        Margin = new Padding(10),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    if (isCaseMatch)
                        caseCard.BackColor = Color.MistyRose;

                    // Create labels
                    Label lbl_caseNoHead = new Label { Text = "Case No: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_caseNo = new Label { Text = caseNo, AutoSize = true };

                    Label lbl_titleHead = new Label { Text = "Case Title: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_title = new Label
                    {
                        Text = title.Length > 100 ? title.Substring(0, 100) + "..." : title,
                        AutoSize = true,
                        Font = new Font("Arial", 9, FontStyle.Bold)
                    };

                    Label lbl_victimHead = new Label { Text = "Victim Count: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_victim = new Label { Text = reader["victim_count"].ToString(), AutoSize = true };

                    Label lbl_timeHead = new Label { Text = "Crime Time: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_time = new Label
                    {
                        Text = reader["crime_time"] != DBNull.Value
                            ? Convert.ToDateTime(reader["crime_time"]).ToString("dd MMM yyyy")
                            : "N/A",
                        AutoSize = true
                    };

                    Label lbl_typeHead = new Label { Text = "Crime Type: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_type = new Label { Text = reader["crime_type"].ToString(), AutoSize = true };

                    Label lbl_statusHead = new Label { Text = "Case Status: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_status = new Label { Text = reader["status"].ToString(), AutoSize = true };

                    Label lbl_locationHead = new Label { Text = "Crime Location: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_location = new Label { Text = reader["crime_location"].ToString(), AutoSize = true };

                    Label lbl_detailsHead = new Label { Text = "Case Details: ", Font = new Font("Arial", 9, FontStyle.Bold) };
                    Label lbl_details = new Label
                    {
                        Text = reader["detailed_description"].ToString(),
                        MaximumSize = new Size(caseCard.Width - 150, 0),
                        AutoSize = true
                    };

                    // Positioning
                    int leftOffset = 10;
                    lbl_caseNoHead.Location = new Point(leftOffset, 10);
                    lbl_caseNo.Location = new Point(lbl_caseNoHead.Right + 3, 10);

                    lbl_titleHead.Location = new Point(leftOffset, 35);
                    lbl_title.Location = new Point(lbl_titleHead.Right + 3, 35);

                    lbl_victimHead.Location = new Point(leftOffset, 60);
                    lbl_victim.Location = new Point(lbl_victimHead.Right + 3, 60);

                    lbl_timeHead.Location = new Point(lbl_victim.Right + 50, 60);
                    lbl_time.Location = new Point(lbl_timeHead.Right + 3, 60);

                    lbl_typeHead.Location = new Point(lbl_time.Right + 30, 60);
                    lbl_type.Location = new Point(lbl_typeHead.Right + 3, 60);

                    lbl_statusHead.Location = new Point(leftOffset, 90);
                    lbl_status.Location = new Point(lbl_statusHead.Right + 3, 90);

                    lbl_locationHead.Location = new Point(lbl_status.Right + 50, 90);
                    lbl_location.Location = new Point(lbl_locationHead.Right + 3, 90);

                    lbl_detailsHead.Location = new Point(leftOffset, 120);
                    lbl_details.Location = new Point(lbl_detailsHead.Right + 10, 120);

                    // Add to card
                    caseCard.Controls.AddRange(new Control[]
                    {
                        lbl_caseNoHead, lbl_caseNo,
                        lbl_titleHead, lbl_title,
                        lbl_victimHead, lbl_victim,
                        lbl_timeHead, lbl_time,
                        lbl_typeHead, lbl_type,
                        lbl_statusHead, lbl_status,
                        lbl_locationHead, lbl_location,
                        lbl_detailsHead, lbl_details
                    });



                    flp_cases.Controls.Add(caseCard);
                }

                reader.Close();
            }
        }


        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerDash dashboard = new OfficerDash();
            dashboard.ShowDialog();
            this.Close();
        }

        private void btn_updateProfile_Click(object sender, EventArgs e)
        {
            this.Hide();

            UpdProfileO updateProfileForm = new UpdProfileO();

            updateProfileForm.ShowDialog();

            this.Close();
        }

        private void btn_newreports_Click(object sender, EventArgs e)
        {
            this.Hide();

            NewReportsO newreports = new NewReportsO();

            newreports.ShowDialog();

            this.Close();
        }

        private void siticoneButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerCases AssignedCases = new OfficerCases();
            AssignedCases.ShowDialog();
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

        private void btn_searchcase_Click(object sender, EventArgs e)
        {
            this.Hide();
            SearchCaseO SearchCaseO = new SearchCaseO();
            SearchCaseO.ShowDialog();
            this.Close();
        }
    }
}