using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VBStore
{
    public class dbhelper
    {
        private string connectionString = "Data Source=DESKTOP-KRAFR0M\\MSSQLSERVER1;Initial Catalog=CNPM_DB;Integrated Security=True";

        public string ConnectionString
        {
            get { return connectionString; }
        }
    }
}