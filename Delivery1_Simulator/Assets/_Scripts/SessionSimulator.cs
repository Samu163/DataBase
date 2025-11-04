using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class SessionData
{
    public int player_id;
    public string start;

    public SessionData(int playerId, string startTime)
    {
        this.player_id = playerId;
        this.start = startTime;
    }
}

[System.Serializable]
public class SessionEndData
{
    public int session_id;
    public string end;

    public SessionEndData(int sessionId, string endTime)
    {
        this.session_id = sessionId;
        this.end = endTime;
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
    public string start;
    public int session_id;
    public string end;
}

public class SessionSimulator : MonoBehaviour
{
    private Dictionary<uint, int> sessionIdMap = new Dictionary<uint, int>();
    private uint localSessionCounter = 0;

    private Dictionary<uint, uint> sessionToPlayerMap = new Dictionary<uint, uint>();

    void OnEnable()
    {
        ServicePointManager.ServerCertificateValidationCallback =
            (sender, certificate, chain, sslPolicyErrors) => true;

        Simulator.OnNewSession += EncodeSessionStart;
        Simulator.OnEndSession += EncodeSessionEnd;
    }

    void OnDisable()
    {
        Simulator.OnNewSession -= EncodeSessionStart;
        Simulator.OnEndSession -= EncodeSessionEnd;
    }

    public int GetDatabaseSessionId(uint tempSessionId)
    {
        return sessionIdMap.ContainsKey(tempSessionId) ? sessionIdMap[tempSessionId] : -1;
    }

    public void EncodeSessionStart(DateTime startTime, uint playerId)
    {
        uint tempSessionId = ++localSessionCounter;
        sessionToPlayerMap[tempSessionId] = playerId;

        SessionData sessionData = new SessionData(
            (int)playerId,
            startTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
        );
        string jsonData = JsonUtility.ToJson(sessionData);
        Debug.Log($"<color=cyan>NEW SESSION:</color> {jsonData} (Temp ID: {tempSessionId}, Player ID: {playerId})");

        StartCoroutine(SendJsonToServer(jsonData,
            "https://citmalumnes.upc.es/~samuelm1/sessionStart.php",
            tempSessionId));
    }

    public void EncodeSessionEnd(DateTime endTime, uint sessionId)
    {
        if (!sessionIdMap.ContainsKey(sessionId))
        {
            Debug.LogWarning($"Session ID {sessionId} not found in mapping. Available keys: {string.Join(", ", sessionIdMap.Keys)}");
            return;
        }

        int dbSessionId = sessionIdMap[sessionId];

        SessionEndData endData = new SessionEndData(
            dbSessionId,
            endTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
        );

        string jsonData = JsonUtility.ToJson(endData);
        Debug.Log($"<color=magenta>END SESSION:</color> {jsonData} (Temp ID: {sessionId}, DB ID: {dbSessionId})");

        StartCoroutine(SendEndToServer(jsonData,
            "https://citmalumnes.upc.es/~samuelm1/sessionEnd.php",
            sessionId));
    }

    private IEnumerator SendJsonToServer(string jsonData, string url, uint tempSessionId)
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
                Debug.LogError($"Error sending session start: {www.error}");
                Debug.LogError($"Response: {www.downloadHandler.text}");
            }
            else
            {
                try
                {
                    var response = JsonUtility.FromJson<SessionResponse>(www.downloadHandler.text);

                    if (response.success)
                    {
                        Debug.Log($"<color=green>Session started! DB Session ID: {response.session_id}, Temp ID: {tempSessionId}</color>");
                        sessionIdMap[tempSessionId] = response.session_id;
                        CallbackEvents.OnNewSessionCallback?.Invoke(tempSessionId);
                    }
                    else
                    {
                        Debug.LogError($"Server error: {response.error}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing response: {e.Message}");
                    Debug.LogError($"Response text: {www.downloadHandler.text}");
                }
            }
        }
    }

    private IEnumerator SendEndToServer(string jsonData, string url, uint tempSessionId)
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
                Debug.LogError($"Error sending session end: {www.error}");
            }
            else
            {
                try
                {
                    var response = JsonUtility.FromJson<SessionResponse>(www.downloadHandler.text);

                    if (response.success)
                    {
                        Debug.Log($"<color=green>Session ended! ID: {response.session_id}</color>");

                        if (sessionToPlayerMap.ContainsKey(tempSessionId))
                        {
                            uint playerId = sessionToPlayerMap[tempSessionId];
                            CallbackEvents.OnEndSessionCallback?.Invoke(playerId);

                            sessionToPlayerMap.Remove(tempSessionId);
                        }
                        else
                        {
                            Debug.LogWarning($"No player mapping found for temp session {tempSessionId}");
                        }

                        sessionIdMap.Remove(tempSessionId);
                    }
                    else
                    {
                        Debug.LogError($"Server error: {response.error}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing response: {e.Message}");
                }
            }
        }
    }
}