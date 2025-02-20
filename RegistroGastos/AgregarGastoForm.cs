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
using System.Configuration;

namespace RegistroGastos
{
    public partial class AgregarGastoForm : Form
    {
        // Definir un evento que notificará a la ventana principal
        public event Action GastoAgregado;

        public AgregarGastoForm()
        {
            InitializeComponent();

            //Label Tipos de Letras
            label1.Font = new Font("Poppins", 12, FontStyle.Bold);
            label2.Font = new Font("Poppins", 12, FontStyle.Bold);
            label3.Font = new Font("Poppins", 12, FontStyle.Bold);

            //btnGuardar
            btnGuardarGasto.FlatStyle = FlatStyle.Flat;
            btnGuardarGasto.FlatAppearance.BorderSize = 0;
            btnGuardarGasto.ForeColor = System.Drawing.ColorTranslator.FromHtml("#fafafa");
            btnGuardarGasto.BackColor = System.Drawing.ColorTranslator.FromHtml("#36802d");  //Color Verde 
        }

        private void AgregarGastoForm_Load(object sender, EventArgs e)
        {
            CargarCategorias();
        }

        private void CargarCategorias()
        {
            try
            {
                using (SqlConnection con = ConexionBD.ObtenerConexion())
                {
                    con.Open();
                    string query = "SELECT IdCategoria, NombreCategoria FROM Categorias"; // SQL para obtener los datos
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Llenar el ComboBox
                    cmbCategorias.DisplayMember = "NombreCategoria";  // Mostrar el nombre en el ComboBox
                    cmbCategorias.ValueMember = "IdCategoria";       // Almacenar el ID como el valor asociado
                    cmbCategorias.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las categorías: " + ex.Message);
            }
        }

        //Recuperar Id de la Categoria seleccionada
        private void cmbCategorias_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Verifica que haya una categoría seleccionada
            if (cmbCategorias.SelectedValue != null)
            {
                // Asigna el IdCategoria al TextBox TxtCategoriaId
                TxtCategoriaId.Text = cmbCategorias.SelectedValue.ToString();
            }
        }

        private void btnGuardarGasto_Click(object sender, EventArgs e)
        {
            // Obtener los valores de los controles
            int.TryParse(TxtCategoriaId.Text, out int Categoria); // Convertir a int
            decimal.TryParse(TxtMonto.Text, out decimal monto); // Convertir a decimal
            string Fecha = CaleFecha.SelectionStart.ToString("yyyy-MM-dd");  // Convertir la fecha a formato yyyy-MM-dd
            string Descripcion = TxtDescripcion.Text;  // Obtener la descripción como texto

            // Validar que los campos no estén vacíos
            if (monto <= 0)
            {
                MessageBox.Show("Por favor, rellene el formulario.");
                TxtMonto.Focus();  // Establecer el foco en el campo de monto
            }
            else if (string.IsNullOrEmpty(Descripcion))
            {
                MessageBox.Show("Por favor, rellene el formulario.");
                TxtDescripcion.Focus();  // Establecer el foco en el campo de descripción
            }
            else
            {
                // Si todo es válido, instanciar la clase que guarda el gasto
                AgregarGasto guardarGasto = new AgregarGasto();

                // Llamada al método para guardar el gasto
                bool resultado = guardarGasto.AgregarG(Categoria, monto, Fecha, Descripcion);

                // Verificar si se guardó correctamente
                if (resultado)
                {
                    MessageBox.Show("Gasto guardado correctamente.");
                    // Notificar a VerGastosForm que se ha agregado un gasto
                    GastoAgregado?.Invoke();  // Invocar el evento
                    TxtCategoriaId.Clear();
                    TxtMonto.Clear();
                    TxtDescripcion.Clear();
                    CaleFecha.SelectionStart = DateTime.Now;  // Restablecer la fecha a la fecha actual
                    TxtCategoriaId.Focus();  // Colocar el cursor en el primer campo
                }
                else
                {
                    MessageBox.Show("Hubo un error al guardar el gasto.");
                }
            }
        }



        private void CaleFecha_DateChanged(object sender, DateRangeEventArgs e)
        {
            TxtFecha.Text = CaleFecha.SelectionStart.ToString("yyyy-MM-dd");
        }

        private void TxtMonto_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verificar si la tecla presionada es un número, la tecla de control (como Backspace) o el punto decimal
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true; // No permitir la tecla presionada
            }

            // Si es el punto decimal
            if (e.KeyChar == '.')
            {
                // Verificar si ya hay un punto decimal en el TextBox
                if (TxtMonto.Text.Contains("."))
                {
                    e.Handled = true; // No permitir más de un punto decimal
                }
            }

            // Limitar a 10 dígitos antes del punto y 2 dígitos después del punto
            // Si el punto ya está presente
            if (TxtMonto.Text.Contains("."))
            {
                // Verificar si hay más de 2 dígitos después del punto
                int digitsAfterDecimal = TxtMonto.Text.Split('.').Last().Length;
                if (digitsAfterDecimal >= 2 && !Char.IsDigit(e.KeyChar) && e.KeyChar != 8)
                {
                    e.Handled = true; // No permitir más de 2 decimales
                }
            }

            // Limitar a 10 caracteres en total (sin contar el punto decimal)
            if (TxtMonto.Text.Length >= 10 && !Char.IsControl(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true; // No permitir más de 10 caracteres en total
            }
        }

    }
}
