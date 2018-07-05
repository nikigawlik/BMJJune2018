using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestHandler : MonoBehaviour {
	public string serverAddress;

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

}
