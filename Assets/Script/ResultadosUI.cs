using System;
using System.Text;
using TMPro;
using UnityEngine;

public class ResultadosUI : MonoBehaviour
{
    [SerializeField] GameObject panelResultados;
    [SerializeField] TextMeshProUGUI textoResultados;

    const string MENSAJE_VACIO = "<color=#999999><i>No hay disparos guardados todavía.</i></color>";
    const string MENSAJE_CARGANDO = "Cargando...";
    const string MENSAJE_BORRANDO = "Borrando...";
    const string SEPARADOR = "――――――――――――――――――――――――";

    public async void MostrarResultados()
    {
        if (panelResultados != null) panelResultados.SetActive(true);
        SetTexto(MENSAJE_CARGANDO);

        try
        {
            var lista = await CloudSaveManager.Instance.ObtenerResultados();

            if (lista == null || lista.Count == 0)
            {
                SetTexto(MENSAJE_VACIO);
                return;
            }

            SetTexto(ConstruirTextoResultados(lista));
        }
        catch (Exception e)
        {
            Debug.LogError($"[ResultadosUI] Error al obtener resultados: {e}");
            SetTexto("<color=#FF6666>Error al cargar los resultados.</color>");
        }
    }

    public async void BorrarHistorial()
    {
        SetTexto(MENSAJE_BORRANDO);

        try
        {
            await CloudSaveManager.Instance.BorrarResultados();
            SetTexto(MENSAJE_VACIO);
        }
        catch (Exception e)
        {
            Debug.LogError($"[ResultadosUI] Error al borrar historial: {e}");
            SetTexto("<color=#FF6666>Error al borrar el historial.</color>");
        }
    }

    public void CerrarPanel()
    {
        if (panelResultados != null) panelResultados.SetActive(false);
    }

    // Recorremos de atrás para adelante para mostrar el disparo más
    // reciente primero (antes se mostraba el más viejo arriba y había
    // que scrollear para ver el último tiro).
    string ConstruirTextoResultados(System.Collections.Generic.List<ResultadoDisparo> lista)
    {
        var sb = new StringBuilder();
        int total = lista.Count;

        for (int i = total - 1; i >= 0; i--)
        {
            var r = lista[i];
            int numero = i + 1; // numeración según el orden real de disparo
            string etiqueta = r.acierto ? "ACIERTO" : "FALLO";

            sb.AppendLine($"<b>#{numero}</b>  <size=80%><color=#AAAAAA>{r.fecha}</color></size>");
            sb.AppendLine($"Ángulo: <b>{r.angulo:F1}°</b>   Fuerza: <b>{r.fuerza:F1}</b>   Masa: <b>{r.masa:F1}</b>");
            sb.AppendLine($"<b>{etiqueta}</b>  —  Distancia: {r.distancia:F2} m");
            sb.AppendLine("<size=50%> </size>"); // espacio entre tarjetas
            sb.AppendLine(SEPARADOR);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    void SetTexto(string texto)
    {
        if (textoResultados != null) textoResultados.text = texto;
    }
}