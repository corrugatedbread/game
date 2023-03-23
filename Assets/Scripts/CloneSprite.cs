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
    // Start is called before the first frame update
    void Start()
    {
        //doesn't work for whatever reason
        Component script = tilemap.GetComponent<TileMap>();
        
    }

    void Awake()
    {
        if (tilemap != null && active)
        {
            print("connect");
            tilemap.GetComponent<TileMap>().PlayerMoved += OnPlayerMoved;
        }
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
            if (tilemap.GetComponent<TileMap>().stepHistory[1] != null)
            {
                tilemap.GetComponent<TileMap>().PlayerMoved -= OnPlayerMoved;
                Destroy(gameObject);
            }
            return;
        }
        print("clone moveeee");
        Vector3 location = tilemap.GetComponent<TileMap>().stepHistory[stage][tilemap.GetComponent<TileMap>().currentSteps];
        print(location);
        transform.position = location + new Vector3(0.5f,0.5f,0);
    }
}
