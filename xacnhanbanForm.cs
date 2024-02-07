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
    public partial class xacnhanbanForm: Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private string sdt;
        private string tenKH;
        private DataTable dtGioHangThanhToan;
        private string makh;
        private decimal thanhtien;
        private string sophieuBanHang;
        private decimal tongTien;
        private int startPoint;

        public DataTable GioHangThanhToan
        {
            get { return dtGioHangThanhToan; }
            set { dtGioHangThanhToan = value; }
        }
        public xacnhanbanForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
        }
        public xacnhanbanForm(string sdt)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            this.sdt = sdt;


        }
        private void GetTenVaMaKH()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT MAKHACHHANG, TENKH FROM KHACHHANG WHERE SDT = @sdt";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@sdt", sdt);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        makh = reader["MAKHACHHANG"].ToString();
                        tenKH = reader["TENKH"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi truy vấn dữ liệu: " + ex.Message);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Kiểm tra nếu giỏ hàng không null
            if (GioHangThanhToan != null)
            {
                // Gán giỏ hàng cho DataGridView hoặc bất kỳ điều gì bạn muốn hiển thị
                guna2DataGridView1.DataSource = GioHangThanhToan;
            }
        }





       


        private decimal TinhTongTien(DataTable gioHang)
        {
            decimal tongTien = 0;
            foreach (DataRow row in gioHang.Rows)
            {
                int soluongban = int.Parse(row["SOLUONG"].ToString());
                decimal dongiaban = decimal.Parse(row["DONGIAMUA"].ToString());
                tongTien += soluongban * dongiaban;
            }
            return tongTien;
        }

        private void ExecuteNonQuery(string query, SqlConnection connection, SqlTransaction transaction)
        {

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        private void xacnhanmuaForm_Load(object sender, EventArgs e)
        {

            GetTenVaMaKH();


            guna2DataGridView1.DataSource = GioHangThanhToan;
            tongTien = 0;

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                if (row.Cells["THANHTIEN"].Value != null)
                {
                    decimal thanhTien;
                    if (decimal.TryParse(row.Cells["THANHTIEN"].Value.ToString(), out thanhTien))
                    {
                        tongTien += thanhTien;
                    }
                }
            }

            tongtienlb.Text = "Tổng tiền: " + tongTien.ToString("N0") + " VNĐ";
            label5.Text = tenKH;
            label6.Text = sdt;
            label7.Text = makh;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            startPoint += 5;
            guna2CircleProgressBar1.Value = startPoint;
            if (guna2CircleProgressBar1.Value == 100)
            {

                guna2CircleProgressBar1.Value = 0;
                timer1.Stop();
                guna2CircleProgressBar1.Visible = false;
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
                            SqlCommand insertPhieuBanHangCommand = new SqlCommand("SP_INSERT_PHIEUMUAHANG", connection, transaction);
                            insertPhieuBanHangCommand.CommandType = CommandType.StoredProcedure;
                            insertPhieuBanHangCommand.Parameters.AddWithValue("@MAKHACHHANG", makh);
                            insertPhieuBanHangCommand.Parameters.AddWithValue("@NGAYLAP", DateTime.Now.Date);
                            insertPhieuBanHangCommand.Parameters.AddWithValue("@TONGTIEN", TinhTongTien(GioHangThanhToan));


                            // Execute the stored procedure to insert the new PHIEUBANHANG record
                            insertPhieuBanHangCommand.ExecuteNonQuery();
                            phieuBanHangCreated = true;

                            // Get the SOPHIEUBANHANG of the newly inserted PHIEUBANHANG
                            SqlCommand getMaxSophieuBanHangCommand = new SqlCommand("select max(sophieumuahang) from phieumuahang", connection, transaction);
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
                                SqlCommand updateSoLuongTonCommand = new SqlCommand("UPDATE SANPHAM SET SOLUONGTON = SOLUONGTON + @SoLuongMua WHERE MASANPHAM = @MaSanPham", connection, transaction);
                                updateSoLuongTonCommand.Parameters.AddWithValue("@SoLuongMua", soLuongMua);
                                updateSoLuongTonCommand.Parameters.AddWithValue("@MaSanPham", maSanPham);
                                updateSoLuongTonCommand.ExecuteNonQuery();
                            }
                        }


                        transaction.Commit();
                        MessageBox.Show("Mua thành công!");
                        guna2CircleProgressBar1.Visible = false;
                       this.Close();
                    }
                    catch (Exception ex)
                    {

                        transaction.Rollback();
                        MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
                    }
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
                decimal dongiaban = decimal.Parse(row["DONGIAMUA"].ToString());


                // Insert a new CT_PHIEUBANHANG record for each product
                SqlCommand insertCTPhieuBanHangCommand = new SqlCommand("INSERT INTO CT_PHIEUMUAHANG (SOPHIEUMUAHANG, MASANPHAM, SOLUONGMUA, DONGIAMUA, THANHTIEN) VALUES (@SOPHIEUBANHANG, @MASANPHAM, @SOLUONGBAN, @DONGIAMUA, @THANHTIEN)", connection, transaction);
                insertCTPhieuBanHangCommand.Parameters.AddWithValue("@SOPHIEUBANHANG", sophieuBanHang);
                insertCTPhieuBanHangCommand.Parameters.AddWithValue("@MASANPHAM", masanpham);
                insertCTPhieuBanHangCommand.Parameters.AddWithValue("@SOLUONGBAN", soluongban);
                insertCTPhieuBanHangCommand.Parameters.AddWithValue("@DONGIAMUA", dongiaban);
                insertCTPhieuBanHangCommand.Parameters.AddWithValue("@THANHTIEN", soluongban * dongiaban);

                // Execute the command to insert the CT_PHIEUBANHANG record
                insertCTPhieuBanHangCommand.ExecuteNonQuery();
            }
        }


        private void btnXacthuc_Click(object sender, EventArgs e)
        {
            guna2CircleProgressBar1.Visible = true;
            timer1.Start();
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
