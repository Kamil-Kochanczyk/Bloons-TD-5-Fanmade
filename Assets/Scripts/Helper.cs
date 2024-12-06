using UnityEngine;
using UnityEngine.InputSystem;

public static class Helper
{
    public static Vector3 GetCursorWorldPosition(float z)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        //position.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        position.z = z;
        return position;
    }

    public static Color ChangeHSVValue(Color rgbColor, float deltaHSVValue)
    {
        float h, s, v;
        Color.RGBToHSV(rgbColor, out h, out s, out v);
        v += deltaHSVValue;
        return Color.HSVToRGB(h, s, v);
    }
}
