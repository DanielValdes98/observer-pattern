# Demostracion del patron Observador

Aplicacion de consola en .NET 10 que demuestra el patron de comportamiento
**Observer (Observador)** mediante cambios en el inventario de un producto.

## Ejecutar

```powershell
cd C:\repos\ObserverStockDemo
dotnet run --project ObserverStockDemo.csproj
```

## Participantes del patron

- **Sujeto / Observable:** `InventarioProducto`. Conserva los observadores y les
  avisa cuando cambia la cantidad.
- **Observador:** `IObservadorInventario`. Define el metodo comun `Actualizar`.
- **Observadores concretos:** `ObservadorPantallaInventario`,
  `ObservadorAlertaExistenciasBajas` y `ObservadorHistorialInventario`.
- **Evento:** `CambioInventario`. Transporta la informacion del cambio sin
  exponer el estado interno del sujeto.

El sujeto solo conoce objetos mediante `IObservadorInventario`; no sabe si
muestran datos, envian alertas o guardan un historial.

## Flujo y puntos de interrupcion sugeridos

1. `Program.cs`, en `inventario.Suscribir(observadorPantalla)`: observar como se
   registran las instancias mediante el contrato `IObservadorInventario`.
2. `InventarioProducto.EstablecerCantidad`, al crear `CambioInventario`: revisar
   la cantidad anterior y la nueva.
3. `InventarioProducto.Notificar`, en `observador.Actualizar(cambio)`: recorrer
   paso a paso la lista. La misma llamada polimorfica entra en tres clases.
4. En el metodo `Actualizar` de cada observador: comprobar que cada uno tiene una
   reaccion independiente.
5. `Program.cs`, en `inventario.Desuscribir(observadorPantalla)`: continuar hasta
   el ultimo cambio y comprobar que la pantalla ya no recibe la notificacion,
   mientras el historial y la alerta si la reciben.

## Principios SOLID

### S — Responsabilidad unica

Cada clase tiene un motivo principal de cambio: `InventarioProducto` administra
el inventario y publica sus cambios; `ObservadorPantallaInventario` muestra las
existencias; `ObservadorHistorialInventario` guarda el historial;
`ObservadorAlertaExistenciasBajas` decide cuando alertar, y
`EnviadorNotificacionesConsola` define como se entrega la alerta.

### O — Abierto/cerrado

Se puede agregar otro observador, por ejemplo uno que persista los cambios en una
base de datos, implementando `IObservadorInventario` y suscribiendolo. No hay que
modificar `InventarioProducto` ni los observadores existentes.

### L — Sustitucion de Liskov

Los tres observadores se pueden utilizar donde se espera un
`IObservadorInventario`. El sujeto invoca `Actualizar` de la misma manera y
ninguna implementacion rompe el contrato.

### I — Segregacion de interfaces

`IObservableInventario`, `IObservadorInventario` e `IEnviadorNotificaciones` son
contratos pequeños y especificos. Una clase que solo recibe cambios no esta
obligada a implementar metodos para suscribir o enviar mensajes.

### D — Inversion de dependencias

`InventarioProducto` depende de `IObservadorInventario`, no de observadores
concretos. Ademas, `ObservadorAlertaExistenciasBajas` depende de
`IEnviadorNotificaciones`, no de la consola. Se podria sustituir por un enviador
de correo sin cambiar la regla de existencias bajas. Las implementaciones se
conectan en `Program.cs`.
