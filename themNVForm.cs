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
    public partial class themNVForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public themNVForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            string taiKhoan = userTextBox.Text;
            string matKhau = passextBox.Text;

            // Kiểm tra xem TAIKHOAN đã tồn tại trong bảng DANGNHAP chưa
            if (TaiKhoanDaTonTai(taiKhoan))
            {
                MessageBox.Show("TAIKHOAN đã tồn tại. Vui lòng chọn một TAIKHOAN khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Nếu TAIKHOAN chưa tồn tại, bạn có thể thêm mới dữ liệu vào bảng DANGNHAP tại đây.
                if (ThemTaiKhoan(taiKhoan, matKhau))
                {
                    MessageBox.Show("Thêm tài khoản thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Thêm tài khoản không thành công. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private bool ThemTaiKhoan(string taiKhoan, string matKhau)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Tạo truy vấn SQL để thêm mới tài khoản và mật khẩu vào bảng DANGNHAP
                string query = "INSERT INTO DANGNHAP (TAIKHOAN, MATKHAU) VALUES (@taiKhoan, @matKhau)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@taiKhoan", taiKhoan);
                    cmd.Parameters.AddWithValue("@matKhau", matKhau);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Kiểm tra số dòng đã được thêm vào cơ sở dữ liệu
                    return rowsAffected > 0;
                }
            }
        }
        private bool TaiKhoanDaTonTai(string taiKhoan)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Tạo truy vấn SQL để kiểm tra xem TAIKHOAN đã tồn tại chưa
                string query = "SELECT COUNT(*) FROM DANGNHAP WHERE TAIKHOAN = @taiKhoan";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@taiKhoan", taiKhoan);

                    int count = (int)cmd.ExecuteScalar();

                    // Nếu count > 0, tức là TAIKHOAN đã tồn tại
                    return count > 0;
                }
            }
        }
    }
}
