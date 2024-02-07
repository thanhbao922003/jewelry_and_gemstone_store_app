using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VBStore
{
    public partial class chitietCustomerForm : Form
    {
        private string maKhachHang;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        public chitietCustomerForm(string maKH)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            maKhachHang = maKH;
            LoadCustomerDetails();
            

        }

        private void LoadCustomerDetails()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * " +
                                   "FROM KHACHHANG " + 
                                   "WHERE KHACHHANG.MAKHACHHANG = @MaKhachHang";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaKhachHang", maKhachHang);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Hiển thị chi tiết sản phẩm và loại sản phẩm trên Form
                                txtMaKH.Text = reader["MAKHACHHANG"].ToString();
                                txtTenKH.Text = reader["TENKH"].ToString();
                                txtDiaChi.Text = reader["DIACHI"].ToString();
                                txtSDT.Text = reader["SDT"].ToString();
                                txtEmail.Text = reader["EMAIL"].ToString();
                                // Thêm các control khác tương ứng
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
        private void createBtn_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
