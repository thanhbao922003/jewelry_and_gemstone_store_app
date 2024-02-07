using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VBStore
{
    public partial class loaidichvu : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private string maloaisanpham;

        public loaidichvu()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;

            // Load all data from LOAISANPHAM table when the form is loaded
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to select all data from LOAISANPHAM
                    string query = "SELECT * FROM LOAISANPHAM";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                    // Create a DataTable to store the data
                    DataTable dataTable = new DataTable();

                    // Fill the DataTable with data from the SqlDataAdapter
                    adapter.Fill(dataTable);

                    // Set the DataTable as the data source for the DataGridView
                    dataGridViewLoaiDV.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void findTextBox_TextChanged(object sender, EventArgs e)
        {
            string keyword = findTextBox.Text.Trim();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to search for data in LOAISANPHAM based on MALOAISANPHAM or TENLOAISANPHAM
                    string query = "SELECT * FROM LOAISANPHAM WHERE MALOAISANPHAM LIKE @keyword OR TENLOAISANPHAM LIKE @keyword";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@keyword", "%" + keyword + "%");

                    // Create a DataTable to store the search results
                    DataTable dataTable = new DataTable();

                    // Fill the DataTable with data from the SqlDataAdapter
                    adapter.Fill(dataTable);

                    // Set the DataTable as the data source for the DataGridView
                    dataGridViewLoaiDV.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            themloaidv themloaidv = new themloaidv();
            themloaidv.ShowDialog();

            // Refresh the data in the DataGridView after adding a new record
            LoadData();
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            // Check if any row is selected
            if (dataGridViewLoaiDV.SelectedRows.Count > 0)
            {
                // Get the selected maloaisanpham
                maloaisanpham = dataGridViewLoaiDV.SelectedRows[0].Cells["MALOAISANPHAM"].Value.ToString();

                // Open the sualoaidv form with the specified maloaisanpham
                sualoaidv sualoaidvForm = new sualoaidv(maloaisanpham);
                sualoaidvForm.ShowDialog();

                // Refresh the data in the DataGridView after editing the record
                LoadData();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hàng để chỉnh sửa.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (dataGridViewLoaiDV.SelectedRows.Count > 0)
            {
                // Get the selected maloaisanpham
                maloaisanpham = dataGridViewLoaiDV.SelectedRows[0].Cells["MALOAISANPHAM"].Value.ToString();

                // Confirm with the user before deleting
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa loại sản phẩm này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Perform the delete operation
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            // SQL query to delete the selected row
                            string query = "DELETE FROM LOAISANPHAM WHERE MALOAISANPHAM = @maloaisanpham";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@maloaisanpham", maloaisanpham);

                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Loại sản phẩm đã được xóa thành công.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    // Refresh the data in the DataGridView after deletion
                                    LoadData();
                                }
                                else
                                {
                                    MessageBox.Show("Xóa loại sản phẩm không thành công.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hàng để xóa.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
