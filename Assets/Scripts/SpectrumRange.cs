using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpectrumRange
{
	private int lowerBound;
	private int upperBound;
	public float sum;
	public float max;

	public SpectrumRange (int lowerBound, int upperBound)
	{
		this.lowerBound = lowerBound;
		this.upperBound = upperBound;
		reset();
	}

	public bool inRange (int index)
	{
		if (index >= lowerBound && index <= upperBound) {
			return true;
		} else {
			return false;
		}
	}

	public void addToSum (float value)
	{
		if (value > max) {
			max = value;
		}
		sum += value;
	}

	public float avg ()
	{
		return sum / (upperBound - lowerBound + 1);
	}

	public void reset()
	{
		sum = 0f;
		max = 0f;
	}
}

