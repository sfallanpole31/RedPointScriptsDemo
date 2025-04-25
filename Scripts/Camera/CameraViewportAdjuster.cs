using UnityEngine;

public class CameraViewportAdjuster : MonoBehaviour
{
    void Update()
    {
        Camera.main.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
    }
}
