using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VBStore
{
    public partial class themCustomerForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public themCustomerForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text) || string.IsNullOrWhiteSpace(phoneNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(emailTextBox.Text) || string.IsNullOrWhiteSpace(addressTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidEmail(emailTextBox.Text))
            {
                MessageBox.Show("Email không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM KHACHHANG WHERE SDT = @PhoneNumber OR EMAIL = @Email";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumberTextBox.Text);
                    checkCommand.Parameters.AddWithValue("@Email", emailTextBox.Text);

                    int existingCount = (int)checkCommand.ExecuteScalar();
                    if (existingCount > 0)
                    {
                        MessageBox.Show("Số điện thoại hoặc email đã tồn tại trong cơ sở dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Insert new customer record
                string insertQuery = "EXEC SP_INSERT_KHACHHANG @Ten, @DiaChi, @PhoneNumber,@Email;";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@Ten", nameTextBox.Text);
                    insertCommand.Parameters.AddWithValue("@DiaChi", addressTextBox.Text);
                    insertCommand.Parameters.AddWithValue("@PhoneNumber", phoneNumberTextBox.Text);
                    insertCommand.Parameters.AddWithValue("@Email", emailTextBox.Text);

                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Thông tin khách hàng đã được lưu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearTextBoxes();
                    }
                    else
                    {
                        MessageBox.Show("Không thể lưu thông tin khách hàng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ClearTextBoxes()
        {
            nameTextBox.Text = string.Empty;
            phoneNumberTextBox.Text = string.Empty;
            emailTextBox.Text = string.Empty;
            addressTextBox.Text = string.Empty;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
