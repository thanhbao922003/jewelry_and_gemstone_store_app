using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace VBStore
{
    public partial class suaDVForm : Form
    {
        private string maLoaiDV;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        public suaDVForm(string maDV)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            maLoaiDV = maDV;
            
            LoadCTDV();
        }

        private void LoadCTDV()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM DICHVU WHERE MALOAIDICHVU = @MaLoaiDV";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaLoaiDV", maLoaiDV);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtMaDV.Text = reader["MALOAIDICHVU"].ToString();
                                txtTenDV.Text = reader["TENDICHVU"].ToString();
                                if (reader["DONGIA"] != DBNull.Value)
                                {
                                    txtDonGia.Text = reader["DONGIA"].ToString();
                                }
                                // Cập nhật các trường thông tin khác nếu cần
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy thông tin dịch vụ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close(); // Đóng form nếu không tìm thấy thông tin
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); // Đóng form nếu có lỗi
            }
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Câu lệnh SQL để cập nhật dịch vụ
                    string query = "UPDATE DICHVU SET DONGIA = @DonGia, TENDICHVU = @TenDV WHERE MALOAIDICHVU = @MaLoaiDV";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DonGia", decimal.Parse(txtDonGia.Text));
                        command.Parameters.AddWithValue("@TenDV", txtTenDV.Text);
                        command.Parameters.AddWithValue("@MaLoaiDV", txtMaDV.Text);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (!decimal.TryParse(txtDonGia.Text, out decimal donGia))
                        {
                            MessageBox.Show("Đơn giá phải là một số hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // Không tiếp tục nếu đơn giá không hợp lệ
                        }
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Sửa thông tin dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Đóng Form khi sửa thành công (tùy theo yêu cầu của bạn)
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Sửa thông tin dịch vụ thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
