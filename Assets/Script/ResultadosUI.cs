using System.Text;
using TMPro;
using UnityEngine;

public class ResultadosUI : MonoBehaviour
{
    [SerializeField] GameObject panelResultados;
    [SerializeField] TextMeshProUGUI textoResultados;

    public async void MostrarResultados()
    {
        if (panelResultados != null) panelResultados.SetActive(true);
        if (textoResultados != null) textoResultados.text = "Cargando...";

        var lista = await CloudSaveManager.Instance.ObtenerResultados();

        if (lista == null || lista.Count == 0)
        {
            if (textoResultados != null)
                textoResultados.text = "<color=#999999><i>No hay disparos guardados todavía.</i></color>";
            return;
        }

        StringBuilder sb = new StringBuilder();

        // Recorremos de atrás para adelante para mostrar el disparo
        // más reciente primero (antes se mostraba el más viejo arriba
        // y había que scrollear para ver el último tiro).
        int total = lista.Count;
        for (int i = total - 1; i >= 0; i--)
        {
            var r = lista[i];
            int numero = i + 1; // numeración según el orden real de disparo
            string colorResultado = r.acierto ? "#4CAF50" : "#E57373"; // verde o rojo
            string etiqueta = r.acierto ? "ACIERTO" : "FALLO";

            sb.AppendLine($"<b>#{numero}</b>  <size=80%><color=#AAAAAA>{r.fecha}</color></size>");
            sb.AppendLine($"Ángulo: <b>{r.angulo:F1}°</b>   Fuerza: <b>{r.fuerza:F1}</b>   Masa: <b>{r.masa:F1}</b>");
            sb.AppendLine(r.acierto
                ? $"<color={colorResultado}><b>{etiqueta}</b></color>  —  Distancia: {r.distancia:F2} m  —  {r.objetosAfectados} obj. afectados"
                : $"<color={colorResultado}><b>{etiqueta}</b></color>  —  Distancia: {r.distancia:F2} m");
            sb.AppendLine("<size=50%> </size>"); // espacio entre tarjetas
            sb.AppendLine("――――――――――――――――――――――――");
            sb.AppendLine();
        }

        if (textoResultados != null) textoResultados.text = sb.ToString();
    }

    public async void BorrarHistorial()
    {
        if (textoResultados != null) textoResultados.text = "Borrando...";

        await CloudSaveManager.Instance.BorrarResultados();

        if (textoResultados != null)
            textoResultados.text = "<color=#999999><i>No hay disparos guardados todavía.</i></color>";
    }

    public void CerrarPanel()
    {
        if (panelResultados != null) panelResultados.SetActive(false);
    }
}