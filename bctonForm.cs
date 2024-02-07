using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace VBStore
{
    public partial class bctonForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public bctonForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            dis();
        }

        private void dis()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Truy vấn SQL để lấy tất cả dữ liệu từ bảng BAOCAOTON
                    string query = "SELECT * FROM BAOCAOTON";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                    // Tạo một DataTable để lưu trữ dữ liệu
                    DataTable dataTable = new DataTable();

                    // Đổ dữ liệu từ SqlDataAdapter vào DataTable
                    adapter.Fill(dataTable);

                    // Gán DataTable làm nguồn dữ liệu cho DataGridView hoặc thành phần giao diện khác
                    guna2DataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void findTextBox_TextChanged(object sender, EventArgs e)
        {
            string keyword = findTextBox.Text.Trim();
            DateTime fromDate = fromDateTimePicker.Value.Date;
            DateTime toDate = toDateTimePicker.Value.Date.AddDays(1); // Add one day to include the entire end date.

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Truy vấn SQL để lấy dữ liệu từ bảng BAOCAOTON với điều kiện tìm kiếm
                    string query = "SELECT * FROM BAOCAOTON WHERE (MABAOCAO LIKE @keyword OR MASANPHAM LIKE @keyword) AND NGAYLAPBAOCAO >= @fromDate AND NGAYLAPBAOCAO < @toDate";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    adapter.SelectCommand.Parameters.AddWithValue("@fromDate", fromDate);
                    adapter.SelectCommand.Parameters.AddWithValue("@toDate", toDate);

                    // Tạo một DataTable để lưu trữ dữ liệu
                    DataTable dataTable = new DataTable();

                    // Đổ dữ liệu từ SqlDataAdapter vào DataTable
                    adapter.Fill(dataTable);

                    // Gán DataTable làm nguồn dữ liệu cho DataGridView hoặc thành phần giao diện khác
                    guna2DataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            thembtcForm thembtc = new thembtcForm();
            thembtc.Show();
            dis();
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                string mabaocao = guna2DataGridView1.SelectedRows[0].Cells["MABAOCAO"].Value.ToString();
                suabtcForm suaCustomerForm = new suabtcForm(mabaocao);
                suaCustomerForm.ShowDialog();
                dis();
                // Sau khi sửa thông tin, load lại dữ liệu
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để sửa thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void delBtn_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                string mabaocao = guna2DataGridView1.SelectedRows[0].Cells["MABAOCAO"].Value.ToString();
                xoabctForm suaCustomerForm = new xoabctForm(mabaocao);
                suaCustomerForm.ShowDialog();
                dis();
                // Sau khi sửa thông tin, load lại dữ liệu
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để sửa thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            thongke thongke1 = new thongke();
            thongke1.Show();

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            dis();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            try
            {
                string keyword = findTextBox.Text.Trim();
                DateTime fromDate = fromDateTimePicker.Value.Date;
                DateTime toDate = toDateTimePicker.Value.Date.AddDays(1); // Add one day to include the entire end date.

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to get data from the BAOCAOTON table based on the date range
                    string query = "SELECT * FROM BAOCAOTON WHERE NGAYLAPBAOCAO >= @fromDate AND NGAYLAPBAOCAO < @toDate";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@fromDate", fromDate);
                    adapter.SelectCommand.Parameters.AddWithValue("@toDate", toDate);

                    // If a keyword is provided, add it to the query
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " AND (MABAOCAO LIKE @keyword OR MASANPHAM LIKE @keyword)";
                        adapter.SelectCommand.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    }

                    // Create a DataTable to store the data
                    DataTable dataTable = new DataTable();

                    // Fill the DataTable with data from the database
                    adapter.Fill(dataTable);

                    // Set the DataTable as the data source for the DataGridView or other UI component
                    guna2DataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
