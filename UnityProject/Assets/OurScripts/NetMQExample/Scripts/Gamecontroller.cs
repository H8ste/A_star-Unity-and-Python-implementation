using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gamecontroller : MonoBehaviour
{
    public GameObject tileRef;
    public List<GameObject> allTiles = new List<GameObject>();
    private ServerHandler _server;
    private int _preChosenTile = -1;

    // Start is called once when the component with Gamecontroller script attached is spawned
    void Start()
    {
        // Instantiate the server used
        _server = new ServerHandler();

        // Creates all the tiles in the game
        for (int z = 0; z < 25; z++)
            for (int x = 0; x < 40; x++)
            {
                allTiles.Add(Instantiate(tileRef, new Vector3(x, 0, z), Quaternion.identity));
                allTiles[allTiles.Count - 1].GetComponent<TileScript>().tileType = (TileScript.ColorTypes)Random.Range(0, 4);
                allTiles[allTiles.Count - 1].GetComponent<TileScript>().CalculateTypeCost();
            }

        // Instantiates the thread the server will run on
        _server.Start();
        // Begins the run method of the server
        _server.Continue();
        
        // Sends all the tiles created as a string, stringformat:     (SENDTYPE:ELM1;ELM2;ELM3.....)
        // Returns either true or false based if it was succesful
        if(SendToServer(ServerHandler.ServerState.SendingTILES, SendAllTiles(allTiles))){
            Debug.Log("Sent tiles to server succesfully");
        } else{
            Debug.Log("Files were not sent to server");
        }
    }



    // Update is called once per frame
    void Update()
    {
        // If mouse button is clicked
        if (Input.GetButtonDown("Fire1"))
        {
            // Get mouse postion and check if it is within the index of tiles and clickable
            int _chosenTile = GetPositionClicked();
            if (_chosenTile >=0 && _chosenTile <= 999 && allTiles[_chosenTile].GetComponent<TileScript>().tileType != TileScript.ColorTypes.Lava){
                // If not first time to run: deSelect prechosen tile
                if (_preChosenTile != -1)
                {
                 allTiles[_preChosenTile].GetComponent<TileScript>().deSelect();
                }
                // Selects clicked tile
                allTiles[_chosenTile].GetComponent<TileScript>().Select();
                _preChosenTile = _chosenTile;
            }
        
        }
    }

    /// <summary>Sends All tiles to python server. Contains: a cost for each tile in correct order</summary>
    string SendAllTiles(List<GameObject> TileList)
    {
        // returnString describes the type of Data sent
        // in this function, 0 is used due to:   0: "Send all tiles" ,  1: "Send selected tile"
        string returnString = "0:";
        for (int i = 0; i < TileList.Count; i++)
        {
            string tempString = "" + TileList[i].GetComponent<TileScript>().tyleCost;

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

