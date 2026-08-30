using DemoObservadorInventario.Contratos;

namespace DemoObservadorInventario.Dominio;

public sealed class InventarioProducto : IObservableInventario
{
    private readonly List<IObservadorInventario> _observadores = [];

    public InventarioProducto(string nombreProducto, int cantidadInicial)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreProducto);
        ArgumentOutOfRangeException.ThrowIfNegative(cantidadInicial);

        NombreProducto = nombreProducto;
        Cantidad = cantidadInicial;
    }

    public string NombreProducto { get; }
    public int Cantidad { get; private set; }

    public void Suscribir(IObservadorInventario observador)
    {
        ArgumentNullException.ThrowIfNull(observador);

        if (!_observadores.Contains(observador))
        {
            _observadores.Add(observador);
        }
    }

    public void Desuscribir(IObservadorInventario observador)
    {
        ArgumentNullException.ThrowIfNull(observador);
        _observadores.Remove(observador);
    }

    public void EstablecerCantidad(int nuevaCantidad)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(nuevaCantidad);

        if (nuevaCantidad == Cantidad)
        {
            return;
        }

        var cambio = new CambioInventario(NombreProducto, Cantidad, nuevaCantidad);
        Cantidad = nuevaCantidad;
        Notificar(cambio);
    }

    private void Notificar(CambioInventario cambio)
    {
        foreach (var observador in _observadores.ToArray())
        {
            observador.Actualizar(cambio);
        }
    }
}
