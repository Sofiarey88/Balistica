using UnityEngine;
using TMPro;

[System.Serializable]
public class ProyectilData
{
    [Tooltip("Tiempo en segundos antes de destruir el proyectil automáticamente.")]
    public float lifeTime = 5f;

    [Tooltip("Tag que identifica el suelo.")]
    public string groundTag = "Ground";

    [Tooltip("Texto UI para mostrar la distancia; puede quedar vacío (null).")]
    public TextMeshProUGUI textoDistancia;

    [Tooltip("Formato para ToString de la distancia (ej. \"F2\").")]
    public string distanceFormat = "F2";
}

public class Proyectil : MonoBehaviour
{
    [SerializeField]
    ProyectilData data;

    Vector3 startPosition;
    float distanciaRecorrida = 0f;

    void Start()
    {
        startPosition = transform.position;

        if (data != null && data.lifeTime > 0f)
        {
            Destroy(gameObject, data.lifeTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (data == null) return;

        if (collision.gameObject.CompareTag(data.groundTag))
        {
            distanciaRecorrida = Vector3.Distance(startPosition, transform.position);
            Debug.Log("Distance traveled: " + distanciaRecorrida.ToString(data.distanceFormat));

            if (data.textoDistancia != null)
            {
                data.textoDistancia.text = "Distancia: " +
                    distanciaRecorrida.ToString(data.distanceFormat) + " m";
            }
        }
    }
}
