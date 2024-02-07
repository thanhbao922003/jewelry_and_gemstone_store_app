using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace VBStore
{
    public partial class dmhForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private string sophieumuahang;

        public dmhForm()
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

                    // SQL query to join CT_PHIEUMUAHANG with PHIEUMUAHANG to get the desired data
                    string query = @"
                SELECT CT.SOPHIEUMUAHANG, CT.MASANPHAM, CT.SOLUONGMUA, CT.DONGIAMUA, CT.THANHTIEN, P.MAKHACHHANG, P.NGAYLAP
                FROM CT_PHIEUMUAHANG CT
                INNER JOIN PHIEUMUAHANG P ON CT.SOPHIEUMUAHANG = P.SOPHIEUMUAHANG";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Set the data source for guna2DataGridView1
                        guna2DataGridView1.DataSource = dataTable;
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
                    string query = @"
                SELECT CT.MASANPHAM, CT.SOLUONGMUA, CT.DONGIAMUA, CT.THANHTIEN, P.MAKHACHHANG, P.NGAYLAP
                FROM CT_PHIEUMUAHANG CT
                INNER JOIN PHIEUMUAHANG P ON CT.SOPHIEUMUAHANG = P.SOPHIEUMUAHANG";

                    if (!string.IsNullOrEmpty(findTextBox.Text))
                    {
                        string searchText = findTextBox.Text.Trim();

                        // Add a condition to filter based on MASANPHAM or MAKHACHHANG
                        query += $@"
                WHERE CT.MASANPHAM LIKE '%{searchText}%' OR P.MAKHACHHANG LIKE '%{searchText}%'";
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Set the data source for guna2DataGridView1
                        guna2DataGridView1.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void detailBtn_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.SelectedRows.Count > 0)
            {
                sophieumuahang = guna2DataGridView1.SelectedRows[0].Cells["SOPHIEUMUAHANG"].Value.ToString();
                phieumuahang Phieubh = new phieumuahang(sophieumuahang);
                Phieubh.Show();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để xem chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
