using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStream : MonoBehaviour
{
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    // Process and handle the received landmarks data
    static void ProcessLandmarksData(string data)
    {
        // Process and handle the received landmarks data
        Debug.Log("Received landmarks data: " + data);
        // You can parse the JSON data and perform further processing here
    }

    // Asynchronously stream pose landmarks data from the Flask API
    static async Task StreamLandmarksAsync(CancellationToken cancellationToken,
                                           string videoPath = null,
                                           bool staticImageMode = false,
                                           double minDetectionConfidence = 0.5,
                                           double minTrackingConfidence = 0.5,
                                           bool detectSingle = false)
    {
        var baseUrl = "http://localhost:5000";
        var apiUrl = "/landmarks";

        using (var client = new HttpClient())
        {
            // Build the query parameters for the API request
            var queryParams = $"?video_path={videoPath}" +
                              $"&static_image_mode={staticImageMode}" +
                              $"&min_detection_confidence={minDetectionConfidence}" +
                              $"&min_tracking_confidence={minTrackingConfidence}" +
                              $"&detect_single={detectSingle}";

            // Send the GET request to the API
            var responseStream = await client.GetStreamAsync(baseUrl + apiUrl + queryParams);

            // Read the response stream and process each line of data
            using (var reader = new System.IO.StreamReader(responseStream))
            {
                while (!reader.EndOfStream)
                {
                    if (cancellationToken.IsCancellationRequested) //If requested, exit task
                    {
                        // Clean up resources?
                        break;
                    }
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(line))
                    {
                        // Process each line of landmarks data
                        ProcessLandmarksData(line);
                    }
                }
            }
        }
    }

    private Task myGet;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        // Call the asynchronous method to stream pose landmarks data
        myGet = StreamLandmarksAsync(cancellationTokenSource.Token,
                             videoPath: null,
                             staticImageMode: false,
                             minDetectionConfidence: 0.5,
                             minTrackingConfidence: 0.5,
                             detectSingle: true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnApplicationQuit()
    {
        // Request cancellation of the task
        cancellationTokenSource.Cancel();
    }
}
