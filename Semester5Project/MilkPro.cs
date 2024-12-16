using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using LiveCharts.Dtos;
using System.Globalization;

namespace Semester5Project
{
    public partial class MilkPro : Form
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
        private string connectionString = ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString;

        public MilkPro()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            InitializePanelEvents(); // Initialize events for the panels

        }

        //blic double RemainingMilk { get; set; }
        private void LoadMilkData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT Date, TotalMilk, RemainingMilk FROM MilkTbl ORDER BY Date";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView2.DataSource = dt; // Assuming milkDataGridView is the grid for MilkTbl
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in LoadMilkData: {ex.Message}");
            }
        }



        private void InitializePanelEvents()
        {
            // Assign events for panels from panel2 to panel8
            //panel2.MouseEnter += new EventHandler(Panel_MouseEnter);
            //panel2.MouseLeave += new EventHandler(Panel_MouseLeave);
            //panel2.Click += new EventHandler(Panel_Click);

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

        private void button2_Click(object sender, EventArgs e)
        {
            panel20.Visible = !panel20.Visible;
        }


        private void MilkPro_Load(object sender, EventArgs e)
        {
            LoadData();
            //UpdateMilkTbl();
            LoadMilkData();
        }

        // Event handler to reload the MilkTbl when updated
        private void OnMilkTblUpdated()
        {
            LoadMilkData();
        }


        private void label2_Click(object sender, EventArgs e)
        {
            Cows Ob = new Cows();
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
            // Open the MilkSales form and subscribe to its event
            MilkSales milkSalesForm = new MilkSales();

            // Subscribe to the MilkTblUpdated event
            milkSalesForm.MilkTblUpdated += OnMilkTblUpdated;

            milkSalesForm.Show();
            this.Hide();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            Finance Ob = new Finance();
            Ob.Show();
            this.Hide();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            DashBoard Ob = new DashBoard();
            Ob.Show();
            this.Hide();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO MilkProTbl (CowID, CowName, TotalMilk, Date) 
                                     VALUES (@CowID, @CowName, @TotalMilk, @Date)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CowID", cowid.Text);
                        cmd.Parameters.AddWithValue("@CowName", cowname.Text);
                        // Convert cowtotalmilk.Text to a float
                        if (!int.TryParse(cowtotalmilk.Text, out int totalMilk))
                        {
                            MessageBox.Show("Invalid Total Milk value. Please enter a numeric value.");
                            return;
                        }
                        cmd.Parameters.AddWithValue("@TotalMilk", totalMilk);

                        //cmd.Parameters.AddWithValue("@Date", DateTime.ParseExact(cowdate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture));
                        if (DateTime.TryParseExact(cowdate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                        {
                            cmd.Parameters.AddWithValue("@Date", parsedDate);
                        }
                        else
                        {
                            string inputDate = cowdate.Text.Trim();
                            MessageBox.Show($"Trimmed input date: {inputDate}");

                            return;
                        }

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        UpdateMilkTbl(parsedDate, totalMilk, false);
                        UpdateTotalMilkInMilkTbl(parsedDate);

                        MessageBox.Show("Record inserted successfully.");
                        LoadData(); // Refresh the DataGridView
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE MilkProTbl 
                                     SET CowID = @CowID, 
                                         CowName = @CowName, 
                                         TotalMilk = @TotalMilk, 
                                         Date = @Date 
                                     WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", milkID.Text); // Assuming a hidden field or selected ID from a grid
                        cmd.Parameters.AddWithValue("@CowID", cowid.Text);
                        cmd.Parameters.AddWithValue("@CowName", cowname.Text);
                        cmd.Parameters.AddWithValue("@TotalMilk", cowtotalmilk.Text);
                        //cmd.Parameters.AddWithValue("@Date", DateTime.ParseExact(cowdate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture));
                        if (DateTime.TryParseExact(cowdate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                        {
                            cmd.Parameters.AddWithValue("@Date", parsedDate);
                        }
                        else
                        {
                            MessageBox.Show("Invalid date format. Please enter the date in the format dd.MM.yyyy.");
                            return;
                        }


                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        MessageBox.Show("Record updated successfully.");
                        LoadData(); // Refresh the DataGridView
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Fetch the Date and TotalMilk for the record to be deleted
                    string fetchQuery = @"SELECT [Date], [TotalMilk] FROM MilkProTbl WHERE Id = @Id";
                    DateTime parsedDate = DateTime.MinValue;
                    int totalMilk = 0;

                    using (SqlCommand fetchCmd = new SqlCommand(fetchQuery, conn))
                    {
                        fetchCmd.Parameters.AddWithValue("@Id", milkID.Text); // Assuming a hidden field or selected ID from a grid

                        conn.Open();
                        using (SqlDataReader reader = fetchCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                parsedDate = Convert.ToDateTime(reader["Date"]);
                                totalMilk = Convert.ToInt32(reader["TotalMilk"]);
                            }
                        }
                        conn.Close();
                    }

                    // If the record doesn't exist, show an error and exit
                    if (parsedDate == DateTime.MinValue)
                    {
                        MessageBox.Show("Record not found. Unable to delete.");
                        return;
                    }

                    // Delete the record from MilkProTbl
                    string deleteQuery = @"DELETE FROM MilkProTbl WHERE Id = @Id";
                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@Id", milkID.Text);

                        conn.Open();
                        deleteCmd.ExecuteNonQuery();
                        conn.Close();
                    }

                    // Update MilkTbl to reflect the deletion
                    UpdateMilkTbl(parsedDate, totalMilk, true);
                    UpdateTotalMilkInMilkTbl(parsedDate);

                    MessageBox.Show("Record deleted successfully.");
                    LoadData(); // Refresh the DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void UpdateTotalMilkInMilkTbl(DateTime date)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Calculate the sum of TotalMilk from MilkProTbl
                    string sumQuery = @"SELECT ISNULL(SUM(TotalMilk), 0) FROM MilkProTbl WHERE Date = @Date";
                    int totalMilkSum = 0;

                    using (SqlCommand sumCmd = new SqlCommand(sumQuery, conn))
                    {
                        sumCmd.Parameters.AddWithValue("@Date", date);
                        conn.Open();
                        totalMilkSum = Convert.ToInt32(sumCmd.ExecuteScalar());
                        conn.Close();
                    }

                    if (totalMilkSum == 0)
                    {
                        // Delete the record from MilkTbl if TotalMilk becomes 0
                        string deleteQuery = @"DELETE FROM MilkTbl WHERE Date = @Date";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@Date", date);
                            conn.Open();
                            deleteCmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                    else
                    {
                        // Update TotalMilk in MilkTbl
                        string updateQuery = @"UPDATE MilkTbl SET TotalMilk = @TotalMilk WHERE Date = @Date";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@TotalMilk", totalMilkSum);
                            updateCmd.Parameters.AddWithValue("@Date", date);
                            conn.Open();
                            updateCmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in UpdateTotalMilkInMilkTbl: {ex.Message}");
            }
        }


        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Load data for dataGrid (MilkProTbl)
                    string query = @"SELECT * FROM MilkProTbl";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        dataGrid.DataSource = table; // Assuming you have a DataGridView named dataGrid
                    }

                    // Load data for dataGridView2 (MilkTbl)
                    string query2 = @"SELECT * FROM MilkTbl ORDER BY Date ASC";
                    using (SqlDataAdapter adapter2 = new SqlDataAdapter(query2, conn))
                    {
                        DataTable table2 = new DataTable();
                        adapter2.Fill(table2);
                        dataGridView2.DataSource = table2; // Assuming you have a DataGridView named dataGridView2
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if a row is selected
            if (dataGrid.SelectedRows.Count > 0)
            {
                // Get the selected row's Cow ID (assuming the first column contains the Cow ID)
                string selectedCowId = dataGrid.SelectedRows[0].Cells["Id"].Value.ToString();        
            }
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGrid.Rows[e.RowIndex];
                milkID.Text = row.Cells["Id"].Value.ToString();
                cowid.Text = row.Cells["IDcow"].Value.ToString();
                cowname.Text = row.Cells["nameCow"].Value.ToString();
                cowtotalmilk.Text = row.Cells["TotalMilk"].Value.ToString();
                cowdate.Text = row.Cells["Date"].Value.ToString();
            }

        }

        private void ClearFields()
        {
            cowid.Clear();
            cowname.Clear();
            milkID.Clear();
            cowtotalmilk.Clear(); 
            cowdate.Value = DateTime.Now;            
        }

        private void dataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            btnInsert_Click(sender, e);
            //UpdateMilkTbl();
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            btnDelete_Click(sender, e);
            //UpdateMilkTbl();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            btnUpdate_Click(sender, e);
            //UpdateMilkTbl();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void UpdateMilkTbl(DateTime date, int totalMilk, bool isDeletion)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Get the most recent RemainingMilk value in MilkTbl
                    string recentQuery = @"SELECT TOP 1 RemainingMilk FROM MilkTbl WHERE Date <= @Date ORDER BY Date DESC";
                    int recentRemainingMilk = 0;

                    using (SqlCommand recentCmd = new SqlCommand(recentQuery, conn))
                    {
                        recentCmd.Parameters.AddWithValue("@Date", date);
                        conn.Open();
                        object result = recentCmd.ExecuteScalar();
                        conn.Close();

                        if (result != null)
                        {
                            recentRemainingMilk = Convert.ToInt32(result);
                        }
                    }

                    // Adjust RemainingMilk based on the operation (insertion or deletion)
                    int updatedRemainingMilk = isDeletion ? recentRemainingMilk - totalMilk : recentRemainingMilk + totalMilk;

                    // Check if the current date already exists in MilkTbl
                    string checkQuery = @"SELECT COUNT(*) FROM MilkTbl WHERE Date = @Date";
                    int count = 0;

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Date", date);
                        conn.Open();
                        count = (int)checkCmd.ExecuteScalar();
                        conn.Close();
                    }

                    if (count > 0)
                    {
                        // Update the RemainingMilk value for the existing record
                        string updateQuery = @"UPDATE MilkTbl SET RemainingMilk = @RemainingMilk WHERE Date = @Date";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@RemainingMilk", updatedRemainingMilk);
                            updateCmd.Parameters.AddWithValue("@Date", date);
                            conn.Open();
                            updateCmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                    else
                    {
                        // Insert a new record with updated RemainingMilk
                        string insertQuery = @"INSERT INTO MilkTbl (Date, TotalMilk, RemainingMilk) 
                                       VALUES (@Date, @TotalMilk, @RemainingMilk)";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@Date", date);
                            insertCmd.Parameters.AddWithValue("@TotalMilk", totalMilk);
                            insertCmd.Parameters.AddWithValue("@RemainingMilk", updatedRemainingMilk);
                            conn.Open();
                            insertCmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in UpdateMilkTbl: {ex.Message}");
            } 
        }



        public double RemainingMILK{ get; set; }

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
                        dataGridView2.DataSource = milkTable; // Assuming dataGridView2 is the table in MilkPro
                    }
                }
            }
        }

    }
}
