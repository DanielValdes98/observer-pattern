using DemoObservadorInventario.Dominio;
using DemoObservadorInventario.Estrategias;
using DemoObservadorInventario.Fachadas;
using DemoObservadorInventario.Infraestructura;
using DemoObservadorInventario.Observadores;
using DemoObservadorInventario.Servicios;

var inventario = new InventarioProducto("Teclado", 10);
var historial = new ObservadorHistorialInventario();
var salida = new StringWriter();
var alertas = new StringWriter();
var pantalla = new ObservadorPantallaInventario(salida);
inventario.Suscribir(pantalla);
inventario.Suscribir(new ObservadorAlertaExistenciasBajas(new EnviadorNotificacionesConsola(alertas), 5));
var reposicion = new ServicioReposicion(new ReposicionLoteFijo(8));
var tienda = new FachadaInventario(inventario, reposicion, historial);
tienda.Vender(3);
tienda.Vender(3);
Verificar(tienda.ConsultarExistencias() == 4, "Ventas descuentan existencias.");
Verificar(salida.ToString().Contains("4 unidades") && alertas.ToString().Contains("4 unidades"), "Pantalla y alerta reciben cambios.");
Verificar(tienda.Reponer() == 8 && tienda.ConsultarExistencias() == 12, "Reposicion por lote.");
reposicion.CambiarEstrategia(new ReposicionHastaObjetivo(20));
Verificar(tienda.Reponer() == 8 && tienda.ConsultarExistencias() == 20, "Cambio de estrategia en ejecucion.");
Verificar(tienda.Reponer() == 0 && historial.Cambios.Count == 4, "Sin cambio no hay evento.");
reposicion.CambiarEstrategia(new ReposicionHastaObjetivo(15));
Verificar(tienda.Reponer() == 0 && tienda.ConsultarExistencias() == 20, "Reponer no reduce existencias.");
DebeFallar<InvalidOperationException>(() => tienda.Vender(21));
DebeFallar<ArgumentOutOfRangeException>(() => tienda.Vender(0));
DebeFallar<ArgumentOutOfRangeException>(() => tienda.Vender(-1));
DebeFallar<ArgumentOutOfRangeException>(() => new ReposicionLoteFijo(0));
DebeFallar<ArgumentOutOfRangeException>(() => new ReposicionHastaObjetivo(-1));
DebeFallar<ArgumentNullException>(() => reposicion.CambiarEstrategia(null!));
Verificar(tienda.ConsultarExistencias() == 20 && historial.Cambios.Count == 4, "Errores no modifican ni notifican.");
inventario.Desuscribir(pantalla);
string antes = salida.ToString();
tienda.Vender(17);
Verificar(salida.ToString() == antes && alertas.ToString().Contains("3 unidades"), "Desuscripcion conserva los otros observadores.");
Verificar(tienda.ConsultarHistorial().SequenceEqual(new[]
{
    new CambioInventario("Teclado", 10, 7), new CambioInventario("Teclado", 7, 4),
    new CambioInventario("Teclado", 4, 12), new CambioInventario("Teclado", 12, 20),
    new CambioInventario("Teclado", 20, 3)
}), "Historial exacto de cinco cambios.");
var lleno = new InventarioProducto("Limite", int.MaxValue);
var historialLimite = new ObservadorHistorialInventario();
var tiendaLimite = new FachadaInventario(lleno, new ServicioReposicion(new ReposicionLoteFijo(1)), historialLimite);
DebeFallar<OverflowException>(() => tiendaLimite.Reponer());
Verificar(lleno.Cantidad == int.MaxValue && historialLimite.Cambios.Count == 0, "Desbordamiento no modifica ni notifica.");
Console.WriteLine("Todas las verificaciones pasaron.");

static void Verificar(bool condicion, string mensaje)
{
    if (!condicion) throw new Exception(mensaje);
    Console.WriteLine($"OK: {mensaje}");
}

static void DebeFallar<T>(Action accion) where T : Exception
{
    try { accion(); }
    catch (T) { return; }
    throw new Exception($"Se esperaba {typeof(T).Name}.");
}
