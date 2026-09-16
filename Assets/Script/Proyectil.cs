using UnityEngine;
using TMPro;

[System.Serializable]
public class ProyectilData
{
    public float lifeTime = 5f;
    public string groundTag = "Ground";
    public string targetTag = "Target";
    public TextMeshProUGUI textoDistancia;
    public string distanceFormat = "F2";
    public float radioImpacto = 0f; // usado para "objetos afectados"
}

public class Proyectil : MonoBehaviour
{
    [SerializeField] ProyectilData data;

    Vector3 startPosition;
    float anguloDisparo, fuerzaDisparo, masaDisparo;

    void Start()
    {
        startPosition = transform.position;
        if (data != null && data.lifeTime > 0f)
            Destroy(gameObject, data.lifeTime);
    }

    public void ConfigurarDisparo(float angulo, float fuerza, float masa)
    {
        anguloDisparo = angulo;
        fuerzaDisparo = fuerza;
        masaDisparo = masa;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (data == null) return;

        bool esTarget = collision.gameObject.CompareTag(data.targetTag);
        bool esGround = collision.gameObject.CompareTag(data.groundTag);

        if (!esTarget && !esGround) return;

        float distancia = Vector3.Distance(startPosition, transform.position);
        Debug.Log("Distance traveled: " + distancia.ToString(data.distanceFormat));

        if (data.textoDistancia != null)
            data.textoDistancia.text = "Distancia: " + distancia.ToString(data.distanceFormat) + " m";

        int objetosAfectados = 0;
        if (esTarget && data.radioImpacto > 0f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, data.radioImpacto);
            objetosAfectados = hits.Length;
        }

        var resultado = new ResultadoDisparo(
            anguloDisparo, fuerzaDisparo, masaDisparo, esTarget, distancia, objetosAfectados
        );

        _ = CloudSaveManager.Instance.GuardarResultado(resultado);
    }
}