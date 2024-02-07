using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace VBStore
{
    public partial class phieumuahang : Form
    {
        private string sophieumuahang;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public phieumuahang()
        {
            InitializeComponent();
        }

        public phieumuahang(string SOPHIEUMUAHANG)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            this.sophieumuahang = SOPHIEUMUAHANG;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve data for labels
                    string infoQuery = @"
                        SELECT PMH.SOPHIEUMUAHANG, PMH.NGAYLAP, KH.MAKHACHHANG, KH.DIACHI, KH.SDT
                        FROM PHIEUMUAHANG PMH
                        INNER JOIN KHACHHANG KH ON PMH.MAKHACHHANG = KH.MAKHACHHANG
                        WHERE PMH.SOPHIEUMUAHANG = @SOPHIEUMUAHANG";

                    using (SqlCommand infoCommand = new SqlCommand(infoQuery, connection))
                    {
                        infoCommand.Parameters.AddWithValue("@SOPHIEUMUAHANG", sophieumuahang);
                        SqlDataReader reader = infoCommand.ExecuteReader();

                        if (reader.Read())
                        {
                            label5.Text = reader["SOPHIEUMUAHANG"].ToString();
                            label6.Text = reader["NGAYLAP"].ToString();
                            label7.Text = reader["MAKHACHHANG"].ToString();
                            label8.Text = reader["DIACHI"].ToString();
                            label9.Text = reader["SDT"].ToString();
                        }

                        reader.Close();
                    }

                    // Retrieve data for DataGridView
                    string dataQuery = @"
                        SELECT ROW_NUMBER() OVER (ORDER BY CT.MASANPHAM) AS STT,
                               SP.TENSP,
                               LS.TENLOAISANPHAM,
                               CT.SOLUONGMUA,
                               DV.TENDONVITINH,
                               CT.DONGIAMUA,
                               CT.THANHTIEN
                        FROM CT_PHIEUMUAHANG CT
                        INNER JOIN SANPHAM SP ON CT.MASANPHAM = SP.MASANPHAM
                        INNER JOIN LOAISANPHAM LS ON SP.MALOAISANPHAM = LS.MALOAISANPHAM
                        INNER JOIN DONVITINH DV ON LS.MADONVITINH = DV.MADONVITINH
                        WHERE CT.SOPHIEUMUAHANG = @SOPHIEUMUAHANG";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(dataQuery, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@SOPHIEUMUAHANG", sophieumuahang);

                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Set the data source for dataGridView
                        s.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
