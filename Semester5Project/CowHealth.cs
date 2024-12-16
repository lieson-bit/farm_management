using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Semester5Project

{
    public partial class CowHealth : Form
    {

        private DataTable cowHealthData = new DataTable(); // To store cow health data with check state
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
           int nLeftRect,
           int nTopRect,
           int nRightRect,
           int nBottomRect,
           int nWidthEllipse,
           int nHeightEllipse
       );
        public CowHealth()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            InitializePanelEvents(); // Initialize events for the panels
            dataGrid.CellContentClick += DataGrid_CellContentClick;
            dataGrid.CellContentClick += dataGrid_CellContentClick_1;
        }


        private void DataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is a checkbox
            if (e.ColumnIndex == dataGrid.Columns["Select"].Index && e.RowIndex >= 0)
            {
                // Get the checkbox cell
                DataGridViewCheckBoxCell checkBoxCell = (DataGridViewCheckBoxCell)dataGrid.Rows[e.RowIndex].Cells["Select"];
                checkBoxCell.Value = !(checkBoxCell.Value != null && (bool)checkBoxCell.Value);

                // Change the row color based on checkbox status
                if ((bool)checkBoxCell.Value) // Checkbox checked
                {
                    dataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;

                }
                else // Checkbox unchecked
                {
                    dataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White; // Reset to default color
                }
            }
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

            //panel4.MouseEnter += new EventHandler(Panel_MouseEnter);
            //panel4.MouseLeave += new EventHandler(Panel_MouseLeave);
            //panel4.Click += new EventHandler(Panel_Click);

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

        private void CowHealth_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'databaseDataSet4.CowHealthTab' table. You can move, or remove it, as needed.
            //this.cowHealthTabTableAdapter.Fill(this.databaseDataSet4.CowHealthTab);
            LoadData();

        }

        private void label3_Click(object sender, EventArgs e)
        {
            MilkPro Ob = new MilkPro();
            Ob.Show();
            this.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Cows Ob = new Cows();
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

        private void label8_Click(object sender, EventArgs e)
        {
            DashBoard Ob = new DashBoard();
            Ob.Show();
            this.Hide();
        }



        private void LoadData()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                {
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    string query = "SELECT [CowID], [CowName], [Event], [Treatment], [CostTreatm], [VetName], [Diagnosis], [Date] FROM CowHealthTab";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, cn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Ensure checkbox column is added only once
                        if (!dataGrid.Columns.Contains("Select"))
                        {
                            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
                            checkBoxColumn.HeaderText = "Select";
                            checkBoxColumn.Name = "Select";
                            dataGrid.Columns.Add(checkBoxColumn);
                        }

                        // Set the data source
                        dataGrid.DataSource = dt;

                        // Sync the checkbox with Diagnosis value
                        foreach (DataGridViewRow row in dataGrid.Rows)
                        {
                            if (row.Cells["Diagnosis"].Value != null)
                            {
                                bool isChecked = row.Cells["Diagnosis"].Value.ToString().ToLower() == "yes";
                                row.Cells["Select"].Value = isChecked; // Sync checkbox based on Diagnosis
                                row.DefaultCellStyle.BackColor = isChecked ? Color.LightGreen : Color.White;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }



        private void dataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the event is triggered by the checkbox column
            if (e.ColumnIndex == dataGrid.Columns["Select"].Index && e.RowIndex >= 0)
            {
                DataGridViewCheckBoxCell checkBoxCell = (DataGridViewCheckBoxCell)dataGrid.Rows[e.RowIndex].Cells["Select"];
                bool isChecked = Convert.ToBoolean(checkBoxCell.Value);

                int cowId = Convert.ToInt32(dataGrid.Rows[e.RowIndex].Cells["CowID"].Value);

                // Immediately update the Diagnosis column in the database
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                {
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    string updateQuery = "UPDATE CowHealthTab SET [Diagnosis] = @Diagnosis WHERE [CowID] = @CowID";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, cn))
                    {
                        cmd.Parameters.AddWithValue("@Diagnosis", isChecked ? "no" : "yes"); // Toggle between yes/no
                        cmd.Parameters.AddWithValue("@CowID", cowId);
                        cmd.ExecuteNonQuery();

                        // Update the UI
                        dataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = isChecked ? Color.White : Color.LightGreen;
                        checkBoxCell.Value = !isChecked; // Toggle checkbox state
                    }
                }
            }
        }



        private void InsertData()
{
    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
    {
        if (cn.State == ConnectionState.Closed)
            cn.Open();

        // Check if the cow with the given ID and name exists in the Cow table
        using (SqlCommand checkCowCmd = new SqlCommand("SELECT COUNT(1) FROM Cow WHERE CAST([CowName] AS NVARCHAR(MAX)) = @CowName AND [CowID] = @CowID", cn))
        {
            checkCowCmd.Parameters.AddWithValue("@CowID", Convert.ToInt32(cowid.Text));
            checkCowCmd.Parameters.AddWithValue("@CowName", cowname.Text);

            int cowExists = Convert.ToInt32(checkCowCmd.ExecuteScalar());

            if (cowExists == 0)
            {
                MessageBox.Show("The cow with the given ID and name does not exist. Please enter the correct cow details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        // Insert the data into the CowHealthTab
        using (SqlCommand cmd = new SqlCommand("INSERT INTO CowHealthTab ([CowID], [CowName], [Event], [Treatment], [CostTreatm], [VetName], [Diagnosis], [Date]) VALUES (@CowID, @CowName, @Event, @Treatment, @CostTreatm, @VetName, @Diagnosis, @Date)", cn))
        {
            cmd.Parameters.AddWithValue("@CowID", Convert.ToInt32(cowid.Text));
            cmd.Parameters.AddWithValue("@CowName", cowname.Text);
            cmd.Parameters.AddWithValue("@Event", cowevent.Text);
            cmd.Parameters.AddWithValue("@Treatment", cowtreatment.Text);
            cmd.Parameters.AddWithValue("@CostTreatm", cowcosttreatment.Text);
            cmd.Parameters.AddWithValue("@VetName", cowdoctorname.Text);

            // Check if the diagnosis is "yes" or "no" and store accordingly
            string diagnosisValue = cowdiagnosis.Text.Trim().ToLower() == "yes" ? "yes" : "no";
            cmd.Parameters.AddWithValue("@Diagnosis", diagnosisValue);

            cmd.Parameters.AddWithValue("@Date", cowdate.Value);

            cmd.ExecuteNonQuery();
            MessageBox.Show("Data Saved Successfully!");
            LoadData(); // Refresh the data after insert
        }
    }
}






        private void ClearFields()
        {
            cowid.Clear();
            cowname.Clear();
            cowevent.Clear();
            cowtreatment.Clear();
            cowcosttreatment.Clear();
            cowdoctorname.Clear();
            //cowdiagnosis.Clear();
            cowdate.Value = DateTime.Now; // Reset the DateTimePicker to the current date
        }

        private void DeleteData()
        {
            if (dataGrid.SelectedRows.Count > 0)
            {
                object cowIdValue = dataGrid.SelectedRows[0].Cells[0].Value;

                if (cowIdValue != null && int.TryParse(cowIdValue.ToString(), out int cowId))
                {
                    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                    {
                        if (cn.State == ConnectionState.Closed)
                            cn.Open();

                        using (SqlCommand cmd = new SqlCommand("DELETE FROM CowHealthTab WHERE [CowID] = @CowID", cn))
                        {
                            cmd.Parameters.AddWithValue("@CowID", cowId);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Data Deleted Successfully!");
                            LoadData(); // Refresh the data after deletion
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Cow ID selected. Please select a valid row.");
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
                int cowId = Convert.ToInt32(dataGrid.SelectedRows[0].Cells[0].Value);

                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                {
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    using (SqlCommand cmd = new SqlCommand("UPDATE CowHealthTab SET [CowName] = @CowName, [Event] = @Event, [Treatment] = @Treatment, [CostTreatm] = @CostTreatm, [VetName] = @VetName, [Diagnosis] = @Diagnosis, [Date] = @Date WHERE [CowID] = @CowID", cn))
                    {
                        cmd.Parameters.AddWithValue("@CowID", cowId);
                        cmd.Parameters.AddWithValue("@CowName", cowname.Text);
                        cmd.Parameters.AddWithValue("@Event", cowevent.Text);
                        cmd.Parameters.AddWithValue("@Treatment", cowtreatment.Text);
                        cmd.Parameters.AddWithValue("@CostTreatm",(cowcosttreatment.Text));
                        cmd.Parameters.AddWithValue("@VetName", cowdoctorname.Text);

                        // Check the checkbox value in the row and set "yes" or "no" in Diagnosis
                        string diagnosisValue = dataGrid.SelectedRows[0].Cells["Select"].Value != null && (bool)dataGrid.SelectedRows[0].Cells["Select"].Value ? "yes" : "no";
                        cmd.Parameters.AddWithValue("@Diagnosis", diagnosisValue);

                        cmd.Parameters.AddWithValue("@Date", cowdate.Value);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Data Updated Successfully!");
                        LoadData(); // Refresh the data after update
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.");
            }
        }


        private void button7_Click_1(object sender, EventArgs e)
        {
            InsertData();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DeleteData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void panel16_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGrid_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the event is triggered by the checkbox column
            if (e.ColumnIndex == dataGrid.Columns["Select"].Index && e.RowIndex >= 0)
            {
                DataGridViewCheckBoxCell checkBoxCell = (DataGridViewCheckBoxCell)dataGrid.Rows[e.RowIndex].Cells["Select"];
                bool isChecked = Convert.ToBoolean(checkBoxCell.Value);

                int cowId = Convert.ToInt32(dataGrid.Rows[e.RowIndex].Cells["idcow"].Value);

                // Immediately update the Diagnosis column in the database
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                {
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    string updateQuery = "UPDATE CowHealthTab SET [Diagnosis] = @Diagnosis WHERE [CowID] = @CowID";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, cn))
                    {
                        cmd.Parameters.AddWithValue("@Diagnosis", isChecked ? "no" : "yes"); // Toggle between yes/no
                        cmd.Parameters.AddWithValue("@CowID", cowId);
                        cmd.ExecuteNonQuery();

                        // Update the UI
                        dataGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = isChecked ? Color.White : Color.LightGreen;
                        checkBoxCell.Value = !isChecked; // Toggle checkbox state
                    }
                }
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
