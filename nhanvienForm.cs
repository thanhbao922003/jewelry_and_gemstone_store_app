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
    public partial class nhanvienForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public nhanvienForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
        }


        void load_nv()
        {
            // Khởi tạo kết nối đến cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Tạo truy vấn SQL để lấy dữ liệu từ bảng DANGNHAP
                string query = "SELECT TAIKHOAN, MATKHAU FROM DANGNHAP";

                // Tạo một đối tượng SqlDataAdapter để lấy dữ liệu từ truy vấn
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                // Tạo một DataTable để chứa dữ liệu
                DataTable dataTable = new DataTable();

                // Đổ dữ liệu từ SqlDataAdapter vào DataTable
                adapter.Fill(dataTable);

                // Gán DataTable làm nguồn dữ liệu cho guna2DataGridView1
                guna2DataGridView1.DataSource = dataTable;
            }
        }

        private void nhanvienForm_Load(object sender, EventArgs e)
        {
            load_nv();
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            themNVForm themNV = new themNVForm();
            themNV.Show();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có hàng nào được chọn không
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                // Lấy dòng đang được chọn
                DataGridViewRow selectedRow = guna2DataGridView1.SelectedRows[0];

                // Lấy giá trị của cột TAIKHOAN từ dòng đang được chọn
                string taiKhoan = selectedRow.Cells["TAIKHOAN"].Value.ToString();

                // Xác nhận xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa tài khoản này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Thực hiện xóa dữ liệu từ cơ sở dữ liệu
                    if (XoaTaiKhoan(taiKhoan))
                    {
                        // Xóa thành công, cập nhật lại DataGridView
                        load_nv();
                        MessageBox.Show("Xóa tài khoản thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Xóa tài khoản không thành công. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hàng để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private bool XoaTaiKhoan(string taiKhoan)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Tạo truy vấn SQL để xóa tài khoản từ bảng DANGNHAP
                string query = "DELETE FROM DANGNHAP WHERE TAIKHOAN = @taiKhoan";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@taiKhoan", taiKhoan);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Kiểm tra số dòng đã bị xóa
                    return rowsAffected > 0;
                }
            }
        }
    }
}
