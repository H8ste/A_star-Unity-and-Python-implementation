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
                                _state = ServerState.Busy;
                                if (_sendingString != null && _sendingString != "")
                                {
                                    client.SendFrame(_sendingString);
                                    Debug.Log("Message sent to python: " + _sendingString);
                                    
                                    string messRecieve= "";
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