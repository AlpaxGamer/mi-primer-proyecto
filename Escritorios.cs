using System;
using System.Collections.Generic;
using System.Linq;

namespace SIG_O;

// 1. MÓDULO FINANCIERO (Davis)
public static class EscritorioCuentas
{
    public static double TarifaAerea = 6.3;
    public static double TarifaMaritima = 3.0;
    public static double CostoEmbalaje = 0.5;
    public static double PorcentajeIVA = 0.15;
    public static double TipoCambio = 36.62;

    public static void Ejecutar()
    {
        Console.Clear();
        Console.WriteLine("=== 1. ESCRITORIO DE CUENTAS (CÁLCULO FINANCIERO) ===\n");

        string nombreProducto = Validador.LeerCadenaObligatoria("Ingrese el nombre del producto a cotizar: ");
        
        var catalogo = DataContext.ObtenerCatalogo();
        var producto = catalogo.FirstOrDefault(p => p.NombreModelo.Equals(nombreProducto, StringComparison.OrdinalIgnoreCase));

        if (producto == null)
        {
            Console.WriteLine("\n[!] El producto no existe en el catálogo. Regístrelo primero en Bodega.");
            Console.ReadKey();
            return;
        }

        double precioCompra = Validador.LeerDoublePositivo("Ingrese el precio de compra base del proveedor (USD): ");
        Console.WriteLine("Seleccione el método de envío:\n 1. Vía Aérea\n 2. Vía Marítima");
        int via = Validador.LeerIntPositivo("Opción (1 o 2): ");
        double porcentajeGanancia = Validador.LeerDoublePositivo("Ingrese el porcentaje (%) de ganancia deseado: ");

        double tarifaEnvio = (via == 1) ? TarifaAerea : TarifaMaritima;
        double costoLogistico = producto.PesoLibras * tarifaEnvio;
        double costoTotalBase = precioCompra + costoLogistico + CostoEmbalaje;
        double margenGanancia = costoTotalBase * (porcentajeGanancia / 100);
        double subtotal = costoTotalBase + margenGanancia;
        double precioFinalUSD = subtotal * (1 + PorcentajeIVA);
        double precioFinalNIO = precioFinalUSD * TipoCambio;

        producto.PrecioVentaUSD = Math.Round(precioFinalUSD, 2);
        producto.PrecioVentaNIO = Math.Round(precioFinalNIO, 2);
        DataContext.SalvarCatalogo(catalogo);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n--- RESULTADO DE LA COTIZACIÓN ---");
        Console.WriteLine($"Costo Logístico Calculado: ${Math.Round(costoLogistico, 2)}");
        Console.WriteLine($"Precio Final de Venta (USD): ${producto.PrecioVentaUSD}");
        Console.WriteLine($"Precio Final de Venta (NIO): C$ {producto.PrecioVentaNIO}");
        Console.WriteLine("Los precios han sido inyectados automáticamente al Catálogo General.");
        Console.ResetColor();
        Console.ReadKey();
    }
}

