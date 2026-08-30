namespace DemoObservadorInventario.Dominio;

public sealed record CambioInventario(
    string NombreProducto,
    int CantidadAnterior,
    int CantidadActual);
