using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

public class TileMap : MonoBehaviour
{
    public Vector3Int player = new Vector3Int(0,0,0);
    public Tilemap tilemap = null;
    // public Object cloneSprite = null;
    public GameObject cloneSprite;

    public int totalSteps = 0;
    public int currentSteps = 0;
    public int currentStage = 0;

    public List<GameObject> clones = new List<GameObject> {};
    public List<List<Vector3Int>> stepHistory = new List<List<Vector3Int>> {new List<Vector3Int> {}};
    // stepHistory[0] = List<Vector3Int> {player};

    public event Action GoalReached;
    
    public bool checkPlayer = false;
    public event Action PlayerMoved;

    public TileBase goalRed;
    public TileBase firstSpawn;
    public Vector3Int firstSpawnLocation;

    public List<Vector3Int> goals = new List<Vector3Int> {};
    public List<Vector3Int> spawns = new List<Vector3Int> {};
    //order matters
    public List<TileBase> goalTiles = new List<TileBase> {};
    public List<TileBase> spawnTiles = new List<TileBase> {};

    void Start ()
    {
        // stepHistory[currentStage].Add(player);
        tilemap = GetComponent<Tilemap>();
        // cloneSprite = GetComponent<Tilemap>();
        //connect actions
        tilemap.GetComponent<TileMap>().PlayerMoved += OnPlayerMoved;
        tilemap.GetComponent<TileMap>().GoalReached += OnGoalReached;

        LoadTiles();
        
        if (firstSpawnLocation != null)
        {
            player = firstSpawnLocation;
            print("first spawn");
        }

    }

    void Update()
    {
        Vector3Int direction = new Vector3Int(0,0,0);
        if (Input.GetKeyDown("a"))
        {
            MovePlayer(new Vector3Int(-1,0,0));
        } else if (Input.GetKeyDown("d"))
        {
            MovePlayer(new Vector3Int(1,0,0));
        } else if (Input.GetKeyDown("w"))
        {
            MovePlayer(new Vector3Int(0,1,0));
        } else if (Input.GetKeyDown("s"))
        {
            MovePlayer(new Vector3Int(0,-1,0));
        } else if (Input.GetKeyDown("r"))
        {
            CreateClone(currentStage);
            IncreaseStage();
        }

        // MovePlayer(direction);
    }

    void LateUpdate()
    {
        //runs after clones are all updated
        if (checkPlayer)
        {
            print("checking");
            for (int i = 0; i < goals.Count; i++)
            {
                if (player == goals[i]) {
                    print("goal reached");
                    Spawn();
                    GoalReached?.Invoke();
                    break;
                }
            }
            checkPlayer = false;
        }
    }

    void LoadTiles ()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] tileArray = tilemap.GetTilesBlock(bounds);
        List<TileBase> goalsTemp = new List<TileBase> {};
        List<Vector3Int> goalsUnsorted = new List<Vector3Int> {};
        // List<Vector3Int> goalsSorted;
        List<TileBase> spawnsTemp = new List<TileBase> {};
        List<Vector3Int> spawnsUnsorted = new List<Vector3Int> {};

        //for debugging
        print(bounds);
        print("gsdfiuh");
        for (int index = 0; index < tileArray.Length; index++)
        {
            print(tileArray[index]);
        }

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = tileArray[x + y * bounds.size.x];
                if (goalTiles.Contains(tile))
                {
                    goalsUnsorted.Add(new Vector3Int(bounds.position.x + x, bounds.position.y + y, 0));
                    goalsTemp.Add(tile);
                    // goals[goalTiles.IndexOf(tile)] = new Vector3Int(bounds.position.x + x, bounds.position.y + y, 0);
                    print("goal found");
                    // print(goals[0]);

                } else if (spawnTiles.Contains(tile))
                {
                    spawnsUnsorted.Add(new Vector3Int(bounds.position.x + x, bounds.position.y + y, 0));
                    spawnsTemp.Add(tile);
                    // spawns[spawnTiles.IndexOf(tile)] = new Vector3Int(bounds.position.x + x, bounds.position.y + y, 0);
                    print("spawn found");
                } else if (tile == firstSpawn)
                {
                    firstSpawnLocation = new Vector3Int(bounds.position.x + x, bounds.position.y + y, 0);
                    print("first spawn found");
                } else if (tile != null)
                {
                    Debug.Log(x + "," + y + " " + tile.name);

                } else
                {
                    Debug.Log(x + "," + y + " null");
                }
            }
        }

        //sort lists
        

        for (int i = 0; i < goalsUnsorted.Count(); i++)
        {
            // goalsTemp.Add(tileArray[goals[i].x + goals[i].y * bounds.size.x])
            // goalsSorted.Add(goalTiles.IndexOf(tileArray[goals[i].x + goals[i].y * bounds.size.x]));

            goals.Add(goalsUnsorted[goalsTemp.IndexOf(goalTiles[i])]);
            spawns.Add(spawnsUnsorted[spawnsTemp.IndexOf(spawnTiles[i])]);

            print(goals[i]);
            print(i);
        }
        // goals = goals.OrderBy(x => goals.FindIndex(x)).ToList();
    }


    void MovePlayer (Vector3Int direction)
    {
        if (tilemap.GetTile(player + direction) == null || goalTiles.Contains(tilemap.GetTile(player + direction)))
        {
            currentSteps += 1;
            player = player + direction;
            // print(stepHistory[currentStage]);
            stepHistory[currentStage].Add(player);
            print(stepHistory[currentStage][currentSteps - 1]);

            PlayerMoved?.Invoke();
        }
    }

    void CreateClone (int cloneStage)
    {

        print("why");
        // clones[currentStage];
        clones.Add(Instantiate(cloneSprite));
        clones[currentStage].GetComponent<CloneSprite>().Activate();
        clones[currentStage].GetComponent<CloneSprite>().stage = cloneStage;
        
    }

    void IncreaseStage () {
        totalSteps += currentSteps;
        currentSteps = 0;

        currentStage += 1;
        stepHistory.Add(new List<Vector3Int> {});
        print(stepHistory[1]);

        stepHistory[currentStage].Add(player);
    }

    void MoveClone ()
    {

    }

    void OnPlayerMoved ()
    {
        print("tilemapppp");
        checkPlayer = true;
    }

    void OnGoalReached ()
    {
        CreateClone(currentStage);
        IncreaseStage();
    }

    void Spawn ()
    {
        if (goalTiles[currentStage] != null)
        {}
    }
}

