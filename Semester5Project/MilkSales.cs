using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Semester5Project
{
    public partial class MilkSales : Form
    {
        // Event to notify subscribers of data changes
        public event Action MilkTblUpdated;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
           int nLeftRect,
           int nTopRect,
           int nRightRect,
           int nBottomRect,
           int nWidthEllipse,
           int nHeightEllipse
       );
        public MilkSales()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            InitializePanelEvents(); // Initialize events for the panels
            UpdateRemainingMilkLabel();

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

            //panel6.MouseEnter += new EventHandler(Panel_MouseEnter);
            //panel6.MouseLeave += new EventHandler(Panel_MouseLeave);
            //panel6.Click += new EventHandler(Panel_Click);

            panel7.MouseEnter += new EventHandler(Panel_MouseEnter);
            panel7.MouseLeave += new EventHandler(Panel_MouseLeave);
            panel7.Click += new EventHandler(Panel_Click);

            panel8.MouseEnter += new EventHandler(Panel_MouseEnter);
            panel8.MouseLeave += new EventHandler(Panel_MouseLeave);
            panel8.Click += new EventHandler(Panel_Click);
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

        private void MilkSales_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();
                PlotMilkSalesChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }

        }

        private void FadeOutAndSwitchForms(Form currentForm, Form newForm)
        {
            Timer timer = new Timer();
            timer.Interval = 20; // 50 milliseconds for smooth transition
            timer.Tick += (sender, e) =>
            {
                if (currentForm.Opacity > 0)
                {
                    currentForm.Opacity -= 0.05; // Fade out
                }
                else
                {
                    // Once the current form is fully invisible, stop the timer
                    timer.Stop();

                    // Hide current form and show new form
                    currentForm.Hide();
                    newForm.Show();
                    newForm.Opacity = 0;  // Make the new form invisible initially
                    FadeIn(newForm);
                }
            };
            timer.Start();
        }

        private void FadeIn(Form form)
        {
            Timer fadeInTimer = new Timer();
            fadeInTimer.Interval = 20;  // 50 milliseconds for smooth transition
            fadeInTimer.Tick += (sender, e) =>
            {
                if (form.Opacity < 1)
                {
                    form.Opacity += 0.05;  // Fade in
                }
                else
                {
                    fadeInTimer.Stop();
                }
            };
            fadeInTimer.Start();
        }

        public void ReloadMilkProData()
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM MilkTbl ORDER BY [Date] DESC", cn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable milkTable = new DataTable();
                        adapter.Fill(milkTable);
                        dataGrid.DataSource = milkTable; // Assuming dataGridView2 is the table in MilkPro
                    }
                }
            }
        }

        DashBoard dash;
        Finance fina;
        Bleeding bleed;
        CowHealth cowH;
        MilkPro milkp;
        Cows cow;

        private void label2_Click(object sender, EventArgs e)
        {
            cow = new Cows();
            FadeOutAndSwitchForms(this, cow);
        }

        
        private void label3_Click(object sender, EventArgs e)
        {
            MilkPro Ob = new MilkPro();
            Ob.ReloadMilkProData(); // Ensure updated data is displayed
            Ob.Show();
            this.Hide();
        }
        
        private void label4_Click(object sender, EventArgs e)
        {
            cowH = new CowHealth();
            FadeOutAndSwitchForms(this, cowH);
        }
       
        private void label5_Click(object sender, EventArgs e)
        {
            bleed = new Bleeding();
            FadeOutAndSwitchForms(this, bleed);
        }

        private void label6_Click(object sender, EventArgs e)
        {
            MilkSales Ob = new MilkSales();
            Ob.UpdateRemainingMilkLabel(); // Refresh RemainingMilk label
            Ob.Show();
            this.Hide();
        }


        private void label7_Click(object sender, EventArgs e)
        {
            fina = new Finance();
            FadeOutAndSwitchForms(this, fina);
        }
        
        private void label8_Click(object sender, EventArgs e)
        {
            dash = new DashBoard();
            FadeOutAndSwitchForms(this, dash);
        }

        // Insert data from DataGridView
        private void SaveDataToDatabase()
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                using (SqlTransaction transaction = cn.BeginTransaction()) // Begin transaction
                {
                    try
                    {
                        foreach (DataGridViewRow row in dataGrid.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                DateTime date = Convert.ToDateTime(row.Cells["Date"].Value);
                                int pricePerLtr = Convert.ToInt32(row.Cells["Price PerLtr"].Value);
                                int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                                int totalPrice = Convert.ToInt32(row.Cells["Total Price"].Value);

                                // Insert into MilkSalesTbl
                                using (SqlCommand insertCmd = new SqlCommand("INSERT INTO MilkSalesTbl ([Date], [Price PerLtr], [Quantity], [Total Price]) VALUES (@Date, @PricePerLtr, @Quantity, @TotalPrice)", cn, transaction))
                                {
                                    insertCmd.Parameters.AddWithValue("@Date", date);
                                    insertCmd.Parameters.AddWithValue("@PricePerLtr", pricePerLtr);
                                    insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                                    insertCmd.Parameters.AddWithValue("@TotalPrice", totalPrice);

                                    insertCmd.ExecuteNonQuery();
                                }

                                // Update RemainingMilk in MilkTbl
                                using (SqlCommand updateCmd = new SqlCommand("UPDATE MilkTbl SET RemainingMilk = RemainingMilk - @Quantity WHERE CAST([Date] AS DATE) = @Date", cn, transaction))
                                {
                                    updateCmd.Parameters.AddWithValue("@Quantity", quantity);
                                    updateCmd.Parameters.AddWithValue("@Date", date.Date);

                                    int rowsAffected = updateCmd.ExecuteNonQuery();
                                    if (rowsAffected == 0)
                                    {
                                        throw new Exception($"No records found in MilkTbl for the date {date:yyyy-MM-dd}.");
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                        MilkTblUpdated?.Invoke();
                        MessageBox.Show("Data saved and Remaining Milk updated successfully.");
                        UpdateRemainingMilkLabel(); // Refresh remaining milk
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }


        private DataTable GetMilkSalesData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                using (SqlDataAdapter da = new SqlDataAdapter("SELECT [Date], [Total Price] FROM MilkSalesTbl", cn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private Dictionary<int, int> GetMonthlyTotals(DataTable data, int year)
        {
            var monthlyTotals = new Dictionary<int, int>();

            foreach (DataRow row in data.Rows)
            {
                DateTime date = Convert.ToDateTime(row["Date"]);
                if (date.Year == year)
                {
                    int month = date.Month;
                    int totalPrice = Convert.ToInt32(row["Total Price"]);

                    if (monthlyTotals.ContainsKey(month))
                    {
                        monthlyTotals[month] += totalPrice;
                    }
                    else
                    {
                        monthlyTotals[month] = totalPrice;
                    }
                }
            }

            return monthlyTotals;
        }

        private void PlotMilkSalesChart()
        {
            DataTable data = GetMilkSalesData();
            int currentYear = DateTime.Now.Year;
            int previousYear = currentYear - 1;

            var currentYearTotals = GetMonthlyTotals(data, currentYear);
            var previousYearTotals = GetMonthlyTotals(data, previousYear);

            // Assuming your chart is a LiveCharts.WinForms.CartesianChart named cartesianChart
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
                Title = "Total Price Sold"
            });

            var currentYearValues = new ChartValues<int>();
            var previousYearValues = new ChartValues<int>();

            for (int i = 1; i <= 12; i++)
            {
                currentYearValues.Add(currentYearTotals.ContainsKey(i) ? currentYearTotals[i] : 0);
                previousYearValues.Add(previousYearTotals.ContainsKey(i) ? previousYearTotals[i] : 0);
            }

            cartesianChart.Series.Add(new LiveCharts.Wpf.LineSeries
            {
                Title = currentYear.ToString(),
                Values = currentYearValues
            });

            cartesianChart.Series.Add(new LiveCharts.Wpf.LineSeries
            {
                Title = previousYear.ToString(),
                Values = previousYearValues
            });
            cartesianChart.LegendLocation = LiveCharts.LegendLocation.Right;
        }

        private void LoadData()
        {
            // This part loads a single record from the MilkSalesTbl table into UI controls.
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM MilkSalesTbl", cn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        milkdate.Value = Convert.ToDateTime(dt.Rows[0]["Date"]);
                        percost.Text = dt.Rows[0]["Price PerLtr"].ToString();
                        cowtotalmilk.Text = dt.Rows[0]["Quantity"].ToString();
                        totalprice.Text = dt.Rows[0]["Total Price"].ToString();
                    }
                }
            }

            // This part loads all records from the MilkSalesTbl table into a DataGridView.
            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                {
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    string query = "SELECT [Date], [Price PerLtr], [Quantity], [Total Price] FROM MilkSalesTbl";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, cn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGrid.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
            ClearFields();
        }

        // Insert data from TextBoxes
        private void InsertData()
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                using (SqlTransaction transaction = cn.BeginTransaction()) // Begin transaction
                {
                    try
                    {
                        // Step 1: Get input values and calculate total price
                        int pricePerLtr = Convert.ToInt32(percost.Text);
                        int quantity = Convert.ToInt32(cowtotalmilk.Text);
                        int totalPrice = pricePerLtr * quantity;

                        // Step 2: Insert into MilkSalesTbl
                        using (SqlCommand insertCmd = new SqlCommand("INSERT INTO MilkSalesTbl ([Date], [Price PerLtr], [Quantity], [Total Price]) VALUES (@Date, @PricePerLtr, @Quantity, @TotalPrice)", cn, transaction))
                        {
                            insertCmd.Parameters.AddWithValue("@Date", milkdate.Value);
                            insertCmd.Parameters.AddWithValue("@PricePerLtr", pricePerLtr);
                            insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                            insertCmd.Parameters.AddWithValue("@TotalPrice", totalPrice);

                            insertCmd.ExecuteNonQuery();
                        }

                        // Step 3: Insert into IncomeTbl
                        using (SqlCommand incomeInsertCmd = new SqlCommand("INSERT INTO IncomeTbl (IncDate, IncPurpose, IncAmt, Empid) VALUES (@IncDate, @IncPurpose, @IncAmt, @Empid)", cn, transaction))
                        {
                            incomeInsertCmd.Parameters.AddWithValue("@IncDate", milkdate.Value); // Use Date from MilkSalesTbl
                            incomeInsertCmd.Parameters.AddWithValue("@IncPurpose", "Milk Sales"); // Default purpose
                            incomeInsertCmd.Parameters.AddWithValue("@IncAmt", totalPrice); // Total Price from MilkSalesTbl
                            incomeInsertCmd.Parameters.AddWithValue("@Empid", 1); // Default Empid
                            incomeInsertCmd.ExecuteNonQuery();
                        }


                        // Step 3: Fetch the latest RemainingMilk from MilkTbl
                        double remainingMilk;
                        DateTime recentDate;
                        using (SqlCommand selectCmd = new SqlCommand("SELECT TOP 1 RemainingMilk, [Date] FROM MilkTbl ORDER BY [Date] DESC", cn, transaction))
                        {
                            using (SqlDataReader reader = selectCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    remainingMilk = Convert.ToDouble(reader["RemainingMilk"]);
                                    recentDate = Convert.ToDateTime(reader["Date"]);
                                }
                                else
                                {
                                    // Handle the case where no records exist
                                    throw new Exception("No records found in MilkTbl to update RemainingMilk.");
                                }
                            }
                        }

                        // Step 4: Update RemainingMilk in MilkTbl
                        double newRemainingMilk = remainingMilk - quantity;
                        if (newRemainingMilk < 0)
                        {
                            throw new Exception("Not enough milk available to complete the sale.");
                        }

                        using (SqlCommand updateCmd = new SqlCommand("UPDATE MilkTbl SET RemainingMilk = @RemainingMilk WHERE [Date] = @Date", cn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@RemainingMilk", newRemainingMilk);
                            updateCmd.Parameters.AddWithValue("@Date", recentDate);

                            updateCmd.ExecuteNonQuery();
                        }

                        // Step 5: Commit the transaction
                        transaction.Commit();
                        MilkTblUpdated?.Invoke();
                        // Step 6: Update UI with new RemainingMilk
                        MessageBox.Show("Data Saved and Remaining Milk updated successfully!");
                        UpdateRemainingMilkLabel();
                        LoadData();// Refresh RemainingMilk label
                        ClearFields(); // Clear input fields after insertion
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction in case of error
                        transaction.Rollback();
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }




        // Update label with total remaining milk
        private void UpdateRemainingMilkLabel()
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 RemainingMilk FROM MilkTbl ORDER BY [Date] DESC", cn))
                {
                    var result = cmd.ExecuteScalar();
                    double remainingMilk = result != DBNull.Value ? Convert.ToDouble(result) : 0;

                    remainingmilk.Text = $" {remainingMilk} ltr";
                }
            }
        }

        
        private void ClearFields()
        {
            milkdate.Value = DateTime.Now; // Reset the DateTimePicker to the current date
            percost.Clear();
            cowtotalmilk.Clear();
            totalprice.Clear();
        }

        private void DeleteData()
        {
            if (dataGrid.SelectedRows.Count > 0)
            {
                // Get the selected row's date and quantity
                object dateValue = dataGrid.SelectedRows[0].Cells["DateS"].Value;
                object quantityValue = dataGrid.SelectedRows[0].Cells["Qua"].Value;

                if (dateValue != null && quantityValue != null &&
                    DateTime.TryParse(dateValue.ToString(), out DateTime saleDate) &&
                    int.TryParse(quantityValue.ToString(), out int quantity))
                {
                    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                    {
                        if (cn.State == ConnectionState.Closed)
                            cn.Open();

                        using (SqlTransaction transaction = cn.BeginTransaction()) // Begin transaction
                        {
                            try
                            {
                                // Step 1: Delete the record from MilkSalesTbl
                                using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM MilkSalesTbl WHERE [Date] = @Date", cn, transaction))
                                {
                                    deleteCmd.Parameters.AddWithValue("@Date", saleDate);
                                    deleteCmd.ExecuteNonQuery();
                                }

                                // Step 2: Delete the corresponding record from IncomeTbl
                                using (SqlCommand incomeDeleteCmd = new SqlCommand("DELETE FROM IncomeTbl WHERE IncDate = @IncDate AND IncPurpose = @IncPurpose", cn, transaction))
                                {
                                    incomeDeleteCmd.Parameters.AddWithValue("@IncDate", saleDate); // Match the date
                                    incomeDeleteCmd.Parameters.AddWithValue("@IncPurpose", "Milk Sales"); // Match the purpose
                                    incomeDeleteCmd.ExecuteNonQuery();
                                }


                                // Step 2: Fetch the most recent RemainingMilk and date from MilkTbl
                                double remainingMilk;
                                DateTime recentDate;

                                using (SqlCommand selectCmd = new SqlCommand("SELECT TOP 1 RemainingMilk, [Date] FROM MilkTbl ORDER BY [Date] DESC", cn, transaction))
                                {
                                    using (SqlDataReader reader = selectCmd.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            remainingMilk = Convert.ToDouble(reader["RemainingMilk"]);
                                            recentDate = Convert.ToDateTime(reader["Date"]);
                                        }
                                        else
                                        {
                                            throw new Exception("No records found in MilkTbl to update RemainingMilk.");
                                        }
                                    }
                                }



                                // Step 3: Update RemainingMilk for the most recent date
                                double newRemainingMilk = remainingMilk + quantity;

                                using (SqlCommand updateCmd = new SqlCommand("UPDATE MilkTbl SET RemainingMilk = @RemainingMilk WHERE [Date] = @Date", cn, transaction))
                                {
                                    updateCmd.Parameters.AddWithValue("@RemainingMilk", newRemainingMilk);
                                    updateCmd.Parameters.AddWithValue("@Date", recentDate);
                                    updateCmd.ExecuteNonQuery();
                                }

                                // Step 4: Commit the transaction
                                transaction.Commit();
                                UpdateRemainingMilkLabel();
                                MessageBox.Show("Data Deleted and Remaining Milk updated successfully!");
                                LoadData(); // Refresh the data after deletion
                            }
                            catch (Exception ex)
                            {
                                // Rollback transaction in case of error
                                transaction.Rollback();
                                MessageBox.Show($"Error: {ex.Message}");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid selection. Please ensure the selected row contains valid data.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }


        private void UpdateData()
        {
            if (dataGrid.SelectedRows.Count > 0)
            {
                object dateValue = dataGrid.SelectedRows[0].Cells[0].Value;

                if (dateValue != null && DateTime.TryParse(dateValue.ToString(), out DateTime saleDate))
                {
                    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                    {
                        if (cn.State == ConnectionState.Closed)
                            cn.Open();

                        using (SqlCommand cmd = new SqlCommand("UPDATE MilkSalesTbl SET [Price PerLtr] = @PricePerLtr, [Quantity] = @Quantity, [Total Price] = @TotalPrice WHERE [Date] = @Date", cn))
                        {
                            cmd.Parameters.AddWithValue("@Date", milkdate);
                            cmd.Parameters.AddWithValue("@PricePerLtr", Convert.ToInt32(percost.Text));
                            cmd.Parameters.AddWithValue("@Quantity", Convert.ToInt32(cowtotalmilk.Text));
                            cmd.Parameters.AddWithValue("@TotalPrice", Convert.ToInt32(totalprice.Text));

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Data Updated Successfully!");
                            LoadData(); // Refresh the data after update
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Date selected. Please select a valid row.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Check if textboxes have values (this assumes that your textboxes must have valid input if they are being used)
            bool isTextboxDataEntered = !string.IsNullOrWhiteSpace(percost.Text) &&
                                        !string.IsNullOrWhiteSpace(cowtotalmilk.Text) &&
                                        !string.IsNullOrWhiteSpace(totalprice.Text) &&
                                        milkdate.Value != null;

            // Assuming that if a row in the dataGrid is selected, you're working with the DataGridView
            bool isDataGridRowSelected = dataGrid.SelectedRows.Count > 0;

            // Check which condition applies
            if (isTextboxDataEntered)
            {
                // Data is being entered from the textboxes
                InsertData();
                ClearFields();            }
            else if (isDataGridRowSelected)
            {
                // Data is being entered from the DataGridView
                SaveDataToDatabase();
            }
            else
            {
                // No valid input detected, show a warning message
                MessageBox.Show("Please enter data in the textboxes or select a row in the DataGridView.");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DeleteData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Update();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel20.Visible = !panel20.Visible;
            if (!panel20.Visible)
            {
                cartesianChart.Dock = DockStyle.Fill;
            }else if (panel20.Visible)
            {
                cartesianChart.Dock = DockStyle.None;
            }
        }

        private void cartesianChart_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {
            if (!panel20.Visible)
            {
                cartesianChart.Dock = DockStyle.Fill;
            }
            else if (panel20.Visible)
            {
                cartesianChart.Dock = DockStyle.None;
            }
        }

        private void percost_TextChanged(object sender, EventArgs e)
        {
            CalculateAndDisplayTotalPrice();
        }

        private void cowtotalmilk_TextChanged(object sender, EventArgs e)
        {
            CalculateAndDisplayTotalPrice();
        }

        private void CalculateAndDisplayTotalPrice()
        {
            if (int.TryParse(percost.Text, out int pricePerLtr) && int.TryParse(cowtotalmilk.Text, out int quantity))
            {
                int totalPrice = pricePerLtr * quantity;
                totalprice.Text = totalPrice.ToString();
            }
            else
            {
                totalprice.Text = "0"; // Default value if the inputs are not valid integers
            }
        }


        private void exit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit(); // Closes the entire application
            }
        }
    }
}
