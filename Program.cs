using DemoObservadorInventario.Dominio;
using DemoObservadorInventario.Infraestructura;
using DemoObservadorInventario.Observadores;

Console.WriteLine("=== Demostracion del patron Observador: inventario de una tienda ===\n");

// Aqui se crean y conectan las dependencias:
var inventario = new InventarioProducto("Teclado mecanico", cantidadInicial: 10);

// Observadores - reacciona a los cambios de inventario:
var observadorPantalla = new ObservadorPantallaInventario(Console.Out);

var observadorHistorial = new ObservadorHistorialInventario();

var enviadorNotificaciones = new EnviadorNotificacionesConsola(Console.Out);
var observadorAlerta = new ObservadorAlertaExistenciasBajas(
    enviadorNotificaciones,
    umbral: 5);

// Se suscriben los observadores al inventario
inventario.Suscribir(observadorPantalla);
inventario.Suscribir(observadorHistorial);
inventario.Suscribir(observadorAlerta);

// Se realizan cambios en el inventario
inventario.EstablecerCantidad(7);
inventario.EstablecerCantidad(4);
inventario.EstablecerCantidad(12);

// Se desuscribe un observador y se realiza otro cambio
Console.WriteLine("\n--- La pantalla se desuscribe ---\n");
inventario.Desuscribir(observadorPantalla);
inventario.EstablecerCantidad(3);

Console.WriteLine("\n--- Historial registrado ---");
foreach (var cambio in observadorHistorial.Cambios)
{
    Console.WriteLine(
        $"{cambio.NombreProducto}: {cambio.CantidadAnterior} -> {cambio.CantidadActual}");
}
