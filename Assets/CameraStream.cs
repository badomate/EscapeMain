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

    [System.Serializable]
    private class DataContainer
    {
        public List<Vector3Data> data;
    }


    [System.Serializable]
    private class Vector3Data
    {
        public float x;
        public float y;
        public float z;
    }

    public List<Vector3> vector3List = new List<Vector3>();

    // Process and handle the received landmarks data
    void ProcessLandmarksData(string jsonData)
    {
        jsonData = "{\"data\": " + jsonData.Substring(5) + "}"; //this is for an old version of the API (cf410d14). With the new version and its format, different changes need to be made
        vector3List.Clear(); // Clear the list before deserialization

        //Debug.Log(jsonData);
        // Deserialize the JSON string into an array of Vector3Data objects
        DataContainer dataContainer = JsonUtility.FromJson<DataContainer>(jsonData);


        //Debug.Log("Processed");

        foreach (Vector3Data data in dataContainer.data) //the amount of landmarks seems to always be 33 no matter how obscured the person is
        {
            Vector3 vector3 = new Vector3(data.x, data.y, data.z);
            vector3List.Add(vector3);
        }

        //Debug.Log("Vector3 List Count: " + vector3List.Count);
        
    }

    // Asynchronously stream pose landmarks data from the Flask API
    async Task StreamLandmarksAsync(CancellationToken cancellationToken,
                                           string videoPath = null,
                                           bool staticImageMode = false,
                                           int modelComplexity = 1,
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
                              $"&model_complexity={modelComplexity}" +
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

    //string myJson = "{'data': [{'x': 0.53, 'y': 0.59, 'z': -0.57}, { 'x': 0.5321588516235352, 'y': 0.5559157133102417, 'z': -0.5116672515869141}]}";

    private Task myGet;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        //DataContainer dataContainer = JsonUtility.FromJson<DataContainer>(myJson);
        // Call the asynchronous method to stream pose landmarks data
        myGet = StreamLandmarksAsync(cancellationTokenSource.Token,
                             videoPath: null,
                             staticImageMode: false,
                             modelComplexity: 1,
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
