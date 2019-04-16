/*
Sampo's Gaming Company
Leo Tamminen
*/

// Use this class to get smooth float value eg. over multiple frames.
public class SmoothFloat
{
	// Smoothing array info
	private int index;
	private readonly float [] array;
	private readonly int length;

	// Get smoothed value.
	public float Value { get; private set; }

	// Set min and max to clamp values when putting in
	public SmoothFloat (int arrayLength = 10, float? min = null, float? max = null)
	{
		array = new float [arrayLength];
		index = 0;
		length = arrayLength;

		// Make one null check here, so we don't have to check everytime
		if (min == null || max == null)
		{
			Put = PutImplement;	
		}
		else 
		{
			float _min = min ?? 0.0f;
			float _max = max ?? 1.0f;
			if (_max < _min) _max = _min;
			
			Put = (value) =>
			{
				// Inline clamp
				if (value < _min) value = _min;
				else if (value > _max) value = _max;
 
				PutImplement(value);
			};
		}
	}

	// Put in next value. 
	// NOTE: This is defined in constructor.
	public System.Action<float> Put;

	// This does actual adding value.
	private void PutImplement(float value)
	{
		array[index] = value;
		index = (index + 1) % length;
		Value = array.Average();
	}
}
