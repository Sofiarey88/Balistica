using System;

[Serializable]
public class ResultadoDisparo
{
    public float angulo;
    public float fuerza;
    public float masa;
    public bool acierto;
    public float distancia;
    public int objetosAfectados;
    public string fecha;
    public int puntaje; // NUEVO: puntos ganados en este disparo

    // Puntos por cada objeto con tag "Target" que toca el proyectil.
    // Cambiá este valor si querés otro puntaje por objetivo.
    const int PUNTOS_POR_TARGET = 50;

    public ResultadoDisparo(float angulo, float fuerza, float masa, bool acierto, float distancia, int objetosAfectados)
    {
        this.angulo = angulo;
        this.fuerza = fuerza;
        this.masa = masa;
        this.acierto = acierto;
        this.distancia = distancia;
        this.objetosAfectados = objetosAfectados;
        this.fecha = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss");
        this.puntaje = objetosAfectados * PUNTOS_POR_TARGET; // NUEVO: suma por cada target tocado
    }
}