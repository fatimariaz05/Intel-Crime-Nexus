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
    public partial class FileFIR : Form
    {
        private string reportId;  
        public FileFIR(string reportId)
        {

            InitializeComponent();
            this.reportId = reportId;

            this.Load += new EventHandler(FileFIR_Load);

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

        private void FileFIR_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update


            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();

                // 1. Generate a unique FIR number
                Random rnd = new Random();
                int firNo;
                bool isUnique = false;

                do
                {
                    firNo = rnd.Next(1, 100000);
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM FIR WHERE fir_no = @firNo", conn);
                    checkCmd.Parameters.AddWithValue("@firNo", firNo);
                    int count = (int)checkCmd.ExecuteScalar();
                    isUnique = count == 0;
                } while (!isUnique);

                txb_fir_no.Text = firNo.ToString();
                txb_fir_no.ReadOnly = true;

                // 2. Get next FIR ID
                SqlCommand idCmd = new SqlCommand("SELECT ISNULL(MAX(fir_id), 0) + 1 FROM FIR", conn);
                int firId = (int)idCmd.ExecuteScalar();
                txb_fir_no.Tag = firId; // store temporarily in tag

                // 3. Get officer info (station + id)
                SqlCommand officerCmd = new SqlCommand("SELECT police_station, officer_id FROM Officer WHERE username = @username", conn);
                officerCmd.Parameters.AddWithValue("@username", UserSession.UserName);
                SqlDataReader reader = officerCmd.ExecuteReader();
                if (reader.Read())
                {
                    txb_filed_station.Text = reader["police_station"].ToString();
                    txb_filed_station.ReadOnly = false;

                    txb_filed_station.Tag = reader["officer_id"].ToString(); // save officer_id in tag
                }
                reader.Close();

                // 4. Date auto-set (DateTimePicker does this automatically)

                txb_report_id.Text = reportId;
                txb_report_id.ReadOnly = true; // Optional, depends on UX
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

        private void siticoneButton1_Click(object sender, EventArgs e)
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

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btn_file_Click(object sender, EventArgs e)
        {
            int firId = Convert.ToInt32(txb_fir_no.Tag);
            int firNo = Convert.ToInt32(txb_fir_no.Text);
            string fileDate = file_date.Value.ToString("yyyy-MM-dd");
            string fileLocation = txb_filed_station.Text;
            string filedBy = txb_filed_station.Tag.ToString(); // officer_id
            string details = txb_details.Text;

            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // --- Step 1: Insert FIR ---
                    SqlCommand insertFIR = new SqlCommand(@"
                INSERT INTO FIR (fir_id, fir_no, filed_date, filed_location, filed_by, report_id, fir_details) 
                VALUES (@fir_id, @fir_no, @filed_date, @filed_location, @filed_by, @report_id, @details)", conn, transaction);

                    insertFIR.Parameters.AddWithValue("@fir_id", firId);
                    insertFIR.Parameters.AddWithValue("@fir_no", firNo);
                    insertFIR.Parameters.AddWithValue("@filed_date", fileDate);
                    insertFIR.Parameters.AddWithValue("@filed_location", fileLocation);
                    insertFIR.Parameters.AddWithValue("@filed_by", filedBy);
                    insertFIR.Parameters.AddWithValue("@report_id", reportId);
                    insertFIR.Parameters.AddWithValue("@details", details);

                    insertFIR.ExecuteNonQuery();

                    // --- Step 2: Get case_id ---
                    SqlCommand caseIdCmd = new SqlCommand("SELECT ISNULL(MAX(case_id), 0) + 1 FROM Cases", conn, transaction);
                    int caseId = (int)caseIdCmd.ExecuteScalar();

                    // --- Step 3: Get report_nature as crime_type ---
                    SqlCommand crimeTypeCmd = new SqlCommand("SELECT report_nature, incident_location, incident_date, victim_count FROM Reports WHERE report_id = @report_id", conn, transaction);
                    crimeTypeCmd.Parameters.AddWithValue("@report_id", reportId);

                    string crimeType = "";
                    string crimeLocation = "";
                    DateTime crimeTime = DateTime.Now;
                    int victimCount = 0;

                    using (SqlDataReader reader = crimeTypeCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            crimeType = reader["report_nature"].ToString();
                            crimeLocation = reader["incident_location"].ToString();
                            crimeTime = Convert.ToDateTime(reader["incident_date"]);
                            victimCount = Convert.ToInt32(reader["victim_count"]);
                        }
                        else
                        {
                            throw new Exception("Report not found. Cannot file Case.");
                        }
                    }

                    // --- Step 4: Get crime_category_ID ---
                    SqlCommand categoryCmd = new SqlCommand("SELECT crime_catg_ID FROM Crime_Categories WHERE crime_catg = @crime_type", conn, transaction);
                    categoryCmd.Parameters.AddWithValue("@crime_type", crimeType);
                    object categoryIdObj = categoryCmd.ExecuteScalar();
                    if (categoryIdObj == null)
                        throw new Exception("Crime category not found for crime type: " + crimeType);
                    int crimeCategoryId = Convert.ToInt32(categoryIdObj);

                    // --- Step 5: Generate unique case_no ---
                    string caseNo = "";
                    Random rnd = new Random();
                    bool uniqueCaseNo = false;

                    do
                    {
                        int randomNum = rnd.Next(1, 100000);
                        string year = crimeTime.Year.ToString();
                        caseNo = randomNum.ToString("D4") + "/" + year;

                        SqlCommand checkCaseNo = new SqlCommand("SELECT COUNT(*) FROM Cases WHERE case_no = @case_no", conn, transaction);
                        checkCaseNo.Parameters.AddWithValue("@case_no", caseNo);
                        int count = (int)checkCaseNo.ExecuteScalar();

                        uniqueCaseNo = count == 0;

                    } while (!uniqueCaseNo);

                    // --- Step 6: Insert into Cases ---
                    SqlCommand insertCase = new SqlCommand(@"
                INSERT INTO Cases 
                (case_id, case_no, crime_type, crime_category_ID, crime_location, crime_time, victim_count, status, created_at, fir_id, report_id) 
                VALUES 
                (@case_id, @case_no, @crime_type, @crime_category_ID, @crime_location, @crime_time, @victim_count, @status, @created_at, @fir_id, @report_id)", conn, transaction);

                    insertCase.Parameters.AddWithValue("@case_id", caseId);
                    insertCase.Parameters.AddWithValue("@case_no", caseNo);
                    insertCase.Parameters.AddWithValue("@crime_type", crimeType);
                    insertCase.Parameters.AddWithValue("@crime_category_ID", crimeCategoryId);
                    insertCase.Parameters.AddWithValue("@crime_location", crimeLocation);
                    insertCase.Parameters.AddWithValue("@crime_time", crimeTime);
                    insertCase.Parameters.AddWithValue("@victim_count", victimCount);
                    insertCase.Parameters.AddWithValue("@status", "Under Investigation");
                    insertCase.Parameters.AddWithValue("@created_at", DateTime.Now);
                    insertCase.Parameters.AddWithValue("@fir_id", firId);
                    insertCase.Parameters.AddWithValue("@report_id", reportId);

                    insertCase.ExecuteNonQuery();

                    // --- Step 7: Commit everything ---
                    transaction.Commit();
                    MessageBox.Show("FIR filed and Case created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Failed to file FIR and Case.\nError: " + ex.Message);
                }
            }
        }


        private void txb_report_id_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
