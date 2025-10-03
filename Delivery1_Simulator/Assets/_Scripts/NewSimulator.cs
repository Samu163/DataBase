using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Clase para almacenar los datos del jugador
[System.Serializable]
public class PlayerData
{
    public string name;
    public string country;
    public int age;
    public float gender;
    public string dayTime;

    public PlayerData(string name, string country, int age, float gender, string dayTime)
    {
        this.name = name;
        this.country = country;
        this.age = age;
        this.gender = gender;
        this.dayTime = dayTime;
    }
}

public class NewSimulator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Simulator.OnNewPlayer += StoreData;       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Método que maneja el envío de datos
    public void StoreData(string name, string country, int age, float gender, DateTime dayTime)
    {
        PlayerData playerData = new PlayerData(name, country, age, gender, dayTime.ToString("yyyy-MM-dd HH:mm:ss"));

        string jsonData = JsonUtility.ToJson(playerData);

        StartCoroutine(SendDataToServer(jsonData));
    }

    // Corutina que envía los datos al servidor
    private IEnumerator SendDataToServer(string jsonData)
    {
        //// URL del servidor (reemplaza con tu endpoint)
        string url = "https://citmalumnes.upc.es/~samuelm1/testHelloWorldData.php";

        // Crear un UnityWebRequest con la URL y el método POST
        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);

        // Convertir los datos a bytes
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        // Configurar el contenido del request
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Establecer los encabezados para indicar que estamos enviando JSON
        request.SetRequestHeader("Content-Type", "application/json");

        // Enviar la solicitud y esperar la respuesta
        yield return request.SendWebRequest();

        // Verificar si hubo algún error
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Datos enviados exitosamente"+ request.downloadHandler.text);

        }
        else
        {
            Debug.LogError("Error al enviar datos: " + request.error);
        }
    }
}




