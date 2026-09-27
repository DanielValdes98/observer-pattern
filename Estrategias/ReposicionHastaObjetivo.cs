using DemoObservadorInventario.Contratos;

namespace DemoObservadorInventario.Estrategias;

public sealed class ReposicionHastaObjetivo : IEstrategiaReposicion
{
    private readonly int _objetivo;

    public ReposicionHastaObjetivo(int objetivo)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(objetivo);
        _objetivo = objetivo;
    }

    public int CalcularUnidades(int cantidadActual)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(cantidadActual);
        return Math.Max(0, _objetivo - cantidadActual);
    }
}
