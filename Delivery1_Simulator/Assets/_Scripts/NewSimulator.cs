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
    public int userId;
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
    void OnEnable()
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

        Debug.Log(jsonData);

        StartCoroutine(SendDataToServer(jsonData));
    }

    // Corutina que envía los datos al servidor
    private IEnumerator SendDataToServer(string jsonData)
    {
        WWWForm form = new WWWForm();

        form.AddField("playerData", jsonData);

        using (UnityWebRequest www = UnityWebRequest.Post("https://citmalumnes.upc.es/~samuelm1/testHelloWorldData.php", form))
        {
            yield return www.SendWebRequest();

            // Comprobamos si la solicitud fue exitosa
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
}