// 2. MÓDULO DE BODEGA / REGISTRO (Carlos)
public static class EscritorioBodega
{
    public static void Ejecutar()
    {
        Console.Clear();
        Console.WriteLine("=== 2. ESCRITORIO DE REGISTRO EN BODEGA ===\n");

        var categorias = DataContext.ObtenerCategorias();
        Console.WriteLine("Seleccione la categoría del artículo:");
        for (int i = 0; i < categorias.Count; i++) Console.WriteLine($" {i + 1}. {categorias[i]}");
        Console.WriteLine($" {categorias.Count + 1}. [Crear Nueva Categoría Dinámica]");

        int opCat = Validador.LeerIntPositivo("Opción: ");
        string categoriaElegida = "";

        if (opCat > 0 && opCat <= categorias.Count)
        {
            categoriaElegida = categorias[opCat - 1];
        }
        else
        {
            categoriaElegida = Validador.LeerCadenaObligatoria("Nombre de la nueva categoría: ");
            categorias.Add(categoriaElegida);
            DataContext.SalvarCategorias(categorias);
        }

        string nombreArticulo = Validador.LeerCadenaObligatoria("Ingrese el nombre del artículo: ");
        double pesoArticulo = Validador.LeerDoublePositivo("Ingrese el peso estimado en libras: ");

        var catalogo = DataContext.ObtenerCatalogo();
        if (!catalogo.Any(p => p.NombreModelo.Equals(nombreArticulo, StringComparison.OrdinalIgnoreCase)))
        {
            catalogo.Add(new ProductoCatalogo
            {
                NombreModelo = nombreArticulo,
                Categoria = categoriaElegida,
                PesoLibras = pesoArticulo,
                PrecioVentaUSD = 0,
                PrecioVentaNIO = 0
            });
            DataContext.SalvarCatalogo(catalogo);
        }

        var existencias = DataContext.ObtenerExistencias();
        string codigoGenerado = DataContext.ObtenerNuevoCodigoUnico(); 
        
        existencias.Add(new UnidadInventario
        {
            CodigoSerie = codigoGenerado,
            NombreModelo = nombreArticulo,
            Vendido = false
        });
        DataContext.SalvarExistencias(existencias);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n--- CONFIRMACIÓN DE REGISTRO ---");
        Console.WriteLine($"Artículo agregado al catálogo y a las existencias físicas.");
        Console.WriteLine($"CÓDIGO ÚNICO AUTOMATIZADO GENERADO: {codigoGenerado}");
        Console.ResetColor();
        Console.ReadKey();
    }
}

// 3. MÓDULO DE VENTAS Y RECIBOS (David)
public static class EscritorioGarantias
{
    public static void Ejecutar()
    {
        Console.Clear();
        Console.WriteLine("=== 3. ESCRITORIO DE RECIBOS Y GARANTÍAS ===\n");

        string cedula = Validador.LeerCadenaObligatoria("Ingrese la cédula del cliente: ");
        string nombreCliente = Validador.LeerCadenaObligatoria("Ingrese el nombre completo del cliente: ");
        string codigoSerie = Validador.LeerCadenaObligatoria("Ingrese el código único de serie de la unidad a vender: ").ToUpper();

        var existencias = DataContext.ObtenerExistencias();
        var unidad = existencias.FirstOrDefault(e => e.CodigoSerie == codigoSerie && !e.Vendido);

        if (unidad == null)
        {
            Console.WriteLine("\n[!] Error: El artículo no existe o ya ha sido marcado como vendido.");
            Console.ReadKey();
            return;
        }

        var catalogo = DataContext.ObtenerCatalogo();
        var producto = catalogo.First(p => p.NombreModelo == unidad.NombreModelo);

        unidad.Vendido = true; 
        DataContext.SalvarExistencias(existencias);

        DateTime fechaActual = DateTime.Now;
        DateTime fechaVencimiento = fechaActual.AddMonths(10); 

        var garantias = DataContext.ObtenerGarantias();
        garantias.Add(new RegistroGarantia
        {
            CedulaComprador = cedula,
            NombreCliente = nombreCliente,
            CodigoSerie = unidad.CodigoSerie,
            NombreModelo = unidad.NombreModelo,
            PrecioPagadoNIO = producto.PrecioVentaNIO,
            FechaCompra = fechaActual,
            FechaVencimiento = fechaVencimiento
        });
        DataContext.SalvarGarantias(garantias);

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("=================================================");
        Console.WriteLine("           COMPROBANTE FORMAL DE GARANTÍA        ");
        Console.WriteLine("=================================================");
        Console.ResetColor();
        Console.WriteLine($"CLIENTE: {nombreCliente} | CÉDULA: {cedula}");
        Console.WriteLine($"ARTÍCULO: {unidad.NombreModelo}");
        Console.WriteLine($"NÚMERO DE SERIE: {unidad.CodigoSerie}");
        Console.WriteLine($"PRECIO ABONADO: C$ {producto.PrecioVentaNIO}");
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine($"FECHA DE EMISIÓN: {fechaActual:dd/MM/yyyy}");
        Console.WriteLine($"VENCIMIENTO DE COBERTURA: {fechaVencimiento:dd/MM/yyyy}");
        Console.WriteLine("VIGENCIA DE COBERTURA: 10 Meses Exactos.");
        Console.WriteLine("=================================================");
        Console.ReadKey();
    }
}

