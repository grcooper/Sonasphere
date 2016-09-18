using UnityEngine;
using System.Collections;

public class DayChange : MonoBehaviour {

    public Material NightSkybox;
    public Material DaySkybox;

    public enum FadingState { OFF, OUT, IN };
    private FadingState fadingState = FadingState.OFF;

    void Start ()
    {
        //SetDay();
    }

    public void SetDay() {
        RenderSettings.skybox = DaySkybox;
        GameObject.Find("Stars").SetActive(false);
        GameObject.Find("Firefly").SetActive(false);
        GameObject.Find("Spotlight").SetActive(false);
        GameObject.Find("SunLight").SetActive(true);
    }

    public void SetNight()
    {
        RenderSettings.skybox = NightSkybox;
        GameObject.Find("Stars").SetActive(true);
        GameObject.Find("Firefly").SetActive(true);
        GameObject.Find("Spotlight").SetActive(true);
        GameObject.Find("SunLight").SetActive(false);
    }


    public Texture2D fadeTexture;
    private float fadeSpeed = 1f;
    private int drawDepth = -1000;

    private float alpha = 1.0f;
    private float fadeDir = -1f;

    void OnGUI()
    {
        if (fadingState == FadingState.OUT)
        {
            alpha -= fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            Color thisAlpha = GUI.color;
            thisAlpha.a = alpha;
            GUI.color = thisAlpha;

            GUI.depth = drawDepth;

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        }
        else if(fadingState == FadingState.IN)
        {
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            Color thisAlpha = GUI.color;
            thisAlpha.a = alpha;
            GUI.color = thisAlpha;

            GUI.depth = drawDepth;

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        }


    }

}
