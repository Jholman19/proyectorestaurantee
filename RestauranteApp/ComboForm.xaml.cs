using System.Windows;

namespace RestauranteApp;

public partial class ComboForm : Window
{
    public ComboForm()
    {
        InitializeComponent();
    }

    public string Nombre => NombreBox.Text.Trim();
    public bool IncluyeDesayuno => DesayunoCheck.IsChecked == true;
    public bool IncluyeAlmuerzo => AlmuerzoCheck.IsChecked == true;
    public bool IncluyeCena => CenaCheck.IsChecked == true;
    public bool Activo => ActivoCheck.IsChecked == true;

    public void SetValues(string nombre, bool des, bool alm, bool cena, bool activo)
    {
        NombreBox.Text = nombre ?? "";
        DesayunoCheck.IsChecked = des;
        AlmuerzoCheck.IsChecked = alm;
        CenaCheck.IsChecked = cena;
        ActivoCheck.IsChecked = activo;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Establecer foco en el TextBox para que sea editable inmediatamente
        NombreBox.Focus();
        NombreBox.SelectAll();
    }

    private void Guardar_Click(object sender, RoutedEventArgs e)
    {
        ErrorText.Text = "";

        if (string.IsNullOrWhiteSpace(Nombre))
        {
            ErrorText.Text = "El nombre del combo es obligatorio.";
            NombreBox.Focus();
            return;
        }

        if (!IncluyeDesayuno && !IncluyeAlmuerzo && !IncluyeCena)
        {
            ErrorText.Text = "Selecciona al menos una comida.";
            return;
        }

        DialogResult = true;
        Close();
    }

    private void Cancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}