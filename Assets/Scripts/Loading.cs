using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Loading : MonoBehaviour
{
    public delegate void LoadingEvents(GameData gameData);
    public delegate void LoadingErrorEvents();
    public static event LoadingEvents OnComplete;
    public static event LoadingErrorEvents OnError;
    
    public RootGameData root;
    [SerializeField][TextArea] private string targetUrl;


    public void StartLoading()
    {
        StartCoroutine(GetGameData());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    IEnumerator GetGameData()
    {
        var request = UnityWebRequest.Get(targetUrl);
        request.SetRequestHeader("Accept", "application/json");
        var reqOperation = request.SendWebRequest();

        while (!reqOperation.isDone)
        {
            yield return null;
        }
        if (request.isNetworkError || request.isHttpError)
        {
           print("Error > " + request.error);
           OnError?.Invoke();
        }
        else
        {
            if (request.isDone)
            {
                var requestResult = request.downloadHandler.text;
               print("Load Complete > " + requestResult);
               root = JsonUtility.FromJson<RootGameData>(requestResult);
               OnComplete?.Invoke(root.gameData);
            }
        }
    }
}

[System.Serializable]
public class RootGameData
{
    public GameData gameData;
}

[System.Serializable]
public class GameData
{
    public GameValues goldPerTapScaling;
    public GameValues upgradeCostScaling;
    public int maxCircles;
    public int firstCircleCost;
}

[System.Serializable]
public class GameValues
{
    public float value1;
    public float value2;
}



