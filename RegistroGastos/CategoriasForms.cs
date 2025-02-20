using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;

namespace RegistroGastos
{
    class CategoriasForms
    {
        public bool AgregarCategoria(string categoria)
        {
            try
            {
                using (SqlConnection con = ConexionBD.ObtenerConexion())
                {
                    con.Open();
                    string query = "INSERT INTO Categorias (NombreCategoria) VALUES (@NombreCategoria)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@NombreCategoria", categoria);
                    int rowsAffected = cmd.ExecuteNonQuery(); // Devuelve el número de filas afectadas

                    // Si rowsAffected es mayor que 0, la inserción fue exitosa
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error, muestra el mensaje y retorna false
                MessageBox.Show("Error al agregar categoría: " + ex.Message);
                return false;
            }
        }
    }
}
