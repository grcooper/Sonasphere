using UnityEngine;
using System.Collections;

public class DayChange : MonoBehaviour {

    public Material NightSkybox;
    public Material DaySkybox;
	
    void Start()
    {
        SetDay();
    }

    void SetDay() {
        RenderSettings.skybox = DaySkybox;
        GameObject.Find("Stars").SetActive(false);
        GameObject.Find("Firefly").SetActive(false);
        GameObject.Find("Moon").GetComponent<Shader>()
        GameObject.Find("Spotlight").SetActive(false);
        GameObject.Find("SunLight").SetActive(true);
    }

}
