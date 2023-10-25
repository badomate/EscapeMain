using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;

public class CameraStream : MonoBehaviour
{
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    [Serializable]
    private class BodyContainer
    {
        public List<BodyData> body;
        public HandContainer hands;
    }

    [Serializable]
    private class HandContainer
    {
        public List<BodyData> left;
        public List<BodyData> right;
    }

    [Serializable]
    private class BodyData
    {
        public int id;
        public List<float> data;
        public string landmarkName;
    }

    public static Pose playerPose = new Pose();
    public Vector3 centerLandmarkOffset = new Vector3(); //coordinates of that center
    public long animationFPS = 0;

    // Process and handle the received landmarks data
    void ProcessLandmarksData(string jsonData)
    {
        jsonData = jsonData.Substring(5);

        //UnityEngine.Debug.Log(jsonData);

        // Deserialize the JSON string into an array of Vector3Data objects
        BodyContainer dataContainer = JsonUtility.FromJson<BodyContainer>(jsonData);
        Vector3 Left = new Vector3(0, 0, 0);
        Vector3 Right = new Vector3(0, 0, 0);

        var combinedData = dataContainer.body.Concat(dataContainer.hands.left);

        foreach (BodyData body in combinedData) //the amount of landmarks seems to always be 33 no matter how obscured the person is
        {
            Pose.Landmark identifiedLandmark = Pose.Landmark.LEFT_WRIST;
            bool included = true; //whether we are going to use it, whether it appears in the switch case somewhere
            switch (body.landmarkName)
            {
                case "Left wrist":
                    identifiedLandmark = Pose.Landmark.LEFT_WRIST;
                    break;
                case "Right wrist":
                    identifiedLandmark = Pose.Landmark.RIGHT_WRIST;
                    break;
                case "Left heel":
                    identifiedLandmark = Pose.Landmark.LEFT_FOOT;
                    break;
                case "Right heel":
                    identifiedLandmark = Pose.Landmark.RIGHT_FOOT;
                    break;
                case "Right elbow":
                    identifiedLandmark = Pose.Landmark.RIGHT_ELBOW;
                    break;
                case "Left elbow":
                    identifiedLandmark = Pose.Landmark.LEFT_ELBOW;
                    break;
                case "Left shoulder":
                    identifiedLandmark = Pose.Landmark.LEFT_SHOULDER;
                    break;
                case "Right shoulder":
                    identifiedLandmark = Pose.Landmark.RIGHT_SHOULDER;
                    break;
                case "Index-4(fingertip)":
                    identifiedLandmark = Pose.Landmark.LEFT_INDEX;
                    break;
                case "Thumb-4(fingertip)":
                    identifiedLandmark = Pose.Landmark.LEFT_THUMB;
                    break;
                case "Middle-4(fingertip)":
                    identifiedLandmark = Pose.Landmark.LEFT_MIDDLE;
                    break;
                case "Ring-4(fingertip)":
                    identifiedLandmark = Pose.Landmark.LEFT_RING;
                    break;
                case "Pinky-4(fingertip)":
                    identifiedLandmark = Pose.Landmark.LEFT_PINKY;
                    break;
                case "Wrist":
                    identifiedLandmark = Pose.Landmark.LEFT_WRIST_ROOT;
                    break;
                default:
                    included = false; //if it didn't match anything we need, don't modify the Pose
                    break;
            }
            Vector3 adjustedVector3 = Vector3.Scale(new Vector3(body.data[0], body.data[1], body.data[2]), new Vector3(-1, -1, -1));

            if (included && !playerPose._landmarkArrangement.ContainsKey(identifiedLandmark))
            {
                playerPose._landmarkArrangement.Add(identifiedLandmark, adjustedVector3);
            }
            else if(included)
            {
                playerPose._landmarkArrangement[identifiedLandmark] = adjustedVector3;
            }

            if (body.landmarkName == "Right hip")
            {
                Right = new Vector3(body.data[3], body.data[4], body.data[5]); //body.data[5]
            }
            else if (body.landmarkName == "Left hip")
            {
                Left = new Vector3(body.data[3], body.data[4], body.data[5]);
            }
        }
        centerLandmarkOffset = (Right + Left) / 2;

    }

    // Asynchronously stream pose landmarks data from the Flask API
    private async Task StreamLandmarksAsync(CancellationToken cancellationToken,
                                           string videoPath = null,
                                           bool staticImageMode = false,
                                           int modelComplexity = 1,
                                           double minDetectionConfidence = 0.5,
                                           double minTrackingConfidence = 0.5)
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
                              $"&min_tracking_confidence={minTrackingConfidence}";

            // Send the GET request to the API
            var responseStream = await client.GetStreamAsync(baseUrl + apiUrl + queryParams);
            
            // Read the response stream and process each line of data
            using (var reader = new System.IO.StreamReader(responseStream))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                while (!reader.EndOfStream)
                {
                    if (cancellationToken.IsCancellationRequested) //If requested, exit task
                    {
                        // Clean up resources?
                        break;
                    }
                    var line = await reader.ReadLineAsync();

                    stopwatch.Stop();

                    long elapsed = stopwatch.ElapsedMilliseconds;
                    //long hz = 1000 / elapsed;
                    if (elapsed > 0)
                    {
                        animationFPS = 1000 / elapsed;
                        //UnityEngine.Debug.Log("Animation FPS: " + 1000 / elapsed);
                    }

                    if (!string.IsNullOrEmpty(line))
                    {
                        // Process each line of landmarks data
                        ProcessLandmarksData(line);
                    }

                    stopwatch.Reset();
                    stopwatch.Start();
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
                             modelComplexity: 0,
                             minDetectionConfidence: 0.5,
                             minTrackingConfidence: 0.5));

      /*string jsonString = "{\"body\": [{\"id\": 0, \"landmarkName\": \"Nose\", \"data\": [0.0683145523071289, -0.5272032618522644, -0.2814151346683502, 0.534261167049408, 0.5520623326301575, -0.8420344591140747]}, {\"id\": 1, \"landmarkName\": \"Left eye inner\", \"data\": [0.06900090724229813, -0.5620826482772827, -0.2750793397426605, 0.542637050151825, 0.48907387256622314, -0.7762095928192139]}]}";

      BodyContainer body = JsonUtility.FromJson<BodyContainer>(jsonString);
        foreach (var landmark in body.body)
        {
            UnityEngine.Debug.Log("Landmark ID: " + landmark.id);
            UnityEngine.Debug.Log("Landmark Name: " + landmark.landmarkName);
            UnityEngine.Debug.Log("Landmark Data: " + string.Join(", ", landmark.data));
        }
      */
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
