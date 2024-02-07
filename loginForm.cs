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
    public partial class loginForm : Form
    {

        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private string maKH;
        private decimal flag = 0;
        public loginForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void loginbtn_Click(object sender, EventArgs e)
        {

            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            if (username == "admin" && password == "admin")
            {
                // Đăng nhập với quyền admin
                flag = 1;
                mainForm mainForm = new mainForm(flag);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                // Kết nối đến cơ sở dữ liệu và kiểm tra tài khoản trong bảng DANGNHAP
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT TAIKHOAN, MATKHAU FROM DANGNHAP WHERE TAIKHOAN = @username AND MATKHAU = @password";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Nếu có kết quả từ truy vấn, tức là tên đăng nhập và mật khẩu khớp
                        flag = 0;
                        mainForm mainForm = new mainForm(flag);
                        mainForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        // Nếu không có kết quả, tức là tên đăng nhập hoặc mật khẩu không chính xác
                        MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
