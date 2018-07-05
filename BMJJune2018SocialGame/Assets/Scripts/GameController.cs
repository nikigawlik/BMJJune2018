using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System;

public class GameController : MonoBehaviour {
	[Header("Screen references")]
	public GameObject splashScreen;
	public GameObject loginScreen;
	public GameObject leaderboard;
	public GameObject scrollable;
	public GameObject attackScreen;
	public GameObject loadingScreen;

	[Header("Connect Screen UI elements")]
	public InputField serverAddressField;
	public Button connectButton;

	[Header("Login Screen UI elements")]
	public InputField playerNameField;
	public Button loginButton;

	[Header("Hit Screen UI elements")]
	public Button hitPersonButton;
	public RawImage image;
	public GameObject frame;

	
    [Serializable]
    public class LoginResponse {
        public string message;
        public Player.PlayerType player;
    }

	private List<TargetRecognizer> possibleTargets;

	private string serverAddress;

	void Awake () {
		possibleTargets = new List<TargetRecognizer>();
	}

	private void Start() {
		connectButton.onClick.AddListener(TryConnectToServer);
		loginButton.onClick.AddListener(TryLogin);
		hitPersonButton.onClick.AddListener(HitPerson);
	}

    public string Url(string path) {
        return "http://" + serverAddress + ":3000/" + path;
    }

	public delegate void Callback(string message);

	public IEnumerator Get(string path, Callback onSuccess, Callback onFail) {
		string url = Url(path);
		UnityWebRequest request = null;

		try {
			request = UnityWebRequest.Get(url);
		} 
		catch(System.UriFormatException e) {
			if(onFail != null) onFail(e.Message);
			yield break;
		}

		using (request) {
			yield return request.SendWebRequest();
			if (request.isNetworkError || request.isHttpError) {
				if(onFail != null) onFail(request.error);
				yield break;
			}

			byte[] results = request.downloadHandler.data;
			string str = Encoding.Default.GetString(results);
			if(onSuccess != null) onSuccess(str);
		}
    }

	public IEnumerator Post(string path, string data, Callback onSuccess, Callback onFail) {
		string url = Url(path);
		UnityWebRequest request = null;

		try {
			// create post request semi-manually, bec. Unity's implementation is shit
			request = new UnityWebRequest(url, "POST");
			byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
			request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
		} 
		catch(System.UriFormatException e) {
			if(onFail != null) onFail(e.Message);
			yield break;
		}

		using (request) {
			yield return request.SendWebRequest();
			if (request.isNetworkError || request.isHttpError) {
				if(onFail != null) onFail(request.error);
				yield break;
			}

			byte[] results = request.downloadHandler.data;
			string str = Encoding.Default.GetString(results);
			if(onSuccess != null) onSuccess(str);
		}
    }

	void TryConnectToServer() {
		serverAddress = serverAddressField.text;
		// show loading circle
		loadingScreen.SetActive(true);
		StartCoroutine(Get("", (string message) => {
			// success
			loadingScreen.SetActive(false);
			
			splashScreen.SetActive(false);
			loginScreen.SetActive(true);
		}, (string message) => {
			// fail
			// TODO show some warning
			Debug.Log("Error: " + message);
			loadingScreen.SetActive(false);
		}));
	}

	void TryLogin() {
		Player p = GetComponent<Player>();
		p.SetName(playerNameField.text);

		string requestJSONData = JsonUtility.ToJson(p.myData);

		loadingScreen.SetActive(true);
		StartCoroutine(Post("login", requestJSONData, (string jsonData) => {
			// success
			loadingScreen.SetActive(false);
			
			LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(jsonData);
			p.myData = loginResponse.player;

			loginScreen.SetActive(false);
			attackScreen.SetActive(true);
		}, (string message) => {
			// fail
			Debug.Log("Error: " + message);
			loadingScreen.SetActive(false);
		}));
	}

	public void AddTarget(TargetRecognizer t) {
		possibleTargets.Add(t);
	}

	public List<TargetRecognizer> getTargets() {
		return possibleTargets;
	}

	void HitPerson() {
		// get available markers
		List<TargetRecognizer> possibleTargets = getTargets();

		foreach (TargetRecognizer t in possibleTargets)
		{
			if(t.isTracking) {
				// do stuff
				Debug.Log("HIT PERSON: " + t.targetName);
				frame.SetActive(false);
				StartCoroutine("TakeSnapshot");
			}
		}
	}

	WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

	public IEnumerator TakeSnapshot()
	{
		yield return frameEnd;

		float cutoff = 0.125f * Screen.height;
		int h = (int) (Screen.height - cutoff * 2);

		Texture2D tex = new Texture2D(Screen.width, h);

		tex.ReadPixels(new Rect(0, cutoff, Screen.width, h), 0, 0);
		tex.Apply();

		byte[] data = tex.EncodeToPNG();
		Debug.Log(data);

		image.texture = tex;

		frame.SetActive(true);

		// continue
		attackScreen.SetActive(false);
		scrollable.SetActive(true);
	}
}
