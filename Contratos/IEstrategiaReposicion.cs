namespace DemoObservadorInventario.Contratos;

// Strategy: cada algoritmo calcula cuantas unidades agregar al inventario.
public interface IEstrategiaReposicion
{
    int CalcularUnidades(int cantidadActual);
}
