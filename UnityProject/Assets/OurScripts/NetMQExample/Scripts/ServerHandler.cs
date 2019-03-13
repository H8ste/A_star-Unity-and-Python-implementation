using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

///     To use this class, you just instantiate, call Start() when you want to start and Stop() when you want to stop.
public class ServerHandler : RunAbleThread
{
    public int FindElementByCoord(Vector3 pos)
    {
        return (((int)pos.z) * 40) + ((int)pos.x);
    }

    public Vector2 FindCoordByElement(int element)
    {
        int tempY = (int)(element / 40);
        return new Vector2(element - (40 * tempY), tempY);
    }
    public enum ServerState
    {
        SendingTILES,
        SendingCLICKPOS,
        Received,
        Default,
        Busy
    }

    public ServerState _state = ServerState.Default;

    public string _sendingString = null;

    private Vector2[] pathStringToArray(string agentPath)
    {
        // Converts incoming string into array, then cast the string array to an int array using LINQ
        int[] myInts = agentPath.Split(';').Select(int.Parse).ToArray();
        Vector2[] returnArr = new Vector2[myInts.Length];
        for (int i = 0; i < returnArr.Length; i++)
        {
            returnArr[i] = FindCoordByElement(myInts[i]);
            // Debug.Log("Element:" + i + ", x:"+ returnArr[i].x + ", y:"+ returnArr[i].y);
        }
        return returnArr;
    }


    ///     Stop requesting when Running=false.
    protected override void Run()
    {
        ForceDotNet.Force();

        using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://localhost:5555");

            while (Running)
            {
                if (Send)
                {
                    switch (_state)
                    {
                        case ServerState.Default:
                            {
                                break;
                            }
                        case ServerState.Received:
                            {
                                break;
                            }
                        case ServerState.SendingCLICKPOS:
                            {
                                _state = ServerState.Busy;
                                if (_sendingString != null && _sendingString != "")
                                {
                                    client.SendFrame(_sendingString);
                                    Debug.Log("Sent the click position and agents to python: " + _sendingString);

                                    string messRecieve = "";
                                    messRecieve = client.ReceiveFrameString();
                                    string[] tempPaths = messRecieve.Split(':');
                                    List<Vector2[]> agentPaths = new List<Vector2[]>();
                                    for (int i = 0; i < tempPaths.Length; i++)
                                    {
                                        // Debug.Log("TEST: " + tempPaths[i]);
                                        Vector2[] temp2DVectorArr = pathStringToArray(tempPaths[i]);
                                        agentPaths.Add(temp2DVectorArr);
                                    }

                                    // Just for printing:
                                    for (int i = 0; i < agentPaths.Count; i++)
                                    {
                                        string debugString ="";
                                        for (int k = agentPaths[i].Length - 1; k >= 0; k--)
                                        {
                                            debugString += "("+agentPaths[i][k].x + "," + agentPaths[i][k].y+")";
                                            debugString += " : ";
                                            
                                        }
                                        Debug.Log("Agent " + (i+1) + " path:" + debugString);
                                    }
                      
                                    _state = ServerState.Default;
                                    _sendingString = "";
                                }
                                break;
                            }
                        case ServerState.SendingTILES:
                            {
                                _state = ServerState.Busy;
                                if (_sendingString != null && _sendingString != "")
                                {
                                    client.SendFrame(_sendingString);
                                    Debug.Log("Message sent to python: " + _sendingString);

                                    string messRecieve = "";
                                    messRecieve = client.ReceiveFrameString();
                                    Debug.Log("Message recieved from python: " + messRecieve);

                                    _state = ServerState.Default;
                                    _sendingString = "";
                                }
                                break;
                            }
                        case ServerState.Busy:
                            {
                                break;
                            }
                    }
                }
            }
        }

        NetMQConfig.Cleanup();
    }
}