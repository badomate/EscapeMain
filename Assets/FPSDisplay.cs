using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public TMPro.TextMeshProUGUI display_Text;
    public TMPro.TextMeshProUGUI display_Text2;
    public CameraStream cameraStreamScript;
    public float updateInterval = 0.5f;
    private float accumDeltaTime = 0.0f;
    private int frames = 0;

    void Update()
    {
        accumDeltaTime += Time.unscaledDeltaTime;
        frames++;

        if (accumDeltaTime >= updateInterval)
        {
            float avgFPS = frames / accumDeltaTime;
            display_Text.text = avgFPS.ToString("F0") + " FPS";
            if (cameraStreamScript)
            {
                display_Text2.text = cameraStreamScript.animationFPS.ToString("F0") + " ANIM FPS";
            }
            else
            {
                display_Text2.enabled = false;
            }

            accumDeltaTime = 0.0f;
            frames = 0;
        }
    }
}