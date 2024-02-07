using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace VBStore
{
    public partial class thembtcForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public thembtcForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            LoadSanPhamComboBox(); // Load danh sách sản phẩm vào ComboBox
        }

        private void LoadSanPhamComboBox()
        {
            // Kết nối cơ sở dữ liệu và lấy danh sách mã sản phẩm
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT MASANPHAM FROM SANPHAM";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string maSanPham = reader["MASANPHAM"].ToString();
                    comboBoxMaSanPham.Items.Add(maSanPham);
                }

                reader.Close();
                connection.Close();
            }

            // Đặt chiều cao của drop-down list
            comboBoxMaSanPham.DropDownHeight = 150;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem tất cả các trường thông tin đã được nhập đầy đủ hay chưa
            if (string.IsNullOrEmpty(txtTonDau.Text) || string.IsNullOrEmpty(txtTonCuoi.Text) ||
                string.IsNullOrEmpty(txtSoLuongMua.Text) || string.IsNullOrEmpty(txtSoLuongBan.Text) ||
                comboBoxMaSanPham.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return; // Không thực hiện thêm nếu thông tin chưa đủ
            }

            string maBaoCao = GenerateMaBaoCao(); // Tạo mã báo cáo tồn mới
            DateTime ngayLapBaoCao = guna2DateTimePicker1.Value;
            string maSanPham = comboBoxMaSanPham.SelectedItem.ToString();
            int tonDau = int.Parse(txtTonDau.Text);
            int tonCuoi = int.Parse(txtTonCuoi.Text);
            int soLuongMuaVao = int.Parse(txtSoLuongMua.Text);
            int soLuongBanRa = int.Parse(txtSoLuongBan.Text);

            // Thực hiện thêm báo cáo tồn vào cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO BAOCAOTON (MABAOCAO, NGAYLAPBAOCAO, MASANPHAM, TONDAU, TONCUOI, SOLUONGMUAVAO, SOLUONGBANRA) " +
                               "VALUES (@MABAOCAO, @NGAYLAPBAOCAO, @MASANPHAM, @TONDAU, @TONCUOI, @SOLUONGMUAVAO, @SOLUONGBANRA)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MABAOCAO", maBaoCao);
                command.Parameters.AddWithValue("@NGAYLAPBAOCAO", ngayLapBaoCao);
                command.Parameters.AddWithValue("@MASANPHAM", maSanPham);
                command.Parameters.AddWithValue("@TONDAU", tonDau);
                command.Parameters.AddWithValue("@TONCUOI", tonCuoi);
                command.Parameters.AddWithValue("@SOLUONGMUAVAO", soLuongMuaVao);
                command.Parameters.AddWithValue("@SOLUONGBANRA", soLuongBanRa);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Thêm báo cáo tồn thành công!");
                    ClearInputs();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Thêm báo cáo tồn thất bại!");
                }

                connection.Close();
            }
        }

        private string GenerateMaBaoCao()
        {
            // Tạo mã báo cáo tồn mới dựa trên số lượng báo cáo tồn hiện có
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT MAX(MABAOCAO) FROM BAOCAOTON";
                SqlCommand command = new SqlCommand(query, connection);
                object result = command.ExecuteScalar();

                if (result != DBNull.Value)
                {
                    int maxMaBaoCao = int.Parse(result.ToString().Substring(2));
                    return "BC" + (maxMaBaoCao + 1).ToString("D3");
                }
                else
                {
                    return "BC001";
                }
            }
        }

        private void ClearInputs()
        {
            guna2DateTimePicker1.Value = DateTime.Now;
            comboBoxMaSanPham.SelectedIndex = -1;
            txtTonDau.Clear();
            txtTonCuoi.Clear();
            txtSoLuongMua.Clear();
            txtSoLuongBan.Clear();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
