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

namespace Semester5Project
{
    public partial class Cows : Form
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

        public Cows()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
            InitializePanelEvents(); // Initialize events for the panels
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;

        }

        private void LoadCowPicture(string cowId)
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                // Query to get cow details by Cow ID
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Cow WHERE Cowid = @CowID", cn))
                {
                    cmd.Parameters.AddWithValue("@CowID", cowId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Update the entry fields with cow data
                        cowid.Text = reader["Cowid"].ToString();
                        cowname.Text = reader["CowName"].ToString();
                        eartagcow.Text = reader["Eartag"].ToString();
                        cowcolor.Text = reader["Color"].ToString();
                        cowbleeding.Text = reader["Bleeding"].ToString();
                        cowage.Text = reader["Year"].ToString();

                        if (reader["DateofBirth"] != DBNull.Value)
                        {
                            cowdateofbirth.Value = Convert.ToDateTime(reader["DateofBirth"]);
                        }
                        else
                        {
                            cowdateofbirth.Value = DateTime.Now; // or any default date
                        }

                        if (reader["Picture"] != DBNull.Value)
                        {
                            byte[] imageBytes = (byte[])reader["Picture"];
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                photo.Image = Image.FromStream(ms);
                            }
                        }
                        else
                        {
                            photo.Image = null; // Clear the image if no picture is found
                        }
                    }
                }
            }
        }


        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row's Cow ID (assuming the first column contains the Cow ID)
                string selectedCowId = dataGridView1.SelectedRows[0].Cells["idcow"].Value.ToString();

                // Load and display the cow's picture
                LoadCowPicture(selectedCowId);
            }
        }


        private void Cows_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'databaseDataSet7.Cow' table. You can move, or remove it, as needed.
            this.cowTableAdapter1.Fill(this.databaseDataSet7.Cow);
            LoadData();
        }

        private void LoadData()
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Cow", cn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        // Assuming you have TextBoxes named txtCowID, txtCowName, etc.
                        cowid.Text = dt.Rows[0]["Cowid"].ToString();
                        cowname.Text = dt.Rows[0]["CowName"].ToString();
                        eartagcow.Text = dt.Rows[0]["Eartag"].ToString();
                        cowcolor.Text = dt.Rows[0]["Color"].ToString();
                        cowbleeding.Text = dt.Rows[0]["Bleeding"].ToString();
                        cowage.Text = dt.Rows[0]["Year"].ToString();
                        // Assuming `cowdateofbirth` is your `DateTimePicker` control
                        if (dt.Rows[0]["DateofBirth"] != DBNull.Value)
                        {
                            cowdateofbirth.Value = Convert.ToDateTime(dt.Rows[0]["DateofBirth"]);
                        }
                        else
                        {
                            // Handle the case where the date is null in the database if necessary
                            cowdateofbirth.Value = DateTime.Now; // or any default date
                        }


                        if (dt.Rows[0]["Picture"] != DBNull.Value)
                        {
                            byte[] imageBytes = (byte[])dt.Rows[0]["Picture"];
                            using (MemoryStream ms = new MemoryStream(imageBytes))
                            {
                                photo.Image = Image.FromStream(ms);
                            }
                        }
                    }
                }
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
                {
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    // Query to select all columns except Filename
                    string query = "SELECT [Cowid], [CowName], [Eartag], [Color], [Bleeding], [Year], [DateofBirth] FROM Cow";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, cn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }

           /* foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                MessageBox.Show(column.Name); // or use MessageBox.Show(column.Name);
            }*/
        }

        private void btnBrowseFilePath_Click(object sender, EventArgs e)
        {
            // Open a file dialog for the user to select a file
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*"; // You can set a filter to specify which types of files you want to allow

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Display the selected file path in the cowfilepath TextBox
                cowfilepath.Text = ofd.FileName;
            }
        }


        private void InsertData()
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO Cow ([Cowid], [CowName], [Eartag], [Color], [Bleeding], [Year], [DateofBirth], [Picture], [Filename]) VALUES (@CowID, @CowName, @EarTag, @Color, @Bleeding, @Year, @DateOfBirth, @Picture, @Filename)", cn))
                {
                    cmd.Parameters.AddWithValue("@CowID", cowid.Text);
                    cmd.Parameters.AddWithValue("@CowName", cowname.Text);
                    cmd.Parameters.AddWithValue("@EarTag", eartagcow.Text);
                    cmd.Parameters.AddWithValue("@Color", cowcolor.Text);
                    cmd.Parameters.AddWithValue("@Bleeding", cowbleeding.Text);
                    cmd.Parameters.AddWithValue("@Year", cowage.Text);
                    cmd.Parameters.AddWithValue("@DateOfBirth", cowdateofbirth.Value);

                    // Handle the picture
                    if (photo.Image != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            photo.Image.Save(ms, photo.Image.RawFormat);
                            byte[] imageBytes = ms.ToArray();
                            cmd.Parameters.AddWithValue("@Picture", imageBytes);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Picture", DBNull.Value);
                    }

                    // Handle the file path
                    if (!string.IsNullOrWhiteSpace(cowfilepath.Text))
                    {
                        cmd.Parameters.AddWithValue("@Filename", Path.GetFileName(cowfilepath.Text));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Filename", DBNull.Value);
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data Saved Successfully!");
                    LoadData(); // Refresh the data after insert
                }
            }
        }


        private void UpdateData()
        {
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                using (SqlCommand cmd = new SqlCommand("UPDATE Cow SET [CowName] = @CowName, [Eartag] = @EarTag, [Color] = @Color, [Bleeding] = @Bleeding, [Year] = @Year, [DateofBirth] = @DateOfBirth, [Picture] = @Picture, [Filename] = @Filename WHERE [CowID] = @CowID", cn))
                {
                    cmd.Parameters.AddWithValue("@CowID", cowid.Text);
                    cmd.Parameters.AddWithValue("@CowName", cowname.Text);
                    cmd.Parameters.AddWithValue("@EarTag", eartagcow.Text);
                    cmd.Parameters.AddWithValue("@Color", cowcolor.Text);
                    cmd.Parameters.AddWithValue("@Bleeding", cowbleeding.Text);
                    cmd.Parameters.AddWithValue("@Year", cowage.Text);
                    cmd.Parameters.AddWithValue("@DateOfBirth", cowdateofbirth.Value);

                    if (photo.Image != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            photo.Image.Save(ms, photo.Image.RawFormat);
                            byte[] imageBytes = ms.ToArray();
                            cmd.Parameters.AddWithValue("@Picture", imageBytes);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Picture", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@Filename", Path.GetFileName(cowfilepath.Text)); // You can use any file name logic here

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data Updated Successfully!");
                    LoadData(); // Refresh the data after update
                }
            }
        }

        private void DeleteData()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                Console.WriteLine(column.Name); // or use MessageBox.Show(column.Name);
            }


            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                string cowIdToDelete;

                // Check if a row is selected in the DataGridView
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Get the selected row's Cowid (assuming the first column contains the Cowid)
                    cowIdToDelete = dataGridView1.SelectedRows[0].Cells["idcow"].Value.ToString();
                }
                else
                {
                    // Fallback to using the Cowid from the text field
                    cowIdToDelete = cowid.Text;
                }


                using (SqlCommand cmd = new SqlCommand("DELETE FROM Cow WHERE [Cowid] = @CowID", cn))
                {
                    cmd.Parameters.AddWithValue("@CowID", cowIdToDelete);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data Deleted Successfully!");
                    ClearFields(); // Clear the fields after delete
                    LoadData(); // Refresh the data after delete
                }
            }
            
        }

        private void ClearFields()
        {
            cowid.Clear();
            cowname.Clear();
            eartagcow.Clear();
            cowcolor.Clear();
            cowbleeding.Clear();
            cowage.Clear();
            cowdateofbirth.Value = DateTime.Now;
            cowfilepath.Clear();
            photo.Image = null;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            // Browse and select an image file for the PictureBox
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                photo.Image = Image.FromFile(ofd.FileName);
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

        private void button7_Click(object sender, EventArgs e)
        {
            InsertData();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DeleteData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void btnupl_Click(object sender, EventArgs e)
        {
            // Browse and select an image file for the PictureBox
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                photo.Image = Image.FromFile(ofd.FileName);
                cowfilepath.Text = ofd.FileName;
            }
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

        private void label8_Click(object sender, EventArgs e)
        {
            DashBoard Ob = new DashBoard();
            Ob.Show();
            this.Hide();
        }

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.cowTableAdapter1.FillBy(this.databaseDataSet7.Cow);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void fillByToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

     

        private void btnSearch_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit(); // Closes the entire application
            }
        }

        private void searchcow_TextChanged(object sender, EventArgs e)
        {
           using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["Semester5Project.Properties.Settings.DatabaseConnectionString"].ConnectionString))
    {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();

                // SQL query for case-insensitive search using LIKE and wildcard search
                using (SqlCommand cmd = new SqlCommand(@"SELECT * FROM Cow 
                                                 WHERE [CowName] COLLATE SQL_Latin1_General_CP1_CI_AS LIKE '%' + @SearchTerm + '%' 
                                                 OR [Eartag] COLLATE SQL_Latin1_General_CP1_CI_AS LIKE '%' + @SearchTerm + '%'", cn))
                {
                    cmd.Parameters.AddWithValue("@SearchTerm", searchcow.Text);

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;

                        if (dt.Rows.Count == 0)
                        {
                            dataGridView1.DataSource = null; // Clear the grid if no match is found
                        }
                    }
                }
            }
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }
    }
}
