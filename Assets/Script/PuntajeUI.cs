using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PuntajeUI : MonoBehaviour
{
    public static PuntajeUI Instance { get; private set; }

    [SerializeField] TextMeshProUGUI textoPuntaje;

    void Awake()
    {
        Instance = this;
    }

    async void Start()
    {
        // Muestra el puntaje actual apenas arranca la escena
        // (por si ya había disparos guardados de antes).
        await ActualizarPuntaje();
    }

    // Se llama desde Proyectil cada vez que se guarda un disparo,
    // así el texto siempre refleja el total actualizado sin necesidad de botón.
    public async Task ActualizarPuntaje()
    {
        int puntajeTotal = await CloudSaveManager.Instance.ObtenerPuntajeTotal();

        if (textoPuntaje != null)
            textoPuntaje.text = $"Puntaje total: {puntajeTotal}";
    }
}