using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace VBStore
{
    public partial class suabtcForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private string mabaocao;

        public suabtcForm(string MABAOCAO)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            this.mabaocao = MABAOCAO;
            label9.Text = MABAOCAO;
            LoadSanPhamComboBox(); // Load danh sách sản phẩm vào ComboBox
            LoadData(); // Tải thông tin từ mã báo cáo đã chọn
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

        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM BAOCAOTON WHERE MABAOCAO = @maBaoCao";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@maBaoCao", mabaocao);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // Đọc thông tin từ SqlDataReader và gán vào các điều khiển trên suabtcForm
                    // Ví dụ:
                    guna2DateTimePicker1.Value = Convert.ToDateTime(reader["NGAYLAPBAOCAO"]);
                    string maSanPham = reader["MASANPHAM"].ToString();
                    txtTonDau.Text = reader["TONDAU"].ToString();
                    txtTonCuoi.Text = reader["TONCUOI"].ToString();
                    txtSoLuongMua.Text = reader["SOLUONGMUAVAO"].ToString();
                    txtSoLuongBan.Text = reader["SOLUONGBANRA"].ToString();

                    // Tìm và chọn mã sản phẩm tương ứng trong ComboBox
                    foreach (string item in comboBoxMaSanPham.Items)
                    {
                        if (item == maSanPham)
                        {
                            comboBoxMaSanPham.SelectedItem = item;
                            break;
                        }
                    }
                }

                reader.Close();
                connection.Close();
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ các điều khiển trên suabtcForm
            DateTime ngayLapBaoCao = guna2DateTimePicker1.Value;
            string maSanPham = comboBoxMaSanPham.SelectedItem.ToString();
            int tonDau = int.Parse(txtTonDau.Text);
            int tonCuoi = int.Parse(txtTonCuoi.Text);
            int soLuongMuaVao = int.Parse(txtSoLuongMua.Text);
            int soLuongBanRa = int.Parse(txtSoLuongBan.Text);

            // Thực hiện cập nhật thông tin báo cáo tồn vào cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE BAOCAOTON SET NGAYLAPBAOCAO = @NGAYLAPBAOCAO, MASANPHAM = @MASANPHAM, TONDAU = @TONDAU, TONCUOI = @TONCUOI, SOLUONGMUAVAO = @SOLUONGMUAVAO, SOLUONGBANRA = @SOLUONGBANRA WHERE MABAOCAO = @MABAOCAO";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MABAOCAO", mabaocao);
                command.Parameters.AddWithValue("@NGAYLAPBAOCAO", ngayLapBaoCao);
                command.Parameters.AddWithValue("@MASANPHAM", maSanPham);
                command.Parameters.AddWithValue("@TONDAU", tonDau);
                command.Parameters.AddWithValue("@TONCUOI", tonCuoi);
                command.Parameters.AddWithValue("@SOLUONGMUAVAO", soLuongMuaVao);
                command.Parameters.AddWithValue("@SOLUONGBANRA", soLuongBanRa);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Cập nhật thông tin báo cáo tồn thành công!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Cập nhật thông tin báo cáo tồn thất bại!");
                }

                connection.Close();
            }
        }

        private void guna2Button2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
        private decimal CalculateProfit(int soLuongMua, int soLuongBan, decimal donGiaMua, decimal donGiaBan)
        {
            return (soLuongBan * donGiaBan) - (soLuongMua * donGiaMua);
        }
    }
}
