using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace VBStore
{
    public partial class capnhapdvForm : Form
    {
        private string sopHieuDichVu;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        public capnhapdvForm(string maDV)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            this.sopHieuDichVu = maDV;

            LoadCTDV();
        }

        private void LoadCTDV()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT " +
                                   "PHIEUDICHVU.SOPHIEUDICHVU, PHIEUDICHVU.NGAYLAP, PHIEUDICHVU.TINHTRANG, " +
                                   "CT_PHIEUDICHVU.MALOAIDICHVU, CT_PHIEUDICHVU.DONGIADICHVU, " +
                                   "CT_PHIEUDICHVU.DONGIADUOCTINH, CT_PHIEUDICHVU.SOLUONG, " +
                                   "CT_PHIEUDICHVU.THANHTIEN, CT_PHIEUDICHVU.TRATRUOC, " +
                                   "CT_PHIEUDICHVU.CONLAI, CT_PHIEUDICHVU.NGAYGIAO, CT_PHIEUDICHVU.TINHTRANG AS TRANGTHAI " +
                                   "FROM PHIEUDICHVU " +
                                   "INNER JOIN CT_PHIEUDICHVU ON PHIEUDICHVU.SOPHIEUDICHVU = CT_PHIEUDICHVU.SOPHIEUDICHVU " +
                                   "WHERE PHIEUDICHVU.SOPHIEUDICHVU = @SOPHIEUDICHVU";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SOPHIEUDICHVU", sopHieuDichVu);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            if (dataTable.Rows.Count > 0)
                            {
                                label10.Text = dataTable.Rows[0]["SOPHIEUDICHVU"].ToString();
                                guna2DateTimePicker1.Value = Convert.ToDateTime(dataTable.Rows[0]["NGAYLAP"]);
                                label14.Text = dataTable.Rows[0]["TINHTRANG"].ToString();

                                label13.Text = dataTable.Rows[0]["MALOAIDICHVU"].ToString();
                                txtDONGIADICHVU.Text = dataTable.Rows[0]["DONGIADICHVU"].ToString();
                                txtDONGIADUOCTINH.Text = dataTable.Rows[0]["DONGIADUOCTINH"].ToString();
                                txtSOLUONG.Text = dataTable.Rows[0]["SOLUONG"].ToString();
                                txtTHANHTIEN.Text = dataTable.Rows[0]["THANHTIEN"].ToString();
                                txtTRATRUOC.Text = dataTable.Rows[0]["TRATRUOC"].ToString();
                                txtCONLAI.Text = dataTable.Rows[0]["CONLAI"].ToString();

                                guna2DateTimePicker2.Value = Convert.ToDateTime(dataTable.Rows[0]["NGAYGIAO"]);
                                label16.Text = dataTable.Rows[0]["TRANGTHAI"].ToString();  // Display TRANGTHAI in label16
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy dữ liệu cho SOPHIEUDICHVU này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtTRATRUOC_TextChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem label14.Text đã là "Hoàn Thành" hay chưa
            if (label14.Text == "Hoàn Thành")
            {
                return; // Không thực hiện xử lý tiếp theo
            }

            // Kiểm tra nếu txtTRATRUOC không rỗng và có thể chuyển đổi thành số
            if (!string.IsNullOrEmpty(txtTRATRUOC.Text) && decimal.TryParse(txtTRATRUOC.Text, out decimal traTruoc))
            {
                // Lấy giá trị THANHTIEN từ txtTHANHTIEN (chắc chắn rằng bạn đảm bảo txtTHANHTIEN luôn chứa số hợp lệ)
                decimal thanhTien = decimal.Parse(txtTHANHTIEN.Text);

                // Tính giá trị còn lại (CONLAI)
                decimal conLai = thanhTien - traTruoc;

                // Kiểm tra nếu conLai nhỏ hơn 0, hiển thị thông báo lỗi
                if (conLai < 0)
                {
                    MessageBox.Show("Vui lòng nhập số tiền trả trước hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTRATRUOC.Text = string.Empty; // Xóa giá trị không hợp lệ
                    txtCONLAI.Text = string.Empty; // Xóa giá trị CONLAI
                    label14.Text = string.Empty; // Xóa giá trị TINHTRANG
                }
                else
                {
                    // Cập nhật giá trị CONLAI
                    txtCONLAI.Text = conLai.ToString();

                    // Đặt giá trị TINHTRANG dựa trên CONLAI
                    if (conLai == 0)
                    {
                        label14.Text = "Hoàn Thành";
                    }
                    else
                    {
                        label14.Text = "Chưa Hoàn Thành";
                    }
                }
            }
            else
            {
                // ... Xử lý trường hợp không thể chuyển đổi txtTRATRUOC thành số hợp lệ
            }
           
            
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Start a transaction to ensure data consistency
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        
                        // Update data in the PHIEUDICHVU table
                        string updatePhieuDichVuQuery = "UPDATE PHIEUDICHVU " +
                                                        "SET NGAYLAP = @NGAYLAP, TONGTIEN = @TONGTIEN, " +
                                                        "TINHTRANG = @TINHTRANG, SOTIENCONLAI = @SOTIENCONLAI, " +
                                                        "SOTIENTRATRUOC = @SOTIENTRATRUOC " +
                                                        "WHERE SOPHIEUDICHVU = @SOPHIEUDICHVU";

                        using (SqlCommand updatePhieuDichVuCommand = new SqlCommand(updatePhieuDichVuQuery, connection, transaction))
                        {
                            // Set parameters for the update query
                            updatePhieuDichVuCommand.Parameters.AddWithValue("@SOPHIEUDICHVU", label10.Text);
                            updatePhieuDichVuCommand.Parameters.AddWithValue("@NGAYLAP", guna2DateTimePicker1.Value);
                            updatePhieuDichVuCommand.Parameters.AddWithValue("@TONGTIEN", decimal.Parse(txtTHANHTIEN.Text));
                            updatePhieuDichVuCommand.Parameters.AddWithValue("@TINHTRANG", label14.Text);
                            updatePhieuDichVuCommand.Parameters.AddWithValue("@SOTIENCONLAI", decimal.Parse(txtCONLAI.Text));
                            updatePhieuDichVuCommand.Parameters.AddWithValue("@SOTIENTRATRUOC", decimal.Parse(txtTRATRUOC.Text));

                            // Execute the update query for PHIEUDICHVU
                            updatePhieuDichVuCommand.ExecuteNonQuery();
                        }

                        // Update data in the CT_PHIEUDICHVU table
                        string updateCTPhieuDichVuQuery = "UPDATE CT_PHIEUDICHVU " +
                                                          "SET DONGIADICHVU = @DONGIADICHVU, DONGIADUOCTINH = @DONGIADUOCTINH, " +
                                                          "SOLUONG = @SOLUONG, THANHTIEN = @THANHTIEN, " +
                                                          "TRATRUOC = @TRATRUOC, CONLAI = @CONLAI, NGAYGIAO = @NGAYGIAO, " +
                                                          "TINHTRANG = @TINHTRANG " +
                                                          "WHERE SOPHIEUDICHVU = @SOPHIEUDICHVU AND MALOAIDICHVU = @MALOAIDICHVU";

                        using (SqlCommand updateCTPhieuDichVuCommand = new SqlCommand(updateCTPhieuDichVuQuery, connection, transaction))
                        {
                            // Set parameters for the update query
                            updateCTPhieuDichVuCommand.Parameters.AddWithValue("@SOPHIEUDICHVU", label10.Text);
                            updateCTPhieuDichVuCommand.Parameters.AddWithValue("@MALOAIDICHVU", label13.Text);
                            updateCTPhieuDichVuCommand.Parameters.AddWithValue("@DONGIADICHVU", decimal.Parse(txtDONGIADICHVU.Text));
                            updateCTPhieuDichVuCommand.Parameters.AddWithValue("@DONGIADUOCTINH", decimal.Parse(txtDONGIADUOCTINH.Text));
                            updateCTPhieuDichVuCommand.Parameters.AddWithValue("@SOLUONG", int.Parse(txtSOLUONG.Text));
                            updateCTPhieuDichVuCommand.Parameters.AddWithValue("@THANHTIEN", decimal.Parse(txtTHANHTIEN.Text));
                            updateCTPhieuDichVuCommand.Parameters.AddWithValue("@TRATRUOC", decimal.Parse(txtTRATRUOC.Text));
                            updateCTPhieuDichVuCommand.Parameters.AddWithValue("@CONLAI", decimal.Parse(txtCONLAI.Text));
                            updateCTPhieuDichVuCommand.Parameters.AddWithValue("@NGAYGIAO", guna2DateTimePicker2.Value);
                            updateCTPhieuDichVuCommand.Parameters.AddWithValue("@TINHTRANG", label14.Text);

                            // Execute the update query for CT_PHIEUDICHVU
                            updateCTPhieuDichVuCommand.ExecuteNonQuery();
                        }

                        // Commit the transaction if all updates are successful
                        transaction.Commit();

                        MessageBox.Show("Dữ liệu đã được cập nhật thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // An error occurred, rollback the transaction
                        transaction.Rollback();

                        MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
