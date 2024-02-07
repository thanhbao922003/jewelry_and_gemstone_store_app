using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VBStore
{
    public partial class SuaTSForm : Form
    {
        private string maSanPham;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public SuaTSForm(string maSP)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            maSanPham = maSP;
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
                                cmbLoaiSP.Text = reader["TENLOAISANPHAM"].ToString();

                                // Load danh sách loại sản phẩm vào ComboBox
                                LoadLoaiSanPhamToComboBox();
                                // Chọn loại sản phẩm hiện tại của sản phẩm
                                cmbLoaiSP.SelectedValue = reader["MALOAISANPHAM"].ToString();
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

        private void LoadLoaiSanPhamToComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT MALOAISANPHAM, TENLOAISANPHAM FROM LOAISANPHAM";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);
                            // Bind dữ liệu vào ComboBox
                            cmbLoaiSP.DataSource = dataTable;
                            cmbLoaiSP.DisplayMember = "TENLOAISANPHAM";
                            cmbLoaiSP.ValueMember = "MALOAISANPHAM";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (IsDataValid()) // Kiểm tra xem đã nhập đủ dữ liệu hay không
            {
                // Thực hiện cập nhật dữ liệu vào cơ sở dữ liệu
                if (UpdateProductData())
                {
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // Đóng Form sau khi cập nhật thành công
                }
                else
                {
                    MessageBox.Show("Lỗi khi cập nhật dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin cần cập nhật!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool UpdateProductData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE SANPHAM " +
                                   "SET TENSP = @TenSP, MALOAISANPHAM = @MaLoaiSP, DONGIABAN = @DonGiaBan, DONGIAMUA = @DonGiaMua, SOLUONGTON = @SoLuongTon " +
                                   "WHERE MASANPHAM = @MaSanPham";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Đặt giá trị cho các tham số
                        command.Parameters.AddWithValue("@TenSP", txtTenSP.Text);
                        command.Parameters.AddWithValue("@MaLoaiSP", cmbLoaiSP.SelectedValue);
                        command.Parameters.AddWithValue("@DonGiaBan", txtDonGiaBan.Text);
                        command.Parameters.AddWithValue("@DonGiaMua", txtDonGiaMua.Text);
                        command.Parameters.AddWithValue("@SoLuongTon", txtSoLuongTon.Text);
                        command.Parameters.AddWithValue("@MaSanPham", maSanPham);

                        // Thực hiện câu lệnh SQL
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; // Trả về true nếu có ít nhất một dòng bị ảnh hưởng (cập nhật thành công)
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Trả về false nếu có lỗi xảy ra
            }
        }
        private bool IsDataValid()
        {
            // Kiểm tra xem đã nhập đủ thông tin hay không
            return !string.IsNullOrEmpty(txtTenSP.Text) && !string.IsNullOrEmpty(txtDonGiaBan.Text) &&
                   !string.IsNullOrEmpty(txtDonGiaMua.Text) && !string.IsNullOrEmpty(txtSoLuongTon.Text);
            // Thêm kiểm tra cho các control khác nếu cần
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
}
