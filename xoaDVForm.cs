using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VBStore
{
    public partial class xoaDVForm : Form
    {
        private string maLoaiDV;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public xoaDVForm(string maDV)
        {
            InitializeComponent();
            maLoaiDV = maDV;
            connectionString = dbHelper.ConnectionString;
            LoadCTDV();
        }

        private void LoadCTDV()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM DICHVU WHERE MALOAIDICHVU = @MaLoaiDV";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaLoaiDV", maLoaiDV);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtMaDV.Text = reader["MALOAIDICHVU"].ToString();
                                txtTenDV.Text = reader["TENDICHVU"].ToString();
                                txtDonGia.Text = reader["DONGIA"].ToString();
                                // Cập nhật các trường thông tin khác nếu cần
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy thông tin dịch vụ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close(); // Đóng form nếu không tìm thấy thông tin
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); // Đóng form nếu có lỗi
            }
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa dịch vụ này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (DeleteService(maLoaiDV))
                {
                    MessageBox.Show("Xóa dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // Đóng Form xoaDVForm sau khi xóa thành công
                }
                else
                {
                    // Không cần hiển thị thông báo ở đây, vì thông báo đã được xử lý trong hàm DeleteService
                }
            }
        }

        private bool DeleteService(string maLoaiDV)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Kiểm tra xem có ràng buộc khóa ngoại với bảng CT_PHIEUDICHVU hay không
                    string checkForeignKeyQuery = "SELECT COUNT(*) FROM CT_PHIEUDICHVU WHERE MALOAIDICHVU = @MaLoaiDV";
                    using (SqlCommand checkForeignKeyCommand = new SqlCommand(checkForeignKeyQuery, connection))
                    {
                        checkForeignKeyCommand.Parameters.AddWithValue("@MaLoaiDV", maLoaiDV);

                        int relatedDataCount = (int)checkForeignKeyCommand.ExecuteScalar();

                        if (relatedDataCount > 0)
                        {
                            // Nếu có dữ liệu liên quan, hiển thị thông báo và không xóa dịch vụ
                            MessageBox.Show("Không thể xóa dịch vụ vì có dữ liệu liên quan trong phiếu dịch vụ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false; // Trả về false để thông báo rằng xóa không thành công
                        }
                    }

                    // Nếu không có dữ liệu liên quan, thực hiện câu lệnh SQL để xóa dịch vụ
                    string query = "DELETE FROM DICHVU WHERE MALOAIDICHVU = @MaLoaiDV";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaLoaiDV", maLoaiDV);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0; // Trả về true nếu có ít nhất một dòng bị ảnh hưởng (xóa thành công)
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Trả về false nếu có lỗi xảy ra
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void xoaDVForm_Load(object sender, EventArgs e)
        {

        }
    }
}
