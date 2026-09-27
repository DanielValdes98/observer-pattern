using DemoObservadorInventario.Dominio;
using DemoObservadorInventario.Infraestructura;
using DemoObservadorInventario.Observadores;
using DemoObservadorInventario.Estrategias;
using DemoObservadorInventario.Fachadas;
using DemoObservadorInventario.Servicios;

Console.WriteLine("=== Inventario: Observer, Strategy y Facade ===\n");

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
inventario.Suscribir(observadorAlerta);

// La fachada conecta el historial y simplifica las operaciones de negocio.
var reposicion = new ServicioReposicion(new ReposicionLoteFijo(8));
var tienda = new FachadaInventario(inventario, reposicion, observadorHistorial);

Console.WriteLine("--- Facade: vender 3 unidades y luego otras 3 ---");
tienda.Vender(3); // 10 -> 7
tienda.Vender(3); // 7 -> 4: se genera una alerta.

Console.WriteLine("\n--- Strategy: reponer un lote fijo de 8 ---");
Console.WriteLine($"Unidades agregadas: {tienda.Reponer()}"); // 4 -> 12

Console.WriteLine("\n--- Strategy: cambiar a reposicion hasta 20 ---");
reposicion.CambiarEstrategia(new ReposicionHastaObjetivo(20));
Console.WriteLine($"Unidades agregadas: {tienda.Reponer()}"); // 12 -> 20
Console.WriteLine($"Segunda reposicion: {tienda.Reponer()}"); // Sin cambio ni avisos.

// Se desuscribe un observador y se realiza otro cambio
Console.WriteLine("\n--- La pantalla se desuscribe ---\n");
inventario.Desuscribir(observadorPantalla);
tienda.Vender(17); // 20 -> 3: solo reciben el cambio la alerta y el historial.

Console.WriteLine($"Existencias finales: {tienda.ConsultarExistencias()}");

Console.WriteLine("\n--- Historial registrado ---");
foreach (var cambio in tienda.ConsultarHistorial())
{
    Console.WriteLine(
        $"{cambio.NombreProducto}: {cambio.CantidadAnterior} -> {cambio.CantidadActual}");
}
