using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VBStore
{
    public partial class chitietTSForm : Form
    {
        private string maSanPham;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public chitietTSForm(string maSP)
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

        private void createBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
