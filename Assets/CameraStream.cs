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

    [Serializable]
    private class BodyContainer
    {
        public List<BodyData> bodies;
    }

    [Serializable]
    private class BodyData
    {
        public int id;
        public List<float> data;
        public string landmarkName;
    }

    public List<Vector3> vector3List = new List<Vector3>(); //coordinates are based on position relative to the center
    public Vector3 centerLandmarkOffset = new Vector3(); //coordinates of that center

    // Process and handle the received landmarks data
    void ProcessLandmarksData(string jsonData)
    {
        jsonData = jsonData.Substring(5);
        //jsonData = "{\"data\": " + jsonData.Substring(5) + "}"; //this is for an old version of the API (cf410d14). With the new version and its format, different changes need to be made
        vector3List.Clear(); // Clear the list before deserialization

        //Debug.Log(jsonData);

        // Deserialize the JSON string into an array of Vector3Data objects
        BodyContainer dataContainer = JsonUtility.FromJson<BodyContainer>(jsonData);
        Vector3 Left = new Vector3(0, 0, 0);
        Vector3 Right = new Vector3(0, 0, 0);

        foreach (BodyData body in dataContainer.bodies) //the amount of landmarks seems to always be 33 no matter how obscured the person is
        {
            Vector3 vector3 = new Vector3(body.data[0], body.data[1], body.data[2]);
            vector3List.Add(vector3);
            if (body.landmarkName == "Right hip")
            {
                Right = new Vector3(body.data[3], body.data[4], body.data[5]);
            }
            else if (body.landmarkName == "Left hip")
            {
                Left = new Vector3(body.data[3], body.data[4], body.data[5]);
            }
        }
        centerLandmarkOffset = (Right + Left) / 2;
        //Debug.Log(centerLandmarkOffset);

    }

    // Asynchronously stream pose landmarks data from the Flask API
    private async Task StreamLandmarksAsync(CancellationToken cancellationToken,
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
                    //await Task.Yield();
                }
            }
        }
    }
    private Task myGet;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Connecting...");
        //string myJson = "data : {\"bodies\": [{\"id\": 0, \"landmarkName\": \"Nose\", \"data\": [0.017760001122951508, -0.5622981190681458, -0.2617194652557373, 0.501261830329895, 0.6356117129325867, -0.5766324400901794]}, {\"id\": 1, \"landmarkName\": \"Left eye inner\", \"data\": [0.02411283180117607, -0.6019347906112671, -0.24616317451000214, 0.5239912867546082, 0.5869146585464478, -0.5480535626411438]}]}";
        //BodyContainer dataContainer = JsonUtility.FromJson<BodyContainer>(myJson);
        //Debug.Log(dataContainer.bodies[0].data.Count);

        // Call the asynchronous method to stream pose landmarks data
        myGet = Task.Run(() => StreamLandmarksAsync(cancellationTokenSource.Token,
                             videoPath: null,
                             staticImageMode: false,
                             modelComplexity: 1,
                             minDetectionConfidence: 0.5,
                             minTrackingConfidence: 0.5,
                             detectSingle: true));
        
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
