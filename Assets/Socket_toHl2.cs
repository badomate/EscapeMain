using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using AuxiliarContent;

public class Socket_toHl2 : MonoBehaviour
{

    Thread thread;
    public int connectionPort = 25001;
    TcpListener server;
    TcpClient client;
    private volatile bool running = true;
    private volatile bool clientAccepted = false;

    public Vector3[] position;
    public static Pose hololensPlayerPose;

    // Start is called before the first frame update
    void Start()
    {
        ThreadStart ts = new ThreadStart(GetData);
        thread = new Thread(ts);
        thread.Start();
        position = new Vector3[Pose.LandmarkIds.Count];
    }

    void GetData()
    {
        // Create the server
        server = new TcpListener(IPAddress.Any, connectionPort);
        server.Start();

        // Create a client to get the data stream
        
        // Start listening
        while (running)
        {
            if (clientAccepted == false)
            {
                AcceptClient();
            }

            // Check if the client is still connected
            if (clientAccepted && !IsClientConnected(client))
            {
                Debug.LogWarning("Client connection lost. Reconnecting...");
                clientAccepted = false;

            }
            Connection();
            Thread.Sleep(100);
        }
        server.Stop();
        if (client != null)
            client.Close();
    }

    void AcceptClient()
    {
        client = server.AcceptTcpClient();
        clientAccepted = true;
        Debug.Log("Client connected.");
    }

    bool IsClientConnected(TcpClient tcpClient)
    {
        try
        {
            if (tcpClient != null && tcpClient.Client != null && tcpClient.Client.Connected)
            {
                if (tcpClient.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(tcpClient.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    void Connection()
    {
        // Read data from the network stream
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

        // Decode the bytes into a string
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        // Make sure we're not getting an empty string
        //dataReceived.Trim();
        if (dataReceived != null && dataReceived != "")
        {
            // Convert the received string of data to the format we are using
            //position = ParseData(dataReceived);
            //CustomDebug.LogAlex("NEW PARSED POSITION = " + string.Join(", ", position));
            hololensPlayerPose = Pose.GetPoseFromString(dataReceived, LevelManager._poseRegex);
            nwStream.Write(buffer, 0, bytesRead);
        }
        else
        {
            Debug.Log("No data was received by TCP.");
        }
    }

    // Use-case specific function, need to re-write this to interpret whatever data is being sent
    public static Vector3[] ParseData(string dataString)
    {
        Vector3[] parsedData = Pose.GetPoseVectorFromString(dataString, LevelManager._poseRegex);
        CustomDebug.LogAlex("ParsedData: " + string.Join(", ", parsedData));
        return parsedData;
    }


    void Update()
    {
        // Set this object's position in the scene according to the position received
        //transform.position = position;

    }

    void OnApplicationQuit()
    {
        running = false;
        //Debug.Log("quitting");
        if (clientAccepted)
        {
            Debug.Log("shutting down TCP thread");
            // Set the flag to stop the thread

            // Wait for the thread to finish its execution
            thread.Join();
        }
    }


}
