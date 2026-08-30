using DemoObservadorInventario.Contratos;
using DemoObservadorInventario.Dominio;

namespace DemoObservadorInventario.Observadores;

public sealed class ObservadorPantallaInventario(TextWriter salida) : IObservadorInventario
{
    public void Actualizar(CambioInventario cambio)
    {
        salida.WriteLine(
            $"[Pantalla] {cambio.NombreProducto}: quedan {cambio.CantidadActual} unidades.");
    }
}
