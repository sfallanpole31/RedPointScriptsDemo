using UnityEngine;

public class CanvasAdjuster : MonoBehaviour
{
    public float distanceToCamera = 10f;

    void update()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = distanceToCamera;
    }
}
