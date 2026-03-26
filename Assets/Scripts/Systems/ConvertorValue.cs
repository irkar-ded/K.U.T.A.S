using UnityEngine;
public class ConvertorValue
{
    public static double ConvertValue(double value, double fromMax, double toMax)
    {
        double scaledValue = value / fromMax;
        double convertedValue = scaledValue * toMax;
        return convertedValue;
    }
    public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
    {
        if (value.x < min.x || value.y < min.y || value.z < min.z ||
            value.x > max.x || value.y > max.y || value.z > max.z)
        {
            value.x = Mathf.Clamp(value.x, min.x, max.x);
            value.y = Mathf.Clamp(value.y, min.y, max.y);
            value.z = Mathf.Clamp(value.z, min.z, max.z);
        }
        return value;
    }
    public static string FormatFloat(float number)=> number % 1 == 0 ? ((int)number).ToString() : number.ToString("0.##");
}
