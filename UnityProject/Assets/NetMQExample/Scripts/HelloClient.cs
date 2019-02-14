using UnityEngine;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class HelloClient : MonoBehaviour
{
    private HelloRequester _helloRequester;
    public bool SendPack = true;

    void Update()
    {
        if (SendPack)
        {
            _helloRequester.Continue();
        } else if (!SendPack)
        {
            _helloRequester.Pause();
        }
    }

    private void Start()
    {
        _helloRequester = new HelloRequester();
        _helloRequester.Start();
    }

    private void OnDestroy()
    {
        _helloRequester.Stop();
    }
}