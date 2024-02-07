using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using BarcodeLib;
using ZXing;
using System.Data.SqlClient;

namespace VBStore
{
    public partial class muahangForm : Form
    {
        // Khai báo biến và DataTables
        private DataTable dtSanPham;
        private DataTable dtGioHang;
        private string sdt;
        private string tenKH;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private string qrstring;
        FilterInfoCollection cameras;
        VideoCaptureDevice cam;
        BarcodeLib.Barcode code128;
        private bool qrCodeReadSuccessfully = false;



        public muahangForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            InitializeDataTables();
            LoadSanPham();
        }

        public muahangForm(string sdt, string tenKH)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            InitializeDataTables();
            LoadSanPham();
            this.sdt = sdt;
            this.tenKH = tenKH;
            tenkhlabel.Text = tenKH;
            sdtlabel.Text = sdt;
            cameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo info in cameras)
            {
                comboBox1.Items.Add(info.Name);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void InitializeDataTables()
        {
            // DataTable cho sản phẩm
            // DataTable cho sản phẩm
            dtSanPham = new DataTable();
            dtSanPham.Columns.Add("MASANPHAM", typeof(string));
            dtSanPham.Columns.Add("TENSP", typeof(string));
            dtSanPham.Columns.Add("DONGIABAN", typeof(decimal));
            dtSanPham.Columns.Add("SOLUONGTON", typeof(int)); // Thêm cột SOLUONGTON


            // DataTable cho giỏ hàng
            dtGioHang = new DataTable();
            dtGioHang.Columns.Add("MASANPHAM", typeof(string));
            dtGioHang.Columns.Add("TENSP", typeof(string));
            dtGioHang.Columns.Add("DONGIABAN", typeof(decimal));
            dtGioHang.Columns.Add("SOLUONG", typeof(int));
            dtGioHang.Columns.Add("THANHTIEN", typeof(decimal));

            // Gán DataTables cho DataGridViews
            dgvSanPham.DataSource = dtSanPham;
            dgvGioHang.DataSource = dtGioHang;
        }

        private void LoadSanPham()
        {
            // Thực hiện truy vấn để lấy dữ liệu sản phẩm từ database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT MASANPHAM, TENSP, DONGIABAN, SOLUONGTON FROM SANPHAM";

                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                // Đổ dữ liệu vào DataTable dtSanPham
                // Đổ dữ liệu vào DataTable dtSanPham
                while (reader.Read())
                {
                    dtSanPham.Rows.Add(reader["MASANPHAM"], reader["TENSP"], reader["DONGIABAN"], reader["SOLUONGTON"]);
                }


                reader.Close();
            }
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem người dùng đã chọn sản phẩm từ DataGridView dgvSanPham chưa
            if (dgvSanPham.SelectedRows.Count > 0)
            {
                // Lấy thông tin sản phẩm được chọn
                DataRowView selectedRow = dgvSanPham.SelectedRows[0].DataBoundItem as DataRowView;
                string maSanPham = selectedRow["MASANPHAM"].ToString();
                int soLuongTon = Convert.ToInt32(selectedRow["SOLUONGTON"]);


                int soLuongThem = 1;
                DataRow existingRow = dtGioHang.AsEnumerable().FirstOrDefault(row => row["MASANPHAM"].ToString() == maSanPham);
                if (existingRow != null)
                {

                    soLuongThem = Convert.ToInt32(existingRow["SOLUONG"]) + 1;
                }

                if (soLuongThem <= soLuongTon)
                {

                    if (existingRow != null)
                    {

                        existingRow["SOLUONG"] = soLuongThem;
                        existingRow["THANHTIEN"] = Convert.ToDecimal(existingRow["SOLUONG"]) * Convert.ToDecimal(existingRow["DONGIABAN"]);
                    }
                    else
                    {
                        // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới
                        DataRow newRow = dtGioHang.NewRow();
                        newRow["MASANPHAM"] = maSanPham;
                        newRow["TENSP"] = selectedRow["TENSP"];
                        newRow["DONGIABAN"] = selectedRow["DONGIABAN"];
                        newRow["SOLUONG"] = soLuongThem;
                        newRow["THANHTIEN"] = Convert.ToDecimal(newRow["DONGIABAN"]) * soLuongThem;

                        dtGioHang.Rows.Add(newRow);
                    }
                }
                else
                {
                    MessageBox.Show("Số lượng tồn không đủ cho việc thêm sản phẩm này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm từ danh sách để thêm vào giỏ hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void btnXacThuc_Click(object sender, EventArgs e)
        {
            if (dtGioHang.Rows.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            xacnhanmuaForm thanhToanForm = new xacnhanmuaForm(sdt);

            // Gán giỏ hàng từ muahangForm sang thanhtoanForm
            thanhToanForm.GioHangThanhToan = dtGioHang;

            // Hiển thị thanhtoanForm
            thanhToanForm.Show();
            this.Hide();
        }

        private void muahangForm_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer3.Enabled = true;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (cam != null && cam.IsRunning) cam.Stop();
            cam = new VideoCaptureDevice(cameras[comboBox1.SelectedIndex].MonikerString);
            cam.NewFrame += Cam_NewFrame;
            cam.Start();
            pictureBox1.Visible = true;
        }

        private void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap b = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = b;
        }

        private void qrcodeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cam != null && cam.IsRunning) cam.Stop();
        }
        private void ExtractQRCodeInformation(string qrstring)
        {
            // Kết nối cơ sở dữ liệu và thực hiện truy vấn để lấy thông tin từ qrstring
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT MASANPHAM, TENSP, DONGIABAN, SOLUONGTON FROM SANPHAM WHERE MASANPHAM = @MaSP"; // Sửa thành MASANPHAM
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@MaSP", qrstring); // Sử dụng qrstring làm tham số truy vấn
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Kiểm tra xem có dòng dữ liệu từ truy vấn không
                    if (reader.Read())
                    {
                        // Dữ liệu sản phẩm có tồn tại
                        string maSP = reader["MASANPHAM"].ToString();
                        string tenSP = reader["TENSP"].ToString();
                        decimal dongiaBan = Convert.ToDecimal(reader["DONGIABAN"]);
                        int soLuongTon = Convert.ToInt32(reader["SOLUONGTON"]);

                        // Kiểm tra xem số lượng tồn có cho phép thêm sản phẩm hay không
                        int soLuongThem = 1; // Số lượng mặc định để thêm
                        DataRow existingRow = dtGioHang.AsEnumerable().FirstOrDefault(row => row["MASANPHAM"].ToString() == maSP);
                        if (existingRow != null)
                        {
                            // Nếu sản phẩm đã tồn tại trong giỏ hàng, lấy số lượng đã thêm
                            soLuongThem = Convert.ToInt32(existingRow["SOLUONG"]) + 1;
                            timer3.Start();
                        }

                        if (soLuongThem <= soLuongTon)
                        {
                            // Số lượng tồn cho phép thêm sản phẩm
                            if (existingRow != null)
                            {
                                // Nếu sản phẩm đã tồn tại trong giỏ hàng, tăng số lượng
                                existingRow["SOLUONG"] = soLuongThem;
                                existingRow["THANHTIEN"] = Convert.ToDecimal(existingRow["SOLUONG"]) * dongiaBan;
                                timer3.Start();
                            }
                            else
                            {
                                // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới
                                DataRow newRow = dtGioHang.NewRow();
                                newRow["MASANPHAM"] = maSP;
                                newRow["TENSP"] = tenSP;
                                newRow["DONGIABAN"] = dongiaBan;
                                newRow["SOLUONG"] = soLuongThem;
                                newRow["THANHTIEN"] = dongiaBan * soLuongThem; // Tính thành tiền cho sản phẩm

                                dtGioHang.Rows.Add(newRow);
                                timer3.Start();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Số lượng tồn không đủ cho việc thêm sản phẩm này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            timer3.Start();
                        }
                    }
                    else
                    {
                        // Dữ liệu sản phẩm không tồn tại
                        MessageBox.Show("Mã QR không hợp lệ. Sản phẩm không tồn tại trong cơ sở dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        timer3.Start();
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    // Xử lý ngoại lệ nếu có
                    MessageBox.Show("Đã xảy ra lỗi khi truy vấn cơ sở dữ liệu: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        void Giaima()
        {
            Bitmap imgQRCode = (Bitmap)pictureBox1.Image;
            if (imgQRCode != null && !qrCodeReadSuccessfully)
            {
                try
                {
                    ZXing.BarcodeReader Reader = new ZXing.BarcodeReader();
                    Result result = Reader.Decode(imgQRCode);
                    if (result != null)
                    {
                        string decoded = result.ToString().Trim();
                        qrstring = decoded;
                        imgQRCode.Dispose();

                        // Gọi hàm để xử lý thông tin từ mã QR
                        ExtractQRCodeInformation(qrstring);

                        // Đánh dấu rằng đã đọc thành công mã QR
                        qrCodeReadSuccessfully = true;

                        // Dừng Timer1
                        timer1.Stop();

                        // Bắt đầu đợi 5 giây trước khi tiếp tục đọc mã QR mới
                        timer3.Start();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "");
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Giaima();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            timer3.Stop();

            // Đặt lại biến đánh dấu cho mã QR đã được đọc
            qrCodeReadSuccessfully = false;

            // Bắt đầu lại Timer1 để tiếp tục đọc mã QR
            timer1.Start();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu không có dòng nào trong dtGioHang thì hiển thị thông báo
            if (dtGioHang.Rows.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Lấy dòng được chọn trong DataGridView dgvGioHang
            if (dgvGioHang.SelectedRows.Count > 0)
            {
                // Lấy chỉ số của dòng được chọn
                int selectedIndex = dgvGioHang.SelectedRows[0].Index;

                // Xóa dòng được chọn khỏi DataTable dtGioHang
                dtGioHang.Rows.RemoveAt(selectedIndex);

                // Tính lại tổng tiền sau khi xóa
                decimal tongTien = 0;
                foreach (DataRow row in dtGioHang.Rows)
                {
                    tongTien += Convert.ToDecimal(row["THANHTIEN"]);
                }


            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm từ giỏ hàng để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
