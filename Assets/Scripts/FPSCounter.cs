using UnityEngine;
using System.Collections;
using UnityEngine.UI;




public class FPSCounter : MonoBehaviour
{





    float deltaTime = 0.0f;
    private Text fpsText;

    public static FPSCounter instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        fpsText = GetComponent<Text>();
    }
    private void Start()
    {

    }

    private string text;
    float fps;
    float msec;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        text = string.Format("{0:0.} fps", fps);
        fpsText.text = text;





    }

    // void OnGUI()
    // {
    //     int w = Screen.width, h = Screen.height;

    //     GUIStyle style = new GUIStyle();

    //     Rect rect = new Rect(w - 60, (h - h * 2 / 100) - 10, 60, (h * 2 / 100) + 10);
    //     // Rect rect = new Rect(0, 0, w, h);

    //     style.alignment = TextAnchor.UpperRight;
    //     style.fontSize = h * 2 / 100;
    //     style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    //     style.normal.background = MakeTex(1, 2, new Color(0f, 0f, 0f, 1f));
    //     float msec = deltaTime * 1000.0f;
    //     float fps = 1.0f / deltaTime;
    //     //string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    //     string text = string.Format("{0:0.} fps", fps);

    //     GUI.Label(rect, text, style);
    // }

    // private Texture2D MakeTex(int width, int height, Color col)
    // {
    //     Color[] pix = new Color[width * height];
    //     for (int i = 0; i < pix.Length; ++i)
    //     {
    //         pix[i] = col;
    //     }
    //     Texture2D result = new Texture2D(width, height);
    //     result.SetPixels(pix);
    //     result.Apply();
    //     return result;
    // }
}