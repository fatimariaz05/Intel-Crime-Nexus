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
using System.Windows.Forms.DataVisualization.Charting; // at top

namespace CISystem
{
    public partial class Analytics : Form
    {
        public Analytics()
        {
            InitializeComponent();
            this.Load += new EventHandler(Analytics_Load);

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

        private void Analytics_Load(object sender, EventArgs e)
        {
            lbl_UserName.Text = UserSession.UserName; // Set the username
            lbl_UserName.Refresh(); // Force UI update


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void crimecat_Click(object sender, EventArgs e)
        {
            // Remove old panel if exists
            Control existingPanel = this.Controls["chartPanel"];
            if (existingPanel != null)
            {
                this.Controls.Remove(existingPanel);
                existingPanel.Dispose();
            }

            // Create panel
            Panel chartPanel = new Panel
            {
                Name = "chartPanel",
                Location = new Point(261, 96),
                Size = new Size(935, 593),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(chartPanel);
            chartPanel.BringToFront(); // ✅ Bring to front

            // Create the PictureBox as a close button at the top-right corner of the panel
            PictureBox removeButton = new PictureBox
            {
                Image = Image.FromFile(@"C:\Users\HP\OneDrive\Desktop\Crime investigation\Iteration 1\remove1.png"),
                SizeMode = PictureBoxSizeMode.StretchImage, // Ensure the image stretches to fit the button
                Size = new Size(20, 20), // Desired size for the icon
                Cursor = Cursors.Hand, // Change cursor to hand to indicate clickable
                Location = new Point(chartPanel.Width - 25, 5), // Position on the top-right corner
                Tag = chartPanel // Store chartPanel reference for later if needed
            };
            removeButton.Click += (s, args) =>
            {
                // On click, remove the panel and dispose it
                this.Controls.Remove(chartPanel);
                chartPanel.Dispose();
            };
            chartPanel.Controls.Add(removeButton);

            // Create chart
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill
            };
            chartPanel.Controls.Add(chart);

            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            // ✅ Improve label visibility and axis formatting
            chartArea.AxisX.LabelStyle.Angle = -45;
            chartArea.AxisX.Interval = 1;
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisX.LabelStyle.IsStaggered = false;
            chartArea.AxisX.Title = "Crime Category";
            chartArea.AxisY.Title = "Number of Cases";

            // ✅ Adjust plot position to lift graph up
            chartArea.InnerPlotPosition = new ElementPosition(10, 0, 85, 94); // Y=3, Height=94 to lift and expose bottom

            Series series = new Series
            {
                Name = "CrimeTypes",
                ChartType = SeriesChartType.Bar,
                IsValueShownAsLabel = true
            };
            chart.Series.Add(series);

            chart.Titles.Add("Crime Category Distribution (Last 10 Years)");

            // Database query to get crime category names and their frequencies
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = @"
            SELECT cc.crime_catg AS CrimeCategory, COUNT(*) AS Total
            FROM Cases c
            INNER JOIN Crime_Categories cc ON c.crime_category_ID = cc.crime_catg_ID
            WHERE c.crime_time >= DATEADD(YEAR, -10, GETDATE())
            GROUP BY cc.crime_catg
            ORDER BY Total DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string crimeCategory = reader["CrimeCategory"].ToString();
                        int count = Convert.ToInt32(reader["Total"]);
                        series.Points.AddXY(crimeCategory, count);
                    }
                }
            }
        }

