using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace VBStore
{
    public partial class xoabctForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private string mabaocao;

        public xoabctForm(string MABAOCAO)
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

        private void guna2Button2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void delBtn_Click(object sender, EventArgs e)
        {
            // Hỏi người dùng xác nhận xóa
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa mã báo cáo này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                // Thực hiện xóa dữ liệu từ cơ sở dữ liệu dựa trên mã báo cáo
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM BAOCAOTON WHERE MABAOCAO = @maBaoCao";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@maBaoCao", mabaocao);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Xóa mã báo cáo thành công!");
                        this.Close(); // Đóng form sau khi xóa thành công
                    }
                    else
                    {
                        MessageBox.Show("Xóa mã báo cáo thất bại!");
                    }

                    connection.Close();
                }
            }
        }
    }
}
