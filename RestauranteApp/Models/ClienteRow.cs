namespace RestauranteApp;

public class ClienteRow
{
    public int ClienteId { get; set; }
    public string Nombre { get; set; } = "";
    public string Documento { get; set; } = "";
    public string Telefono { get; set; } = "";
    public bool Activo { get; set; }
    public string ComboNombre { get; set; } = "";
}
