using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace VBStore
{
    public partial class phieubanhang : Form
    {
        private string sophieubanhang;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public phieubanhang()
        {
            InitializeComponent();
        }

        public phieubanhang(string SOPHIEUBANHANG)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            this.sophieubanhang = SOPHIEUBANHANG;
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
                        SELECT P.SOPHIEUBANHANG, P.NGAYLAP, P.MAKHACHHANG
                        FROM PHIEUBANHANG P
                        WHERE P.SOPHIEUBANHANG = @SOPHIEUBANHANG";

                    using (SqlCommand infoCommand = new SqlCommand(infoQuery, connection))
                    {
                        infoCommand.Parameters.AddWithValue("@SOPHIEUBANHANG", sophieubanhang);
                        SqlDataReader reader = infoCommand.ExecuteReader();

                        if (reader.Read())
                        {
                            label5.Text = reader["SOPHIEUBANHANG"].ToString();
                            label6.Text = reader["NGAYLAP"].ToString();
                            label7.Text = reader["MAKHACHHANG"].ToString();
                        }

                        reader.Close();
                    }

                    // Retrieve data for DataGridView
                    string dataQuery = @"
                        SELECT ROW_NUMBER() OVER (ORDER BY CT.MASANPHAM) AS STT,
                               SP.TENSP,
                               LS.TENLOAISANPHAM,
                               CT.SOLUONGBAN,
                               DV.TENDONVITINH,
                               CT.DONGIABAN,
                               CT.THANHTIEN
                        FROM CT_PHIEUBANHANG CT
                        INNER JOIN SANPHAM SP ON CT.MASANPHAM = SP.MASANPHAM
                        INNER JOIN LOAISANPHAM LS ON SP.MALOAISANPHAM = LS.MALOAISANPHAM
                        INNER JOIN DONVITINH DV ON LS.MADONVITINH = DV.MADONVITINH
                        WHERE CT.SOPHIEUBANHANG = @SOPHIEUBANHANG";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(dataQuery, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@SOPHIEUBANHANG", sophieubanhang);

                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Set the data source for dataGridView
                        dataGridView1.DataSource = dataTable;
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
