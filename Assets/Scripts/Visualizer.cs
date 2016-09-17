using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Visualizer : MonoBehaviour {

	GameObject EstherObject;
	ParticleSystem EstherSystem;

	public float lifetimeBase = 1;
	public float lifetimeScale = 8;

	public float speedBase = 1;
	public float speedScale = 5;

	public float sizeBase = 1;
	public float sizeScale = 4;

    // Holds the spectrum data, has to be a power of 2, might go down to 1024 if lag
    const int spectrumSize = 1024;
	private float[] spectrum = new float[spectrumSize];

	// 20-50hz = 6.8 - 17 - Sub bass
	const int subBassLow = 6;
	const int subBassHigh = 19;

	// 60-250hz = 20.5 - 85.3 - Bass
	const int bassLow = 20;
	const int bassHigh = 85;

	// 250-500hz = 85.3 - 170.6 - Low Midrange
	const int lowMidrangeLow = 86;
	const int lowMidrangeHigh = 170;

	//500-2k = 170.6 - 682.6 - Midrange
	const int midrangeLow = 171;
	const int midrangeHigh = 682;

	// 2k-4k = 682.6 - 1365.3 - Upper Midrange
	const int upperMidrangeLow = 683;
	const int upperMidrangeHigh = 1365;

    /* Below this hurts the ears and I doubt will be in regular music
	6k-20k = 2048 - 6826.6 - Brilliance
	4k-6k = 1365.3 - 2048 - Presence
	*/

    // For use in the moving average algorithm to smooth our data
    const int movingAverageSize = 20;
    private float[] movingAverageBuffer = new float[movingAverageSize];
    public float minValue = 0.001f;

    public int debugLineScale = 400;


    public class Peak
    {
        public int startIndex;
        public int endIndex;
        public List<float> data;
        public float total;
        public float max;
        public float getAverage()
        {
            return total / data.Count;
        }
        public Peak() { data = new List<float>();  }
    };

    private List<Peak> peakArray;


	// Use this for initialization
	void Start () {
		EstherObject = GameObject.Find ("Esther");
		EstherSystem = EstherObject.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		
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

        float subBass = 0f;
		float bass = 0f;
		float lowMidrange = 0f;
		float midrange = 0f;
		float upperMidrange = 0f;

        // For peak calculations, when in the loop do we have a peak started, and where are we in the peak array
        int numPeaks = 0;
        bool startedPeak = false;
        peakArray = new List<Peak>();

		for (int i = 0; i < spectrumSize; ++i)
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
            if (currentAverage * 0.9f + spectrum[i] * 0.1f < minValue)
            {
                spectrum[i] = 0;
            }
            else
            {
                spectrum[i] = currentAverage * 0.9f + spectrum[i] * 0.1f;
            }

            // Two cases, we need to create a new peak or we need to finalize an old peak
            if(i-1 > 0 && spectrum[i-1] == 0 && spectrum[i] != 0)
            {
                startedPeak = true;
                peakArray.Add(new Peak());
                peakArray[numPeaks].startIndex = i;
            }

            if(startedPeak)
            {
                peakArray[numPeaks].total += spectrum[i];
                peakArray[numPeaks].data.Add(spectrum[i]);
                if(peakArray[numPeaks].max < spectrum[i])
                {
                    peakArray[numPeaks].max = spectrum[i];
                }
            }

            // We have started a peak already and are now finishing it.
            if (i-1 > 0 && spectrum[i-1] != 0 && (spectrum[i] == 0 || i == (spectrumSize - 1)) && startedPeak)
            {
                startedPeak = false;
                peakArray[numPeaks].endIndex = i;
                numPeaks++;
            }

			if (i >= subBassLow && i <= subBassHigh)
			{
				subBass += spectrum [i];
			}
			else if (i >= bassLow && i <= bassHigh)
			{
				bass += spectrum [i];
			}
			else if (i >= lowMidrangeLow && i <= lowMidrangeHigh)
			{
				lowMidrange += spectrum [i];
			}
			else if (i >= midrangeLow && i <= midrangeHigh)
			{
				midrange += spectrum [i];
			}
			else if (i >= upperMidrangeLow && i <= upperMidrangeHigh)
			{
				upperMidrange += spectrum [i];
			}
			/*else if( i > upperMidrangeHigh)
			{
				break;
			}*/
		}
        // Log all of the items
        /*Debug.Log("SubBass: " + subBass);
		Debug.Log("Bass: " + bass);
		Debug.Log("LowMidrange: " + lowMidrange);
		Debug.Log("Midrange: " + midrange);
		Debug.Log("upperMidrange: " + upperMidrange);*/


        for (int i = 1; i * 4 < spectrumSize - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, spectrum[i] * debugLineScale, 0), new Vector3(i, spectrum[i + 1] * debugLineScale, 0), Color.red);
            //Debug.DrawLine(new Vector3(i - 1, Mathf.Abs(Mathf.Log(spectrum[i * 4]) * 50), 2), new Vector3(i, Mathf.Abs(Mathf.Log(spectrum[(i + 1) * 4]) * 50), 2), Color.cyan);
            //Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            // Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        }

        Debug.Log("PEAKS");
        for(int i = 0; i < peakArray.Count; ++i)
        {
            Debug.Log("Peak: " + i);
            Debug.Log(peakArray[i].getAverage());
        }

        //EstherSystem. = spectrum [1] * 10;
        EstherSystem.startLifetime = lifetimeBase + (lowMidrange * lifetimeScale);
		EstherSystem.startSpeed = speedBase + (midrange * speedScale);
		EstherSystem.startSize = sizeBase + (subBass + bass * sizeScale);
	}
}
