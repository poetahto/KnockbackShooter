/// <summary>
/// Nonlinear transformations for floats.
/// Adapted from the GDC video [https://www.youtube.com/watch?v=mr5xkf6zSzk]
/// </summary>
public static class Transform1D
{
    public static float SmoothStart2(float t) => t * t;
    public static float SmoothStart3(float t) => t * t * t;
    public static float SmoothStart4(float t) => t * t * t * t;
    public static float SmoothStart5(float t) => t * t * t * t * t;
    public static float SmoothStart6(float t) => t * t * t * t * t * t;
}