using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ButtonLogin : MonoBehaviour {
    public static string serverAddress = "192.168.3.186";

    public Button button1;
    public Button button2;
    public Button button3;

    public static string Url(string path) {
        return "http://" + serverAddress + ":3000/" + path;
    }

    public void Start() {
        if (button1 != null)
            button1.onClick.AddListener(ButtonLoginFunc);
        if (button2 != null)
            button2.onClick.AddListener(ButtonGetPlayersFunc);
        if (button3 != null)
            button3.onClick.AddListener(ButtonLogout);
    }

    public void ButtonLoginFunc() {
        Debug.Log("You have clicked the button!");
    }

    public void ButtonGetPlayersFunc() {
        Debug.Log("Clicked the 'Get Players' button!");
        StartCoroutine("GetPlayers");
    }

    [Serializable]
    public class PlayerType {
        public int id;
        public string name;
        public int score;
        public string photo;
        public string timestamp;
    };

    [Serializable]
    public class PlayerTypeArray {
        public PlayerType[] players;
    }

    public IEnumerator GetPlayers() {
        var url = Url("players");
        using (var request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError) {
                Debug.Log("Error: " + request.error);
                yield break;
            }

            byte[] results = request.downloadHandler.data;
            var str = Encoding.Default.GetString(results);

            Debug.Log("Results: " + str);
            var players = JsonUtility.FromJson<PlayerTypeArray>(str);

            /* TODO: Do something with the player list! */
        }
    }

    public void ButtonLogout() {
        Debug.Log("Logout!");
    }
}