using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    private float originalFOV = 60f; // 原始的视野角度（透视相机）
    private float originalSize = 5f; // 原始的Orthographic Size（正交相机）

    void Update()
    {
        AdjustCamera();
    }

    void AdjustCamera()
    {
        float aspectRatio = (float)Screen.width / Screen.height;

        // 如果使用透视相机
        if (Camera.main.orthographic == false)
        {
            Camera.main.fieldOfView = Mathf.Atan(Mathf.Tan(originalFOV * Mathf.Deg2Rad / 2) / aspectRatio) * 2 * Mathf.Rad2Deg;
        }
        // 如果使用正交相机
        else
        {
            float targetAspect = 16.0f / 9.0f; // 目标宽高比
            float windowAspect = (float)Screen.width / (float)Screen.height;
            float scaleHeight = targetAspect / windowAspect;

            if (scaleHeight < 1.0f)
            {
                Camera.main.orthographicSize = originalSize / scaleHeight;
            }
            else
            {
                Camera.main.orthographicSize = originalSize;
            }
        }
    }
}
