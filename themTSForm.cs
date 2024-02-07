using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using QRCoder;

namespace VBStore
{
    public partial class ThemTSForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public ThemTSForm()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            LoadLoaiSanPham(); // Load danh sách loại sản phẩm vào ListBox
        }

        private void LoadLoaiSanPham()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT MALOAISANPHAM, TENLOAISANPHAM FROM LOAISANPHAM";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string maLoai = reader["MALOAISANPHAM"].ToString();
                                string tenLoai = reader["TENLOAISANPHAM"].ToString();
                                cmbLoaiSP.Items.Add(new LoaiSanPhamItem(maLoai, tenLoai));
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

        private void btnThem_Click(object sender, EventArgs e)
        {
            string maSP = txtMaSP.Text;
            string tenSP = txtTenSP.Text;
        
            string donGiaMuaText = txtDonGiaMua.Text;
            string soLuongTonText = txtSoLuongTon.Text;

            if (string.IsNullOrWhiteSpace(maSP) || string.IsNullOrWhiteSpace(tenSP) || string.IsNullOrWhiteSpace(donGiaMuaText) || string.IsNullOrWhiteSpace(soLuongTonText))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Không thực hiện thêm sản phẩm
            }
            if (!maSP.StartsWith("SP"))
            {
                MessageBox.Show("Mã sản phẩm phải bắt đầu bằng 'SP'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Không thực hiện thêm sản phẩm
            }


            // Check if an item is selected in the ComboBox cmbLoaiSP
            if (cmbLoaiSP.SelectedIndex == 4)
            {
                string maLoaiSP = ((LoaiSanPhamItem)cmbLoaiSP.SelectedItem).MaLoai;
                decimal dongiaMua = decimal.Parse(txtDonGiaMua.Text);
                int soLuongTon = int.Parse(txtSoLuongTon.Text);
                if (soLuongTon == 0 || soLuongTon < 0)
                {
                    MessageBox.Show("Vui lòng nhập số lượng tồn phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "INSERT INTO SANPHAM (MASANPHAM, TENSP, MALOAISANPHAM, DONGIAMUA, SOLUONGTON, MAQR) " +
                                       "VALUES (@MaSP, @TenSP, @MaLoaiSP, @DonGiaMua, @SoLuongTon, @MaQR)";
                        string checkMaSPQuery = "SELECT COUNT(*) FROM SANPHAM WHERE MASANPHAM = @MaSP";
                        using (SqlCommand checkMaSPCommand = new SqlCommand(checkMaSPQuery, connection))
                        {
                            checkMaSPCommand.Parameters.AddWithValue("@MaSP", maSP);

                            int existingCount = (int)checkMaSPCommand.ExecuteScalar();

                            if (existingCount > 0)
                            {
                                MessageBox.Show("Mã sản phẩm đã tồn tại trong cơ sở dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return; // Không thực hiện thêm sản phẩm
                            }

                        }
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@MaSP", maSP);
                            command.Parameters.AddWithValue("@TenSP", tenSP);
                            command.Parameters.AddWithValue("@MaLoaiSP", maLoaiSP);
                            command.Parameters.AddWithValue("@DonGiaMua", dongiaMua);
                            command.Parameters.AddWithValue("@SoLuongTon", soLuongTon);

                            // Tạo mã QR
                            string qrCodect = maSP; // Mã SP dùng làm nội dung mã QR
                            string qrFileName = qrCodect + ".png"; // Tên file mã QR

                            // Tạo đường dẫn lưu tạm file mã QR
                            string qrFilePath = Path.Combine(Path.GetTempPath(), qrFileName);

                            // Tạo mã QR và lưu thành file ảnh
                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodect, QRCodeGenerator.ECCLevel.Q);
                            QRCode qrCode = new QRCode(qrCodeData);
                            Bitmap qrImage = qrCode.GetGraphic(10);
                            qrImage.Save(qrFilePath, System.Drawing.Imaging.ImageFormat.Png);

                            // Tải ảnh lên Cloudinary
                            Account cloudinaryAccount = new Account("deayc8fkw", "587612639861316", "x7CCykyQrvK58ZI9J9-67J_4A8E"); // Thay thế bằng thông tin tài khoản Cloudinary của bạn
                            Cloudinary cloudinary = new Cloudinary(cloudinaryAccount);

                            var uploadParams = new ImageUploadParams()
                            {
                                File = new FileDescription(qrFilePath),
                                PublicId = "VBStore/" + qrCodect, // Tên public ID của ảnh trên Cloudinary
                            };

                            var uploadResult = cloudinary.Upload(uploadParams);

                            if (uploadResult != null)
                            {
                                string qrImageUrl = uploadResult.SecureUri.ToString();

                                command.Parameters.AddWithValue("@MaQR", qrImageUrl);

                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    // Đóng Form khi thêm thành công (tùy theo yêu cầu của bạn)
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Thêm sản phẩm thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            else
            {
                MessageBox.Show("Vui lòng chọn loại sản phẩm phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public class LoaiSanPhamItem
        {
            public string MaLoai { get; set; }
            public string TenLoai { get; set; }

            public LoaiSanPhamItem(string maLoai, string tenLoai)
            {
                MaLoai = maLoai;
                TenLoai = tenLoai;
            }

            public override string ToString()
            {
                return TenLoai;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}