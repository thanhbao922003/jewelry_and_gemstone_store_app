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
    public partial class findcusForm : Form
    {
        private string sdt;
        private string tenKH;
        private string connectionString;
        dbhelper dbHelper = new dbhelper();
        private ChildFormUtility childFormUtility;
        private string maKH;
        public findcusForm()
        {
            InitializeComponent();
        }
        public findcusForm(string sdt)
        {
            InitializeComponent();
            this.sdt = sdt;
            connectionString = dbHelper.ConnectionString;
            childFormUtility = new ChildFormUtility(this);
        }


        private void findcusForm_Load(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT MAKHACHHANG, TENKH, DIACHI, SDT, EMAIL FROM KHACHHANG WHERE SDT = @PhoneNumber";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Update the parameter with the current phone number (sdt)
                    command.Parameters.AddWithValue("@PhoneNumber", sdt);  // Use the updated sdt value

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            maKH = reader.GetString(reader.GetOrdinal("MAKHACHHANG"));
                            nameTextBox.Text = reader.GetString(reader.GetOrdinal("TENKH"));
                            phoneNumberTextBox.Text = reader.GetString(reader.GetOrdinal("SDT"));
                            emailTextBox.Text = reader.GetString(reader.GetOrdinal("EMAIL"));
                            addressTextBox.Text = reader.GetString(reader.GetOrdinal("DIACHI"));
                            tenKH = nameTextBox.Text;
                        }

                    }
                }
            }


        }

        private void muahangBtn_Click(object sender, EventArgs e)
        {
            muahangForm muahangForm = new muahangForm(sdt, tenKH);
            childFormUtility.OpenChildForm(muahangForm);
        }

        private void capnhapBtn_Click(object sender, EventArgs e)
        {
            suaCustomerForm suaCustomerform = new suaCustomerForm(maKH);
            suaCustomerform.ShowDialog();
            suaCustomerform.StartPosition = FormStartPosition.CenterParent;
        }

        private void dichvuBtn_Click(object sender, EventArgs e)
        {
            dungDVForm dungDV = new dungDVForm(sdt);
            childFormUtility.OpenChildForm(dungDV);
        }

        private void btnMuahang_Click(object sender, EventArgs e)
        {
            banhangForm banhang = new banhangForm(sdt);
            childFormUtility.OpenChildForm(banhang);
        }
    }
}