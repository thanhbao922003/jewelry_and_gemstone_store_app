using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace VBStore
{
    public partial class dungDVForm : Form
    {
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private string sdt;
        private string tenKH;
        private DataTable dtSanPham;
        private DataTable dtGioHang;
        decimal tongTien;
        

        public dungDVForm(string SDT)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            InitializeDataTables();
            this.sdt = SDT;
            GetTenVaMaKH();
            tenkhlabel.Text = tenKH; // Cập nhật giá trị cho tenkhlabel
            sdtlabel.Text = sdt;
        }

        private void dungDVForm_Load(object sender, EventArgs e)
        {
            
            DataTable allProducts = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Tạo truy vấn SQL để lấy tất cả sản phẩm
                    string query = "SELECT MALOAIDICHVU, TENDICHVU, DONGIA FROM DICHVU";

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

            // Gán DataTable cho DataGridView hoặc bất kỳ điều khiển nào bạn muốn sử dụng
            if (allProducts != null && allProducts.Rows.Count > 0)
            {
                dgvSanPham.DataSource = allProducts;
            }
            else
            {
                MessageBox.Show("Không có sản phẩm nào.");
            }
           
        }
        private void GetTenVaMaKH()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT TENKH FROM KHACHHANG WHERE SDT = @sdt";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@sdt", sdt);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        tenKH = reader["TENKH"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi truy vấn dữ liệu: " + ex.Message);
            }
        }

        private void InitializeDataTables()
        {
            // DataTable cho sản phẩm
            dtSanPham = new DataTable();
            dtSanPham.Columns.Add("MALOAIDICHVU", typeof(string));
            dtSanPham.Columns.Add("TENDICHVU", typeof(string));
            dtSanPham.Columns.Add("DONGIA", typeof(decimal)); // Thay đổi tên cột ở đây

            // DataTable cho giỏ hàng
            dtGioHang = new DataTable();
            dtGioHang.Columns.Add("MALOAIDICHVU", typeof(string));
            dtGioHang.Columns.Add("TENDICHVU", typeof(string));
            dtGioHang.Columns.Add("DONGIA", typeof(decimal)); // Thay đổi tên cột ở đây
            dtGioHang.Columns.Add("THANHTIEN", typeof(decimal));

            // Gán DataTables cho DataGridViews
            dgvSanPham.DataSource = dtSanPham;
            dgvGioHang.DataSource = dtGioHang;
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem người dùng đã chọn sản phẩm từ DataGridView dgvSanPham chưa
            if (dgvSanPham.SelectedRows.Count > 0)
            {
                // Lấy thông tin sản phẩm được chọn
                DataRowView selectedRow = dgvSanPham.SelectedRows[0].DataBoundItem as DataRowView;
                string maSanPham = selectedRow["MALOAIDICHVU"].ToString();

                // Xóa toàn bộ dữ liệu hiện có trong dtGioHang (nếu có)
                dtGioHang.Clear();

                // Thêm sản phẩm được chọn vào dtGioHang
                DataRow newRow = dtGioHang.NewRow();
                newRow["MALOAIDICHVU"] = maSanPham;
                newRow["TENDICHVU"] = selectedRow["TENDICHVU"];
                newRow["DONGIA"] = selectedRow["DONGIA"];
                newRow["THANHTIEN"] = Convert.ToDecimal(newRow["DONGIA"]);

                dtGioHang.Rows.Add(newRow);
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

            tongtienlb.Text = "Tổng tiền: " + tongTien.ToString("N0") + " VNĐ";

        }

        private void confirmBtn_Click(object sender, EventArgs e)
        {
            if (dtGioHang.Rows.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // Truyền dữ liệu từ dtGioHang sang xacnhanbanForm
            xacnhanDVForm xacnhandv = new xacnhanDVForm(sdt);
            xacnhandv.GioHangThanhToan = dtGioHang.Copy();
            xacnhandv.ShowDialog(); // Hiển thị xacnhanbanForm
            // Đóng form dungDVForm nếu cần
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void delBtn_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu không có dòng nào trong dtGioHang thì hiển thị thông báo
            if (dtGioHang.Rows.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống. Vui lòng thêm sản phẩm trước khi xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                tongTien = 0;
                foreach (DataRow row in dtGioHang.Rows)
                {
                    tongTien += Convert.ToDecimal(row["THANHTIEN"]);
                }

                tongtienlb.Text = "Tổng tiền: " + tongTien.ToString("N0") + " VNĐ";
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm từ giỏ hàng để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
