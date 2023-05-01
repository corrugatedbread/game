using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TileMap : MonoBehaviour
{
    //spaghetti
    public bool doStuff = false;
    public Vector3Int playerLocation = new Vector3Int(0,0,0);
    public Tilemap tilemap = null;
    // public Object cloneSprite = null;
    public GameObject cloneSprite;
    public GameObject JsonReader;

    public int totalSteps = 0;
    public int currentSteps = 0;
    public int currentStage = 0;

    public List<GameObject> clones = new List<GameObject> {};
    public List<Vector3Int> clonesLocation = new List<Vector3Int> {};

    public List<List<Vector3Int>> stepHistory = new List<List<Vector3Int>> {new List<Vector3Int> {}};
    // stepHistory[0] = List<Vector3Int> {playerLocation};

    public event Action GoalReached;
    
    public bool checkPlayer = false;
    public event Action PlayerMoved;
    public event Action Ready;
    public event Action Reset;

    // public TileBase goalRed;
    public TileBase firstSpawn;
    public TileBase wallTile;
    public Vector3Int firstSpawnLocation;

    public List<Vector3Int> goals = new List<Vector3Int> {};
    public List<Vector3Int> activeGoals = new List<Vector3Int> {};
    public List<Vector3Int> spawns = new List<Vector3Int> {};
    public List<Vector3Int> activeSpawns = new List<Vector3Int> {};
    //order matters
    public List<TileBase> goalTiles = new List<TileBase> {};
    public List<TileBase> spawnTiles = new List<TileBase> {};
    public List<Color> colors = new List<Color> { };

    void Start ()
    {
        tilemap = GetComponent<Tilemap>();
        // cloneSprite = GetComponent<Tilemap>();
        //connect actions
        tilemap.GetComponent<TileMap>().PlayerMoved += OnPlayerMoved;
        tilemap.GetComponent<TileMap>().GoalReached += OnGoalReached;

        SetMessage("");
        SetTiles(Persistent.levelIndex);
        LoadTiles();
        doStuff = true;
        // print("first spawn");
        Ready?.Invoke();
    }

    void Update()
    {
        if (doStuff == false)
        {
            return;
        }
        Vector3Int direction = new Vector3Int(0,0,0);
        if (Input.GetKeyDown("a") || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovePlayer(new Vector3Int(-1,0,0));
        } else if (Input.GetKeyDown("d") || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MovePlayer(new Vector3Int(1,0,0));
        } else if (Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MovePlayer(new Vector3Int(0,1,0));
        } else if (Input.GetKeyDown("s") || Input.GetKeyDown(KeyCode.DownArrow))
        {
            MovePlayer(new Vector3Int(0,-1,0));
        }
        // } else if (Input.GetKeyDown("r") || Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     CreateClone(currentStage);
        //     IncreaseStage();
        // }

        // MovePlayer(direction);
    }

    void LateUpdate()
    {
        if (doStuff == false)
        {
            return;
        }
        //runs after clones are all updated
        if (checkPlayer)
        {
            // print("checking");
            for (int i = 0; i < goals.Count; i++)
            {
                if (playerLocation == goals[i]) {
                    print("goal reached");
                    bool youActuallyReachedAnActiveGoalAndTheGameShouldntAnnoyYouByPrintingThatYouRanOutOfTime = DoSomethingWithTheGoal(goals[i]);
                    // GoalReached?.Invoke();
                    if (youActuallyReachedAnActiveGoalAndTheGameShouldntAnnoyYouByPrintingThatYouRanOutOfTime)
                    {
                        return;
                    }
                    break;
                }
            }
            if (clonesLocation.Contains(playerLocation))
            {
                print("you are very much very dead");
                SetMessage("You died!");
                doStuff = false;
                return;
            }
            clonesLocation.Clear();
            if (currentStage != 0 && clones.Count() == 0)
            {
                print("you ran out of time");
                SetMessage("You ran out of time!");
                doStuff = false;
                return;
            }
            checkPlayer = false;
        }
    }

    void SetTiles (int levelIndex)
    {
        doStuff = false;
        while (clones.Count() != 0)
        {
            clones[0].GetComponent<CloneSprite>().StopExisting();
        }
        clones.Clear();

        totalSteps = 0;
        currentSteps = 0;
        currentStage = 0;

        clones = new List<GameObject> {};
        clonesLocation = new List<Vector3Int> {};

        stepHistory = new List<List<Vector3Int>> {new List<Vector3Int> {}};
        playerLocation = new Vector3Int(0,0,0);
        
        List<TileBase> tiles = new List<TileBase> {};
        tilemap.ClearAllTiles();
        int width = JsonReader.GetComponent<JsonReader>().json.levels[levelIndex].width;
        int height = JsonReader.GetComponent<JsonReader>().json.levels[levelIndex].height;

        foreach (int i in JsonReader.GetComponent<JsonReader>().json.levels[levelIndex].tiles)
        {
            //there's probably a much better way of doing this but i'm lazy
            if (i == 0)
            {
                tiles.Add(null);
            } else if (i == 1)
            {
                tiles.Add(wallTile);
            } else if (i == 100)
            {
                tiles.Add(firstSpawn);
            } else if (i == 101)
            {
                tiles.Add(spawnTiles[0]);
            } else if (i == 102)
            {
                tiles.Add(spawnTiles[1]);
            } else if (i == 103)
            {
                tiles.Add(spawnTiles[2]);
            } else if (i == 104)
            {
                tiles.Add(spawnTiles[3]);
            } else if (i == 201)
            {
                tiles.Add(goalTiles[0]);
            } else if (i == 202)
            {
                tiles.Add(goalTiles[1]);
            } else if (i == 203)
            {
                tiles.Add(goalTiles[2]);
            } else if (i == 204)
            {
                tiles.Add(goalTiles[3]);
            } else
            {
                tiles.Add(null);
            }
            print(tiles[tiles.Count() - 1]);

            tilemap.SetTile(new Vector3Int((tiles.Count() - 1) % width, (int)((tiles.Count() - 1) / width), 0), tiles[tiles.Count() - 1]);
        }
        
        doStuff = true;
    }

    void LoadTiles ()
    {
        spawns.Clear();
        goals.Clear();
        // firstSpawnLocation;

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

                } else if (tile == firstSpawn)
                {
                    firstSpawnLocation = new Vector3Int(bounds.position.x + x, bounds.position.y + y, 0);
                    // spawnsUnsorted.Add(new Vector3Int(bounds.position.x + x, bounds.position.y + y, 0));
                    // spawnsTemp.Add(tile);
                    print("first spawn found");
                } else if (spawnTiles.Contains(tile))
                {
                    spawnsUnsorted.Add(new Vector3Int(bounds.position.x + x, bounds.position.y + y, 0));
                    spawnsTemp.Add(tile);
                    // spawns[spawnTiles.IndexOf(tile)] = new Vector3Int(bounds.position.x + x, bounds.position.y + y, 0);
                    print("spawn found");
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
        
        spawnsTemp.Add(firstSpawn);
        for (int i = 0; i < goalsUnsorted.Count(); i++)
        {
            // goalsTemp.Add(tileArray[goals[i].x + goals[i].y * bounds.size.x])
            // goalsSorted.Add(goalTiles.IndexOf(tileArray[goals[i].x + goals[i].y * bounds.size.x]));

            goals.Add(goalsUnsorted[goalsTemp.IndexOf(goalTiles[i])]);
        }
        for (int i = 0; i < spawnsUnsorted.Count(); i++)
        {
            spawns.Add(spawnsUnsorted[spawnsTemp.IndexOf(spawnTiles[i])]);

            print(spawns[i]);
            print("spawn ghdfjkaghkj");
            print(i);
        }
        playerLocation = firstSpawnLocation;
        stepHistory[currentStage].Add(playerLocation);
        Reset?.Invoke();
        // activeGoals.AddRange(goals);
        activeGoals = goals.ToList();
        // activeSpawns = spawns.ToList();
        // activeSpawns.Remove(firstSpawn);
        print(activeGoals.Count());
        // goals = goals.OrderBy(x => goals.FindIndex(x)).ToList();
        doStuff = true;
    }


    void MovePlayer (Vector3Int direction)
    {
        TileBase tile = tilemap.GetTile(playerLocation + direction);
        if (tile == null | tile == firstSpawn | goalTiles.Contains(tile) | spawnTiles.Contains(tile))
        // if (tilemap.GetTile(playerLocation + direction) == null || goalTiles.Contains(tilemap.GetTile(playerLocation + direction)))
        {
            currentSteps += 1;
            playerLocation = playerLocation + direction;
            // print(stepHistory[currentStage]);
            stepHistory[currentStage].Add(playerLocation);
            print(stepHistory[currentStage][currentSteps - 1]);

            PlayerMoved?.Invoke();
        }
    }

    void CreateClone (int cloneStage)
    {

        print("why");
        // clones[currentStage];
        GameObject cloneThing = Instantiate(cloneSprite);
        cloneThing.GetComponent<CloneSprite>().stage = cloneStage;
        cloneThing.GetComponent<CloneSprite>().Activate();
        clones.Add(cloneThing);
    }

    void IncreaseStage () {
        totalSteps += currentSteps;
        currentSteps = 0;

        currentStage += 1;
        stepHistory.Add(new List<Vector3Int> {});
        print(stepHistory[currentStage]);

        stepHistory[currentStage].Add(playerLocation);
        print("spawn location " + playerLocation + stepHistory[currentStage][0]);
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
        print("on goal reached");
    }

    bool DoSomethingWithTheGoal (Vector3Int goal)
    {
        print(activeGoals[0]);
        print(currentStage);
        print(stepHistory);
        checkPlayer = false;
        if (activeGoals.Contains(goal))
        {
            if (currentStage == goals.Count() - 1)
            {
                print("you win");
                SetMessage("You win!");
                doStuff = false;
                return true;
                
            } else if (goal != activeGoals[0])
            {
                return false;
            } else
            {
                //remove goal from steps
                stepHistory[currentStage].RemoveAt(stepHistory[currentStage].Count - 1);
                // currentSteps -= 1;
                Spawn();
                // currentStage += 1;
                GoalReached?.Invoke();
                while (clones.Count() != 0)
                {
                    clones[0].GetComponent<CloneSprite>().StopExisting();
                }
                clones.Clear();

                for (int i = 0; i <= currentStage; i++)
                {
                    CreateClone(i);
                }
                // CreateClone(currentStage);
                IncreaseStage();

                activeGoals.Remove(goal);
                checkPlayer = true;
                // activeSpawns.Remove(spawns[goals.IndexOf(goal)]);
                // PlayerMoved?.Invoke();
                return true;
            }
        }
        return false;
    }

    void Spawn ()
    {
        print("spawning playerLocation");
        // if (spawns[currentStage] != null)
        // {
        playerLocation = spawns[currentStage];
        // PlayerMoved?.Invoke();
        // }
    }

    public void NextLevel()
    {
        if (JsonReader.GetComponent<JsonReader>().json.levels.Length > Persistent.levelIndex)
        {
            SetMessage("");
            Persistent.levelIndex += 1;
            // doStuff = false;
            SetTiles(Persistent.levelIndex);
            LoadTiles();
            // doStuff = true;
        }
    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Retry()
    {
        SetMessage("");
        SetTiles(Persistent.levelIndex);
        LoadTiles();
    }

    public void SetMessage(string message)
    {
        GameObject.Find("Message").GetComponent<TextMeshProUGUI>().text = message;
    }
}

