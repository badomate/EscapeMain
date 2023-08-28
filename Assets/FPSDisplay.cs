using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public TMPro.TextMeshProUGUI display_Text;
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

            accumDeltaTime = 0.0f;
            frames = 0;
        }
    }
}