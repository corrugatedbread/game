using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public Vector3 location = new Vector3(0, 0, 0);
    public Vector3 oldLocation = new Vector3(0, 0, 0);
    public GameObject tilemap;
    public Coroutine idk;
    // Start is called before the first frame update
    void Start()
    {
        tilemap.GetComponent<TileMap>().PlayerMoved += OnPlayerMoved;
        tilemap.GetComponent<TileMap>().Ready += OnReady;
        tilemap.GetComponent<TileMap>().Reset += Reset;
        tilemap.GetComponent<TileMap>().GoalReached += OnGoalReached;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnPlayerMoved()
    {

        // transform.position = tilemap.GetComponent<TileMap>().playerLocation + new Vector3(0.5f,0.5f,0);
        location = tilemap.GetComponent<TileMap>().playerLocation;
        print("animating" + location + oldLocation);
        idk = StartCoroutine(Animate());
    }

    void OnReady()
    {
        Reset();
    }

    public void Reset()
    {
        StopAllCoroutines();
        oldLocation = tilemap.GetComponent<TileMap>().playerLocation;
        location = tilemap.GetComponent<TileMap>().playerLocation;
        transform.position = tilemap.GetComponent<TileMap>().playerLocation + new Vector3(0.5f,0.5f,0);
    }

    public IEnumerator Animate()
    {
        for (float t = 0f; t < 1f; t += 0.05f)
        {
            // print("animating");
            transform.position = Vector3.Lerp(oldLocation, location, t) + new Vector3(0.5f,0.5f,0);
            yield return null;
        }
        oldLocation = location;
    }

    void OnGoalReached() {
        //doesn't work because c# is great
        // StopCoroutine(idk);
        // print("stop coroutine" + idk);

        StopAllCoroutines();
        location = tilemap.GetComponent<TileMap>().playerLocation;
        transform.position = location + new Vector3(0.5f,0.5f,0);
        oldLocation = location;
        // StartCoroutine(Animate());
    }
}
