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
    public partial class Bleeding : Form
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
        public Bleeding()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            InitializePanelEvents(); // Initialize events for the panels
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

            //panel5.MouseEnter += new EventHandler(Panel_MouseEnter);
            //panel5.MouseLeave += new EventHandler(Panel_MouseLeave);
            //panel5.Click += new EventHandler(Panel_Click);

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

        private void Bleeding_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'databaseDataSet5.Bleeding' table. You can move, or remove it, as needed.
            //this.bleedingTableAdapter.Fill(this.databaseDataSet5.Bleeding);
            LoadData();

        }

        private void LoadData()
        {
            // This part loads a single record from the Bleeding table into UI controls.
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Bleeding", cn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        cowid.Text = dt.Rows[0]["CowID"].ToString();
                        cowname.Text = dt.Rows[0]["Cow Name"].ToString();

                        if (dt.Rows[0]["Calving Date"] != DBNull.Value)
                        {
                            calvdate.Value = Convert.ToDateTime(dt.Rows[0]["Calving Date"]);
                        }

                        if (dt.Rows[0]["Bleeding Date"] != DBNull.Value)
                        {
                            bleedingdate.Value = Convert.ToDateTime(dt.Rows[0]["Bleeding Date"]);
                        }
                    }
                }
            }

            // This part loads all records from the Bleeding table into a DataGridView, excluding any unnecessary columns.
            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                {
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    // Query to select relevant columns
                    string query = "SELECT [CowID], [Cow Name], [Calving Date], [Bleeding Date] FROM Bleeding";

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
        }

        private void InsertData()
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Bleeding ([CowID], [Cow Name], [Calving Date], [Bleeding Date]) VALUES (@CowID, @CowName, @CalvingDate, @BleedingDate)", cn))
                {
                    cmd.Parameters.AddWithValue("@CowID", Convert.ToInt32(cowid.Text));
                    cmd.Parameters.AddWithValue("@CowName", cowname.Text);
                    cmd.Parameters.AddWithValue("@CalvingDate", calvdate.Value);
                    cmd.Parameters.AddWithValue("@BleedingDate", bleedingdate.Value);

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
            calvdate.Value = DateTime.Now; // Reset the DateTimePicker to the current date
            bleedingdate.Value = DateTime.Now; // Reset the DateTimePicker to the current date
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

                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Bleeding WHERE [CowID] = @CowID", cn))
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
                object cowIdValue = dataGrid.SelectedRows[0].Cells[0].Value;

                if (cowIdValue != null && int.TryParse(cowIdValue.ToString(), out int cowId))
                {
                    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                    {
                        if (cn.State == ConnectionState.Closed)
                            cn.Open();

                        using (SqlCommand cmd = new SqlCommand("UPDATE Bleeding SET [Cow Name] = @CowName, [Calving Date] = @CalvingDate, [Bleeding Date] = @BleedingDate WHERE [CowID] = @CowID", cn))
                        {
                            cmd.Parameters.AddWithValue("@CowID", cowId);
                            cmd.Parameters.AddWithValue("@CowName", cowname.Text);
                            cmd.Parameters.AddWithValue("@CalvingDate", calvdate.Value);
                            cmd.Parameters.AddWithValue("@BleedingDate", bleedingdate.Value);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Data Updated Successfully!");
                            LoadData(); // Refresh the data after update
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
                MessageBox.Show("Please select a row to update.");
            }
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

        private void button7_Click(object sender, EventArgs e)
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
