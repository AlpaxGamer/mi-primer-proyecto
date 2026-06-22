using System;

namespace SIG_O;

public class Program
{
    public static void Main()
    {
        bool corriendo = true;

        while (corriendo)
        {
            Console.Clear();
            Console.WriteLine("=================================================");
            Console.WriteLine("              SISTEMA ORCANA (SIG-O)             ");
            Console.WriteLine("=================================================");
            Console.WriteLine(" 1. Escritorio de Cuentas");
            Console.WriteLine(" 2. Escritorio de Registro en Bodega");
            Console.WriteLine(" 3. Escritorio de Recibos y Garantías");
            Console.WriteLine(" 4. Módulo de Visualización de Datos");
            Console.WriteLine(" 5. Salir del Sistema");
            Console.WriteLine("=================================================");
            Console.Write("Seleccione una opción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1": EscritorioCuentas.Ejecutar(); break;
                case "2": EscritorioBodega.Ejecutar(); break;
                case "3": EscritorioGarantias.Ejecutar(); break;
                case "4": ModuloVisualizacion.Ejecutar(); break;
                case "5":
                    Console.WriteLine("\n Cerrando sesion de forma segura...");
                    corriendo = false;
                    break;
                default:
                    Console.WriteLine("\n Opcion no valida. Por favor intente de nuevo.");
                    Console.ReadKey();
                    break;
            }
        }
    }
}
