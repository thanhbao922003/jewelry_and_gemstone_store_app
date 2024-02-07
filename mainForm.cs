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
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;

namespace VBStore
{
    public partial class mainForm : Form
    {
        private string sdt;
        private string tenKH;
        private Form currentFormChild;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private string imagesDirectory = @"C:\Workspace\c#\cnpm\VBStore\images\poster";
        private List<string> imagePaths = new List<string>();
        private int currentImageIndex = 0;
        private ChildFormUtility childFormUtility;
        private decimal flag;
        public mainForm(decimal FLAG)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            this.flag = FLAG;
            childFormUtility = new ChildFormUtility(this);
            if(flag == 0)
            {
                guna2Button1.Visible = false;
                guna2Button2.Visible = false;
            }
           
        }

        private void OpenChildFrom(Form childForm)
        {
            if (currentFormChild != null) 
            {
                currentFormChild.Close();
            }
            currentFormChild = childForm;
            childForm.TopLevel = false;
            childForm.Dock = DockStyle.Fill; // Đảm bảo childForm fill hết panel6
            mainPanel.Controls.Add(childForm);
            childForm.BringToFront();
            childForm.WindowState = FormWindowState.Maximized; // Mở childForm ở kích thước lớn nhất
            childForm.Show();
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       

        private void findcusBtn_Click(object sender, EventArgs e)
        {
            string phoneNumber = numberTextBox.Text;
            sdt = phoneNumber;

            // Kết nối đến cơ sở dữ liệu
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Tạo câu truy vấn SQL
                string query = "SELECT COUNT(*) FROM KHACHHANG WHERE SDT = @PhoneNumber";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                    // Thực thi câu truy vấn và trả về số lượng khách hàng có số điện thoại tương ứng
                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        findcusForm cusForm = new findcusForm(sdt);
                        OpenChildFrom(cusForm);
                        backBtn.Visible = true;
                        titlelabel.Text = "Thao tác trên khách hàng";
                        guna2Button2.Visible = false;

                    }
                    else
                    {
                        MessageBox.Show("Số điện thoại không tồn tại trong bảng Khách hàng.");
                    }
                }
            }
        }

        

        private void mainForm_Load(object sender, EventArgs e)
        {
            if (currentFormChild != null)
            {
                backBtn.Visible = true;
            }
            timer1.Interval = 10000;
            coutnKH();
            countDQTS();
            countDVF();
            pie_load();
            SetupLineChart();
            countDVBb();
            countDMH();
            countDBH();
            countHT();



        }
        void pie_load()
        {
            DataTable dataTable = new DataTable();
            Random rand = new Random(); // Create a Random object

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                 SELECT
                     CASE WHEN MALOAISANPHAM = 'LSP001' THEN N'Đá Quý'
                          WHEN MALOAISANPHAM = 'LSP002' THEN N'Vàng'
                          WHEN MALOAISANPHAM = 'LSP003' THEN N'Bạc'
                          WHEN MALOAISANPHAM = 'LSP004' THEN N'Kim Cương'
                          WHEN MALOAISANPHAM = 'LSP005' THEN N'Trang Sức'
                     END AS ProductType,
                     SUM(SOLUONGTON) AS TOTAL_SOLUONGTON
                 FROM
                     SANPHAM
                 WHERE
                     MALOAISANPHAM IN ('LSP001', 'LSP002', 'LSP003', 'LSP004', 'LSP005')
                 GROUP BY
                     CASE WHEN MALOAISANPHAM = 'LSP001' THEN N'Đá Quý'
                          WHEN MALOAISANPHAM = 'LSP002' THEN N'Vàng'
                          WHEN MALOAISANPHAM = 'LSP003' THEN N'Bạc'
                          WHEN MALOAISANPHAM = 'LSP004' THEN N'Kim Cương'         
                          WHEN MALOAISANPHAM = 'LSP005' THEN N'Trang Sức'
                     END";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }

                Series pieSeries = new Series("DoanhThuTheoThang_Tron");
                pieSeries.ChartType = SeriesChartType.Pie;

                foreach (DataRow row in dataTable.Rows)
                {
                    string productType = row["ProductType"].ToString();
                    double totalQuantity = Convert.ToDouble(row["TOTAL_SOLUONGTON"]);

                    DataPoint dataPoint = pieSeries.Points.Add(totalQuantity);
                    dataPoint.LegendText = $"{productType}";

                    // Tùy chỉnh màu sắc cho từng phần tử Pie
                    dataPoint.Color = Color.FromArgb(255, rand.Next(256), rand.Next(256), rand.Next(256));
                }

                ChartBDT.Series.Add(pieSeries);
                ChartBDT.Titles.Add("\tBiểu Đồ Số Lượng Thống Kê Số Lượng Tồn Kho");

                // In đậm title
                ChartBDT.Titles[0].Font = new Font("Arial", 12, FontStyle.Bold);

                // In đậm chú thích
                ChartBDT.Legends[0].Font = new Font("Arial", 10, FontStyle.Bold);

                pieSeries.BorderWidth = 2;
                pieSeries.BorderColor = Color.WhiteSmoke;
                pieSeries.ShadowOffset = 2;

                // Set the background color to transparent
                ChartBDT.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SetupLineChart()
        {
            try
            {
                DataTable dataTable = new DataTable();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
        SELECT
            BC.NGAYLAPBAOCAO AS [ngày],
            SUM(BC.SOLUONGMUAVAO * SP.DONGIAMUA) AS [tổng số tiền mua vào],
            SUM(BC.SOLUONGBANRA * SP.DONGIABAN) AS [tổng số tiền bán ra]
        FROM
            BAOCAOTON BC
        JOIN
            SANPHAM SP ON BC.MASANPHAM = SP.MASANPHAM
        WHERE
            BC.NGAYLAPBAOCAO BETWEEN DATEADD(DAY, -8    , GETDATE()) AND GETDATE()
        GROUP BY
            BC.NGAYLAPBAOCAO
        ORDER BY
            BC.NGAYLAPBAOCAO";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }

                chart1.Titles.Add("Biểu đồ thống kê");

                // Định dạng trục x để chỉ hiển thị phần ngày
                chart1.ChartAreas[0].AxisX.LabelStyle.Format = "dd/MM/yyyy";
                chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                // Thêm dữ liệu từ dataTable vào biểu đồ
                foreach (DataRow row in dataTable.Rows)
                {
                    string ngayString = row["ngày"].ToString();
                    DateTime ngay = DateTime.Parse(ngayString);
                    double tongTienMuaVao = Convert.ToDouble(row["tổng số tiền mua vào"]);
                    double tongTienBanRa = Convert.ToDouble(row["tổng số tiền bán ra"]);

                    chart1.Series["Tổng số tiền mua vào"].Points.AddXY(ngay, tongTienMuaVao);
                    chart1.Series["Tổng số tiền bán ra"].Points.AddXY(ngay, tongTienBanRa);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void coutnKH()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Tạo câu truy vấn SQL để đếm số lượng khách hàng
                string query = "SELECT COUNT(*) FROM KHACHHANG";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Thực thi câu truy vấn và trả về số lượng khách hàng
                    int count = (int)command.ExecuteScalar();
                    countKH.Text = count.ToString(); // Gán kết quả vào Label countKH
                }
            }
        }
        void countDQTS()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create SQL query to count the number of products for each product type
                string query = "SELECT MALOAISANPHAM, COUNT(*) FROM SANPHAM GROUP BY MALOAISANPHAM";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    int totalCountDQ = 0;
                    int totalCountTS = 0;

                    while (reader.Read())
                    {
                        string maLoaiSanPham = reader.GetString(0);
                        int count = reader.GetInt32(1);

                        // Update total counts based on product type
                        if (maLoaiSanPham == "LSP001" || maLoaiSanPham == "LSP002" || maLoaiSanPham == "LSP003" || maLoaiSanPham == "LSP004")
                        {
                            totalCountDQ += count;
                        }
                        else if (maLoaiSanPham == "LSP005")
                        {
                            totalCountTS += count;
                        }
                    }

                    // Assign the total counts to the respective labels
                    countDQ.Text = totalCountDQ.ToString();
                    countTS.Text = totalCountTS.ToString();
                }
            }
        }
        void countDVF()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                
                string query = "SELECT COUNT(*) FROM DICHVU";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                   
                    int count = (int)command.ExecuteScalar();
                    countDV.Text = count.ToString(); 
                }
            }
        }
        void countDVBb()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();


                string query = "SELECT COUNT(*) FROM PHIEUDICHVU";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    int count = (int)command.ExecuteScalar();
                    countBDV.Text = count.ToString();
                }
            }
        }
        void countDMH()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();


                string query = "SELECT COUNT(*) FROM PHIEUMUAHANG";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    int count = (int)command.ExecuteScalar();
                    cDMH.Text = count.ToString();
                }
            }
        }
        void countDBH()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();


                string query = "SELECT COUNT(*) FROM PHIEUBANHANG";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    int count = (int)command.ExecuteScalar();
                    cDBHH.Text = count.ToString();
                }
            }
        }
        void countHT()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();


                string query = "SELECT COUNT(*) FROM BAOCAOTON";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    int count = (int)command.ExecuteScalar();
                    cBCT.Text = count.ToString();
                }
            }
        }
        private void panelgem_Click(object sender, EventArgs e)
        {
            daquyForm gemForm = new daquyForm(flag);
            OpenChildFrom(gemForm);
            backBtn.Visible = true;
            titlelabel.Text = "Đá Quý";
            guna2Button2.Visible = false;
        }

        private void paneljewelry_Click(object sender, EventArgs e)
        {
            trangsucForm trangsuc = new trangsucForm(flag);
            OpenChildFrom(trangsuc);
            backBtn.Visible = true;
            titlelabel.Text = "Trang Sức";
            guna2Button2.Visible = false;
        }

        private void mainForm_ClientSizeChanged(object sender, EventArgs e)
        {
            if (currentFormChild != null)
            {
                currentFormChild.WindowState = this.WindowState; // Set childForm's window state to match mainForm

                // Check if the currentFormChild is an instance of findcusForm
                if (currentFormChild is findcusForm)
                {
                    findcusBtn_Click(findcusBtn, EventArgs.Empty); // Call the method only when findcusForm is displayed
                }
            }
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            if (currentFormChild != null)
            {
                currentFormChild.Close();
            }
            backBtn.Visible = false;
            titlelabel.Text = "VBStore";
            coutnKH();
            countDQTS();
            countDVF();
            guna2Button2.Visible = true;
        }

        private void panelcustomer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelcustomer_Click_1(object sender, EventArgs e)
        {
            customerForm customerForm = new customerForm(sdt,flag);
            OpenChildFrom(customerForm);
            backBtn.Visible = true;
            titlelabel.Text = "Danh sách khách hàng";
            guna2Button2.Visible = false;
        }

        private void panelservice_Click(object sender, EventArgs e)
        {
            dichvuForm dichVu = new dichvuForm(flag);
            OpenChildFrom(dichVu);
            backBtn.Visible = true;
            titlelabel.Text = "Dịch Vụ";
            guna2Button2.Visible = false;
        }

        private void guna2CustomGradientPanel7_Click(object sender, EventArgs e)
        {
            dichvubookedForm dichvubookedForm = new dichvubookedForm();
            OpenChildFrom(dichvubookedForm); 
            backBtn.Visible = true;
            titlelabel.Text = "Phiếu dịch vụ";
            guna2Button2.Visible = false;
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void guna2CustomGradientPanel6_Click(object sender, EventArgs e)
        {
            dmhForm dmhform = new dmhForm();
            OpenChildFrom(dmhform);
            backBtn.Visible = true;
            titlelabel.Text = "Phiếu mua hàng";
            guna2Button2.Visible = false;
        }

        private void guna2CustomGradientPanel5_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void guna2CustomGradientPanel5_Click(object sender, EventArgs e)
        {
            dbhForm dbhform = new dbhForm();
            OpenChildFrom(dbhform);
            backBtn.Visible = true;
            titlelabel.Text = "Phiếu bán hàng";
            guna2Button2.Visible = false;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void muahangBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(numberTextBox.Text)) // Check if the numberTextBox is not empty
            {
                sdt = numberTextBox.Text; // Assign the value from numberTextBox to sdt
                gettenkh();
                muahangForm muahang = new muahangForm(sdt, tenKH);
                OpenChildFrom(muahang);
            }
            else
            {
                MessageBox.Show("Vui lòng nhập số điện thoại");
            }
        }
        void gettenkh()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT TENKH FROM KHACHHANG WHERE SDT = @PhoneNumber";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Update the parameter with the current phone number (sdt)
                    command.Parameters.AddWithValue("@PhoneNumber", sdt);  // Use the updated sdt value

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tenKH = reader.GetString(reader.GetOrdinal("TENKH"));
                           
                        }

                    }
                }
            }
        }

        private void guna2CustomGradientPanel8_Click(object sender, EventArgs e)
        {
            bctonForm dbhform = new bctonForm();
            OpenChildFrom(dbhform);
            titlelabel.Text = "Báo cáo tồn";
            backBtn.Visible = true;
            guna2Button2.Visible = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            loginForm login = new loginForm();
            login.Show();
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            nhanvienForm nhanvien = new nhanvienForm();
            OpenChildFrom(nhanvien);
            backBtn.Visible = true;
            titlelabel.Text = "Quản lí nhân viên";
            guna2Button2.Visible = false;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            loaidichvu loaidichvu = new loaidichvu();
            OpenChildFrom(loaidichvu);
            backBtn.Visible = true;
            titlelabel.Text = "Quản lí loại sản phẩm";
            guna2Button2.Visible = false;
        }

        private void guna2CustomGradientPanel8_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
