using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VBStore
{
    public partial class sualoaidv : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private string maloaisanpham;

        public sualoaidv(string maloaisanpham)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            this.maloaisanpham = maloaisanpham;

            // Load data of the specified maloaidichvu into textboxes
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to select data of the specified maloaidichvu
                    string query = "SELECT * FROM LOAISANPHAM WHERE MALOAISANPHAM = @maloaisanpham";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@maloaisanpham", maloaisanpham);

                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            maloaispTextBox.Text = reader["MALOAISANPHAM"].ToString();
                            tenloaispTextBox.Text = reader["TENLOAISANPHAM"].ToString();
                            madonvitinhTextBox.Text = reader["MADONVITINH"].ToString();
                            loaidaTextBox.Text = reader["LOAIDA"].ToString();
                            hangTextBox.Text = reader["HANG"].ToString();
                            chatlieuTextBox.Text = reader["CHATLIEU"].ToString();
                            trongluongTextBox.Text = reader["TRONGLUONG"].ToString();
                            loinhuantextbox.Text = reader["LOINHUAN"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            // Retrieve data from textboxes
            string tenloaisanpham = tenloaispTextBox.Text.Trim();
            string madonvitinh = madonvitinhTextBox.Text.Trim();
            string loaida = loaidaTextBox.Text.Trim();
            string hang = hangTextBox.Text.Trim();
            string chatlieu = chatlieuTextBox.Text.Trim();

            // Check if essential fields are not empty
            if (string.IsNullOrWhiteSpace(tenloaisanpham) || string.IsNullOrWhiteSpace(madonvitinh))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if numeric fields are valid
            if (!decimal.TryParse(trongluongTextBox.Text, out decimal trongluong) || !decimal.TryParse(loinhuantextbox.Text, out decimal loinuan))
            {
                MessageBox.Show("Vui lòng nhập số hợp lệ cho Trọng lượng và Lợi nhuận.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if MALOAISANPHAM is being changed to an existing value
                    string existingMaloai = CheckExistingMaloai(connection, maloaisanpham, tenloaisanpham);
                    if (!string.IsNullOrEmpty(existingMaloai))
                    {
                        MessageBox.Show($"Mã loại {existingMaloai} đã tồn tại.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // SQL query to update data in the LOAISANPHAM table
                    string query = @"
                UPDATE LOAISANPHAM
                SET TENLOAISANPHAM = @tenloaisanpham, MADONVITINH = @madonvitinh, LOAIDA = @loaida,
                    HANG = @hang, CHATLIEU = @chatlieu, TRONGLUONG = @trongluong, LOINHUAN = @loinuan
                WHERE MALOAISANPHAM = @maloaisanpham";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@tenloaisanpham", tenloaisanpham);
                        command.Parameters.AddWithValue("@madonvitinh", madonvitinh);
                        command.Parameters.AddWithValue("@loaida", loaida);
                        command.Parameters.AddWithValue("@hang", hang);
                        command.Parameters.AddWithValue("@chatlieu", chatlieu);
                        command.Parameters.AddWithValue("@trongluong", trongluong);
                        command.Parameters.AddWithValue("@loinuan", loinuan);
                        command.Parameters.AddWithValue("@maloaisanpham", maloaisanpham);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Thông tin loại sản phẩm đã được cập nhật thành công.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Close the form or take additional actions if needed
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật thông tin loại sản phẩm không thành công.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string CheckExistingMaloai(SqlConnection connection, string maloaisanpham, string tenloaisanpham)
        {
            // SQL query to check if the new MALOAISANPHAM is already in use
            string query = "SELECT MALOAISANPHAM FROM LOAISANPHAM WHERE MALOAISANPHAM = @maloaisanpham AND TENLOAISANPHAM != @tenloaisanpham";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@maloaisanpham", maloaisanpham);
                command.Parameters.AddWithValue("@tenloaisanpham", tenloaisanpham);

                object result = command.ExecuteScalar();

                return result != null ? result.ToString() : string.Empty;
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
