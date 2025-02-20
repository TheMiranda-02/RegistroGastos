using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;


namespace RegistroGastos
{
    public partial class VerGastosForm : Form
    {
        public VerGastosForm()
        {
            InitializeComponent();
        }

        private void VerGastosForm_Load(object sender, EventArgs e)
        {
            //Color de fondo
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");

            //Tipo de letras para los Label
            lblCategoria.Font = new Font("Poppins", 12, FontStyle.Bold);
            label1.Font = new Font("Poppins", 12, FontStyle.Bold);

            //Colores a botones

            //btnAgregarCate
            btnAgregarCate.FlatStyle = FlatStyle.Flat;
            btnAgregarCate.FlatAppearance.BorderSize = 0;
            btnAgregarCate.ForeColor = System.Drawing.ColorTranslator.FromHtml("#fafafa");
            btnAgregarCate.BackColor = System.Drawing.ColorTranslator.FromHtml("#36802d");  //Color Verde 

            //btnAgregarGastos
            btnAgregarGasto.FlatStyle = FlatStyle.Flat;
            btnAgregarGasto.FlatAppearance.BorderSize = 0;
            btnAgregarGasto.ForeColor = System.Drawing.ColorTranslator.FromHtml("#fafafa");
            btnAgregarGasto.BackColor = System.Drawing.ColorTranslator.FromHtml("#36802d");  //Color Verde 

            //btnEliminarGasto
            btnEliminar.FlatStyle = FlatStyle.Flat;
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.ForeColor = System.Drawing.ColorTranslator.FromHtml("#fafafa");
            btnEliminar.BackColor = System.Drawing.ColorTranslator.FromHtml("#ca1444");

            //btn EditarGasto
            btnEditar.FlatStyle = FlatStyle.Flat;
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.ForeColor = System.Drawing.ColorTranslator.FromHtml("#fafafa");
            btnEditar.BackColor = System.Drawing.ColorTranslator.FromHtml("#f8a348");

            //btn EliminarCategoria
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.ForeColor = System.Drawing.ColorTranslator.FromHtml("#fafafa");
            button1.BackColor = System.Drawing.ColorTranslator.FromHtml("#ca1444");

            //btnDescargar
            btnDescargar.FlatStyle = FlatStyle.Flat;
            btnDescargar.FlatAppearance.BorderSize = 0;
            btnDescargar.ForeColor = System.Drawing.ColorTranslator.FromHtml("#fafafa");
            btnDescargar.BackColor = System.Drawing.ColorTranslator.FromHtml("#215a6d");

            //Funciones para la pantalla principal
            CargarDatos();
            CargarCategorias(); // Llenar el ComboBox con las categorías
            // Asegúrate de que el DataGridView tenga auto-generación de columnas activada
            DGWGastos.AutoGenerateColumns = true;

            //Llamamos al método para sumar el monto total
            SumarMontoTotal();

        }

