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

namespace VBStore
{
    public partial class thongke : Form
    {
        
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private ChildFormUtility childFormUtility;
        
        public thongke()
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            SetupLineChart();

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
                    FORMAT(BC.NGAYLAPBAOCAO, 'yyyy-MM') AS [tháng],
                    SUM(BC.SOLUONGMUAVAO * SP.DONGIAMUA) AS [tổng số tiền mua vào],
                    SUM(BC.SOLUONGBANRA * SP.DONGIABAN) AS [tổng số tiền bán ra]
                FROM
                    BAOCAOTON BC
                JOIN
                    SANPHAM SP ON BC.MASANPHAM = SP.MASANPHAM
                WHERE
                    BC.NGAYLAPBAOCAO BETWEEN DATEADD(MONTH, -11, GETDATE()) AND GETDATE()
                GROUP BY
                    FORMAT(BC.NGAYLAPBAOCAO, 'yyyy-MM')
                ORDER BY
                    [tháng]";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }

                chart1.Titles.Add("Biểu đồ thống kê");

                // Định dạng trục x để chỉ hiển thị phần tháng
                chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM";
                chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;

                // Thêm dữ liệu từ dataTable vào biểu đồ
                foreach (DataRow row in dataTable.Rows)
                {
                    string thangString = row["tháng"].ToString();
                    DateTime thang = DateTime.Parse(thangString);
                    double tongTienMuaVao = Convert.ToDouble(row["tổng số tiền mua vào"]);
                    double tongTienBanRa = Convert.ToDouble(row["tổng số tiền bán ra"]);

                    chart1.Series["Tổng số tiền mua vào"].Points.AddXY(thang, tongTienMuaVao);
                    chart1.Series["Tổng số tiền bán ra"].Points.AddXY(thang, tongTienBanRa);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
