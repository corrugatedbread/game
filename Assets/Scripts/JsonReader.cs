using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonReader : MonoBehaviour
{
    public TextAsset raw;

    // public class Level
    // {
    //     public string name;
    //     public int width;
    //     public int height;
    //     public int[] tiles;
    // }

    // [System.Serializable]
    // public class Levels
    // {
    //     public Level[] test;

    // }

    // public Levels idk = new Levels();
    public Levels e;

    [System.Serializable]
    public class Level
    {
        // public string key;
            public string name;
            public int width;
            public int height;
            public int[] tiles;
            public int highScore;
    }
    public class Levels
    {
        public List<Level> levels;
    }

    public Levels json = new Levels();
    public Levels scores = new Levels();
    public string scorePath;

    // Start is called before the first frame update
    void Start()
    {
        //why is this so annoying
        json = JsonUtility.FromJson<Levels>(raw.text);
        // e = JsonUtility.FromJson<Levels>(raw.text);
        // print(e);

        // Levels q = JsonUtility.FromJson<Levels>(raw.text);

        LoadScore();

        List<string> scoresLevels = new List<string> {};

        // scores.levels = new List<Level> { };

        if (scores.levels != null)
        {
            foreach (Level w in scores.levels)
            {
                scoresLevels.Add(w.name);
                print(w.name);
            }
        } else
        {
            scores.levels = new List<Level> {};
        }
            // print(scoresLevels[0]);
            // 

            foreach (Level w in json.levels)
        {
            Debug.Log("Found level: " + w.name + " " + w.width);
            if (!scoresLevels.Contains(w.name))
            {
                scores.levels.Add(new Level {name = w.name});
                Debug.Log("added");
            }

        }

        print(scores.levels[0].highScore);

    }

    public void LoadScore()
    {
        scorePath = Path.Combine(Application.persistentDataPath, "scores.json");
        if (!Directory.Exists(Path.GetDirectoryName(scorePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(scorePath));
        }
        // File.WriteAllText(scorePath, "this is a test");

        // StreamReader reader = new StreamReader(scorePath);

        string scoresRaw = File.ReadAllText(scorePath);

        scores = JsonUtility.FromJson<Levels>(scoresRaw);

        if (scores == null || scoresRaw == "")
        {
            scores = new Levels();
        }
        print(scoresRaw);
        // Levels scores = JsonUtility.FromJson<Levels>(reader.text);

    }

    public void SaveScore()
    {
        string data = JsonUtility.ToJson(scores);
        File.WriteAllText(scorePath, data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
