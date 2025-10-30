using System;
using System.Collections;
using System.Globalization;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

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

[System.Serializable]
public class ServerResponse
{
    public bool success;
    public string message;
    public int id;
    public string error;
    public ResponseData data;
}

[System.Serializable]
public class ResponseData
{
    public string name;
    public string country;
    public int age;
    public float gender;
    public string dayTime;
}

public class NewSimulator : MonoBehaviour
{
    void OnEnable()
    {
        ServicePointManager.ServerCertificateValidationCallback =
            (sender, certificate, chain, sslPolicyErrors) => true;

        Simulator.OnNewPlayer += EncodePlayerData;
    }

    void OnDisable()
    {
        Simulator.OnNewPlayer -= EncodePlayerData;
    }

    public void EncodePlayerData(string name, string country, int age, float gender, DateTime dayTime)
    {
        PlayerData playerData = new PlayerData(
            name.Replace("'", " "),
            country,
            age,
            gender,
            dayTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
        );

        string jsonData = JsonUtility.ToJson(playerData);
        Debug.Log($"<color=yellow> NEW PLAYER:</color> {jsonData}");

        StartCoroutine(SendJsonToServer(jsonData, "https://citmalumnes.upc.es/~samuelm1/testHelloWorldData.php"));
    }

    private IEnumerator SendJsonToServer(string jsonData, string url)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($" Error sending player: {www.error}");
            }
            else
            {
                try
                {
                    var response = JsonUtility.FromJson<ServerResponse>(www.downloadHandler.text);
                    if (response.success)
                    {
                        Debug.Log($"<color=green> Player saved! ID: {response.id} - {response.data.name}</color>");

                        // DISPARAR CALLBACK con el player_id para crear sesión
                        CallbackEvents.OnAddPlayerCallback?.Invoke((uint)response.id);
                    }
                    else
                    {
                        Debug.LogError($" Server error: {response.error}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($" Error parsing response: {e.Message}");
                }
            }
        }
    }
}