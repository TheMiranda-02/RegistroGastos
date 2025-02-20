using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace RegistroGastos
{
    class ConexionBD
    {
        private static string cadena = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadena);
        }
    }
}
