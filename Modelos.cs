using System;

namespace SIG_O;

// =========================================================================
// ENTIDADES DE DATOS (MOLDES DE PERSISTENCIA)
// =========================================================================

public class ProductoCatalogo
{
    public string NombreModelo { get; set; }
    public string Categoria { get; set; }
    public double PesoLibras { get; set; }
    public double PrecioCompraUSD { get; set; }
    public double PrecioVentaUSD { get; set; }
    public double PrecioVentaNIO { get; set; }
}

public class UnidadInventario
{
    public string CodigoSerie { get; set; }
    public string NombreModelo { get; set; }
    public bool Vendido { get; set; }
}

public class RegistroGarantia
{
    public string CedulaComprador { get; set; }
    public string NombreCliente { get; set; }
    public string CodigoSerie { get; set; }
    public string NombreModelo { get; set; }
    public double PrecioPagadoNIO { get; set; }
    public DateTime FechaCompra { get; set; }
    public DateTime FechaVencimiento { get; set; }
}

public class EstadoContador
{
    public int ValorSecuencial { get; set; }
}

// =========================================================================
// COMPONENTE DE VALIDACIÓN ESTRICTA (CONTROL INTERNO)
// =========================================================================

public static class Validador
{
    public static double LeerDoublePositivo(string mensaje)
    {
        double valor;
        while (true)
        {
            Console.Write(mensaje);
            string entrada = Console.ReadLine();
            if (double.TryParse(entrada, out valor) && valor >= 0) return valor;
            MostrarError("Debe ingresar un valor numérico positivo. No se permiten letras ni números negativos.");
        }
    }

    public static int LeerIntPositivo(string mensaje)
    {
        int valor;
        while (true)
        {
            Console.Write(mensaje);
            string entrada = Console.ReadLine();
            if (int.TryParse(entrada, out valor) && valor >= 0) return valor;
            MostrarError("Debe ingresar un número entero positivo válido.");
        }
    }

    public static string LeerCadenaObligatoria(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string entrada = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(entrada)) return entrada;
            MostrarError("Este campo es obligatorio y no puede quedar vacío.");
        }
    }

    private static void MostrarError(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[!] Error de Validación: {mensaje}");
        Console.ResetColor();
    }
}