        private void victimcount_Click(object sender, EventArgs e)
        {
            // Remove old panel if exists
            Control existingPanel = this.Controls["chartPanel"];
            if (existingPanel != null)
            {
                this.Controls.Remove(existingPanel);
                existingPanel.Dispose();
            }

            // Create panel
            Panel chartPanel = new Panel
            {
                Name = "chartPanel",
                Location = new Point(261, 96),
                Size = new Size(935, 593),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(chartPanel);
            chartPanel.BringToFront(); // ✅ Bring panel to front

            // Create the PictureBox as a close button at the top-right corner of the panel
            PictureBox removeButton = new PictureBox
            {
                Image = Image.FromFile(@"C:\Users\HP\OneDrive\Desktop\Crime investigation\Iteration 1\remove1.png"),
                SizeMode = PictureBoxSizeMode.StretchImage, // Ensure the image stretches to fit the button
                Size = new Size(20, 20), // Desired size for the icon
                Cursor = Cursors.Hand, // Change cursor to hand to indicate clickable
                Location = new Point(chartPanel.Width - 25, 5), // Position on the top-right corner
                Tag = chartPanel // Store chartPanel reference for later if needed
            };
            removeButton.Click += (s, args) =>
            {
                // On click, remove the panel and dispose it
                this.Controls.Remove(chartPanel);
                chartPanel.Dispose();
            };
            chartPanel.Controls.Add(removeButton);

            // Create chart
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill
            };
            chartPanel.Controls.Add(chart);

            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            // ✅ Improve label visibility and axis formatting
            chartArea.AxisX.LabelStyle.Angle = -45;
            chartArea.AxisX.Interval = 1;
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisX.LabelStyle.IsStaggered = false;
            chartArea.AxisX.Title = "Victim Count";
            chartArea.AxisY.Title = "Number of Cases";
            chartArea.InnerPlotPosition = new ElementPosition(10, 5, 85, 90);

            Series series = new Series
            {
                Name = "VictimCount",
                ChartType = SeriesChartType.Bar,
                IsValueShownAsLabel = true,
                Color = Color.Purple // Set bar color to purple
            };
            chart.Series.Add(series);

            chart.Titles.Add("Victim Count Distribution (Last 2 Years)");

            // Database query to get the frequency of cases with victim counts from 1 to 10
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = @"
            SELECT victim_count, COUNT(*) AS Total
            FROM Cases
            WHERE victim_count BETWEEN 1 AND 10
            GROUP BY victim_count
            ORDER BY victim_count";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int victimCount = Convert.ToInt32(reader["victim_count"]);
                        int count = Convert.ToInt32(reader["Total"]);
                        series.Points.AddXY(victimCount.ToString(), count);
                    }
                }
            }
        }

        private void crimehotspot_Click(object sender, EventArgs e)
        {
            // Remove old panel if exists
            Control existingPanel = this.Controls["chartPanel"];
            if (existingPanel != null)
            {
                this.Controls.Remove(existingPanel);
                existingPanel.Dispose();
            }

            // Create panel
            Panel chartPanel = new Panel
            {
                Name = "chartPanel",
                Location = new Point(261, 96),
                Size = new Size(935, 593),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(chartPanel);
            chartPanel.BringToFront(); // ✅ Bring panel to front

            // Create the PictureBox as a close button at the top-right corner of the panel
            PictureBox removeButton = new PictureBox
            {
                Image = Image.FromFile(@"C:\Users\HP\OneDrive\Desktop\Crime investigation\Iteration 1\remove1.png"),
                SizeMode = PictureBoxSizeMode.StretchImage, // Ensure the image stretches to fit the button
                Size = new Size(20, 20), // Desired size for the icon
                Cursor = Cursors.Hand, // Change cursor to hand to indicate clickable
                Location = new Point(chartPanel.Width - 25, 5), // Position on the top-right corner
                Tag = chartPanel // Store chartPanel reference for later if needed
            };
            removeButton.Click += (s, args) =>
            {
                // On click, remove the panel and dispose it
                this.Controls.Remove(chartPanel);
                chartPanel.Dispose();
            };
            chartPanel.Controls.Add(removeButton);

            // Create chart
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill
            };
            chartPanel.Controls.Add(chart);

            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            // ✅ Improve label visibility and axis formatting
            chartArea.AxisX.LabelStyle.Angle = -45;
            chartArea.AxisX.Interval = 1;
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisX.LabelStyle.IsStaggered = false;
            chartArea.AxisX.Title = "City";
            chartArea.AxisY.Title = "Number of Crimes";
            chartArea.InnerPlotPosition = new ElementPosition(10, 5, 85, 90);

            Series series = new Series
            {
                Name = "CityCrimeHotspots",
                ChartType = SeriesChartType.Bar,
                IsValueShownAsLabel = true,
                Color = Color.DarkOrange // Set bar color to Dark Orange (or change it)
            };
            chart.Series.Add(series);

            chart.Titles.Add("Crime Hotspots by City (Last 10 Years)");

            // Database query to get the number of crimes in each city
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = @"
            SELECT sc.City AS City, COUNT(*) AS Total
                FROM Cases ca
                INNER JOIN scope_cities sc 
                    ON ca.crime_location LIKE '%' + sc.City + '%'
                WHERE ca.crime_time >= DATEADD(YEAR, -10, GETDATE())
                GROUP BY sc.City
                ORDER BY Total DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string cityName = reader["City"].ToString();
                        int count = Convert.ToInt32(reader["Total"]);
                        series.Points.AddXY(cityName, count);
                    }
                }
            }
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

        private void dsh_btn_Click(object sender, EventArgs e)
        {
            this.Hide();
            OfficerDash dashboard = new OfficerDash();
            dashboard.ShowDialog();
            this.Close();
        }

        private void crimefreq_Click(object sender, EventArgs e)
        {
            // Remove old panel if exists
            Control existingPanel = this.Controls["chartPanel"];
            if (existingPanel != null)
            {
                this.Controls.Remove(existingPanel);
                existingPanel.Dispose();
            }

            // Create panel
            Panel chartPanel = new Panel
            {
                Name = "chartPanel",
                Location = new Point(261, 96),
                Size = new Size(935, 593),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(chartPanel);
            chartPanel.BringToFront(); // ✅ Bring panel to front

            // Create the PictureBox as a close button at the top-right corner of the panel
            PictureBox removeButton = new PictureBox
            {
                Image = Image.FromFile(@"C:\Users\HP\OneDrive\Desktop\Crime investigation\Iteration 1\remove1.png"),
                SizeMode = PictureBoxSizeMode.StretchImage, // Ensure the image stretches to fit the button
                Size = new Size(20, 20), // Desired size for the icon
                Cursor = Cursors.Hand, // Change cursor to hand to indicate clickable
                Location = new Point(chartPanel.Width - 25, 5), // Position on the top-right corner
                Tag = chartPanel // Store chartPanel reference for later if needed
            };
            removeButton.Click += (s, args) =>
            {
                // On click, remove the panel and dispose it
                this.Controls.Remove(chartPanel);
                chartPanel.Dispose();
            };
            chartPanel.Controls.Add(removeButton);

            // Create chart
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill
            };
            chartPanel.Controls.Add(chart);

            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            // ✅ Axis formatting for vertical bars
            chartArea.AxisX.LabelStyle.Angle = 0; // No rotation on x-axis labels
            chartArea.AxisX.Interval = 1; // Interval 1 to show each month
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = true; // Show grid lines on y-axis for better readability
            chartArea.AxisX.Title = "Month";
            chartArea.AxisY.Title = "Number of Crimes";

            // Change chart type to Bar
            Series series = new Series
            {
                Name = "CrimeMonthlyTrend",
                ChartType = SeriesChartType.Bar, // Vertical bars for monthly distribution
                IsValueShownAsLabel = true,
                Color = Color.RoyalBlue,
                BorderWidth = 3
            };
            chart.Series.Add(series);

            chart.Titles.Add("Monthly Crime Frequency (Last 12 Months)");

            // Get crime count per month for last 12 months
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = @"
SELECT 
    FORMAT(crime_time, 'yyyy-MM') AS [Month],
    COUNT(*) AS Total
FROM Cases
WHERE crime_time >= DATEADD(MONTH, -11, GETDATE())
GROUP BY FORMAT(crime_time, 'yyyy-MM')
ORDER BY [Month] ASC";

                // Create a list of months for the last 12 months
                List<string> months = new List<string>();
                for (int i = 11; i >= 0; i--)
                {
                    months.Add(DateTime.Now.AddMonths(-i).ToString("yyyy-MM"));
                }

                // Add the months and crime data
                foreach (string month in months)
                {
                    bool foundMonth = false;
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string monthFromDb = reader["Month"].ToString();
                            int count = Convert.ToInt32(reader["Total"]);

                            if (month == monthFromDb)
                            {
                                series.Points.AddXY(DateTime.ParseExact(month, "yyyy-MM", null).ToString("MMM-yyyy"), count);
                                foundMonth = true;
                            }
                        }
                    }

                    // If no crimes for this month, explicitly add it with 0 crimes
                    if (!foundMonth)
                    {
                        series.Points.AddXY(DateTime.ParseExact(month, "yyyy-MM", null).ToString("MMM-yyyy"), 0);
                    }
                }
            }

        }

        private void crimetimeday_Click(object sender, EventArgs e)
        {
            // Remove old panel if exists
            Control existingPanel = this.Controls["chartPanel"];
            if (existingPanel != null)
            {
                this.Controls.Remove(existingPanel);
                existingPanel.Dispose();
            }

            // Create panel
            Panel chartPanel = new Panel
            {
                Name = "chartPanel",
                Location = new Point(261, 96),
                Size = new Size(935, 593),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(chartPanel);
            chartPanel.BringToFront(); // ✅ Bring panel to front

            // Create the PictureBox as a close button at the top-right corner of the panel
            PictureBox removeButton = new PictureBox
            {
                Image = Image.FromFile(@"C:\Users\HP\OneDrive\Desktop\Crime investigation\Iteration 1\remove1.png"),
                SizeMode = PictureBoxSizeMode.StretchImage, // Ensure the image stretches to fit the button
                Size = new Size(20, 20), // Desired size for the icon
                Cursor = Cursors.Hand, // Change cursor to hand to indicate clickable
                Location = new Point(chartPanel.Width - 25, 5), // Position on the top-right corner
                Tag = chartPanel // Store chartPanel reference for later if needed
            };
            removeButton.Click += (s, args) =>
            {
                // On click, remove the panel and dispose it
                this.Controls.Remove(chartPanel);
                chartPanel.Dispose();
            };
            chartPanel.Controls.Add(removeButton);

            // Create chart
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill
            };
            chartPanel.Controls.Add(chart);

            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            // ✅ Axis customization
            chartArea.AxisX.Interval = 1;
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisX.Title = "Time of Day";
            chartArea.AxisY.Title = "Number of Crimes";
            chartArea.InnerPlotPosition = new ElementPosition(10, 5, 85, 90);

            Series series = new Series
            {
                Name = "CrimeTimeOfDay",
                ChartType = SeriesChartType.Bar,
                IsValueShownAsLabel = true,
                Color = Color.MediumPurple
            };
            chart.Series.Add(series);

            chart.Titles.Add("Crime Time of Day Distribution");

            // SQL: Categorize crime_time into time ranges
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = @"
        SELECT 
            CASE 
                WHEN DATEPART(HOUR, crime_time) BETWEEN 6 AND 11 THEN 'Morning'
                WHEN DATEPART(HOUR, crime_time) BETWEEN 12 AND 17 THEN 'Afternoon'
                WHEN DATEPART(HOUR, crime_time) BETWEEN 18 AND 21 THEN 'Evening'
                ELSE 'Night'
            END AS TimeOfDay,
            COUNT(*) AS Total
        FROM Cases
        GROUP BY 
            CASE 
                WHEN DATEPART(HOUR, crime_time) BETWEEN 6 AND 11 THEN 'Morning'
                WHEN DATEPART(HOUR, crime_time) BETWEEN 12 AND 17 THEN 'Afternoon'
                WHEN DATEPART(HOUR, crime_time) BETWEEN 18 AND 21 THEN 'Evening'
                ELSE 'Night'
            END";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string timeOfDay = reader["TimeOfDay"].ToString();
                        int count = Convert.ToInt32(reader["Total"]);
                        series.Points.AddXY(timeOfDay, count);
                    }
                }
            }

        }

        private void crimestatus_Click(object sender, EventArgs e)
        {
            // Remove old panel if exists
            Control existingPanel = this.Controls["chartPanel"];
            if (existingPanel != null)
            {
                this.Controls.Remove(existingPanel);
                existingPanel.Dispose();
            }

            // Create panel
            Panel chartPanel = new Panel
            {
                Name = "chartPanel",
                Location = new Point(261, 96),
                Size = new Size(935, 593),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(chartPanel);
            chartPanel.BringToFront(); // ✅ Bring panel to front

            // Create the PictureBox as a close button at the top-right corner of the panel
            PictureBox removeButton = new PictureBox
            {
                Image = Image.FromFile(@"C:\Users\HP\OneDrive\Desktop\Crime investigation\Iteration 1\remove1.png"),
                SizeMode = PictureBoxSizeMode.StretchImage, // Ensure the image stretches to fit the button
                Size = new Size(20, 20), // Desired size for the icon
                Cursor = Cursors.Hand, // Change cursor to hand to indicate clickable
                Location = new Point(chartPanel.Width - 25, 5), // Position on the top-right corner
                Tag = chartPanel // Store chartPanel reference for later if needed
            };
            removeButton.Click += (s, args) =>
            {
                // On click, remove the panel and dispose it
                this.Controls.Remove(chartPanel);
                chartPanel.Dispose();
            };
            chartPanel.Controls.Add(removeButton);

            // Create chart
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill
            };
            chartPanel.Controls.Add(chart);

            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            // ✅ Hide axis for pie chart
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisX.LabelStyle.Enabled = false;
            chartArea.AxisY.LabelStyle.Enabled = false;

            Series series = new Series
            {
                Name = "CaseStatus",
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                LabelForeColor = Color.Black
            };
            series["PieLabelStyle"] = "Outside";
            series["PieLineColor"] = "Black";
            series.Label = "#VALX - #PERCENT{P0} (~#VAL)";
            series.LegendText = "#VALX";
            chart.Series.Add(series);

            chart.Titles.Add("Crime Status Overview");

            // SQL: Group by case_status
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = @"
SELECT status, COUNT(*) AS Total
FROM Cases
GROUP BY status";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string status = reader["status"].ToString();
                        int count = Convert.ToInt32(reader["Total"]);
                        series.Points.AddXY(status, count);
                    }
                }
            }

        }

        private void crimetimespan_Click(object sender, EventArgs e)
        {
            // Remove old panel if exists
            Control existingPanel = this.Controls["chartPanel"];
            if (existingPanel != null)
            {
                this.Controls.Remove(existingPanel);
                existingPanel.Dispose();
            }

            // Create panel
            Panel chartPanel = new Panel
            {
                Name = "chartPanel",
                Location = new Point(261, 96),
                Size = new Size(935, 593),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
            this.Controls.Add(chartPanel);
            chartPanel.BringToFront(); // ✅ Bring panel to front

            // Create the PictureBox as a close button at the top-right corner of the panel
            PictureBox removeButton = new PictureBox
            {
                Image = Image.FromFile(@"C:\Users\HP\OneDrive\Desktop\Crime investigation\Iteration 1\remove1.png"),
                SizeMode = PictureBoxSizeMode.StretchImage, // Ensure the image stretches to fit the button
                Size = new Size(20, 20), // Desired size for the icon
                Cursor = Cursors.Hand, // Change cursor to hand to indicate clickable
                Location = new Point(chartPanel.Width - 25, 5), // Position on the top-right corner
                Tag = chartPanel // Store chartPanel reference for later if needed
            };
            removeButton.Click += (s, args) =>
            {
                // On click, remove the panel and dispose it
                this.Controls.Remove(chartPanel);
                chartPanel.Dispose();
            };
            chartPanel.Controls.Add(removeButton);

            // Create chart
            Chart chart = new Chart
            {
                Dock = DockStyle.Fill
            };
            chartPanel.Controls.Add(chart);

            ChartArea chartArea = new ChartArea();
            chart.ChartAreas.Add(chartArea);

            // Improve readability
            chartArea.AxisX.Title = "Day of the Week";
            chartArea.AxisY.Title = "Number of Crimes";
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisX.Interval = 1;

            Series series = new Series
            {
                Name = "DayOfWeekCrimes",
                ChartType = SeriesChartType.Bar,
                IsValueShownAsLabel = true,
                Color = Color.SteelBlue
            };
            chart.Series.Add(series);

            chart.Titles.Add("Crime Distribution by Day of Week");

            // SQL: Get crime count grouped by weekday name
            using (SqlConnection conn = new SqlConnection(DB.connectionString))
            {
                conn.Open();
                string query = @"
        SELECT DATENAME(WEEKDAY, crime_time) AS DayOfWeek, COUNT(*) AS Total
        FROM Cases
        GROUP BY DATENAME(WEEKDAY, crime_time)
        ORDER BY 
            DATEPART(WEEKDAY, MIN(crime_time))"; // Ensures order is Mon-Sun

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string day = reader["DayOfWeek"].ToString();
                        int count = Convert.ToInt32(reader["Total"]);
                        series.Points.AddXY(day, count);
                    }
                }
            }

        }
    }
}
