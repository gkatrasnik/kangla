namespace kangla.DeviceSimulator;

public static class SoilMoistureCalibration
{
    public static int CalculatePercentage(int rawValue, int dryValue = 3200, int wetValue = 1500)
    {
        if (dryValue == wetValue)
        {
            throw new ArgumentException("Dry and wet calibration values must differ.");
        }

        var mappedValue = (rawValue - dryValue) * 100 / (wetValue - dryValue);
        return Math.Clamp(mappedValue, 0, 100);
    }
}
