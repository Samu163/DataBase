using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class RevenueData
{
    public int Item;
    public string dayTime;
    public int session_id;
    public float price;

    public RevenueData(int item, string dayTime, int sessionId, float price)
    {
        this.Item = item;
        this.dayTime = dayTime;
        this.session_id = sessionId;
        this.price = price;
    }
}

[System.Serializable]
public class RevenueResponse
{
    public bool success;
    public string message;
    public int purchase_id;
    public string error;
    public RevenueResponseData data;
}

[System.Serializable]
public class RevenueResponseData
{
    public int Item;
    public string dayTime;
    public int session_id;
    public float price;
    public int purchase_id;
}

public class RevenueSimulator : MonoBehaviour
{
    // Precios de los items
    private float[] itemPrices = new float[] { 0f, 0.99f, 1.99f, 4.99f, 9.99f, 19.99f };

    // Nombres de items para logging
    private string[] itemNames = new string[] { "None", "Bronze Pack", "Silver Pack", "Gold Pack", "Platinum Pack", "Diamond Pack" };

    void OnEnable()
    {
        Simulator.OnBuyItem += OnItemBought;
    }

    void OnDisable()
    {
        Simulator.OnBuyItem -= OnItemBought;
    }

    private void OnItemBought(int itemId, DateTime purchaseTime, uint sessionId)
    {
        float price = GetItemPrice(itemId);
        string itemName = GetItemName(itemId);

        RevenueData revenueData = new RevenueData(
            itemId,
            purchaseTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            (int)sessionId,
            price
        );

        string jsonData = JsonUtility.ToJson(revenueData);
        Debug.Log($"<color=yellow> PURCHASE - Session {sessionId}:</color> {itemName} (${price})");

        StartCoroutine(SendRevenueToServer(jsonData));
    }

    private float GetItemPrice(int itemId)
    {
        if (itemId >= 0 && itemId < itemPrices.Length)
            return itemPrices[itemId];
        return 0f;
    }

    private string GetItemName(int itemId)
    {
        if (itemId >= 0 && itemId < itemNames.Length)
            return itemNames[itemId];
        return "Unknown Item";
    }

    private IEnumerator SendRevenueToServer(string jsonData)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest www = new UnityWebRequest("https://citmalumnes.upc.es/~samuelm1/purchases.php", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($" Error sending revenue: {www.error}");
            }
            else
            {
                try
                {
                    var response = JsonUtility.FromJson<RevenueResponse>(www.downloadHandler.text);
                    if (response.success)
                    {
                        string itemName = GetItemName(response.data.Item);
                        Debug.Log($"<color=green> Purchase saved! ID: {response.purchase_id}</color>");
                        Debug.Log($"    {itemName} - ${response.data.price}");

                        // DISPARAR CALLBACK de compra completada (termina la sesión)
                        CallbackEvents.OnItemBuyCallback?.Invoke((uint)response.data.session_id);
                    }
                    else
                    {
                        Debug.LogError($" Revenue error: {response.error}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($" Error parsing revenue response: {e.Message}");
                }
            }
        }
    }
}