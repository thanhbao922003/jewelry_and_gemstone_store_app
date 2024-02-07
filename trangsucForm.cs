using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using Guna.UI2.AnimatorNS;

namespace VBStore
{
    public partial class trangsucForm : Form
    {
        private string sdt;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private decimal flag;

        public trangsucForm(decimal flag)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            this.flag = flag;
        }

        private void trangsucForm_Load(object sender, EventArgs e)
        {
            LoadProducts(); // Gọi phương thức để load dữ liệu sản phẩm
            role_load();
        }
        void role_load()
        {
            if (flag == 0)
            {
                createBtn.Visible = false;

                guna2Button1.Visible = false;
                guna2Button2.Visible = false;
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Thay đổi điều kiện để chỉ lấy sản phẩm có mã LSP01 hoặc LSP02
                    string query = "SELECT SANPHAM.MASANPHAM AS 'Mã sản phẩm', " +
                                   "TENSP AS 'Tên sản phẩm', " +
                                   "DONGIABAN AS 'Đơn giá bán', " +
                                   "DONGIAMUA AS 'Đơn giá mua', " +
                                   "SOLUONGTON AS 'Số lượng tồn' " +
                                   "FROM SANPHAM INNER JOIN LOAISANPHAM ON SANPHAM.MALOAISANPHAM = LOAISANPHAM.MALOAISANPHAM " +
                                   "WHERE SANPHAM.MALOAISANPHAM IN ('LSP005')";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            guna2DataGridView1.DataSource = dataTable;
                            // Đặt tên cho các cột
                            if (guna2DataGridView1.Columns.Count >= 5)
                            {
                                guna2DataGridView1.Columns[0].HeaderText = "Mã sản phẩm";
                                guna2DataGridView1.Columns[1].HeaderText = "Tên sản phẩm";
                                guna2DataGridView1.Columns[2].HeaderText = "Đơn giá bán";
                                guna2DataGridView1.Columns[3].HeaderText = "Đơn giá mua";
                                guna2DataGridView1.Columns[4].HeaderText = "Số lượng tồn";
                            }

                            // Đổ dữ liệu vào guna2DataGridView1

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void findTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Dynamically adjust the SQL query based on the entered text
                    string query = "SELECT SANPHAM.MASANPHAM AS 'Mã sản phẩm', " +
                                   "TENSP AS 'Tên sản phẩm', " +
                                   "DONGIABAN AS 'Đơn giá bán', " +
                                   "DONGIAMUA AS 'Đơn giá mua', " +
                                   "SOLUONGTON AS 'Số lượng tồn' " +
                                   "FROM SANPHAM INNER JOIN LOAISANPHAM ON SANPHAM.MALOAISANPHAM = LOAISANPHAM.MALOAISANPHAM " +
                                   "WHERE SANPHAM.MALOAISANPHAM IN ('LSP005')";

                    // Check if the findTextBox is not empty
                    if (!string.IsNullOrEmpty(findTextBox.Text))
                    {
                        // Add a condition to filter based on the product name
                        query += $" AND TENSP LIKE '%{findTextBox.Text}%'";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            guna2DataGridView1.DataSource = dataTable;

                            // Set column headers as before
                            if (guna2DataGridView1.Columns.Count >= 5)
                            {
                                guna2DataGridView1.Columns[0].HeaderText = "Mã sản phẩm";
                                guna2DataGridView1.Columns[1].HeaderText = "Tên sản phẩm";
                                guna2DataGridView1.Columns[2].HeaderText = "Đơn giá bán";
                                guna2DataGridView1.Columns[3].HeaderText = "Đơn giá mua";
                                guna2DataGridView1.Columns[4].HeaderText = "Số lượng tồn";
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

        private void btnQR_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                // Lấy mã sản phẩm từ hàng được chọn
                string maSanPham = guna2DataGridView1.SelectedRows[0].Cells["Mã sản phẩm"].Value.ToString();

                // Khởi tạo biến qrImageUrl để lưu URL ảnh QR
                string qrImageUrl = string.Empty;

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Truy vấn SQL để lấy URL ảnh QR dựa trên MASANPHAM
                        string query = "SELECT MAQR FROM SANPHAM WHERE MASANPHAM = @MaSanPham";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@MaSanPham", maSanPham);
                            qrImageUrl = command.ExecuteScalar()?.ToString();
                        }
                    }

                    if (!string.IsNullOrEmpty(qrImageUrl))
                    {
                        // Tạo thư mục nếu chưa tồn tại
                        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "qrcode");
                        Directory.CreateDirectory(folderPath);

                        // Tạo đường dẫn tới tệp ảnh QR trên máy tính
                        string qrImagePath = Path.Combine(folderPath, maSanPham + ".png");

                        // Tải ảnh QR về máy tính
                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(qrImageUrl, qrImagePath);
                        }

                        MessageBox.Show($"Ảnh QR đã được tải xuống và lưu tại: {qrImagePath}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy URL ảnh QR cho sản phẩm này.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để tải QR.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void createBtn_Click_1(object sender, EventArgs e)
        {
            ThemTSForm themTSForm = new ThemTSForm();
            themTSForm.ShowDialog();
            LoadProducts();
        }

        private void detailBtn_Click_1(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                string maSanPham = guna2DataGridView1.SelectedRows[0].Cells["Mã sản phẩm"].Value.ToString();
                chitietTSForm chiTietForm = new chitietTSForm(maSanPham);
                chiTietForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để xem chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                string maSanPham = guna2DataGridView1.SelectedRows[0].Cells["Mã sản phẩm"].Value.ToString();
                SuaTSForm suaTSForm = new SuaTSForm(maSanPham);
                suaTSForm.ShowDialog();
                LoadProducts(); // Sau khi sửa thông tin, load lại dữ liệu
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để sửa thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = guna2DataGridView1.SelectedRows[0];
                string maSanPham = selectedRow.Cells["Mã sản phẩm"].Value.ToString();

                // Truyền mã sản phẩm vào Form XoaTSForm khi mở Form này
                XoaTSForm xoaTSForm = new XoaTSForm(maSanPham);
                xoaTSForm.ShowDialog();
                LoadProducts();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để xóa.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void findTextBox_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Dynamically adjust the SQL query based on the entered text
                    string query = "SELECT SANPHAM.MASANPHAM AS 'Mã sản phẩm', " +
                                   "TENSP AS 'Tên sản phẩm', " +
                                   "DONGIABAN AS 'Đơn giá bán', " +
                                   "DONGIAMUA AS 'Đơn giá mua', " +
                                   "SOLUONGTON AS 'Số lượng tồn' " +
                                   "FROM SANPHAM INNER JOIN LOAISANPHAM ON SANPHAM.MALOAISANPHAM = LOAISANPHAM.MALOAISANPHAM " +
                                   "WHERE SANPHAM.MALOAISANPHAM IN ('LSP005')";

                    // Check if the findTextBox is not empty
                    if (!string.IsNullOrEmpty(findTextBox.Text))
                    {
                        // Add a condition to filter based on the product code (MASANPHAM) or product name (TENSP)
                        query += $" AND (SANPHAM.MASANPHAM LIKE '%{findTextBox.Text}%' OR TENSP LIKE '%{findTextBox.Text}%')";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            guna2DataGridView1.DataSource = dataTable;

                            // Set column headers as before
                            if (guna2DataGridView1.Columns.Count >= 5)
                            {
                                guna2DataGridView1.Columns[0].HeaderText = "Mã sản phẩm";
                                guna2DataGridView1.Columns[1].HeaderText = "Tên sản phẩm";
                                guna2DataGridView1.Columns[2].HeaderText = "Đơn giá bán";
                                guna2DataGridView1.Columns[3].HeaderText = "Đơn giá mua";
                                guna2DataGridView1.Columns[4].HeaderText = "Số lượng tồn";
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
    }
}