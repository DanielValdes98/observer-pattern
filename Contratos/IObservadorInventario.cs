using DemoObservadorInventario.Dominio;

namespace DemoObservadorInventario.Contratos;

// Observador: contrato que reciben todos los interesados en los cambios.
public interface IObservadorInventario
{
    void Actualizar(CambioInventario cambio);
}
