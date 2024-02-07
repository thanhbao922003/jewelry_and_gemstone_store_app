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
using System.Drawing.Printing;
using System.Transactions;

namespace VBStore
{
    public partial class thanhtoanForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private string sdt;
        private string tenKH;
        private DataTable dtGioHangThanhToan;
        private string makh;
        private decimal thanhtien;
        private string sophieuBanHang;

        public DataTable GioHangThanhToan
        {
            get { return dtGioHangThanhToan; }
            set { dtGioHangThanhToan = value; }
        }
        public thanhtoanForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
        }
        public thanhtoanForm(string sdt, string tenkh)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            this.sdt = sdt;
            this.tenKH = tenkh;
            tenkhlabel.Text = tenKH;
            sdtlabel.Text = sdt;
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Kiểm tra nếu giỏ hàng không null
            if (GioHangThanhToan != null)
            {
                // Gán giỏ hàng cho DataGridView hoặc bất kỳ điều gì bạn muốn hiển thị
                dgvGioHangThanhToan.DataSource = GioHangThanhToan;
            }
        }

        private void btnDaThanhToan_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                bool phieuBanHangCreated = false;

                try
                {
                    if (GioHangThanhToan != null)
                    {
                        SqlCommand getMAKHCommand = new SqlCommand("SELECT MAKHACHHANG FROM KHACHHANG WHERE SDT = @SDT", connection, transaction);
                        getMAKHCommand.Parameters.AddWithValue("@SDT", sdt);
                        makh = getMAKHCommand.ExecuteScalar() as string;

                        // Insert a new PHIEUBANHANG record
                        SqlCommand insertPhieuBanHangCommand = new SqlCommand("SP_INSERT_PHIEUBANHANG", connection, transaction);
                        insertPhieuBanHangCommand.CommandType = CommandType.StoredProcedure;
                        insertPhieuBanHangCommand.Parameters.AddWithValue("@MAKHACHHANG", makh);
                        insertPhieuBanHangCommand.Parameters.AddWithValue("@NGAYLAP", DateTime.Now.Date);
                        insertPhieuBanHangCommand.Parameters.AddWithValue("@TONGTIEN", TinhTongTien(GioHangThanhToan));

                        // Execute the stored procedure to insert the new PHIEUBANHANG record
                        insertPhieuBanHangCommand.ExecuteNonQuery();
                        phieuBanHangCreated = true;

                        // Get the SOPHIEUBANHANG of the newly inserted PHIEUBANHANG
                        SqlCommand getMaxSophieuBanHangCommand = new SqlCommand("select max(sophieubanhang) from phieubanhang", connection, transaction);
                        sophieuBanHang = getMaxSophieuBanHangCommand.ExecuteScalar().ToString();
                    }

                    if (phieuBanHangCreated)
                    {
                        // Gọi hàm addCT_PBH và truyền vào kết nối và giao dịch hiện tại
                        addCT_PBH(connection, transaction);

                        // Cập nhật số lượng tồn của các sản phẩm
                        foreach (DataRow row in GioHangThanhToan.Rows)
                        {
                            string maSanPham = row["MASANPHAM"].ToString();
                            int soLuongMua = Convert.ToInt32(row["SOLUONG"]);

                            // Thực hiện truy vấn để cập nhật số lượng tồn
                            SqlCommand updateSoLuongTonCommand = new SqlCommand("UPDATE SANPHAM SET SOLUONGTON = SOLUONGTON - @SoLuongMua WHERE MASANPHAM = @MaSanPham", connection, transaction);
                            updateSoLuongTonCommand.Parameters.AddWithValue("@SoLuongMua", soLuongMua);
                            updateSoLuongTonCommand.Parameters.AddWithValue("@MaSanPham", maSanPham);
                            updateSoLuongTonCommand.ExecuteNonQuery();
                        }
                    }

                    
                    transaction.Commit();
                    MessageBox.Show("Thanh toán thành công!");
                }
                catch (Exception ex)
                {
                    
                    transaction.Rollback();
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
                }
            }
        }



        void addCT_PBH(SqlConnection connection, SqlTransaction transaction)
        {
            // Loop through the shopping cart items and insert them into CT_PHIEUBANHANG
            foreach (DataRow row in GioHangThanhToan.Rows)
            {
                string masanpham = row["MASANPHAM"].ToString();
                int soluongban = int.Parse(row["SOLUONG"].ToString());
                decimal dongiaban = decimal.Parse(row["DONGIABAN"].ToString());
                

                // Insert a new CT_PHIEUBANHANG record for each product
                SqlCommand insertCTPhieuBanHangCommand = new SqlCommand("INSERT INTO CT_PHIEUBANHANG (SOPHIEUBANHANG, MASANPHAM, SOLUONGBAN, DONGIABAN, THANHTIEN) VALUES (@SOPHIEUBANHANG, @MASANPHAM, @SOLUONGBAN, @DONGIABAN, @THANHTIEN)", connection, transaction);
                insertCTPhieuBanHangCommand.Parameters.AddWithValue("@SOPHIEUBANHANG", sophieuBanHang);
                insertCTPhieuBanHangCommand.Parameters.AddWithValue("@MASANPHAM", masanpham);
                insertCTPhieuBanHangCommand.Parameters.AddWithValue("@SOLUONGBAN", soluongban);
                insertCTPhieuBanHangCommand.Parameters.AddWithValue("@DONGIABAN", dongiaban);
                insertCTPhieuBanHangCommand.Parameters.AddWithValue("@THANHTIEN", soluongban * dongiaban);

                // Execute the command to insert the CT_PHIEUBANHANG record
                insertCTPhieuBanHangCommand.ExecuteNonQuery();
            }
        }



        private decimal TinhTongTien(DataTable gioHang)
        {
            decimal tongTien = 0;
            foreach (DataRow row in gioHang.Rows)
            {
                int soluongban = int.Parse(row["SOLUONG"].ToString());
                decimal dongiaban = decimal.Parse(row["DONGIABAN"].ToString());
                tongTien += soluongban * dongiaban;
            }
            return tongTien;
        }

        private void ExecuteNonQuery(string query, SqlConnection connection, SqlTransaction transaction)
        {
            // Implement code to execute the provided SQL query on your database
            // This depends on the method you are using to interact with the database (e.g., ADO.NET SqlCommand)
            // Example:
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
