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
    public partial class Finance : Form
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
        public Finance()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            InitializePanelEvents(); // Initialize events for the panels
            //UpdateExpenditure();
            //updateIncome();
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

            //panel7.MouseEnter += new EventHandler(Panel_MouseEnter);
            //panel7.MouseLeave += new EventHandler(Panel_MouseLeave);
            //panel7.Click += new EventHandler(Panel_Click);

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

        private void Finance_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'databaseDataSet11.IncomeTbl' table. You can move, or remove it, as needed.
            this.incomeTblTableAdapter.Fill(this.databaseDataSet11.IncomeTbl);
            // TODO: This line of code loads data into the 'databaseDataSet9.ExpenditureTbl' table. You can move, or remove it, as needed.
            this.expenditureTblTableAdapter.Fill(this.databaseDataSet9.ExpenditureTbl);

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

        private void label8_Click(object sender, EventArgs e)
        {
            DashBoard Ob = new DashBoard();
            Ob.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel20.Visible = !panel20.Visible;
        }

        private void expenditureTblBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void amount_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoadIncomeData()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                {
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    string query = "SELECT [Incid], [IncDate], [IncPurpose], [IncAmt], [Empid] FROM IncomeTbl";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, cn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Bind the DataTable to the GridInc DataGridView
                        GridInc.DataSource = dt;

                        // Optionally, rename the columns in the DataGridView to match the desired display names
                        GridInc.Columns["Incid"].HeaderText = "incid";
                        GridInc.Columns["IncDate"].HeaderText = "incDate";
                        GridInc.Columns["IncPurpose"].HeaderText = "incPurpose";
                        GridInc.Columns["IncAmt"].HeaderText = "incAmt";
                        GridInc.Columns["Empid"].HeaderText = "empid";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading income data: " + ex.Message);
            }
        }


        private void LoadExpenditureData()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                {
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    string query = "SELECT [Expid], [ExpDate], [ExpPurpose], [ExpAmount], [Empid] FROM ExpenditureTbl";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, cn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Bind the DataTable to the ExpGrid DataGridView
                        ExpGrid.DataSource = dt;

                        // Optionally, rename the columns in the DataGridView to match desired display names
                        ExpGrid.Columns["Expid"].HeaderText = "expid";
                        ExpGrid.Columns["ExpDate"].HeaderText = "ExpDate";
                        ExpGrid.Columns["ExpPurpose"].HeaderText = "expPurpose";
                        ExpGrid.Columns["ExpAmount"].HeaderText = "expAmount";
                        //ExpGrid.Columns["EmpidData"].HeaderText = "empidData";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }


        private void SaveExpenditure()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                {
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    string query = "INSERT INTO ExpenditureTbl (ExpDate, ExpPurpose, ExpAmount, Empid) VALUES (@ExpDate, @ExpPurpose, @ExpAmount, @Empid)";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@ExpDate", milkdate.Value); // Assuming 'date' is a DateTimePicker
                        cmd.Parameters.AddWithValue("@ExpPurpose", percost.Text);
                        cmd.Parameters.AddWithValue("@ExpAmount", Convert.ToInt32(amount.Text));
                        cmd.Parameters.AddWithValue("@Empid", Convert.ToInt32(employeeid.Text));

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Expenditure Saved Successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving expenditure: " + ex.Message);
            }
        }

        private void DeleteExpenditure()
        {
            if (ExpGrid.SelectedRows.Count > 0)
            {
                int expId = Convert.ToInt32(ExpGrid.SelectedRows[0].Cells["Expid"].Value);

                try
                {
                    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                    {
                        if (cn.State == ConnectionState.Closed)
                            cn.Open();

                        string query = "DELETE FROM ExpenditureTbl WHERE Expid = @Expid";

                        using (SqlCommand cmd = new SqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@Expid", expId);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Expenditure Deleted Successfully!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting expenditure: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private void UpdateExpenditure()
        {
            if (ExpGrid.SelectedRows.Count > 0)
            {
                int expId = Convert.ToInt32(ExpGrid.SelectedRows[0].Cells["Expid"].Value);

                try
                {
                    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                    {
                        if (cn.State == ConnectionState.Closed)
                            cn.Open();

                        string query = "UPDATE ExpenditureTbl SET ExpDate = @ExpDate, ExpPurpose = @ExpPurpose, ExpAmount = @ExpAmount, Empid = @Empid WHERE Expid = @Expid";

                        using (SqlCommand cmd = new SqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@Expid", expId);
                            cmd.Parameters.AddWithValue("@ExpDate", milkdate.Value); // Assuming 'date' is a DateTimePicker
                            cmd.Parameters.AddWithValue("@ExpPurpose", percost.Text);
                            cmd.Parameters.AddWithValue("@ExpAmount", Convert.ToInt32(amount.Text));
                            cmd.Parameters.AddWithValue("@Empid", Convert.ToInt32(employeeid.Text));

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Expenditure Updated Successfully!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating expenditure: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void save_Click(object sender, EventArgs e)
        {
            // Get the selected value or text from the combobox
            string selectedAction = combobox.SelectedItem.ToString(); // Adjust the name 'comboBox' to the actual combobox name

            // Check the selected action and call the appropriate function
            if (selectedAction == "Expenditure")
            {
                SaveExpenditure();  // Call SaveExpenditure if 'Expenditure' is selected
                LoadExpenditureData();
            }
            else if (selectedAction == "Income")
            {
                saveIncome(); // Call SaveIncome if 'Income' is selected
                LoadIncomeData();
            }
            else
            {
                MessageBox.Show("Please select a valid option from the combobox.");
            }
            
            
        }

        private void delete_Click(object sender, EventArgs e)
        {
            // Get the selected value or text from the combobox
            string selectedAction = combobox.SelectedItem.ToString(); // Adjust the name 'comboBox' to the actual combobox name

            // Check the selected action and call the appropriate function
            if (selectedAction == "Expenditure")
            {
                DeleteExpenditure();  // Call DeleteExpenditure if 'Expenditure' is selected
                LoadExpenditureData();
            }
            else if (selectedAction == "Income")
            {
                deleteIncome(); // Call DeleteIncome if 'Income' is selected
                LoadIncomeData();
            }
            else
            {
                MessageBox.Show("Please select a valid option from the combobox.");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Get the selected value or text from the combobox
            string selectedAction = combobox.SelectedItem.ToString(); // Adjust the name 'comboBox' to the actual combobox name

            // Check the selected action and call the appropriate function
            if (selectedAction == "Expenditure")
            {
                UpdateExpenditure();  // Call DeleteExpenditure if 'Expenditure' is selected
                LoadExpenditureData();
            }
            else if (selectedAction == "Income")
            {
                updateIncome(); // Call DeleteIncome if 'Income' is selected
                LoadIncomeData();
            }
            else
            {
                MessageBox.Show("Please select a valid option from the combobox.");
            }
           
        }
        

        private void saveIncome()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                {
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    string query = "INSERT INTO IncomeTbl (IncDate, IncPurpose, IncAmt, Empid) VALUES (@IncDate, @IncPurpose, @IncAmt, @Empid)";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        // Set parameters with the values from the UI
                        cmd.Parameters.AddWithValue("@IncDate", dateincome.Value);
                        cmd.Parameters.AddWithValue("@IncPurpose", purposeInc.Text);
                        cmd.Parameters.AddWithValue("@IncAmt", Convert.ToInt32(amountInc.Text));
                        cmd.Parameters.AddWithValue("@Empid", Convert.ToInt32(emploInc.Text));

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Income data saved successfully!");

                        // Optionally reload the data grid after saving
                        LoadIncomeData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving income data: " + ex.Message);
            }
        }

        private void deleteIncome()
        {
            try
            {
                // Ensure a row is selected in GridInc before attempting to delete
                if (GridInc.SelectedRows.Count > 0)
                {
                    // Retrieve the Incid value from the selected row
                    int incId = Convert.ToInt32(GridInc.SelectedRows[0].Cells["Incid"].Value);

                    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                    {
                        if (cn.State == ConnectionState.Closed)
                            cn.Open();

                        // SQL query to delete the row with the selected Incid
                        string query = "DELETE FROM IncomeTbl WHERE Incid = @Incid";

                        using (SqlCommand cmd = new SqlCommand(query, cn))
                        {
                            cmd.Parameters.AddWithValue("@Incid", incId);
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Income data deleted successfully!");

                            // Reload the data grid after deleting to refresh the data
                            LoadIncomeData();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a row to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting income data: " + ex.Message);
            }
        }


        private void updateIncome()
        {
            try
            {
                if (ExpGrid.SelectedRows.Count > 0)
                {
                    int incId = Convert.ToInt32(ExpGrid.SelectedRows[0].Cells["Incid"].Value);

                    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                    {
                        if (cn.State == ConnectionState.Closed)
                            cn.Open();

                        string query = "UPDATE IncomeTbl SET IncDate = @IncDate, IncPurpose = @IncPurpose, IncAmt = @IncAmt, Empid = @Empid WHERE Incid = @Incid";

                        using (SqlCommand cmd = new SqlCommand(query, cn))
                        {
                            // Set parameters with the updated values from the UI
                            cmd.Parameters.AddWithValue("@IncDate", dateincome.Value);
                            cmd.Parameters.AddWithValue("@IncPurpose", purposeInc.Text);
                            cmd.Parameters.AddWithValue("@IncAmt", Convert.ToInt32(amountInc.Text));
                            cmd.Parameters.AddWithValue("@Empid", Convert.ToInt32(emploInc.Text));
                            cmd.Parameters.AddWithValue("@Incid", incId);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Income data updated successfully!");

                            // Optionally reload the data grid after updating
                            LoadIncomeData();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a row to update.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating income data: " + ex.Message);
            }
        }

        void ClearField()
        {
            // Get the selected value or text from the combobox
            string selectedAction = combobox.SelectedItem.ToString(); // Adjust the name 'comboBox' to the actual combobox name

            // Check the selected action and call the appropriate function
            if (selectedAction == "Expenditure")
            {
                milkdate.Value = DateTime.Now;
                percost.Clear();
                amount.Clear();
                employeeid.Clear();
            }
            else if (selectedAction == "Income")
            {
                dateincome.Value = DateTime.Now;
                purposeInc.Clear();
                amountInc.Clear();
                emploInc.Clear();
            }
            else
            {
                MessageBox.Show("Please select a valid option from the combobox.");
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClearField();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit(); // Closes the entire application
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
