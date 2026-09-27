using DemoObservadorInventario.Contratos;

namespace DemoObservadorInventario.Estrategias;

public sealed class ReposicionLoteFijo : IEstrategiaReposicion
{
    private readonly int _unidades;

    public ReposicionLoteFijo(int unidades)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unidades);
        _unidades = unidades;
    }

    public int CalcularUnidades(int cantidadActual)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(cantidadActual);
        return _unidades;
    }
}