// 4. MÓDULO DE REPORTES Y VISUALIZACIÓN (David)
public static class ModuloVisualizacion
{
    public static void Ejecutar()
    {
        bool enMenu = true;
        while (enMenu)
        {
            Console.Clear();
            Console.WriteLine("=== 4. MÓDULO DE VISUALIZACIÓN DE DATOS ===\n");
            Console.WriteLine("1. Bodega Sencilla (Catálogo agrupado y conteo síncrono)");
            Console.WriteLine("2. Bodega Compleja (Detalle físico unitario)");
            Console.WriteLine("3. Historial de Recibos y Garantías");
            Console.WriteLine("4. Regresar al Menú Principal");
            int opcion = Validador.LeerIntPositivo("\nSeleccione el reporte deseado: ");

            switch (opcion)
            {
                case 1: MostrarBodegaSencilla(); break;
                case 2: MostrarBodegaCompleja(); break;
                case 3: MostrarGarantias(); break;
                case 4: enMenu = false; break;
                default: Console.WriteLine("Opción no válida."); Console.ReadKey(); break;
            }
        }
    }

    private static void MostrarBodegaSencilla()
    {
        Console.Clear();
        Console.WriteLine("--- REPORTE: BODEGA SENCILLA ---\n");
        var catalogo = DataContext.ObtenerCatalogo();
        var existencias = DataContext.ObtenerExistencias();

        var categorias = catalogo.GroupBy(c => c.Categoria);
        foreach (var cat in categorias)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n[CATEGORÍA: {cat.Key.ToUpper()}]");
            Console.ResetColor();

            foreach (var prod in cat)
            {
                int stock = existencias.Count(e => e.NombreModelo == prod.NombreModelo && !e.Vendido);
                Console.Write($"- {prod.NombreModelo} | Peso: {prod.PesoLibras} lbs | Precio: C$ {prod.PrecioVentaNIO} | STOCK: {stock}");
                
                if (stock <= 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" [! ALERTA: STOCK CRÍTICO]");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
        Console.ReadKey();
    }

    private static void MostrarBodegaCompleja()
    {
        Console.Clear();
        Console.WriteLine("--- REPORTE: BODEGA COMPLEJA ---\n");
        var existencias = DataContext.ObtenerExistencias();

        foreach (var e in existencias)
        {
            string estado = e.Vendido ? "Vendido" : "Disponible";
            if (!e.Vendido) Console.ForegroundColor = ConsoleColor.Green;
            
            Console.WriteLine($"Código: {e.CodigoSerie,-12} | Modelo: {e.NombreModelo,-20} | Estado: {estado}");
            Console.ResetColor();
        }
        Console.ReadKey();
    }

    private static void MostrarGarantias()
    {
        Console.Clear();
        Console.WriteLine("--- HISTORIAL DE CLIENTES Y GARANTÍAS ---\n");
        var garantias = DataContext.ObtenerGarantias();

        foreach (var g in garantias)
        {
            Console.WriteLine($"Cliente: {g.NombreCliente} | Artículo: {g.NombreModelo}");
            Console.WriteLine($"Serie: {g.CodigoSerie} | Vence: {g.FechaVencimiento:dd/MM/yyyy}");
            Console.WriteLine(new string('-', 40));
        }
        Console.ReadKey();
    }
}