using Semester5Project.Properties;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace Semester5Project
{
    public partial class DashBoard : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
           int nLeftRect,
           int nTopRect,
           int nRightRect,
           int nBottomRect,
           int nWidthEllipse,
           int nHeightEllipse
       );

        private Image iconIncomeImage;

        public DashBoard()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            InitializePanelEvents(); // Initialize events for the panels
            PreloadImages(); // Preload images for faster response
        }

        private void PreloadImages()
        {
            // Preload the image once to avoid delay on each MouseLeave event
            iconIncomeImage = Image.FromFile("C:\\Users\\1\\OneDrive\\Desktop\\Semester5Project\\images\\iconincome.png");
            pictureBox9.Image = iconIncomeImage; // Set initial image
        }

        private void InitializePanelEvents()
        {
            // Assign events for panels from panel2 to panel8
            panel2.MouseEnter += new EventHandler(Panel_MouseEnter);
            panel2.MouseLeave += new EventHandler(Panel_MouseLeave);
            panel2.Click += new EventHandler(Panel_Click);

            panel3.MouseEnter += new EventHandler(Panel_MouseEnter);
            panel3.MouseLeave += new EventHandler(Panel_MouseLeave);
            panel3.Click += new EventHandler(Panel_Click);

            panel4.MouseEnter += new EventHandler(Panel_MouseEnter);
            panel4.MouseLeave += new EventHandler(Panel_MouseLeave);
            panel4.Click += new EventHandler(Panel_Click);

            panel5.MouseEnter += new EventHandler(Panel_MouseEnter);
            panel5.MouseLeave += new EventHandler(Panel_MouseLeave);
            panel5.Click += new EventHandler(Panel_Click);

            panel6.MouseEnter += new EventHandler(Panel_MouseEnter);
            panel6.MouseLeave += new EventHandler(Panel_MouseLeave);
            panel6.Click += new EventHandler(Panel_Click);

            panel7.MouseEnter += new EventHandler(Panel_MouseEnter);
            panel7.MouseLeave += new EventHandler(Panel_MouseLeave);
            panel7.Click += new EventHandler(Panel_Click);

            //panel8.MouseEnter += new EventHandler(Panel_MouseEnter);
            //panel8.MouseLeave += new EventHandler(Panel_MouseLeave);
            //panel8.Click += new EventHandler(Panel_Click);
        }

        private void Panel_MouseEnter(object sender, EventArgs e)
        {
            // Change panel background color when mouse hovers over it
            Panel panel = sender as Panel;
            panel.BackColor = Color.LightGray;  // You can change this color as needed
        }

        private void Panel_MouseLeave(object sender, EventArgs e)
        {
            // Revert the panel background color to transparent when mouse leaves
            Panel panel = sender as Panel;
            panel.BackColor = Color.Transparent;
        }

        private void Panel_Click(object sender, EventArgs e)
        {
            // Handle the click event on the panel
            Panel panel = sender as Panel;

            // Extract the panel number from its name (assuming the panels are named "panel1", "panel2", ..., "panel7")
            string panelName = panel.Name;
            int panelNumber;

            if (int.TryParse(panelName.Substring(5), out panelNumber) && panelNumber >= 1 && panelNumber <= 7)
            {
                // Find the corresponding labelN_Click method dynamically using reflection
                var methodName = $"label{panelNumber}_Click";
                var method = this.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (method != null)
                {
                    // Invoke the labelN_Click method
                    method.Invoke(this, new object[] { sender, EventArgs.Empty });
                }
                else
                {
                    MessageBox.Show($"No method found for {methodName}");
                }
            }
            else
            {
                MessageBox.Show("Invalid panel clicked or panel name does not match the expected format.");
            }
        }


        private void DashBoard_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            Cows Ob = new Cows();
            Ob.Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            MilkPro Ob = new MilkPro();
            Ob.Show();
            this.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            CowHealth Ob = new CowHealth();
            Ob.Show();
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            Bleeding Ob = new Bleeding();
            Ob.Show();
            this.Hide();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            MilkSales Ob = new MilkSales();
            Ob.Show();
            this.Hide();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Finance Ob = new Finance();
            Ob.Show();
            this.Hide();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void exit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit(); // Closes the entire application
            }
        }

        private DataTable GetMonthlyIncomeData()
        {
            DataTable dt = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT MONTH(IncDate) AS Month, SUM(IncAmt) AS TotalIncome " +
                    "FROM IncomeTbl " +
                    "WHERE YEAR(IncDate) = @Year " +
                    "GROUP BY MONTH(IncDate) " +
                    "ORDER BY Month", cn))
                {
                    cmd.Parameters.AddWithValue("@Year", DateTime.Now.Year);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private void PlotIncomeChart()
        {
            DataTable data = GetMonthlyIncomeData();

            cartesianChart.Series.Clear();
            cartesianChart.AxisX.Clear();
            cartesianChart.AxisY.Clear();

            // Define months
            var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            cartesianChart.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Month",
                Labels = months
            });

            cartesianChart.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Income Amount",
                LabelFormatter = value => value.ToString("C0")  // Currency format
            });

            // Initialize ChartValues for monthly income
            var monthlyIncomeValues = new LiveCharts.ChartValues<int>();
            var monthlyIncomeDictionary = data.AsEnumerable()
                                              .ToDictionary(row => row.Field<int>("Month"), row => row.Field<int>("TotalIncome"));

            for (int i = 1; i <= 12; i++)
            {
                monthlyIncomeValues.Add(monthlyIncomeDictionary.ContainsKey(i) ? monthlyIncomeDictionary[i] : 0);
            }

            cartesianChart.Series.Add(new LiveCharts.Wpf.ColumnSeries
            {
                Title = DateTime.Now.Year.ToString(),
                Values = monthlyIncomeValues
            });

            cartesianChart.LegendLocation = LiveCharts.LegendLocation.Right;
        }

        private Dictionary<int, int> GetMonthlyExpenditureTotals(DataTable data, int year)
        {
            var monthlyTotals = new Dictionary<int, int>();

            foreach (DataRow row in data.Rows)
            {
                DateTime date = Convert.ToDateTime(row["ExpDate"]);
                if (date.Year == year)
                {
                    int month = date.Month;
                    int amount = Convert.ToInt32(row["ExpAmount"]);

                    if (monthlyTotals.ContainsKey(month))
                    {
                        monthlyTotals[month] += amount;
                    }
                    else
                    {
                        monthlyTotals[month] = amount;
                    }
                }
            }

            return monthlyTotals;
        }

        private DataTable GetExpenditureData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                using (SqlDataAdapter da = new SqlDataAdapter("SELECT [ExpDate], [ExpAmount] FROM ExpenditureTbl", cn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private void PlotExpenditureChart()
        {
            DataTable data = GetExpenditureData();
            int currentYear = DateTime.Now.Year;

            var monthlyTotals = GetMonthlyExpenditureTotals(data, currentYear);

            // Clear existing chart data and axes
            cartesianChart.Series.Clear();
            cartesianChart.AxisX.Clear();
            cartesianChart.AxisY.Clear();

            cartesianChart.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Month",
                Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }
            });

            cartesianChart.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Total Expenditure"
            });

            var values = new ChartValues<int>();
            for (int i = 1; i <= 12; i++)
            {
                values.Add(monthlyTotals.ContainsKey(i) ? monthlyTotals[i] : 0);
            }

            cartesianChart.Series.Add(new LiveCharts.Wpf.LineSeries
            {
                Title = "Expenditure",
                Values = values
            });
            cartesianChart.LegendLocation = LiveCharts.LegendLocation.Right;
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            PlotExpenditureChart();
        }



        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            PlotIncomeChart();
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {

        }

        private DataTable GetIncomeData()
        {
            DataTable incomeData = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                string query = "SELECT IncDate, IncAmt FROM IncomeTbl";
                using (SqlCommand command = new SqlCommand(query, cn)) // Use 'cn' here instead of 'incomeData'
                {
                    cn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(incomeData);
                    }
                }
            }

            return incomeData;
        }



        private Dictionary<int, int> GetMonthlyIncomeTotals(DataTable incomeData, int year)
        {
            var monthlyIncomeTotals = new Dictionary<int, int>();

            foreach (DataRow row in incomeData.Rows)
            {
                DateTime incDate = Convert.ToDateTime(row["IncDate"]);
                int incAmt = Convert.ToInt32(row["IncAmt"]);

                // Check if the entry is in the specified year
                if (incDate.Year == year)
                {
                    int month = incDate.Month;

                    if (monthlyIncomeTotals.ContainsKey(month))
                    {
                        monthlyIncomeTotals[month] += incAmt;
                    }
                    else
                    {
                        monthlyIncomeTotals[month] = incAmt;
                    }
                }
            }

            return monthlyIncomeTotals;
        }


        // Function to calculate monthly balances
        private Dictionary<int, int> CalculateMonthlyBalances(int year)
        {
            // Get income and expenditure data for calculations
            DataTable incomeData = GetIncomeData();
            DataTable expenditureData = GetExpenditureData();

            var incomeTotals = GetMonthlyIncomeTotals(incomeData, year);
            var expenditureTotals = GetMonthlyExpenditureTotals(expenditureData, year);

            // Dictionary to hold monthly balances
            var monthlyBalances = new Dictionary<int, int>();

            for (int month = 1; month <= 12; month++)
            {
                int income = incomeTotals.ContainsKey(month) ? incomeTotals[month] : 0;
                int expenditure = expenditureTotals.ContainsKey(month) ? expenditureTotals[month] : 0;
                monthlyBalances[month] = income - expenditure; // Calculate balance
            }

            return monthlyBalances;
        }

        // Method to plot monthly balance as a histogram
        private void PlotBalanceHistogram()
        {
            int currentYear = DateTime.Now.Year;
            var monthlyBalances = CalculateMonthlyBalances(currentYear);

            // Clear existing chart data and axes
            cartesianChart.Series.Clear();
            cartesianChart.AxisX.Clear();
            cartesianChart.AxisY.Clear();

            // Configure X and Y axes
            cartesianChart.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Month",
                Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" }
            });

            cartesianChart.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Balance",
                LabelFormatter = value => value.ToString("C") // Format as currency if desired
            });

            // Plot the balance as a histogram (column chart)
            var values = new ChartValues<int>();
            for (int month = 1; month <= 12; month++)
            {
                values.Add(monthlyBalances.ContainsKey(month) ? monthlyBalances[month] : 0);
            }

            cartesianChart.Series.Add(new LiveCharts.Wpf.ColumnSeries
            {
                Title = "Monthly Balance",
                Values = values
            });
            cartesianChart.LegendLocation = LiveCharts.LegendLocation.Right;
        }

        // Event handler for pictureBox11 click
        private void pictureBox11_Click(object sender, EventArgs e)
        {
            PlotBalanceHistogram();
        }


        private void DisplayCowsNeedingAttention()
        {
            // Clear any existing nodes in the TreeView
            treeView1.Nodes.Clear();

            // Create the root nodes for the TreeView
            TreeNode sickCowsNode = new TreeNode("Sick Cows");
            TreeNode bleedingCowsNode = new TreeNode("Bleeding Cows");
            //TreeNode milkProductionNode = new TreeNode("Milk Production per Cow");

            // Add the root nodes to the TreeView
            treeView1.Nodes.Add(sickCowsNode);
            treeView1.Nodes.Add(bleedingCowsNode);
            //treeView1.Nodes.Add(milkProductionNode);

            // Retrieve data for Sick Cows where Diagnosis is 'no'
            string sickCowQuery = "SELECT CowName, Event, CostTreatm FROM CowHealthTab WHERE CAST(Diagnosis AS VARCHAR(MAX)) = 'no'";

            using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\1\\OneDrive\\Desktop\\Semester5Project\\Database.mdf;Integrated Security=True;Connect Timeout=30"))
            {
                connection.Open();
                SqlCommand sickCowCommand = new SqlCommand(sickCowQuery, connection);
                using (SqlDataReader reader = sickCowCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create a new TreeNode for each sick cow
                        TreeNode sickCowNode = new TreeNode(reader["CowName"].ToString());
                        sickCowNode.Nodes.Add("Event: " + reader["Event"].ToString());
                        sickCowNode.Nodes.Add("Treatment Cost: " + reader["CostTreatm"].ToString());

                        // Add the sick cow node to the Sick Cows root node
                        sickCowsNode.Nodes.Add(sickCowNode);
                    }
                }
            }

            // Retrieve data for Bleeding Cows where Bleeding is not '0'
            string bleedingQuery = "SELECT CowName, Bleeding, Color FROM Cow WHERE CAST(Bleeding AS VARCHAR(MAX)) != '0'";
            using (SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\1\\OneDrive\\Desktop\\Semester5Project\\Database.mdf;Integrated Security=True;Connect Timeout=30"))
            {
                connection.Open();
                SqlCommand bleedingCommand = new SqlCommand(bleedingQuery, connection);
                using (SqlDataReader reader = bleedingCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create a new TreeNode for each bleeding cow
                        TreeNode bleedingCowNode = new TreeNode(reader["CowName"].ToString());
                        bleedingCowNode.Nodes.Add("Bleeding: " + reader["Bleeding"].ToString());
                        bleedingCowNode.Nodes.Add("Color: " + reader["Color"].ToString());

                        // Add the bleeding cow node to the Bleeding Cows root node
                        bleedingCowsNode.Nodes.Add(bleedingCowNode);
                    }
                }
            }


            // Expand all nodes for better visibility
            treeView1.ExpandAll();
        }



        private void pictureBox15_Click(object sender, EventArgs e)
        {
            DisplayCowsNeedingAttention();
        }
    }
}
