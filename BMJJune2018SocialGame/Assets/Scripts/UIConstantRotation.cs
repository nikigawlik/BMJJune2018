using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConstantRotation : MonoBehaviour {
	public float speed = 1f;
	// Update is called once per frame
	void Update () {
		RectTransform rt = GetComponent<RectTransform>();
		rt.Rotate(0, 0, Time.deltaTime * speed);
	}
}
