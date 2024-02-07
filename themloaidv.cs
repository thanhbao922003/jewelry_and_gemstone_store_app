using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VBStore
{
    public partial class themloaidv : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public themloaidv()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            // Retrieve data from textboxes
            string maloaisanpham = maloaispTextBox.Text.Trim();
            string tenloaisanpham = tenloaispTextBox.Text.Trim();
            string madonvitinh = madonvitinhTextBox.Text.Trim();
            string loaida = loaidaTextBox.Text.Trim();
            string hang = hangTextBox.Text.Trim();
            string chatlieu = chatlieuTextBox.Text.Trim();
            decimal trongluong = Convert.ToDecimal(trongluongTextBox.Text);
            decimal loinuan = Convert.ToDecimal(loinhuantextbox.Text);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to insert data into the LOAISANPHAM table
                    string query = @"
                        INSERT INTO LOAISANPHAM (MALOAISANPHAM, TENLOAISANPHAM, MADONVITINH, LOAIDA, HANG, CHATLIEU, TRONGLUONG, LOINHUAN)
                        VALUES (@maloaisanpham, @tenloaisanpham, @madonvitinh, @loaida, @hang, @chatlieu, @trongluong, @loinuan)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@maloaisanpham", maloaisanpham);
                        command.Parameters.AddWithValue("@tenloaisanpham", tenloaisanpham);
                        command.Parameters.AddWithValue("@madonvitinh", madonvitinh);
                        command.Parameters.AddWithValue("@loaida", loaida);
                        command.Parameters.AddWithValue("@hang", hang);
                        command.Parameters.AddWithValue("@chatlieu", chatlieu);
                        command.Parameters.AddWithValue("@trongluong", trongluong);
                        command.Parameters.AddWithValue("@loinuan", loinuan);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Loại sản phẩm đã được thêm thành công.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Close the form or take additional actions if needed
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Thêm loại sản phẩm không thành công.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
