using DemoObservadorInventario.Contratos;
using DemoObservadorInventario.Dominio;

namespace DemoObservadorInventario.Observadores;

public sealed class ObservadorHistorialInventario : IObservadorInventario
{
    private readonly List<CambioInventario> _cambios = [];

    public IReadOnlyList<CambioInventario> Cambios => _cambios.AsReadOnly();

    public void Actualizar(CambioInventario cambio)
    {
        _cambios.Add(cambio);
    }
}
