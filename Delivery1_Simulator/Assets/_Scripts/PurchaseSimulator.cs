using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PurchaseData
{
    public int session_id;
    public int item;
    public string dayTime;
    public float price;

    public PurchaseData(int sessionId, int itemId, string purchaseTime, float itemPrice)
    {
        this.session_id = sessionId;
        this.item = itemId;
        this.dayTime = purchaseTime;
        this.price = itemPrice;
    }
}

[System.Serializable]
public class PurchaseResponse
{
    public bool success;
    public string message;
    public int purchase_id;
    public string error;
}

public class PurchaseSimulator : MonoBehaviour
{
    private const string PURCHASE_URL = "https://citmalumnes.upc.es/~samuelm1/purchase.php";
    public SessionSimulator sessionSimulator;

    private Dictionary<int, float> itemPrices = new Dictionary<int, float>()
    {
        { 1, 0.99f }, 
        { 2, 1.99f },  
        { 3, 9.99f },   
        { 4, 49.99f }, 
        { 5, 99.99f }   
    };

    void OnEnable()
    {
        Simulator.OnBuyItem += HandlePurchase;
    }

    void OnDisable()
    {
        Simulator.OnBuyItem -= HandlePurchase;
    }
    private void HandlePurchase(int itemId, DateTime purchaseTime, uint sessionId)
    {
        int dbSessionId = sessionSimulator.GetDatabaseSessionId(sessionId);

        float price = itemPrices.ContainsKey(itemId) ? itemPrices[itemId] : 0;

        PurchaseData purchaseData = new PurchaseData(
            dbSessionId,
            itemId,
            purchaseTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            price
        );

        string jsonData = JsonUtility.ToJson(purchaseData);
        Debug.Log($"<color=yellow>NEW PURCHASE:</color> {jsonData}");

        StartCoroutine(SendPurchase(jsonData, sessionId));
    }

    private IEnumerator SendPurchase(string jsonData, uint sessionId)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(PURCHASE_URL, UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error sending purchase: {www.error}");
            }
            else
            {
                try
                {
                    var response = JsonUtility.FromJson<PurchaseResponse>(www.downloadHandler.text);

                    if (response.success)
                    {
                        Debug.Log($"<color=green>Purchase recorded! ID: {response.purchase_id}</color>");
                        CallbackEvents.OnItemBuyCallback?.Invoke(sessionId);
                    }
                    else
                    {
                        Debug.LogError($"Server error recording purchase: {response.error}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing purchase response: {e.Message}\nResponse: {www.downloadHandler.text}");
                }
            }
        }
    }
}