using DemoObservadorInventario.Contratos;
using DemoObservadorInventario.Dominio;

namespace DemoObservadorInventario.Servicios;

// Contexto de Strategy: delega el calculo en una estrategia intercambiable.
public sealed class ServicioReposicion
{
    private IEstrategiaReposicion _estrategia;

    public ServicioReposicion(IEstrategiaReposicion estrategia)
    {
        ArgumentNullException.ThrowIfNull(estrategia);
        _estrategia = estrategia;
    }

    public void CambiarEstrategia(IEstrategiaReposicion estrategia)
    {
        ArgumentNullException.ThrowIfNull(estrategia);
        _estrategia = estrategia;
    }

    public int Reponer(InventarioProducto inventario)
    {
        ArgumentNullException.ThrowIfNull(inventario);
        int unidades = _estrategia.CalcularUnidades(inventario.Cantidad);
        ArgumentOutOfRangeException.ThrowIfNegative(unidades);
        inventario.EstablecerCantidad(checked(inventario.Cantidad + unidades));
        return unidades;
    }
}
