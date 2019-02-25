using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gamecontroller : MonoBehaviour
{
    Collider object_Collider;
    public GameObject tileRef;
    public List<GameObject> allTiles = new List<GameObject>();

    private ServerHandler _server;


    //Sending DATA to PYTHON <-> Receiving DATA from PYTHON


    //Struct used to send only the server-required KNOWLEDGE
    private struct tileSEND
    {
        public int x, y, tileCost;
    }


    void Start()
    {
        _server = new ServerHandler();


        for (int z = 0; z < 25; z++)
            for (int x = 0; x < 40; x++)
            {
                allTiles.Add(Instantiate(tileRef, new Vector3(x, 0, z), Quaternion.identity));
                allTiles[allTiles.Count - 1].GetComponent<TileScript>().tileType = (TileScript.ColorTypes)Random.Range(0, 4);
                allTiles[allTiles.Count - 1].GetComponent<TileScript>().CalculateTypeCost();
            }
        _server.Start();
        _server.Continue();

        if(SendToServer(ServerHandler.ServerState.SendingTILES, SendAllTiles(allTiles))){
            //Debug.Log("Sent tiles to server succesfully");
        } else{
            Debug.Log("Files were not sent to server, see error message");
        }
    }



    // Update is called once per frame
    void Update()
    {
        //If mouse button is clicked
        if (Input.GetButtonDown("Fire1"))
        {
            int _chosenTile = GetPositionClicked();
            Debug.Log("Element: " + _chosenTile + ":   x = " + FindCoordByElement(_chosenTile).x + "    &    y = " + FindCoordByElement(_chosenTile).y);
            //allTiles[_chosenTile].
            //send tile 
        }
    }

    /// <summary>Sends All tiles to python server. Contains: a cost for each tile in correct order</summary>
    string SendAllTiles(List<GameObject> TileList)
    {
        string returnString = "0:";
        for (int i = 0; i < TileList.Count; i++)
        {
            string tempString = "" + TileList[i].GetComponent<TileScript>().tyleCost;
            Debug.Log("What is this:? " + TileList[i].gameObject.GetComponent<TileScript>().tyleCost);
            if (i != TileList.Count-1)
            {
                tempString = string.Concat(tempString,";");
            }
            returnString = string.Concat(returnString, tempString);
        }
        return returnString;
    }

    bool SendToServer(ServerHandler.ServerState newState, string msgToServer)
    {
        if (_server._state != ServerHandler.ServerState.Busy)
        {
            Debug.Log("Message sent to server: " + msgToServer);
            _server._sendingString = msgToServer;
            SetServerState(newState);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetServerState(ServerHandler.ServerState newState)
    {
        _server._state = newState;
    }

    int FindElementByCoord(Vector3 pos)
    {
        return (((int)pos.z) * 40) + ((int)pos.x);
    }

    Vector2 FindCoordByElement(int element)
    {
        int tempY = (int)(element / 40);
        return new Vector2(element - (40 * tempY), tempY);
    }

    int GetPositionClicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        Vector3 _MousePostion = new Vector3(-1, -1, -1);
        if (Physics.Raycast(ray))
        {
            _MousePostion = ray.origin;
            _MousePostion.x += .5f;
            _MousePostion.z += .5f;
        }
        //calculate element in array
        return FindElementByCoord(_MousePostion);

    }

    private void OnDestroy()
    {
        _server.Stop();
    }

}

