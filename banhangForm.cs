using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace VBStore
{
    public partial class banhangForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private string sdt;
        private DataTable dtSanPham;
        private DataTable dtGioHang;
        decimal tongTien;
        public DataTable GioHangData { get; set; }

        public banhangForm(string SDT)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            InitializeDataTables();
            this.sdt = SDT;
            


        }

        // Hàm lấy thông tin mua hàng dựa trên số điện thoại
        private DataTable GetPurchaseInfoByPhoneNumber(string phoneNumber)
        {
            DataTable purchaseInfo = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Tạo câu truy vấn SQL để lấy thông tin mua hàng
                    string query = @"SELECT CT.SOPHIEUBANHANG, PB.NGAYLAP, CT.MASANPHAM, SP.TENSP, SP.DONGIAMUA
                                     FROM CT_PHIEUBANHANG CT
                                     INNER JOIN PHIEUBANHANG PB ON CT.SOPHIEUBANHANG = PB.SOPHIEUBANHANG
                                     INNER JOIN SANPHAM SP ON CT.MASANPHAM = SP.MASANPHAM
                                     WHERE PB.MAKHACHHANG = (SELECT MAKHACHHANG FROM KHACHHANG WHERE SDT = @PhoneNumber)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(purchaseInfo);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lấy thông tin mua hàng: " + ex.Message);
                }
            }

            return purchaseInfo;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            
            DataTable purchaseInfo = GetPurchaseInfoByPhoneNumber(sdt);

            
            if (purchaseInfo != null && purchaseInfo.Rows.Count > 0)
            {
                // Ví dụ DataGridView
                spKHmua.DataSource = purchaseInfo;

                
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin mua hàng cho số điện thoại này.");
            }
        }
        private void InitializeDataTables()
        {
            // DataTable cho sản phẩm
            dtSanPham = new DataTable();
            dtSanPham.Columns.Add("MASANPHAM", typeof(string));
            dtSanPham.Columns.Add("TENSP", typeof(string));
            dtSanPham.Columns.Add("DONGIAMUA", typeof(decimal)); // Change column name here
            dtSanPham.Columns.Add("SOLUONGTON", typeof(int)); // Thêm cột SOLUONGTON

            // DataTable cho giỏ hàng
            dtGioHang = new DataTable();
            dtGioHang.Columns.Add("MASANPHAM", typeof(string));
            dtGioHang.Columns.Add("TENSP", typeof(string));
            dtGioHang.Columns.Add("DONGIAMUA", typeof(decimal)); // Change column name here
            dtGioHang.Columns.Add("SOLUONG", typeof(int));
            dtGioHang.Columns.Add("THANHTIEN", typeof(decimal));



            // Gán DataTables cho DataGridViews
            spKHmua.DataSource = dtSanPham;
            dtgh.DataSource = dtGioHang;
        }

        private void tatcaspBtn_Click(object sender, EventArgs e)
        {
            DataTable allProducts = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Create a SQL query to select all products
                    string query = "SELECT MASANPHAM, TENSP, DONGIAMUA, SOLUONGTON FROM SANPHAM";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(allProducts);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lấy thông tin sản phẩm: " + ex.Message);
                }
            }

            // Bind the DataTable to your DataGridView or any other control you want to use
            if (allProducts != null && allProducts.Rows.Count > 0)
            {
                // For example, if you want to display in a DataGridView named spKHmua
                spKHmua.DataSource = allProducts;
            }
            else
            {
                MessageBox.Show("Không có sản phẩm nào.");
            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        { // Kiểm tra xem người dùng đã chọn sản phẩm từ DataGridView dgvSanPham chưa
          // Kiểm tra xem người dùng đã chọn sản phẩm từ DataGridView dgvSanPham chưa
            if (spKHmua.SelectedRows.Count > 0)
            {
                // Lấy thông tin sản phẩm được chọn
                DataRowView selectedRow = spKHmua.SelectedRows[0].DataBoundItem as DataRowView;
                string maSanPham = selectedRow["MASANPHAM"].ToString();

                int soLuongThem = 1;
                DataRow existingRow = dtGioHang.AsEnumerable().FirstOrDefault(row => row["MASANPHAM"].ToString() == maSanPham);
                if (existingRow != null)
                {
                    soLuongThem = Convert.ToInt32(existingRow["SOLUONG"]) + 1;
                }

                if (existingRow != null)
                {
                    existingRow["SOLUONG"] = soLuongThem;
                    existingRow["THANHTIEN"] = Convert.ToDecimal(existingRow["SOLUONG"]) * Convert.ToDecimal(existingRow["DONGIAMUA"]); // Change column name here
                }
                else
                {
                    // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới
                    DataRow newRow = dtGioHang.NewRow();
                    newRow["MASANPHAM"] = maSanPham;
                    newRow["TENSP"] = selectedRow["TENSP"];
                    newRow["DONGIAMUA"] = selectedRow["DONGIAMUA"]; // Change column name here
                    newRow["SOLUONG"] = soLuongThem;
                    newRow["THANHTIEN"] = Convert.ToDecimal(newRow["DONGIAMUA"]) * soLuongThem; // Change column name here

                    dtGioHang.Rows.Add(newRow);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm từ danh sách để thêm vào giỏ hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            tongTien = 0;
            foreach (DataRow row in dtGioHang.Rows)
            {
                tongTien += Convert.ToDecimal(row["THANHTIEN"]);
            }

            // Gán giá trị tổng tiền vào Label tongtienlb
            tongtienlb.Text = "Tổng tiền: " + tongTien.ToString("N0") + " VNĐ"; // Format số thành chuỗi có dấu phẩy ngăn cách hàng nghìn
        }


        private void confirmBtn_Click(object sender, EventArgs e)
        {
            if (dtGioHang.Rows.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // Truyền dữ liệu từ dtGioHang sang xacnhanbanForm
            xacnhanbanForm xacNhanBanForm = new xacnhanbanForm(sdt);
            xacNhanBanForm.GioHangThanhToan = dtGioHang.Copy(); // Copy dữ liệu từ dtGioHang sang GioHangData của xacnhanbanForm
            xacNhanBanForm.ShowDialog(); // Hiển thị xacnhanbanForm
             // Đóng form banhangForm nếu cần
        }

        private void tongtienlb_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu không có dòng nào trong dtGioHang thì hiển thị thông báo
            if (dtGioHang.Rows.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Lấy dòng được chọn trong DataGridView dtgh
            if (dtgh.SelectedRows.Count > 0)
            {
                // Lấy chỉ số của dòng được chọn
                int selectedIndex = dtgh.SelectedRows[0].Index;

                // Xóa dòng được chọn khỏi DataTable dtGioHang
                dtGioHang.Rows.RemoveAt(selectedIndex);

                // Tính lại tổng tiền sau khi xóa
                tongTien = 0;
                foreach (DataRow row in dtGioHang.Rows)
                {
                    tongTien += Convert.ToDecimal(row["THANHTIEN"]);
                }

                // Cập nhật hiển thị tổng tiền
                tongtienlb.Text = "Tổng tiền: " + tongTien.ToString("N0") + " VNĐ";
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm từ giỏ hàng để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
