using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;


///     You can copy this class and modify Run() to suits your needs.
///     To use this class, you just instantiate, call Start() when you want to start and Stop() when you want to stop.




public class ServerHandler : RunAbleThread
{

    public enum ServerState
    {
        SendingTILES,
        SendingCLICKPOS,
        Received,
        Default,
        Busy
    }

    bool sendAllTiles = false;
    public ServerState _state = ServerState.Default;

    public string _sendingString = null;


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
                                break;
                            }
                        case ServerState.SendingTILES:
                            {
                                _state = ServerState.Default;
                                if (_sendingString != null)
                                {
                                    // var test = new NetMQMessage(4);
                                    // test.AppendEmptyFrame();
                                    // test.Append(_sendingString);
                                    // test.AppendEmptyFrame();
                                    // test.Append(";Sent to server");
                                    // client.SendMultipartMessage(test);
                                    client.SendFrame("Sent from server succesfully Hello");
                                    Debug.Log("Fuck OFFFFFFF");
                                }
                                break;
                            }
                        case ServerState.Busy:
                            {
                                break;
                            }
                    }
                    // if (_state == ServerState.SendingTILES)
                    // {

                    // }
                    // else if (_state)
                    // {
                    //     //string message = client.ReceiveFrameString();
                    //     client.SendFrame("Hello");

                    //     string message = null;
                    //     bool gotMessage = false;

                    //     while (Running)
                    //     {
                    //         gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
                    //         if (gotMessage) break;
                    //     }
                    //     if (gotMessage) Debug.Log("Received " + message);
                    // }
                }
            }
        }

        NetMQConfig.Cleanup();
    }
}