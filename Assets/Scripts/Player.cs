using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public Vector3 location = new Vector3(0, 0, 0);
    public Vector3 oldLocation = new Vector3(0, 0, 0);
    public Color color = new Vector4(0, 0, 0, 1);
    public Color oldColor = new Vector4(0, 0, 0, 1);
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
        location = tilemap.GetComponent<TileMap>().playerLocation;
        oldLocation = location;
        transform.position = location + new Vector3(0.5f,0.5f,0);
        color = tilemap.GetComponent<TileMap>().colors[tilemap.GetComponent<TileMap>().currentStage] * 0.98f;
        oldColor = color;
        GetComponent<SpriteRenderer>().color = color;
    }

    public void ChangeColor(Color newColor)
    {
        color = newColor;
        StartCoroutine(AnimateColor());
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

    public IEnumerator AnimateColor()
    {
        for (float t = 0f; t < 1f; t += 0.03f)
        {
            // print("animating");
            GetComponent<SpriteRenderer>().color = Color.Lerp(oldColor, color, t);
            yield return null;
        }
        oldColor = color;
    }

    void OnGoalReached() {
        //doesn't work because c# is great
        // StopCoroutine(idk);
        // print("stop coroutine" + idk);

        StopAllCoroutines();
        location = tilemap.GetComponent<TileMap>().playerLocation;
        transform.position = location + new Vector3(0.5f,0.5f,0);
        oldLocation = location;
        ChangeColor(tilemap.GetComponent<TileMap>().colors[tilemap.GetComponent<TileMap>().currentStage + 1] * 0.98f);
        // StartCoroutine(Animate());
    }
}
