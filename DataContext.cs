using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SIG_O;

public static class DataContext
{
    private const string PathCatalogo = "catalogo_general.json";
    private const string PathExistencias = "existencias_bodega.json";
    private const string PathGarantias = "registro_garantias.json";
    private const string PathCategorias = "categorias_dinamicas.json";
    private const string PathContador = "contador_global.json";

    public static List<ProductoCatalogo> ObtenerCatalogo() => LeerJson<ProductoCatalogo>(PathCatalogo);
    public static void SalvarCatalogo(List<ProductoCatalogo> lista) => EscribirJson(PathCatalogo, lista);

    public static List<UnidadInventario> ObtenerExistencias() => LeerJson<UnidadInventario>(PathExistencias);
    public static void SalvarExistencias(List<UnidadInventario> lista) => EscribirJson(PathExistencias, lista);

    public static List<RegistroGarantia> ObtenerGarantias() => LeerJson<RegistroGarantia>(PathGarantias);
    public static void SalvarGarantias(List<RegistroGarantia> lista) => EscribirJson(PathGarantias, lista);

    public static List<string> ObtenerCategorias()
    {
        var cats = LeerJson<string>(PathCategorias);
        if (!cats.Any())
        {
            cats = new List<string> { "Tecnología", "Legos", "Relojes" };
            EscribirJson(PathCategorias, cats);
        }
        return cats;
    }
    public static void SalvarCategorias(List<string> lista) => EscribirJson(PathCategorias, lista);

    public static string ObtenerNuevoCodigoUnico()
    {
        var estado = LeerEstadoContador();
        estado.ValorSecuencial++; 
        
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(PathContador, JsonSerializer.Serialize(estado, options));

        return $"ORC-{estado.ValorSecuencial:D6}";
    }

    private static EstadoContador LeerEstadoContador()
    {
        if (!File.Exists(PathContador)) return new EstadoContador { ValorSecuencial = 0 };
        return JsonSerializer.Deserialize<EstadoContador>(File.ReadAllText(PathContador)) ?? new EstadoContador { ValorSecuencial = 0 };
    }

    private static List<T> LeerJson<T>(string path)
    {
        if (!File.Exists(path)) return new List<T>();
        return JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path)) ?? new List<T>();
    }

    private static void EscribirJson<T>(string path, List<T> lista)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(path, JsonSerializer.Serialize(lista, options));
    }
}