using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TargetSpawner : MonoBehaviour {
    public Transform prefab;

    // Use this for initialization
    void Start () {
        Transform test1 = Instantiate(prefab, new Vector3(2.0F, 0, 0), Quaternion.identity);
        ImageTargetBehaviour test2 = test1.GetComponent<ImageTargetBehaviour>();
 
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}


