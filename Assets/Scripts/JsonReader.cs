using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
    public class Levels
    {
        public Level[] levels;
    }

    public Levels json = new Levels();

    // Start is called before the first frame update
    void Start()
    {
        //why is this so annoying
        json = JsonUtility.FromJson<Levels>(raw.text);
        e = JsonUtility.FromJson<Levels>(raw.text);
        print(e);

        Levels q = JsonUtility.FromJson<Levels>(raw.text);
 
        foreach (Level w in q.levels)
        {
            Debug.Log("Found level: " + w.name + " " + w.width);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
