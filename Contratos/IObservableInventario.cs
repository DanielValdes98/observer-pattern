namespace DemoObservadorInventario.Contratos;

// Sujeto: administra suscriptores sin conocer sus clases concretas.
public interface IObservableInventario
{
    void Suscribir(IObservadorInventario observador);
    void Desuscribir(IObservadorInventario observador);
}
