using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VBStore
{
    public partial class xoaDQForm : Form
    {
        private string maSanPham;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public xoaDQForm(string maSP)
        {
            InitializeComponent();
            maSanPham = maSP;
            connectionString = dbHelper.ConnectionString;
            LoadProductDetails();
        }

        private void LoadProductDetails()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT SANPHAM.*, LOAISANPHAM.TENLOAISANPHAM " +
                                   "FROM SANPHAM INNER JOIN LOAISANPHAM ON SANPHAM.MALOAISANPHAM = LOAISANPHAM.MALOAISANPHAM " +
                                   "WHERE SANPHAM.MASANPHAM = @MaSanPham";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaSanPham", maSanPham);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Hiển thị chi tiết sản phẩm và loại sản phẩm trên Form
                                txtMaSP.Text = reader["MASANPHAM"].ToString();
                                txtTenSP.Text = reader["TENSP"].ToString();
                                txtDonGiaBan.Text = reader["DONGIABAN"].ToString();
                                txtDonGiaMua.Text = reader["DONGIAMUA"].ToString();
                                txtSoLuongTon.Text = reader["SOLUONGTON"].ToString();
                                txtLoaiSP.Text = reader["TENLOAISANPHAM"].ToString();
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

        private void btnXoa_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Thực hiện câu lệnh SQL để xóa sản phẩm
                if (DeleteProduct(maSanPham))
                {
                    MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // Đóng Form XoaTSForm sau khi xóa thành công
                }
                else
                {
                    MessageBox.Show("Lỗi khi xóa sản phẩm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool DeleteProduct(string maSanPham)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Thực hiện câu lệnh SQL để xóa sản phẩm
                    string query = "DELETE FROM SANPHAM WHERE MASANPHAM = @MaSanPham";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaSanPham", maSanPham);
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

        private void createBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
