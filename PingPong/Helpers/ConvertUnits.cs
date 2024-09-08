using Microsoft.Xna.Framework;

namespace PingPong.Helpers;

public static class ConvertUnits
{
    // Conversion ratio: how many pixels represent one meter in your physics simulation
    private static float _displayUnitsToSimUnitsRatio = 100f;  // 1 meter = 100 pixels
    private static float _simUnitsToDisplayUnitsRatio = 1f / _displayUnitsToSimUnitsRatio;

    /// <summary>
    /// Converts from display units (pixels) to simulation units (meters).
    /// </summary>
    public static float ToSimUnits(float displayUnits)
    {
        return displayUnits * _simUnitsToDisplayUnitsRatio;
    }

    /// <summary>
    /// Converts from display units (pixels) to simulation units (meters).
    /// </summary>
    public static nkast.Aether.Physics2D.Common.Vector2 ToSimUnits(Vector2 displayUnits)
    {
        return new nkast.Aether.Physics2D.Common.Vector2(
                       displayUnits.X * _simUnitsToDisplayUnitsRatio,
                                  displayUnits.Y * _simUnitsToDisplayUnitsRatio
                              );
    }

    /// <summary>
    /// Converts from simulation units (meters) to display units (pixels).
    /// </summary>
    public static float ToDisplayUnits(float simUnits)
    {
        return simUnits * _displayUnitsToSimUnitsRatio;
    }

    /// <summary>
    /// Converts from simulation units (meters) to display units (pixels).
    /// </summary>
    public static Vector2 ToDisplayUnits(Vector2 simUnits)
    {
        return simUnits * _displayUnitsToSimUnitsRatio;
    }

    /// <summary>
    /// Sets the conversion ratio between display units (pixels) and simulation units (meters).
    /// </summary>
    public static void SetDisplayUnitToSimUnitRatio(float ratio)
    {
        _displayUnitsToSimUnitsRatio = ratio;
        _simUnitsToDisplayUnitsRatio = 1f / ratio;
    }

    public static Vector2 ToDisplayUnits(nkast.Aether.Physics2D.Common.Vector2 physicsBodyPosition)
    {
        // Convert Aether Physics2D Vector2 (meters) to MonoGame Vector2 (pixels)
        return new Vector2(
            physicsBodyPosition.X * _displayUnitsToSimUnitsRatio,
            physicsBodyPosition.Y * _displayUnitsToSimUnitsRatio
        );
    }

}