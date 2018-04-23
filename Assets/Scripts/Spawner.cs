using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	public GameObject enemy;
    public Transform spawnPoint;
    public bool spawned;
    void Start ()
	{

    }


    void OnTriggerEnter2D(Collider2D cam)
    {
        if (cam.tag == "MainCamera" && spawned == false)
        {
            Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            spawned = true;
        }
    }
}
