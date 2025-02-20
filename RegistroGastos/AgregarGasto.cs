using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace RegistroGastos
{
    class AgregarGasto
    {
        public bool AgregarG(int idCategoria, decimal monto, string fecha, string descripcion)
        {
            using (SqlConnection con = ConexionBD.ObtenerConexion())
            {
                con.Open();
                string query = "INSERT INTO Gastos (IdCategoria, Monto, FechaGasto, Descripción) VALUES (@idCategoria, @monto, @FechaGasto, @descripcion)";
                SqlCommand cmd = new SqlCommand(query, con);

                // Agregar los parámetros de manera más precisa
                cmd.Parameters.Add("@idCategoria", System.Data.SqlDbType.Int).Value = idCategoria;
                cmd.Parameters.Add("@monto", System.Data.SqlDbType.Decimal).Value = monto;
                cmd.Parameters.Add("@FechaGasto", System.Data.SqlDbType.Date).Value = fecha;  // Asegurando que el tipo de dato sea Date
                cmd.Parameters.Add("@descripcion", System.Data.SqlDbType.NVarChar, 500).Value = descripcion;  // Usando NVarChar para caracteres especiales

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
