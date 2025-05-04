using UnityEngine;
using System.IO;
using System;

public class SaveTime : MonoBehaviour
{
    string filePath;

    [Serializable]
    public class LevelData
    {
        public string number;
        public float highTime;
    }

    public LevelData levelEx;

    void Start()
    {
        filePath = Application.persistentDataPath + "/savefile.txt";

        // Si ya existe un archivo, lo cargamos
        if (File.Exists(filePath))
        {
            string savedJson = File.ReadAllText(filePath);
            levelEx = JsonUtility.FromJson<LevelData>(savedJson);
            Debug.Log("Datos cargados: Nivel " + levelEx.number + ", Mejor tiempo: " + levelEx.highTime);
        }
        else
        {
            // Si no hay archivo, inicializamos datos
            levelEx = new LevelData
            {
                number = "1",         // Puedes cambiarlo segun el nivel
                highTime = float.MaxValue // Para que cualquier tiempo sea mejor al principio
            };
        }
    }

    public float GetHightTime()
    {
        return levelEx.highTime;
    }

    public void TrySetNewTime(float newTime)
    {
        if (newTime < levelEx.highTime)
        {
            levelEx.highTime = newTime;
            SaveToFile();
            Debug.Log("Nuevo mejor tiempo: " + newTime);
        }
        else
        {
            Debug.Log("Tiempo no mejorado. Tiempo actual: " + newTime + ", Mejor tiempo: " + levelEx.highTime);
        }
    }

    void SaveToFile()
    {
        string json = JsonUtility.ToJson(levelEx);
        File.WriteAllText(filePath, json);
    }

    // Para probar desde teclado
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            float tiempoRandom = UnityEngine.Random.Range(10f, 60f);
            Debug.Log("Tiempo aleatorio generado: " + tiempoRandom);
            TrySetNewTime(tiempoRandom);
            Debug.Log("File Path: " + filePath);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("Mejor tiempo registrado: " + GetHightTime());
        }
    }
}