        //Código para hacer que en un DataGridView aparesca datos de un Base de Datos
        private void CargarDatos()
        {
            try
            {
                using (SqlConnection con = ConexionBD.ObtenerConexion())
                {
                    con.Open();

                    string query = @"SELECT g.IdGasto, 
                                    c.NombreCategoria AS Categoria, 
                                    c.IdCategoria,   -- Se obtiene el IdCategoria aquí
                                    g.Monto, 
                                    g.FechaGasto, 
                                    g.Descripción 
                             FROM Gastos g
                             INNER JOIN Categorias c ON g.IdCategoria = c.IdCategoria";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Asignar los datos al DataGridView
                    DGWGastos.DataSource = dt;

                    // Cambiar el color del encabezado de las columnas
                    DGWGastos.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#03194c"); // Fondo azul
                    DGWGastos.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White; // Texto blanco
                    DGWGastos.ColumnHeadersDefaultCellStyle.Font = new Font("Poppins", 10, FontStyle.Bold); // Fuente en negrita

                    // Cambiar el color de las filas del encabezado (si es necesario, puede ajustarse también)
                    DGWGastos.EnableHeadersVisualStyles = false;

                    // Ocultar la columna IdCategoria
                    if (DGWGastos.Columns["IdCategoria"] != null)
                    {
                        DGWGastos.Columns["IdCategoria"].Visible = false;
                    }

                    DGWGastos.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener los datos: " + ex.Message);
            }
        }


        private void SumarMontoTotal()
        {
            try
            {
                decimal totalMonto = 0;

                // Iteramos por las filas del DataGridView
                foreach (DataGridViewRow row in DGWGastos.Rows)
                {
                    // Verificamos si la celda tiene un valor válido (de tipo numérico)
                    if (row.Cells["Monto"].Value != null && decimal.TryParse(row.Cells["Monto"].Value.ToString(), out decimal monto))
                    {
                        // Sumamos el valor al total
                        totalMonto += monto;
                    }
                }

                // Asignamos el total al TextBox (puedes formatearlo como desees)
                txtMontoTotal.Text = totalMonto.ToString("C2"); // Formato como moneda
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al sumar los montos: " + ex.Message);
            }


        }

        //Códgo para hacer que en un combobox aparezca datos en un ComboBox
        private void CargarCategorias()
        {
            try
            {
                using (SqlConnection con = ConexionBD.ObtenerConexion())
                {
                    con.Open();

                    string query = "SELECT IdCategoria, NombreCategoria FROM Categorias";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Asignar el DataTable como fuente de datos del ComboBox
                    cbxCategorias.DataSource = dt;
                    cbxCategorias.DisplayMember = "NombreCategoria"; // Mostrar nombres
                    cbxCategorias.ValueMember = "IdCategoria"; // Guardar ID en el Value

                    // Agregar opción "Todas" al inicio del ComboBox
                    DataRow row = dt.NewRow();
                    row["IdCategoria"] = 0;
                    row["NombreCategoria"] = "Todas";
                    dt.Rows.InsertAt(row, 0);

                    cbxCategorias.SelectedIndex = 0; // Seleccionar "Todas" por defecto
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las categorías: " + ex.Message);
            }
        }

        private void cbxCategorias_SelectedIndexChanged(object sender, EventArgs e)
        {
            FiltrarPorCategoria();
        }
        private void FiltrarPorCategoria()
        {
            try
            {
                using (SqlConnection con = ConexionBD.ObtenerConexion())
                {
                    con.Open();

                    int idCategoriaSeleccionada;

                    if (cbxCategorias.SelectedValue is DataRowView drv)
                    {
                        idCategoriaSeleccionada = Convert.ToInt32(drv["IdCategoria"]);
                    }
                    else
                    {
                        idCategoriaSeleccionada = Convert.ToInt32(cbxCategorias.SelectedValue);
                    }

                    string query = @"SELECT g.IdGasto, 
                                    c.NombreCategoria AS Categoria, 
                                    c.IdCategoria,   -- Se obtiene el IdCategoria aquí también
                                    g.Monto, 
                                    g.FechaGasto, 
                                    g.Descripción 
                             FROM Gastos g
                             INNER JOIN Categorias c ON g.IdCategoria = c.IdCategoria";

                    if (idCategoriaSeleccionada != 0) // Si no es "Todas"
                    {
                        query += " WHERE g.IdCategoria = @IdCategoria";
                    }

                    SqlDataAdapter da = new SqlDataAdapter(query, con);

                    if (idCategoriaSeleccionada != 0)
                    {
                        da.SelectCommand.Parameters.AddWithValue("@IdCategoria", idCategoriaSeleccionada);
                    }

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    DGWGastos.DataSource = dt;
                    DGWGastos.ClearSelection();

                    // Volver a calcular el total de montos después de filtrar
                    SumarMontoTotal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al filtrar por categoría: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar si hay una fila seleccionada
                if (DGWGastos.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Por favor, selecciona un gasto para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtener el ID del gasto seleccionado
                int idGasto = Convert.ToInt32(DGWGastos.SelectedRows[0].Cells["IdGasto"].Value);

                // Confirmación antes de eliminar
                DialogResult result = MessageBox.Show("¿Estás seguro de que deseas eliminar este gasto?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return;

                // Conectar con la base de datos y ejecutar la eliminación
                using (SqlConnection con = ConexionBD.ObtenerConexion())
                {
                    con.Open();
                    string query = "DELETE FROM Gastos WHERE IdGasto = @IdGasto";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@IdGasto", idGasto);
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Gasto eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarDatos(); // Volver a cargar los datos para actualizar el DataGridView
                            SumarMontoTotal();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el gasto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el gasto: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregarGasto_Click(object sender, EventArgs e)
        {
            AgregarGastoForm VentanaAgregar = new AgregarGastoForm();
            // Pasar una referencia al método de actualización del DataGridView en VerGastosForm
            VentanaAgregar.GastoAgregado += () => {
                CargarDatos();
                SumarMontoTotal();
            };
            VentanaAgregar.Show();
        }

        private void btnAgregarCate_Click(object sender, EventArgs e)
        {
            AgregarCategoriaForm VentanaAgregarCate = new AgregarCategoriaForm();
            VentanaAgregarCate.CatAgregado += () => {
                CargarCategorias();
            };
            VentanaAgregarCate.Show();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (DGWGastos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona un gasto para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener los datos del gasto seleccionado
            int idGasto = Convert.ToInt32(DGWGastos.SelectedRows[0].Cells["IdGasto"].Value);
            decimal monto = Convert.ToDecimal(DGWGastos.SelectedRows[0].Cells["Monto"].Value);
            string descripcion = DGWGastos.SelectedRows[0].Cells["Descripción"].Value.ToString();
            DateTime fechaGasto = Convert.ToDateTime(DGWGastos.SelectedRows[0].Cells["FechaGasto"].Value);
            int idCategoria = Convert.ToInt32(DGWGastos.SelectedRows[0].Cells["IdCategoria"].Value);

            // Abrir el formulario de edición con los datos actuales
            EditarGasto ventanaEditar = new EditarGasto(idGasto, monto, descripcion, fechaGasto, idCategoria);
            ventanaEditar.ShowDialog();

            CargarDatos();
            SumarMontoTotal();
        }

        // Boton para abrir la ventana donde se elimina la Categoria
        private void button1_Click(object sender, EventArgs e)
        {
            EliminarCategoriaForm VentanaEliminarCate = new EliminarCategoriaForm();
            VentanaEliminarCate.CategoriaEliminada += () =>{
                CargarCategorias();
            };
            VentanaEliminarCate.Show();
        }

        private void btnDescargar_Click(object sender, EventArgs e)
        {
            DescargarCSV();
        }

        private void DescargarCSV()
        {
            try
            {
                if (DGWGastos.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Crear el cuadro de diálogo para guardar el archivo
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Archivo CSV (*.csv)|*.csv";
                saveFileDialog.Title = "Guardar como";
                saveFileDialog.FileName = "Gastos.csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, new UTF8Encoding(true)))
                    {
                        // Escribir encabezados
                        for (int i = 0; i < DGWGastos.Columns.Count; i++)
                        {
                            if (DGWGastos.Columns[i].Visible) // Solo incluir columnas visibles
                            {
                                sw.Write(DGWGastos.Columns[i].HeaderText);
                                if (i < DGWGastos.Columns.Count - 1) sw.Write(",");
                            }
                        }
                        sw.WriteLine();

                        // Escribir filas de datos
                        foreach (DataGridViewRow row in DGWGastos.Rows)
                        {
                            for (int i = 0; i < DGWGastos.Columns.Count; i++)
                            {
                                if (DGWGastos.Columns[i].Visible) // Solo incluir columnas visibles
                                {
                                    sw.Write(row.Cells[i].Value?.ToString());
                                    if (i < DGWGastos.Columns.Count - 1) sw.Write(",");
                                }
                            }
                            sw.WriteLine();
                        }
                    }

                    MessageBox.Show("Datos exportados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }

}
