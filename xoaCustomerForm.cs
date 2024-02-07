using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace VBStore
{
    public partial class xoaCustomerForm : Form
    {
        private string maKhachHang;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        public delegate void DataChangedEventHandler(string deletedCustomerId);
        public event DataChangedEventHandler DataChanged;
        public xoaCustomerForm(string maKH)
        {
            InitializeComponent();
            maKhachHang = maKH;
            connectionString = dbHelper.ConnectionString;
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
                                // Hiển thị chi tiết sản phẩm và loại sản phẩm trên Form
                                txtMaKH.Text = reader["MAKHACHHANG"].ToString();
                                txtTenKH.Text = reader["TENKH"].ToString();
                                txtDiaChi.Text = reader["DIACHI"].ToString();
                                txtSDT.Text = reader["SDT"].ToString();
                                txtEmail.Text = reader["EMAIL"].ToString();
                                // Thêm các control khác tương ứng
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
        private bool DeleteCustomerData(string maKhachHang)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Thực hiện câu lệnh SQL để xóa sản phẩm
                    string query = "DELETE FROM KHACHHANG WHERE MAKHACHHANG = @MaKhachHang";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; // Trả về true nếu có ít nhất một dòng bị ảnh hưởng (xóa thành công)
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    MessageBox.Show("Không thể xóa, vì sản phẩm có chứa với dữ liệu khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false; // Trả về false nếu có lỗi xảy ra
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Trả về false nếu có lỗi xảy ra
            }
        }
        private void ReloadParentForm(string deletedCustomerId)
        {
            // Kiểm tra xem sự kiện DataChanged đã được đăng ký hay chưa
            if (DataChanged != null)
            {
                // Gọi sự kiện để thông báo cho form cha biết rằng dữ liệu đã thay đổi
                DataChanged.Invoke(deletedCustomerId);
            }
        }


        private void editBtn_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Thực hiện câu lệnh SQL để xóa sản phẩm
                if (DeleteCustomerData(maKhachHang))
                {
                    MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ReloadParentForm(maKhachHang);
                    this.Close(); // Đóng Form XoaTSForm sau khi xóa thành công

                }
                else
                {
                    MessageBox.Show("Lỗi khi xóa khách hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
