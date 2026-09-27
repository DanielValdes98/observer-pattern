using DemoObservadorInventario.Dominio;
using DemoObservadorInventario.Observadores;
using DemoObservadorInventario.Servicios;

namespace DemoObservadorInventario.Fachadas;

// Facade: ofrece operaciones de negocio sobre inventario, reposicion e historial.
public sealed class FachadaInventario
{
    private readonly InventarioProducto _inventario;
    private readonly ServicioReposicion _reposicion;
    private readonly ObservadorHistorialInventario _historial;

    public FachadaInventario(
        InventarioProducto inventario,
        ServicioReposicion reposicion,
        ObservadorHistorialInventario historial)
    {
        ArgumentNullException.ThrowIfNull(inventario);
        ArgumentNullException.ThrowIfNull(reposicion);
        ArgumentNullException.ThrowIfNull(historial);
        _inventario = inventario;
        _reposicion = reposicion;
        _historial = historial;
        _inventario.Suscribir(_historial);
    }

    public int ConsultarExistencias() => _inventario.Cantidad;

    public IReadOnlyList<CambioInventario> ConsultarHistorial() => _historial.Cambios;

    public void Vender(int unidades)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unidades);
        if (unidades > _inventario.Cantidad)
        {
            throw new InvalidOperationException("No hay existencias suficientes para la venta.");
        }

        _inventario.EstablecerCantidad(_inventario.Cantidad - unidades);
    }

    public int Reponer() => _reposicion.Reponer(_inventario);
}
