using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Visualizer : MonoBehaviour {

    /*public float lightScale = 100;
    Light BassKickLight;
    Light MelodyLowLight;
    Light MelodyMidLight;
    Light SnareLight;*/


    public float lifetimeBase = 1;
	public float lifetimeScale = 8;

	public float speedBase = 1;
	public float speedScale = 5;

	public float sizeBase = 1;
	public float sizeScale = 4;

    // Holds the spectrum data, has to be a power of 2, might go down to 1024 if lag
    const int spectrumSize = 2048;
	private float[] spectrum = new float[spectrumSize];

	// 20-50hz = 6.8 - 17 - Sub bass
	SpectrumRange subBass = new SpectrumRange(6, 19);

	// 60-250hz = 20.5 - 85.3 - Bass
	SpectrumRange bass = new SpectrumRange(20, 85);

	// 250-500hz = 85.3 - 170.6 - Low Midrange
	SpectrumRange lowMidrange = new SpectrumRange(86, 170);

	//500-2k = 170.6 - 682.6 - Midrange
	SpectrumRange midrange = new SpectrumRange(171, 682);

	// 2k-4k = 682.6 - 1365.3 - Upper Midrange
	SpectrumRange upperMidrange = new SpectrumRange(683, 1365);

	// 4k-6k = 1365.3 - 2048 - Presence
	SpectrumRange presence = new SpectrumRange(1366, 2047);

    // For use in the moving average algorithm to smooth our data
    const int movingAverageSize = 20;
    private float[] movingAverageBuffer = new float[movingAverageSize];
    public float minValue = 0.001f;

    public int debugLineScale = 400;

    public GameObject ps;

	// Use this for initialization
	void Start () {
        /* BassKickLight = GameObject.Find("Bass Kick").GetComponent<Light>();
         MelodyLowLight = GameObject.Find("Melody Low").GetComponent<Light>();
         MelodyMidLight = GameObject.Find("Melody Mid").GetComponent<Light>();
         SnareLight = GameObject.Find("Snare").GetComponent<Light>();*/

        ps.GetComponent<ParticleSystem>().Stop();
	}
	
	// Update is called once per frame
	void Update() {
		
		AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);

        /*for (int i = 0; i < spectrumSize; i = i + 100)
		{
			Debug.Log ("I: " + i + " : " + spectrum [i]);
		}*/

        // Precalc the first moving average points

        int lastAddedAverageIndex = 0;
        float currentAverage = 0;

        for (int i = 0; i < movingAverageSize && i < spectrumSize; ++i)
        {
            movingAverageBuffer[i] = spectrum[i];
            currentAverage += spectrum[i];
        }
        currentAverage /= movingAverageSize;

        for (int i = 0; i < spectrumSize; i++)
        {

            //Average out the value using moving average
            if(i > movingAverageSize / 2)
            {
                if (i + (movingAverageSize / 2) < spectrumSize)
                {
                    movingAverageBuffer[lastAddedAverageIndex % movingAverageSize] = spectrum[i + (movingAverageSize / 2)];
                }
                else
                {
                    movingAverageBuffer[lastAddedAverageIndex % movingAverageSize] = 0;
                }
                ++lastAddedAverageIndex;
                currentAverage = 0;
                for(int q = 0; q < movingAverageSize; ++q)
                {
                    currentAverage += movingAverageBuffer[q];
                }
                currentAverage /= Mathf.Min(movingAverageSize, (spectrumSize - i) + (movingAverageSize / 2));
            }

            // if (currentAverage * 0.9f + spectrum[i] * 0.1f < minValue)
            // {
            //     spectrum[i] = 0;
            // }
            // else
            // {
                spectrum[i] = currentAverage * 0.9f + spectrum[i] * 0.1f;
            // }

            if (subBass.inRange(i))
            {
                subBass.addToSum(spectrum [i]);
            }
            else if (bass.inRange(i))
            {
                bass.addToSum(spectrum [i]);
            }
            else if (lowMidrange.inRange(i))
            {
                lowMidrange.addToSum(spectrum [i]);
            }
            else if (midrange.inRange(i))
            {
                midrange.addToSum(spectrum [i]);
            }
            else if (upperMidrange.inRange(i))
            {
                upperMidrange.addToSum(spectrum [i]);
            }
            else if(presence.inRange(i))
            {
                presence.addToSum(spectrum [i]);
            } else {
                // Debug.Log(i);
            }
            // Log all of the items
            /*Debug.Log("SubBass: " + subBass);
            Debug.Log("Bass: " + bass);
            Debug.Log("LowMidrange: " + lowMidrange);
            Debug.Log("Midrange: " + midrange);
            Debug.Log("upperMidrange: " + upperMidrange);*/
        }

// 		for (int i = 1; i < (spectrumSize / 1) - 1; i++)
//         {
// 			// 20-50hz = 6.8 - 17 - Sub bass
// 			if (subBass.inRange(i)) {
// 				Debug.DrawLine (new Vector3 (i - 1, spectrum [i] * debugLineScale, 0), new Vector3 (i, spectrum [i + 1] * debugLineScale, 0), Color.red);
// 			}
// 			// 60-250hz = 20.5 - 85.3 - Bass
// 			else if (bass.inRange(i)) {
// 				Debug.DrawLine (new Vector3 (i - 1, spectrum [i] * debugLineScale, 0), new Vector3 (i, spectrum [i + 1] * debugLineScale, 0), Color.green);
// 			}
// 			// 250-500hz = 85.3 - 170.6 - Low Midrange
// 			else if (lowMidrange.inRange(i)) {
// 				Debug.DrawLine (new Vector3 (i - 1, spectrum [i] * debugLineScale, 0), new Vector3 (i, spectrum [i + 1] * debugLineScale, 0), Color.blue);
// 			}
// 			//500-2k = 170.6 - 682.6 - Midrange
// 			else if (midrange.inRange(i)) {
// 				Debug.DrawLine (new Vector3 (i - 1, spectrum [i] * debugLineScale, 0), new Vector3 (i, spectrum [i + 1] * debugLineScale, 0), Color.cyan);
// 			} else {
// //				Debug.DrawLine (new Vector3 (i - 1, spectrum [i] * debugLineScale, 0), new Vector3 (i, spectrum [i + 1] * debugLineScale, 0), Color.cyan);
// 			}
//             //Debug.DrawLine(new Vector3(i - 1, Mathf.Abs(Mathf.Log(spectrum[i * 4]) * 50), 2), new Vector3(i, Mathf.Abs(Mathf.Log(spectrum[(i + 1) * 4]) * 50), 2), Color.cyan);
//             //Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
//             // Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
//         }

        // Debug.Log("PEAKS");
        // for(int i = 0; i < peakArray.Count; ++i)
        // {
        //     Debug.Log("Peak: " + i);
        //     Debug.Log(peakArray[i].getAverage());
        // }
        float bassKickValue = subBass.max > 0.09f ? subBass.max : 0;
        float snareValue = 9 * (lowMidrange.avg() * 0.16f + midrange.avg() * 0.17f + upperMidrange.max * 0.38f + presence.max * 0.29f);
        if (bassKickValue > 0.1f) {
            snareValue *= 0.1f;
        } else if (snareValue <= 0.05f) {
            snareValue = 0;
        }

        /*BassKickLight.intensity = bassKickValue * lightScale;
		MelodyLowLight.intensity = bass.max * 3 * lightScale;
		MelodyMidLight.intensity = lowMidrange.max * 3 * lightScale;
		SnareLight.intensity = snareValue * lightScale;*/
        // Debug.Log("subBass" + subBass.sum);
        // Debug.Log("bass" + bass.sum);
        // Debug.Log("lowMidrange" + lowMidrange.sum);
        // Debug.Log("midrange" + midrange.sum);
        // Debug.Log("uper" + upperMidrange.sum);
        // Debug.Log("pres" + presence.sum);

        Visualize(bassKickValue, bass.max * 3, lowMidrange.max * 3, snareValue);

        subBass.reset();
        bass.reset();
        lowMidrange.reset();
        midrange.reset();
        upperMidrange.reset();
        presence.reset();
	}

    public Vector3 sunStartScale = new Vector3(100, 100, 100);
    public int sunExtraScale = 2;
    //public float sunStartScale = 100f;

    public float waterStartScale = 5f;

    public float fireFlyStartScale = 0.5f;

    public Vector3 fireWorksStartPos = new Vector3(0, 300, 400);

    public int msofLastFireWork = 0;
    public int secofLastFireWork = 0;
    public int fireWorkTimeBuffer = 300;

    public void Visualize(float bassKick, float melodyLow, float melodyMid, float snare)
    {
        // Sun Changes
        GameObject mun = GameObject.Find("Mun");
        mun.transform.localScale = sunStartScale + (sunStartScale * bassKick * sunExtraScale);

        // Water
        GameObject water = GameObject.Find("Water");
        water.GetComponent<WaveGen>().scale = waterStartScale * melodyLow;

        //Fire Flies
        GameObject fireFlies1 = GameObject.Find("Firefly1");
        fireFlies1.GetComponent<ParticleSystem>().startSize = fireFlyStartScale * melodyMid;
        GameObject fireFlies2 = GameObject.Find("Firefly2");
        fireFlies2.GetComponent<ParticleSystem>().startSize = fireFlyStartScale * melodyMid;
        GameObject fireFlies3 = GameObject.Find("Firefly3");
        fireFlies3.GetComponent<ParticleSystem>().startSize = fireFlyStartScale * melodyMid;
        GameObject fireFlies4 = GameObject.Find("Firefly4");
        fireFlies4.GetComponent<ParticleSystem>().startSize = fireFlyStartScale * melodyMid;

        // Here we need the fire works
        // Pick a random, either splash or fire works
        if (snare > 0.06f && ((Mathf.Abs(System.DateTime.Now.Millisecond - msofLastFireWork) > fireWorkTimeBuffer) || (System.DateTime.Now.Second != secofLastFireWork)))
        {
            Vector3 spawnPos = new Vector3((Random.value * 700) + fireWorksStartPos.x - 350, (Random.value * 400) + fireWorksStartPos.y - 50, fireWorksStartPos.z);
            GameObject partSys = (GameObject)Instantiate(ps, spawnPos, new Quaternion(0,0,0,0));
            partSys.GetComponent<ParticleSystem>().Play();
            msofLastFireWork = System.DateTime.Now.Millisecond;
            secofLastFireWork = System.DateTime.Now.Second;
        }
        
    }
}
