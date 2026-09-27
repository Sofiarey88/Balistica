using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

public class CloudSaveManager : MonoBehaviour
{
    public static CloudSaveManager Instance { get; private set; }
    const string KEY_RESULTADOS = "resultados_disparos";
    bool inicializado = false;

    // Fuente de verdad en memoria. Se carga UNA sola vez y a partir
    // de ahí solo se agrega en local y se sube, sin volver a leer
    // la nube antes de cada guardado (eso era lo que causaba la
    // condición de carrera).
    List<ResultadoDisparo> listaLocal = null;

    // Evita que dos guardados corran en paralelo si se dispara muy rápido.
    readonly SemaphoreSlim semaforoGuardado = new SemaphoreSlim(1, 1);

    async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        await InicializarUGS();
    }

    async Task InicializarUGS()
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            inicializado = true;
            Debug.Log("UGS inicializado. PlayerId: " + AuthenticationService.Instance.PlayerId);
        }
        catch (Exception e)
        {
            Debug.LogError("Error inicializando UGS: " + e);
            LogInnerExceptions(e);
        }
    }

    // Imprime toda la cadena de InnerException y, si corresponde,
    // todas las excepciones agrupadas dentro de un AggregateException.
    // Esto es lo que permite ver la causa REAL detrás del mensaje
    // genérico "Some services couldn't be initialized".
    void LogInnerExceptions(Exception e)
    {
        var inner = e.InnerException;
        int nivel = 1;
        while (inner != null)
        {
            Debug.LogError($"Inner exception (nivel {nivel}): {inner}");
            inner = inner.InnerException;
            nivel++;
        }

        if (e is AggregateException agg)
        {
            foreach (var ex in agg.InnerExceptions)
                Debug.LogError("Aggregate inner: " + ex);
        }
    }

    // Carga la lista de la nube UNA sola vez y la cachea en listaLocal.
    // Las llamadas siguientes reutilizan la copia en memoria.
    async Task<List<ResultadoDisparo>> ObtenerListaLocal()
    {
        if (listaLocal != null)
            return listaLocal;

        if (!inicializado) await InicializarUGS();
        if (!inicializado)
        {
            listaLocal = new List<ResultadoDisparo>();
            return listaLocal;
        }

        try
        {
            var claves = new HashSet<string> { KEY_RESULTADOS };
            var datos = await CloudSaveService.Instance.Data.Player.LoadAsync(claves);

            if (datos.TryGetValue(KEY_RESULTADOS, out var item))
                listaLocal = item.Value.GetAs<List<ResultadoDisparo>>() ?? new List<ResultadoDisparo>();
            else
                listaLocal = new List<ResultadoDisparo>();
        }
        catch (Exception e)
        {
            Debug.LogError("Error cargando desde UGS: " + e);
            LogInnerExceptions(e);
            listaLocal = new List<ResultadoDisparo>();
        }

        return listaLocal;
    }

    public async Task GuardarResultado(ResultadoDisparo resultado)
    {
        // Serializa los guardados: si llega un segundo disparo mientras
        // el primero todavía se está guardando, espera su turno en vez
        // de leer datos viejos de la nube.
        await semaforoGuardado.WaitAsync();
        try
        {
            var lista = await ObtenerListaLocal();
            lista.Add(resultado);

            if (!inicializado) await InicializarUGS();
            if (!inicializado) return;

            var datos = new Dictionary<string, object> { { KEY_RESULTADOS, lista } };
            await CloudSaveService.Instance.Data.Player.SaveAsync(datos);

            Debug.Log("Resultado guardado en UGS. Total: " + lista.Count);
        }
        catch (Exception e)
        {
            Debug.LogError("Error guardando en UGS: " + e);
            LogInnerExceptions(e);
        }
        finally
        {
            semaforoGuardado.Release();
        }
    }

    public async Task<List<ResultadoDisparo>> ObtenerResultados()
    {
        // Si ya tenemos la lista en memoria (porque ya se guardó o
        // cargó antes), la devolvemos directamente para que la UI
        // siempre vea el estado más actualizado.
        var lista = await ObtenerListaLocal();
        return lista;
    }

    // Suma el puntaje de todos los disparos guardados.
    // No hace falta guardar un número aparte: se calcula a partir
    // de la misma lista que ya se guarda en la nube.
    public async Task<int> ObtenerPuntajeTotal()
    {
        var lista = await ObtenerListaLocal();
        int total = 0;
        foreach (var r in lista)
            total += r.puntaje;
        return total;
    }

    // Borra todo el historial guardado (útil para limpiar datos de prueba).
    public async Task BorrarResultados()
    {
        await semaforoGuardado.WaitAsync();
        try
        {
            listaLocal = new List<ResultadoDisparo>();

            if (!inicializado) await InicializarUGS();
            if (!inicializado) return;

            var datos = new Dictionary<string, object> { { KEY_RESULTADOS, listaLocal } };
            await CloudSaveService.Instance.Data.Player.SaveAsync(datos);

            Debug.Log("Historial de resultados borrado.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error borrando historial en UGS: " + e);
            LogInnerExceptions(e);
        }
        finally
        {
            semaforoGuardado.Release();
        }
    }
}