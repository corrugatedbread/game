using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CloneSprite : MonoBehaviour
{
    public GameObject tilemap;
    public Component script;
    public bool active = true;
    public int stage = 0;
    public Vector3Int location = new Vector3Int(0, 0, 0);
    Vector3 oldLocation = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        //doesn't work for whatever reason
        // Component script = tilemap.GetComponent<TileMap>();
        // if (tilemap != null && active)
        // {
        //     print("connect");
        //     tilemap.GetComponent<TileMap>().PlayerMoved += OnPlayerMoved;
        // }

        // if (active)
        // {
        //     print("active");
        //     Activate();
        // }
    }

    void Awake()
    {
        //idk why this is in Awake instead of Start
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        active = true;
        print("connect");
        tilemap.GetComponent<TileMap>().PlayerMoved += OnPlayerMoved;

        location = tilemap.GetComponent<TileMap>().stepHistory[stage][0];
        print(location);
        tilemap.GetComponent<TileMap>().clonesLocation.Add(location);
        transform.position = location + new Vector3(0.5f,0.5f,0);
        oldLocation = location;

        GetComponent<SpriteRenderer>().color = tilemap.GetComponent<TileMap>().colors[stage] * 0.7f;
    }

    void OnPlayerMoved()
    {
        if (!active)
        {
            return;
        }

        Component script = tilemap.GetComponent<TileMap>();
        print("clone move");
        print("stage: " + stage);
        if (tilemap.GetComponent<TileMap>().currentSteps >= tilemap.GetComponent<TileMap>().stepHistory[stage].Count)
        {
            if (tilemap.GetComponent<TileMap>().stepHistory[stage + 1] != null)
            {
                StopExisting();
            }
            return;
        }
        print("clone moveeee");
        //move
        location = tilemap.GetComponent<TileMap>().stepHistory[stage][tilemap.GetComponent<TileMap>().currentSteps];
        print(location);
        tilemap.GetComponent<TileMap>().clonesLocation.Add(location);
        StartCoroutine(Animate());
        // transform.position = location + new Vector3(0.5f,0.5f,0);
    }

    IEnumerator Animate()
    {
        for (float t = 0f; t < 1f; t += 0.05f)
        {
            transform.position = Vector3.Lerp(oldLocation, location, t) + new Vector3(0.5f,0.5f,0);
            yield return null;
        }
        oldLocation = location;
    }


    public void StopExisting() {
        tilemap.GetComponent<TileMap>().PlayerMoved -= OnPlayerMoved;
        tilemap.GetComponent<TileMap>().clones.Remove(gameObject);
        
        Destroy(gameObject);
    }


}
