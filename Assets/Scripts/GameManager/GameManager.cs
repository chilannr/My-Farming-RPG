using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    public Weather currentWeather;
    public PixelPerfectCamera pixelPerfectCamera;
    public AspectRatioFitter aspectRatioFitter;
    public CanvasScaler canvasScaler;

    protected override void Awake()
    {
        base.Awake();
        SetScreenResolution();
        SetStartingWeather();
    }

    private void SetScreenResolution()
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        float screenAspect = (float)screenWidth / (float)screenHeight;

        if (!Application.isMobilePlatform)
        {
            // 桌面平台设置分辨率
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        }
        else
        {
            Application.targetFrameRate = 60;
            aspectRatioFitter.aspectRatio = screenAspect;
            // 移动设备设置参考分辨率
            canvasScaler.referenceResolution = new Vector2((int)(270f * screenAspect), 270);
            pixelPerfectCamera.refResolutionX = (int)(270f * screenAspect);
            pixelPerfectCamera.refResolutionY = 270;
        }
    }

    private void SetStartingWeather()
    {
        // 设置起始天气
        currentWeather = Weather.dry;
    }
}