using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
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

	public PlayerType myData;

	private void Start() {
		myData.id = 0;
	}

	public void SetName(string name) {
		myData.name = name;
	}
}
