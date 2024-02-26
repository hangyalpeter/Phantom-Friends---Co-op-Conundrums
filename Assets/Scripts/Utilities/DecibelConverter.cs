using UnityEngine;

public class DecibelConverter 
{
    public static float ConvertLinearToDecibel(float linearVolume)
    {
        return Mathf.Log10(Mathf.Max(0.0001f, linearVolume)) * 20.0f;
    }

    public static float ConvertDecibelToLinear(float decibelVolume)
    {
        return Mathf.Pow(10, decibelVolume / 20.0f);
    }
}
