# Inventario con Observer, Strategy y Facade

Aplicación de consola en **.NET 10**. Conserva Observer y agrega dos patrones de
los adjuntos: **Strategy** (comportamiento) y **Facade** (estructural). Son fáciles
de demostrar con operaciones del inventario: vender y reponer productos.
Los datos y el historial se mantienen en memoria durante la ejecución.

## Ejecutar y verificar

Desde la raíz del repositorio, con el SDK de .NET 10 instalado:

```powershell
dotnet run --project ObserverStockDemo.csproj
dotnet run --project Pruebas/Pruebas.csproj
```

El segundo comando ejecuta verificaciones automáticas sin paquetes externos.
Comprueba ventas, ambas estrategias, cambio de estrategia, notificaciones,
desuscripción, historial, operaciones inválidas y desbordamiento de cantidades.
Un fallo produce una excepción y un código de salida de error.

## 1. Observer: avisar cuando cambia el inventario

- **Sujeto:** `Dominio/InventarioProducto.cs`, implementa `IObservableInventario`.
- **Contrato:** `Contratos/IObservadorInventario.cs`, método `Actualizar`.
- **Observadores:** `ObservadorPantallaInventario`, `ObservadorHistorialInventario`
  y `ObservadorAlertaExistenciasBajas`.
- **Evento:** `CambioInventario`, contiene producto, cantidad anterior y actual.

`EstablecerCantidad` modifica las existencias y notifica a los suscriptores.
Si la cantidad no cambia, no genera evento. La pantalla muestra el cambio,
el historial lo registra y la alerta avisa si quedan 5 unidades o menos.
El envío de alertas se simula en consola.

**Aporte:** el inventario no conoce los detalles de cada reacción. Se pueden
agregar observadores sin cambiar su lógica.

**Cómo explicarlo:** «Cuando cambia la cantidad, el inventario avisa a todos los
suscriptores. Cada uno decide qué hacer con ese mismo cambio».

## 2. Strategy: elegir cómo reponer productos

| Participante | Código | Responsabilidad |
| --- | --- | --- |
| Estrategia | `Contratos/IEstrategiaReposicion.cs` | Define `CalcularUnidades(cantidadActual)` |
| Estrategia concreta | `Estrategias/ReposicionLoteFijo.cs` | Agrega siempre un lote configurado |
| Estrategia concreta | `Estrategias/ReposicionHastaObjetivo.cs` | Calcula lo que falta para llegar al objetivo |
| Contexto | `Servicios/ServicioReposicion.cs` | Usa y permite cambiar la estrategia |

`ServicioReposicion` mantiene una referencia a `IEstrategiaReposicion`. Su método
`Reponer` delega el cálculo y aplica el resultado mediante `EstablecerCantidad`.
No necesita preguntar qué clase de estrategia recibió. `CambiarEstrategia`
permite reemplazar el algoritmo durante la ejecución.

Ejemplo de uso, con las dependencias de `Program.cs`:

```csharp
var reposicion = new ServicioReposicion(new ReposicionLoteFijo(8));
var tienda = new FachadaInventario(inventario, reposicion, observadorHistorial);
tienda.Reponer(); // Si hay 4, agrega 8 y quedan 12.
reposicion.CambiarEstrategia(new ReposicionHastaObjetivo(20));
tienda.Reponer(); // Si hay 12, agrega 8 y quedan 20.
tienda.Reponer(); // Ya hay 20: agrega 0 y no notifica.
```

La estrategia hasta objetivo nunca retira unidades si ya se supera el objetivo.
Los lotes y objetivos deben ser positivos; una suma que exceda `int.MaxValue`
se rechaza antes de cambiar el inventario.

**Aporte:** separa las reglas de reposición del inventario y del servicio que las
aplica. Para agregar otra regla basta implementar el contrato y seleccionarla.

**Cómo explicarlo:** «La tarea es reponer, pero la forma de calcular la cantidad
puede cambiar. Una estrategia agrega un lote fijo y otra completa un objetivo.
El servicio trabaja con el mismo contrato para ambas».

## 3. Facade: operar el inventario con una interfaz sencilla

`Fachadas/FachadaInventario.cs` coordina `InventarioProducto`, `ServicioReposicion`
y `ObservadorHistorialInventario`. Recibe esas dependencias en el constructor
y suscribe el historial al inventario.

Expone cuatro operaciones:

- `Vender(unidades)`: valida que sean positivas y que haya existencias, y descuenta.
- `Reponer()`: delega al servicio y devuelve las unidades agregadas.
- `ConsultarExistencias()`: devuelve la cantidad actual.
- `ConsultarHistorial()`: permite leer los cambios registrados.

El cliente llama `tienda.Vender(3)` o `tienda.Reponer()` sin coordinar por su cuenta
el cálculo, la modificación y la consulta del historial. Las notificaciones se
producen mediante Observer al modificar el inventario. Una venta inválida no
altera existencias ni genera notificaciones.

**Aporte:** ofrece una interfaz sencilla para varios componentes. `Program.cs`
conecta las dependencias y configura la estrategia; la fachada facilita las
operaciones habituales. Los componentes siguen disponibles, por ejemplo para
desuscribir la pantalla o cambiar la estrategia.

**Cómo explicarlo:** «La fachada es el punto de entrada para las operaciones de
la tienda. Le pido vender o reponer y ella delega en los componentes
correspondientes. Así el cliente necesita conocer menos detalles del sistema».

## Demostración paso a paso

| Acción | Existencias | Resultado |
| --- | --- | --- |
| Crear inventario | 10 | Se conectan los tres observadores |
| Vender 3 | 10 → 7 | Pantalla e historial reaccionan; alerta no cumple el umbral |
| Vender 3 | 7 → 4 | También se muestra la alerta |
| Reponer lote de 8 | 4 → 12 | Observer comunica el cambio |
| Cambiar estrategia y reponer hasta 20 | 12 → 20 | Mismo servicio, otro algoritmo |
| Reponer otra vez | 20 → 20 | Sin evento ni entrada adicional en el historial |
| Desuscribir pantalla y vender 17 | 20 → 3 | Historial y alerta siguen recibiendo cambios |

El historial final contiene exactamente cinco cambios:
`10 → 7`, `7 → 4`, `4 → 12`, `12 → 20`, `20 → 3`.

Para exponerlo con el depurador, coloca puntos de interrupción en
`FachadaInventario.Vender`, `ServicioReposicion.Reponer`, los dos métodos
`CalcularUnidades` y `InventarioProducto.Notificar`. Observa cómo una operación
pasa por la fachada, utiliza una estrategia cuando repone y termina notificando
mediante Observer.

## Relación con SOLID

- **Responsabilidad única:** las estrategias calculan; el servicio aplica la
  reposición; la fachada coordina operaciones; cada observador tiene su reacción.
- **Abierto/cerrado:** nuevas estrategias y observadores se agregan mediante sus
  contratos sin modificar el servicio de reposición ni el sujeto.
- **Sustitución de Liskov:** ambas estrategias devuelven unidades no negativas
  para existencias válidas y pueden sustituirse en el mismo contexto.
- **Segregación de interfaces:** los contratos son pequeños y específicos.
- **Inversión de dependencias:** el servicio depende de `IEstrategiaReposicion`,
  el sujeto de `IObservadorInventario` y la alerta de `IEnviadorNotificaciones`.
