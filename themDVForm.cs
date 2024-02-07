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
    public partial class themDVForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        public themDVForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtMaDV_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTenDV_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDonGia_TextChanged(object sender, EventArgs e)
        {

        }

        private void createBtn_Click(object sender, EventArgs e)
        {// Lấy thông tin từ các controls trên form
            string maDV = txtMaDV.Text;
            string tenDV = txtTenDV.Text;
            string donGiaText = txtDonGia.Text;
            if (!decimal.TryParse(donGiaText, out decimal donGia))
            {
                // Nếu không thể chuyển đổi thành số decimal, hiển thị thông báo lỗi và không thực hiện thêm dịch vụ
                MessageBox.Show("Đơn giá không hợp lệ. Vui lòng nhập một số hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Không thực hiện thêm dịch vụ
            }

            // Thực hiện kiểm tra dữ liệu đầu vào nếu cần

            // Kiểm tra xem có nhập đầy đủ thông tin hay không
            if (string.IsNullOrWhiteSpace(maDV) || string.IsNullOrWhiteSpace(tenDV) || donGia <= 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin dịch vụ và đảm bảo giá trị đơn giá lớn hơn 0.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Không thực hiện thêm dịch vụ
            }

            // Kiểm tra xem mã dịch vụ có bắt đầu bằng "DV" hay không
            if (!maDV.StartsWith("DV"))
            {
                MessageBox.Show("Mã dịch vụ phải bắt đầu bằng 'DV'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Không thực hiện thêm dịch vụ
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Kiểm tra xem mã dịch vụ đã tồn tại trong cơ sở dữ liệu hay chưa
                    string checkMaDVQuery = "SELECT COUNT(*) FROM DICHVU WHERE MALOAIDICHVU = @MaDV";
                    using (SqlCommand checkMaDVCommand = new SqlCommand(checkMaDVQuery, connection))
                    {
                        checkMaDVCommand.Parameters.AddWithValue("@MaDV", maDV);

                        int existingCount = (int)checkMaDVCommand.ExecuteScalar();

                        if (existingCount > 0)
                        {
                            MessageBox.Show("Mã dịch vụ đã tồn tại trong cơ sở dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; // Không thực hiện thêm dịch vụ
                        }
                    }

                    // Câu lệnh SQL để thêm dịch vụ
                    string query = "INSERT INTO DICHVU (MALOAIDICHVU, TENDICHVU, DONGIA) " +
                                   "VALUES (@MaDV, @TenDV, @DonGia)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaDV", maDV);
                        command.Parameters.AddWithValue("@TenDV", tenDV);
                        command.Parameters.AddWithValue("@DonGia", donGia);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Thêm dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Đóng Form khi thêm thành công (tùy theo yêu cầu của bạn)
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Thêm dịch vụ thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
