using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VBStore
{
    public partial class chitietDVForm : Form
    {
        private string maLoaiDV;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        public chitietDVForm(string maDV)
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
                                txtDonGia.Text = reader["DONGIA"].ToString();
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

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
