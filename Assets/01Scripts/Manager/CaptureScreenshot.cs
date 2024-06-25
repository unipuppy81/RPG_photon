using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureScreenshot : MonoBehaviour
{
    public Camera characterCamera;
    public string screenshotFileName = "CharacterScreenshot.png";

    void Start()
    {
        Capture();
    }

    public void Capture()
    {
        // Set the camera's target texture to a temporary render texture
        RenderTexture rt = new RenderTexture(1024, 1024, 24);
        characterCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(1024, 1024, TextureFormat.RGB24, false);

        // Render the camera's view
        characterCamera.Render();

        // Read pixels from the render texture
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 1024, 1024), 0, 0);
        screenShot.Apply();

        // Reset the camera's target texture
        characterCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // Encode the texture to PNG and save it to the file system
        byte[] bytes = screenShot.EncodeToPNG();
        string filePath = System.IO.Path.Combine(Application.dataPath, screenshotFileName);
        System.IO.File.WriteAllBytes(filePath, bytes);

        Debug.Log($"Screenshot saved to {filePath}");
    }
}
