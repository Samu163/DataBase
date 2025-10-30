using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class SessionData
{
    public int player_id;
    public string start;
    public string end;

    public SessionData(int playerId, string start, string end = null)
    {
        this.player_id = playerId;
        this.start = start;
        this.end = end;
    }
}

[System.Serializable]
public class SessionResponse
{
    public bool success;
    public string message;
    public int session_id;
    public string error;
    public SessionResponseData data;
}

[System.Serializable]
public class SessionResponseData
{
    public int player_id;
    public int session_id;
    public string start;
    public string end;
}

public class SessionSimulator : MonoBehaviour
{
    // Diccionario para guardar info de sesiones activas
    private Dictionary<uint, SessionInfo> activeSessions = new Dictionary<uint, SessionInfo>();

    private class SessionInfo
    {
        public int playerId;
        public DateTime startTime;
    }

    void OnEnable()
    {
        Simulator.OnNewSession += OnNewSession;
        Simulator.OnEndSession += OnEndSession;
    }

    void OnDisable()
    {
        Simulator.OnNewSession -= OnNewSession;
        Simulator.OnEndSession -= OnEndSession;
    }

    private void OnNewSession(DateTime startTime, uint playerId)
    {
        // Crear sesión SIN campo end
        SessionData sessionData = new SessionData(
            (int)playerId,
            startTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
        );

        string jsonData = JsonUtility.ToJson(sessionData);
        Debug.Log($"<color=cyan> NEW SESSION - Player {playerId}:</color> {jsonData}");

        StartCoroutine(SendSessionToServer(jsonData, playerId, startTime));
    }

    private void OnEndSession(DateTime endTime, uint sessionId)
    {
        if (!activeSessions.ContainsKey(sessionId))
        {
            Debug.LogWarning($" Session {sessionId} not found!");
            return;
        }

        var sessionInfo = activeSessions[sessionId];
        TimeSpan duration = endTime - sessionInfo.startTime;

        Debug.Log($"<color=magenta> END SESSION {sessionId}:</color> Duration: {duration.TotalSeconds:F0}s");

        StartCoroutine(UpdateSessionEnd(endTime, (int)sessionId));
        activeSessions.Remove(sessionId);
    }

    private IEnumerator SendSessionToServer(string jsonData, uint playerId, DateTime startTime)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest www = new UnityWebRequest("https://citmalumnes.upc.es/~samuelm1/testHelloWorldData.php", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($" Error sending session: {www.error}");
                Debug.LogError($"Response: {www.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"<color=green> Session Response:</color> {www.downloadHandler.text}");

                try
                {
                    var response = JsonUtility.FromJson<SessionResponse>(www.downloadHandler.text);
                    if (response.success)
                    {
                        Debug.Log($"<color=green> Session created! ID: {response.session_id}</color>");

                        // Guardar la sesión activa
                        activeSessions[(uint)response.session_id] = new SessionInfo
                        {
                            playerId = (int)playerId,
                            startTime = startTime
                        };

                        // Disparar callback con el session_id
                        CallbackEvents.OnNewSessionCallback?.Invoke((uint)response.session_id);
                    }
                    else
                    {
                        Debug.LogError($" Session error: {response.error}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($" Error parsing session response: {e.Message}");
                    Debug.LogError($"Raw response: {www.downloadHandler.text}");
                }
            }
        }
    }

    private IEnumerator UpdateSessionEnd(DateTime endTime, int sessionId)
    {
        string endTimeStr = endTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        // Crear un JSON simple solo con el campo end
        string jsonData = $"{{\"end\":\"{endTimeStr}\"}}";

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        string url = $"https://citmalumnes.upc.es/~samuelm1/testHelloWorldData.php";

        using (UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($" Error updating session end: {www.error}");
                Debug.LogError($"Response: {www.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"<color=green> Session {sessionId} ended successfully!</color>");

                // Disparar callback de fin de sesión
                CallbackEvents.OnEndSessionCallback?.Invoke((uint)sessionId);
            }
        }
    }
}