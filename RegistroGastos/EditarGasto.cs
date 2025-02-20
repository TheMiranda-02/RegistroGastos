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

namespace RegistroGastos
{
    public partial class EditarGasto : Form
    {
        private int idGasto;

        public EditarGasto(int idGasto, decimal monto, string descripcion, DateTime fecha, int idCategoria)
        {
            InitializeComponent();

            // Rellenar los controles con los datos actuales
            this.idGasto = idGasto;
            txtMonto.Text = monto.ToString();
            txtDescripcion.Text = descripcion;
            dtpFechaGasto.Value=fecha; // Asigna la fecha al MonthCalendar
            CargarCategorias(idCategoria);

            //Label Tipos de Letras
            lblCat.Font = new Font("Poppins", 12, FontStyle.Bold);
            lblMonto.Font = new Font("Poppins", 12, FontStyle.Bold);
            lblDesc.Font = new Font("Poppins", 12, FontStyle.Bold);

            //btnEditar
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.ForeColor = System.Drawing.ColorTranslator.FromHtml("#fafafa");
            btnGuardar.BackColor = System.Drawing.ColorTranslator.FromHtml("#36802d");
        }

        private void CargarCategorias(int idCategoriaSeleccionada)
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
                    cbxCategorias.DisplayMember = "NombreCategoria";
                    cbxCategorias.ValueMember = "IdCategoria";

                    // Seleccionar la categoría actual
                    cbxCategorias.SelectedValue = idCategoriaSeleccionada;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las categorías: " + ex.Message);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                decimal monto = Convert.ToDecimal(txtMonto.Text);
                string descripcion = txtDescripcion.Text;
                DateTime fechaGasto = dtpFechaGasto.Value;
                int idCategoria = Convert.ToInt32(cbxCategorias.SelectedValue);
                // Actualizar el gasto en la base de datos
                using (SqlConnection con = ConexionBD.ObtenerConexion())
                {
                    con.Open();
                    string query = @"UPDATE Gastos 
                                 SET Monto = @Monto, 
                                     Descripción = @Descripcion, 
                                     FechaGasto = @FechaGasto, 
                                     IdCategoria = @IdCategoria 
                                 WHERE IdGasto = @IdGasto";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Monto", monto);
                        cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                        cmd.Parameters.AddWithValue("@FechaGasto", fechaGasto);
                        cmd.Parameters.AddWithValue("@IdCategoria", idCategoria);
                        cmd.Parameters.AddWithValue("@IdGasto", idGasto);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Gasto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close(); // Cerrar el formulario de edición
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar el gasto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los cambios: " + ex.Message);
            }
        }
            
        }
    }

