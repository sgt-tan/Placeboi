using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour {
	// Use this for initialization
	void Start () {
        Destroy(gameObject, 2);
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "ground" || col.gameObject.tag == "Player")
            Destroy(gameObject);
    }
}
