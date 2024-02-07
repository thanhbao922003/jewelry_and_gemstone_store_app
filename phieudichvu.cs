using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using Guna.UI2.WinForms;

namespace VBStore
{
    public partial class phieudichvu : Form
    {
        private string sophieudichvu;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();

        public phieudichvu()
        {
            InitializeComponent();
        }

        public phieudichvu(string SOPHIEUDICHVU)
        {
            InitializeComponent();
            connectionString = dbHelper.ConnectionString;
            this.sophieudichvu = SOPHIEUDICHVU;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to get data for the SOPHIEUDICHVU
                    string query = $@"
                        SELECT PD.SOPHIEUDICHVU, PD.NGAYLAP, KH.MAKHACHHANG, KH.SDT, KH.TENKH, PD.TONGTIEN,
                               PD.SOTIENTRATRUOC, PD.SOTIENCONLAI
                        FROM PHIEUDICHVU PD
                        INNER JOIN KHACHHANG KH ON PD.MAKHACHHANG = KH.MAKHACHHANG
                        WHERE PD.SOPHIEUDICHVU = '{sophieudichvu}'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            // Assign values to labels
                            label5.Text = reader["SOPHIEUDICHVU"].ToString();
                            label6.Text = reader["NGAYLAP"].ToString();
                            label7.Text = reader["MAKHACHHANG"].ToString();
                            label8.Text = reader["TENKH"].ToString();
                            label9.Text = reader["SDT"].ToString();
                            label8.Text = reader["TONGTIEN"].ToString();
                            label10.Text = reader["SOTIENTRATRUOC"].ToString();
                            label13.Text = reader["SOTIENCONLAI"].ToString();

                            // Close the data reader
                            reader.Close();

                            // Load CT_PHIEUDICHVU data
                            LoadCTData();
                        }
                        else
                        {
                            reader.Close();
                            MessageBox.Show("No data found for SOPHIEUDICHVU: " + sophieudichvu, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCTData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to get CT_PHIEUDICHVU data
                    string query = $@"
                SELECT DV.TENDICHVU, DV.DONGIA, CD.DONGIADUOCTINH, CD.SOLUONG,
                       CD.THANHTIEN, CD.NGAYGIAO, CD.TINHTRANG, CD.TRATRUOC, CD.CONLAI
                FROM CT_PHIEUDICHVU CD
                INNER JOIN DICHVU DV ON CD.MALOAIDICHVU = DV.MALOAIDICHVU
                WHERE CD.SOPHIEUDICHVU = '{sophieudichvu}'";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Add an STT column to the dataTable
                        DataColumn sttColumn = new DataColumn("STT", typeof(int));
                        dataTable.Columns.Add(sttColumn);

                        // Assign a value to the STT column
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            dataTable.Rows[i]["STT"] = i + 1;
                        }

                        // Set the data source for guna2DataGridView1
                        guna2DataGridView1.DataSource = dataTable;

                        // Add an STT column to the guna2DataGridView1
                        DataGridViewColumn sttGridColumn = new DataGridViewTextBoxColumn();
                        sttGridColumn.DataPropertyName = "STT";
                        sttGridColumn.HeaderText = "STT";

                        // Insert the STT column at the beginning
                        guna2DataGridView1.Columns.Insert(0, sttGridColumn);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
    }
}
