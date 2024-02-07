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
    public partial class xacnhanDVForm : Form
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
        private decimal tiennhan;
        private decimal tiencuoicung;
        private decimal tcc;
        private int startPoint = 0;
        private DateTime ngayGiao;  // Added variable to store selected delivery date

        public DataTable GioHangThanhToan
        {
            get { return dtGioHangThanhToan; }
            set { dtGioHangThanhToan = value; }
        }

        public xacnhanDVForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            // Initialize with the current date as default
        }

        public xacnhanDVForm(string sdt)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            this.sdt = sdt;
            // Initialize with the current date as default
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

            if (GioHangThanhToan != null)
            {
                guna2DataGridView1.DataSource = GioHangThanhToan;
            }
        }

        private decimal TinhTongTien(DataTable gioHang)
        {
            decimal tongTien = 0;
            foreach (DataRow row in gioHang.Rows)
            {
                decimal dongiaban = decimal.Parse(row["DONGIA"].ToString());
                tongTien += dongiaban;
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

            decimal giaTriMacDinh = 100000;
            label9.Text = "Phí:" + (giaTriMacDinh).ToString("N0") + " VNĐ";
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

            tcc = giaTriMacDinh + tongTien;
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

                    try
                    {
                        decimal tongTien = TinhTongTien(GioHangThanhToan);
                        decimal tienTratruoc = tiennhan;
                        decimal tienConLai = tiencuoicung;
                        string trangThai = "Chưa Hoàn Thành";

                        SqlCommand insertPhieuDichVuCommand = new SqlCommand("SP_INSERT_PHIEUDICHVU", connection, transaction);
                        insertPhieuDichVuCommand.CommandType = CommandType.StoredProcedure;
                        insertPhieuDichVuCommand.Parameters.AddWithValue("@MAKHACHHANG", makh);
                        insertPhieuDichVuCommand.Parameters.AddWithValue("@NGAYLAP", DateTime.Now.Date);
                        insertPhieuDichVuCommand.Parameters.AddWithValue("@TONGTIEN", tongTien);
                        insertPhieuDichVuCommand.Parameters.AddWithValue("@TINHTRANG", trangThai);
                        insertPhieuDichVuCommand.Parameters.AddWithValue("@SOTIENTRATRUOC", tienTratruoc);
                        insertPhieuDichVuCommand.Parameters.AddWithValue("@SOTIENCONLAI", tienConLai);
                        insertPhieuDichVuCommand.ExecuteNonQuery();

                        SqlCommand getMaxSophieuDichVuCommand = new SqlCommand("select max(sophieudichvu) from phieudichvu", connection, transaction);
                        sophieuBanHang = getMaxSophieuDichVuCommand.ExecuteScalar().ToString();
                        add_PDV(connection, transaction);
                        transaction.Commit();
                        MessageBox.Show("Thanh toán thành công!");
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

        void add_PDV(SqlConnection connection, SqlTransaction transaction)
        {
            foreach (DataRow row in GioHangThanhToan.Rows)
            {
                string sophieudichvu = sophieuBanHang;
                string maloaidichvu = row["MALOAIDICHVU"].ToString();
                decimal dongiadichvu = decimal.Parse(row["DONGIA"].ToString());
                int soluong = 1;
                decimal dongiaduoctinh = 100000;
                decimal thanhtien = dongiadichvu * soluong + dongiaduoctinh;
                decimal tratruoc = tiennhan;
                decimal conlai = thanhtien - tratruoc;

                string tinhtrang = "Chưa Giao";
                if (conlai <= 0)
                {
                    tinhtrang = "Đã Giao";
                }

                SqlCommand insertCTPhieuDichVuCommand = new SqlCommand("INSERT INTO CT_PHIEUDICHVU (SOPHIEUDICHVU, MALOAIDICHVU, DONGIADICHVU, DONGIADUOCTINH, SOLUONG, THANHTIEN, TRATRUOC, CONLAI, NGAYGIAO, TINHTRANG) VALUES (@SOPHIEUDICHVU, @MALOAIDICHVU, @DONGIADICHVU, @DONGIADUOCTINH, @SOLUONG, @THANHTIEN, @TRATRUOC, @CONLAI, @NGAYGIAO, @TINHTRANG)", connection, transaction);
                insertCTPhieuDichVuCommand.Parameters.AddWithValue("@SOPHIEUDICHVU", sophieudichvu);
                insertCTPhieuDichVuCommand.Parameters.AddWithValue("@MALOAIDICHVU", maloaidichvu);
                insertCTPhieuDichVuCommand.Parameters.AddWithValue("@DONGIADICHVU", dongiadichvu);
                insertCTPhieuDichVuCommand.Parameters.AddWithValue("@DONGIADUOCTINH", dongiaduoctinh);
                insertCTPhieuDichVuCommand.Parameters.AddWithValue("@SOLUONG", soluong);
                insertCTPhieuDichVuCommand.Parameters.AddWithValue("@THANHTIEN", thanhtien);
                insertCTPhieuDichVuCommand.Parameters.AddWithValue("@TRATRUOC", tratruoc);
                insertCTPhieuDichVuCommand.Parameters.AddWithValue("@CONLAI", conlai);
                insertCTPhieuDichVuCommand.Parameters.AddWithValue("@NGAYGIAO", ngayGiao);  // Use the selected delivery date
                insertCTPhieuDichVuCommand.Parameters.AddWithValue("@TINHTRANG", tinhtrang);

                insertCTPhieuDichVuCommand.ExecuteNonQuery();
            }
        }

        private void btnXacthuc_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tienNhanTB.Text))
            {
                if (tiennhanlb.Visible == true)
                {
                   if(tiennhan <= (tongTien / 2))
                    {
                        MessageBox.Show("Tổng tiền cần trả phải nhỏ hơn hoặc bằng 50% giá tiền tổng");
                        return;
                    }
                    else
                    {
                        ngayGiao = deliveryDate.Value;
                        guna2CircleProgressBar1.Visible = true;
                        timer1.Start();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng ấn nút xác thực đã nhận tiền.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập tiền đã nhận.");
            }
        }

        private void tienNhanTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tienNhanTB.Text))
            {
                if (decimal.TryParse(tienNhanTB.Text, out tiennhan))
                {
                    tienNhanTB.Text = tiennhan.ToString();
                    tiencuoicung = tcc - tiennhan;
                    tiennhanlb.Visible = true;
                    tonglb.Visible = true;
                    tiennhanlb.Text = "Tổng nhận: " + tiennhan.ToString("N0") + " VNĐ";
                    tonglb.Text = "Tổng cần trả: " + tiencuoicung.ToString("N0") + " VNĐ";
                }
                else
                {
                    MessageBox.Show("Giá trị nhập không hợp lệ.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập tiền đã nhận.");
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
