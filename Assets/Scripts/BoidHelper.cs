using UnityEngine;

public static class BoidHelper
{
    // Shamelessly stolen from https://github.com/SebLague/Boids/blob/master/Assets/Scripts/BoidHelper.cs
    const int viewDirections = 300;
    public static readonly Vector3[] directions;

    static BoidHelper()
    {
        directions = new Vector3[BoidHelper.viewDirections];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < viewDirections; i++)
        {
            float t = (float)i / viewDirections;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);
            directions[i] = new Vector3(x, y, z);
        }
    }
}
