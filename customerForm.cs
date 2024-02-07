using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;


namespace VBStore
{
    public partial class customerForm : Form
    {
        private string sdt;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private ChildFormUtility childFormUtility;
        private decimal flag;
        public customerForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;

        }
        public customerForm(string sdt, decimal flag)
        {
            InitializeComponent();
            this.sdt = sdt;
            connectionString = dbHelper.ConnectionString;
            childFormUtility = new ChildFormUtility(this);
            this.flag = flag;

        }

        private void customerForm_Load(object sender, EventArgs e)
        {
            loadCustomer();
        }

        private void loadCustomer()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT MAKHACHHANG AS 'Mã khách hàng', " +
                                   "TENKH AS 'Tên khách hàng', " +
                                   "DIACHI AS 'Địa chỉ', " +
                                   "SDT AS 'Số điện thoại', " +
                                   "EMAIL AS 'Email' " +
                                   "FROM KHACHHANG ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            guna2DataGridView1.DataSource = dataTable;
                            // Đặt tên cho các cột
                            if (guna2DataGridView1.Columns.Count >= 7)
                            {
                                guna2DataGridView1.Columns[0].HeaderText = "Mã khách hàng";
                                guna2DataGridView1.Columns[1].HeaderText = "Tên khách hàng";
                                guna2DataGridView1.Columns[2].HeaderText = "Địa chỉ";
                                guna2DataGridView1.Columns[3].HeaderText = "Số điện thoại";
                                guna2DataGridView1.Columns[4].HeaderText = "Email";
                                guna2DataGridView1.Columns[5].HeaderText = "Sửa";
                                guna2DataGridView1.Columns[6].HeaderText = "Xóa";

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




        private void createBtn_Click(object sender, EventArgs e)
        {
            themCustomerForm themCustomerForm = new themCustomerForm();
            themCustomerForm.ShowDialog();
            loadCustomer();
        }



        private void detailBtn_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                string maKhachHang = guna2DataGridView1.SelectedRows[0].Cells["Mã khách hàng"].Value.ToString();
                chitietCustomerForm chiTietCustomerForm = new chitietCustomerForm(maKhachHang);
                chiTietCustomerForm.ShowDialog();

            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để xem chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                string maKhachHang = guna2DataGridView1.SelectedRows[0].Cells["Mã khách hàng"].Value.ToString();
                suaCustomerForm suaCustomerForm = new suaCustomerForm(maKhachHang);
                suaCustomerForm.ShowDialog();
                loadCustomer(); // Sau khi sửa thông tin, load lại dữ liệu
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để sửa thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void guna2Button2_Click_1(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = guna2DataGridView1.SelectedRows[0];
                string maKhachHang = selectedRow.Cells["Mã khách hàng"].Value.ToString();

                // Truyền mã sản phẩm vào Form XoaTSForm khi mở Form này
                xoaCustomerForm xoaCustomerForm = new xoaCustomerForm(maKhachHang);
                xoaCustomerForm.ShowDialog();
                loadCustomer();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để xóa.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void findTextBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = findTextBox.Text.Trim();

            // Perform the search based on the searchText
            if (!string.IsNullOrEmpty(searchText))
            {
                DataTable dataTable = new DataTable();

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "SELECT MAKHACHHANG AS 'Mã khách hàng', " +
                                       "TENKH AS 'Tên khách hàng', " +
                                       "DIACHI AS 'Địa chỉ', " +
                                       "SDT AS 'Số điện thoại', " +
                                       "EMAIL AS 'Email' " +
                                       "FROM KHACHHANG " +
                                       "WHERE MAKHACHHANG LIKE @searchText OR TENKH LIKE @searchText";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                            {
                                adapter.Fill(dataTable);
                            }
                        }
                    }

                    guna2DataGridView1.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // If the search text is empty, reload the full customer list
                loadCustomer();
            }
        }
    }
}
