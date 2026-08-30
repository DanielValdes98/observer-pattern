using DemoObservadorInventario.Contratos;
using DemoObservadorInventario.Dominio;

namespace DemoObservadorInventario.Observadores;

public sealed class ObservadorAlertaExistenciasBajas(
    IEnviadorNotificaciones enviadorNotificaciones,
    int umbral) : IObservadorInventario
{
    public void Actualizar(CambioInventario cambio)
    {
        if (cambio.CantidadActual <= umbral)
        {
            enviadorNotificaciones.Enviar(
                $"Existencias bajas de {cambio.NombreProducto}: {cambio.CantidadActual} unidades.");
        }
    }
}
