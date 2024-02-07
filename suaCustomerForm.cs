using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VBStore
{
    public partial class suaCustomerForm : Form
    {
        private string maKhachHang;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public suaCustomerForm(string maKH)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            maKhachHang = maKH;
            LoadCustomerDetails();
        }

        private void LoadCustomerDetails()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * " +
                                   "FROM KHACHHANG " +
                                   "WHERE KHACHHANG.MAKHACHHANG = @MaKhachHang";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtMaKH.Text = reader["MAKHACHHANG"].ToString();
                                txtTenKH.Text = reader["TENKH"].ToString();
                                txtDiaChi.Text = reader["DIACHI"].ToString();
                                txtSDT.Text = reader["SDT"].ToString();
                                txtEmail.Text = reader["EMAIL"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            if (IsDataValid())
            {
                if (UpdateCustomerData())
                {
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Lỗi khi cập nhật dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        private bool UpdateCustomerData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE KHACHHANG " +
                                   "SET TENKH = @TenKH, DIACHI = @DiaChi, SDT = @SDT, EMAIL = @Email " +
                                   "WHERE MAKHACHHANG = @MaKhachHang";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TenKH", txtTenKH.Text);
                        command.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);
                        command.Parameters.AddWithValue("@SDT", txtSDT.Text);
                        command.Parameters.AddWithValue("@Email", txtEmail.Text);
                        command.Parameters.AddWithValue("@MaKhachHang", maKhachHang);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool IsDataValid()
        {
            if (string.IsNullOrEmpty(txtTenKH.Text) || string.IsNullOrEmpty(txtDiaChi.Text) ||
                string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtSDT.Text))
            {
                return false;
            }

            // Kiểm tra ràng buộc: email và sdt không trùng với khách hàng khác
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) " +
                                   "FROM KHACHHANG " +
                                   "WHERE MAKHACHHANG != @MaKhachHang " +
                                   "AND (EMAIL = @Email OR SDT = @SDT)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                        command.Parameters.AddWithValue("@Email", txtEmail.Text);
                        command.Parameters.AddWithValue("@SDT", txtSDT.Text);

                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("Email hoặc SĐT đã tồn tại cho khách hàng khác!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
