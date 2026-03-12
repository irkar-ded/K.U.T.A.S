public class ConvertorValue
{
    public static double ConvertValue(double value, double fromMax, double toMax)
    {
        double scaledValue = (value) / fromMax;
        double convertedValue = (scaledValue * toMax);
        return convertedValue;
    }
    public static float ConvertNumber(float inputNumber) { return 1 - inputNumber / 100f; }
    public static float GetAnimValue(float startSpeed,float newSpeed) => startSpeed / newSpeed;
}
