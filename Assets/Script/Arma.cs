using UnityEngine;

public class Arma : MonoBehaviour
{
    [SerializeField]
    ArmaData data;

    void Start()
    {
        if (data == null)
        {
            Debug.LogError("ArmaData no asignado.");
            enabled = false;
            return;
        }

        if (data.proyectil == null)
        {
            Debug.LogError("Proyectil no asignado.");
            enabled = false;
            return;
        }

        if (data.spawnPoint == null)
        {
            Debug.LogError("Spawn Point no asignado.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject newProyectil = Instantiate(
                data.proyectil,
                data.spawnPoint.position,
                data.spawnPoint.rotation
            );

            Rigidbody rb = newProyectil.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Peso
                if (data.weightSlider != null)
                {
                    rb.mass = data.weightSlider.value;
                }

                // Fuerza
                rb.AddForce(
                    data.spawnPoint.up * data.force,
                    ForceMode.Impulse
                );
            }

            Destroy(newProyectil, 5f);
        }
    }

    public void ChangeForce()
    {
        data.force = data.forceSlider.value;

        if (data.forceText != null)
        {
            data.forceText.text =
                "Force: " + data.force.ToString("F1");
        }
    }

    public void ChangeAngle()
    {
        float angle = data.angleSlider.value;

        transform.rotation = Quaternion.Euler(
            angle,
            0,
            0
        );

        if (data.angleText != null)
        {
            data.angleText.text =
                "Angle: " + angle.ToString("F1") + "°";
        }
    }

    public void ChangeWeight()
    {
        float weight = data.weightSlider.value;

        if (data.weightText != null)
        {
            data.weightText.text =
                "Weight: " + weight.ToString("F1");
        }
    }
}