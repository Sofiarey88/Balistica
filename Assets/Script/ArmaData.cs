
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ArmaData
{
    [Header("Proyectil")]
    public GameObject proyectil;
    public Transform spawnPoint;
    public float force = 10f;

    [Header("UI - Fuerza")]
    public Slider forceSlider;
    public TextMeshProUGUI forceText;

    [Header("UI - Ángulo")]
    public Slider angleSlider;
    public TextMeshProUGUI angleText;

    [Header("UI - Peso")]
    public Slider weightSlider;
    public TextMeshProUGUI weightText;
}
