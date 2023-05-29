using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;


public class Levels : MonoBehaviour
{
    public int width = Screen.width;
    public int height = Screen.height;
    public GameObject button;
    public GameObject canvas;
    public GameObject JsonReader;
    public int buttonWidth = 220;
    public int buttonHeight = 220;

    void Start()
    {
        // SceneManager.sceneLoaded += OnSceneLoaded;
        idk();
        
    }

    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)

    void idk()
    {
        width = Screen.width;
        height = Screen.height;
        int levelCount = JsonReader.GetComponent<JsonReader>().json.levels.Count;
        float margin = ((width % buttonWidth) + buttonWidth)/ 2;
        int row = (int)(width / buttonWidth);

        print(width);
        print(buttonWidth);
        print(row);
        Vector3 offset = new Vector3(margin - (width / 2), 0, 0);

        // var button2 = new Button { text = "Press Me" };
        // button.clicked += () =>
        // {  
        //     Debug.Log("Button was pressed!");
        // };

        for (int i = 0; i < levelCount; i++)
        {
            
            // new Vector3(70 * (i % row), 70 * Mathf.Floor(i / row), 0) + offset
            //
            GameObject b = Instantiate(button) as GameObject;
            UnityEngine.UI.Button cSharpIsDumb = b.GetComponent<Button>() as UnityEngine.UI.Button;
            // Button cSharpIsDumb = canvas.transform.GetChild(canvas.transform.childCount - 1);

            RectTransform transform = b.GetComponent<RectTransform>();
            print(b);
            b.transform.parent = canvas.transform;
            transform.anchoredPosition = new Vector2(buttonWidth * (i % row) + offset.x, (-buttonHeight) * Mathf.Floor(i / row) + offset.y + 200);
            // cSharpIsDumb.text = i;
            b.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = i.ToString();
            b.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text += "\n" + JsonReader.GetComponent<JsonReader>().json.levels[i].name;
            // cSharpIsDumb.onClick += OnClick;

            // cSharpIsDumb.clicked += () =>
            // {
            //     Debug.Log(i);
            // };

            print(new Vector3(buttonWidth * (i % row), buttonHeight * Mathf.Floor(i / row), 0) + offset);
        }
    }

    public void OnClick(GameObject e)
    {
        print(e.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text);
        // very scuffed
        Persistent.levelIndex = int.Parse(e.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text.Split("\n")[0]);
        SceneManager.LoadScene("Game");
        print("clicked");
    }

    public void LoadLevel()
    {

    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
