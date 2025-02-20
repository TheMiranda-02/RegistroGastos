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
using System.Windows.Forms.DataVisualization.Charting;

namespace RegistroGastos
{
    public partial class AgregarCategoriaForm : Form
    {
        public event Action CatAgregado;

        public AgregarCategoriaForm()
        {
            InitializeComponent();

            //Color de fondo
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");

            //Label Tipo de Letras
            label2.Font = new Font("Poppins", 12, FontStyle.Bold);

            //Boton Agregar
            btnGuardarCat.FlatStyle = FlatStyle.Flat;
            btnGuardarCat.FlatAppearance.BorderSize = 0;
            btnGuardarCat.ForeColor = System.Drawing.ColorTranslator.FromHtml("#fafafa");
            btnGuardarCat.BackColor = System.Drawing.ColorTranslator.FromHtml("#36802d");  //Color Verde

        }

        private void btnGuardarCat_Click(object sender, EventArgs e)
        {
            string textoCategoria = txtCategoria.Text.Trim(); // Recuperar y quitar espacios extra del texto del TextBox

            // Verificar si el campo está vacío
            if (string.IsNullOrEmpty(textoCategoria))
            {
                MessageBox.Show("Ingrese una Categoría", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Salir del método si el campo está vacío
            }

            CategoriasForms guardado = new CategoriasForms(); // Instanciar objeto para manejar la lógica

            // Llamar al método AgregarCategoria y verificar si la inserción fue exitosa
            bool exito = guardado.AgregarCategoria(textoCategoria);

            if (exito)
            {
                CatAgregado?.Invoke();
                // Si la inserción fue exitosa, mostrar mensaje y recargar el gráfico
                MessageBox.Show("Categoría agregada con éxito.");
                CargarGrafico(); // Recargar el gráfico para reflejar la nueva categoría
                txtCategoria.Clear();
            }
            else
            {
                // Si la inserción falló, mostrar un mensaje de error
                MessageBox.Show("Hubo un error al agregar la categoría.");
            }
        }


        private void AgregarCategoriaForm_Load(object sender, EventArgs e)
        {
            CargarGrafico();
        }

        // Método para cargar las categorías y sus registros al gráfico
        public void CargarGrafico()
        {
            try
            {
                // Limpiar puntos existentes antes de agregar nuevos datos
                Gastos.Series["Gastos"].Points.Clear();

                using (SqlConnection con = ConexionBD.ObtenerConexion())
                {
                    con.Open();

                    string query = @"
                SELECT C.NombreCategoria, SUM(G.Monto) AS TotalMonto 
                FROM Gastos G 
                INNER JOIN Categorias C ON G.IdCategoria = C.IdCategoria 
                GROUP BY C.NombreCategoria 
                ORDER BY TotalMonto DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Leer cada fila del resultado y agregarla al gráfico
                    while (reader.Read())
                    {
                        string categoria = reader["NombreCategoria"].ToString();
                        decimal totalMonto = Convert.ToDecimal(reader["TotalMonto"]);

                        // Agregar el punto al gráfico, donde el eje X será el nombre de la categoría 
                        // y el eje Y será el total de monto gastado en esa categoría
                        int pointIndex = Gastos.Series["Gastos"].Points.AddXY(categoria, totalMonto);

                        // Configurar la etiqueta para mostrar solo el porcentaje
                        Gastos.Series["Gastos"].Points[pointIndex].Label = "#PERCENT{P2}";

                        // Mostrar el nombre de la categoría en la leyenda
                        Gastos.Series["Gastos"].Points[pointIndex].LegendText = categoria;
                    }
                }

                // Configurar el gráfico como Doughnut
                Gastos.Series["Gastos"].ChartType = SeriesChartType.Doughnut;
                Gastos.Series["Gastos"].IsValueShownAsLabel = true;

                // Configurar la leyenda
                Gastos.Legends[0].Enabled = true;  // Asegurar que la leyenda esté activa
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el gráfico: " + ex.Message);
            }
        }

        private void txtCategoria_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsLetter(e.KeyChar) && e.KeyChar !=8 && e.KeyChar != ' ')
            {
                e.Handled = true;   //No permitir el ingreso de caracteries numericos
            }
        }
    }
}
