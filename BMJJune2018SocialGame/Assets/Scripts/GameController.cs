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
	public Text ingameInfoText;

	
    [Serializable]
    public class LoginResponse {
        public string message;
        public Player.PlayerType player;
    }

	private List<TargetRecognizer> possibleTargets;

	void Awake () {
		possibleTargets = new List<TargetRecognizer>();
	}

	private void Start() {
		connectButton.onClick.AddListener(TryConnectToServer);
		loginButton.onClick.AddListener(TryLogin);
		hitPersonButton.onClick.AddListener(HitPerson);
	}

	void TryConnectToServer() {
		WebRequestHandler wrh = GetComponent<WebRequestHandler>();
		wrh.serverAddress = serverAddressField.text;

		// show loading circle
		loadingScreen.SetActive(true);
		StartCoroutine(wrh.Get("", (string message) => {
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
		WebRequestHandler wrh = GetComponent<WebRequestHandler>();
		Player p = GetComponent<Player>();
		
		p.SetName(playerNameField.text);

		string requestJSONData = JsonUtility.ToJson(p.myData);

		loadingScreen.SetActive(true);
		StartCoroutine(wrh.Post("login", requestJSONData, (string jsonData) => {
			// success
			loadingScreen.SetActive(false);
			
			LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(jsonData);
			p.myData = loginResponse.player;

			loginScreen.SetActive(false);
			attackScreen.SetActive(true);

			ingameInfoText.text = "Your Name: " + p.myData.name + " Your ID: " + p.myData.id;
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
