using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing;

namespace RegistroGastos
{
    public partial class EliminarCategoriaForm : Form
    {
        public event Action CategoriaEliminada;

        public EliminarCategoriaForm()
        {
            InitializeComponent();

            //Tipo de letra para el Label1
            label1.Font = new Font("Poppins", 16, FontStyle.Bold);

            //btn Eliminar Categoria 
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.ForeColor = System.Drawing.ColorTranslator.FromHtml("#fafafa");
            button1.BackColor = System.Drawing.ColorTranslator.FromHtml("#ca1444");
        }

        private void EliminarCategoriaForm_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        //Código para hacer que en un DataGridView aparesca datos de un Base de Datos
        private void CargarDatos()
        {
            try
            {
                using (SqlConnection con = ConexionBD.ObtenerConexion())
                {
                    con.Open();

                    string query = @"SELECT IdCategoria, NombreCategoria FROM Categorias";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Asignar los datos al DataGridView
                    DGWDeleteCate.DataSource = dt;

                    // Cambiar el color del encabezado de las columnas
                    DGWDeleteCate.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#03194c"); // Fondo azul
                    DGWDeleteCate.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White; // Texto blanco
                    DGWDeleteCate.ColumnHeadersDefaultCellStyle.Font = new Font("Poppins",8, FontStyle.Bold); // Fuente en negrita

                    // Cambiar el color de las filas del encabezado (si es necesario, puede ajustarse también)
                    DGWDeleteCate.EnableHeadersVisualStyles = false;

                    //Limpiar Fila cuando se usa el Boton de eliminar
                    DGWDeleteCate.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los datos: " + ex.Message);
            }
        }

        //Eliminar Categoria
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar si hay una fila seleccionada
                if (DGWDeleteCate.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Por favor, selecciona una categoría para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtener el ID de la categoría seleccionada
                int idCategoria = Convert.ToInt32(DGWDeleteCate.SelectedRows[0].Cells["IdCategoria"].Value);

                // Confirmación antes de eliminar
                DialogResult result = MessageBox.Show("¿Estás seguro de que deseas eliminar esta categoría?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return;

                // Conectar con la base de datos y ejecutar la eliminación
                using (SqlConnection con = ConexionBD.ObtenerConexion())
                {
                    con.Open();
                    string query = "DELETE FROM Categorias WHERE IdCategoria = @IdCategoria";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@IdCategoria", idCategoria);
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Categoría eliminada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarDatos(); // Volver a cargar los datos para actualizar el DataGridView
                            CategoriaEliminada?.Invoke();  // Invocar el evento
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar la categoría. Verifica si está en uso.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar la categoría, asegurate que no se use: ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
    }
}
