using DemoObservadorInventario.Contratos;

namespace DemoObservadorInventario.Infraestructura;

public sealed class EnviadorNotificacionesConsola(TextWriter salida) : IEnviadorNotificaciones
{
    public void Enviar(string mensaje)
    {
        salida.WriteLine($"[Alerta] {mensaje}");
    }
}
