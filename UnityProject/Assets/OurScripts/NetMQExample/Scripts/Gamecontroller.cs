using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Gamecontroller : MonoBehaviour
{
    public GameObject tileRef;
    public GameObject agentRef;
    public List<GameObject> allTiles = new List<GameObject>();
    public List<GameObject> allAgents = new List<GameObject>();
    private int agentCount = 5;
    private ServerHandler _server;
    private int _preChosenTile = -1;
    public float overallSpeed = 4f;

    // Start is called once when the component with Gamecontroller script attached is spawned
    void Start()
    {
        // Instantiate the server used
        _server = new ServerHandler();

        // Creates all the tiles in the game
        for (int z = 0; z < 25; z++)
            for (int x = 0; x < 40; x++)
            {
                // Instantiates a prefab referenced within tileRef (SET IN INSPECTOR) -> Adds created object to allTiles List
                allTiles.Add(Instantiate(tileRef, new Vector3(x, 0, z), Quaternion.identity));
                // Randomly sets the tiletype of the created tile
                allTiles[allTiles.Count - 1].GetComponent<TileScript>().tileType = (TileScript.ColorTypes)UnityEngine.Random.Range(0, 5);
                // Based on its own tiletype, a tyle cost is calculated for each element (tile)
                allTiles[allTiles.Count - 1].GetComponent<TileScript>().CalculateTypeCost();
            }


        // Creates all the agents
        int[] agentPositions = new int[] { -1, -1, -1, -1, -1 };
        for (int i = 0; i < agentCount; i++)
        {
            agentPositions[i] = calculateNewAgentPosition(i, agentPositions);
            allAgents.Add(Instantiate(agentRef, new Vector3(FindCoordByElement(agentPositions[i]).x, 0, FindCoordByElement(agentPositions[i]).y), Quaternion.identity));
        }
        // Instantiates the thread the server will run on
        _server.Start();
        // Begins the run method of the server
        _server.Continue();

        // Sends all the tiles created as a string, stringformat:     (SENDTYPE:ELM1;ELM2;ELM3.....)
        // Returns either true or false based if it was succesful
        if (SendToServer(ServerHandler.ServerState.SendingTILES, SendAllTiles(allTiles)))
        {
            Debug.Log("Sent tiles to server succesfully");
        }
        else
        {
            Debug.Log("Tiles were not sent to server");
        }
    }

    // Checks if the postition of the agent has already been used, 
    // if then it calculates a new position recursively
    int calculateNewAgentPosition(int index, int[] agentPositions)
    {
        // Spawns them in random positions
        int tempXpos = UnityEngine.Random.Range(0, 40);
        int tempZpos = UnityEngine.Random.Range(0, 25);
        int calculatedElement = FindElementByCoord(new Vector3(tempXpos, 0, tempZpos));

        // Checks if the calculated agentPosition has been used before
        // If it has yet been used, it creates a new agent with calculated position
        while (Array.Exists<int>(agentPositions, element => element == calculatedElement))
        {
            print("This position has already been taken, calculating a new position");
            calculatedElement = calculateNewAgentPosition(index, agentPositions);
        }
        print("Found new position");
        return calculatedElement;
    }

    // Update is called once per frame
    void Update()
    {
        // If mouse button is clicked
        if (Input.GetButtonDown("Fire1"))
        {
            // Get mouse postion and check if it is within the index of tiles and clickable
            int _chosenTile = GetPositionClicked();
            if (_chosenTile >= 0 && _chosenTile <= 999 && allTiles[_chosenTile].GetComponent<TileScript>().tileType != TileScript.ColorTypes.Lava)
            {
                // If not first time to run: deSelect prechosen tile
                if (_preChosenTile != -1)
                {
                    allTiles[_preChosenTile].GetComponent<TileScript>().deSelect();
                }
                // Selects clicked tile
                allTiles[_chosenTile].GetComponent<TileScript>().Select();
                _preChosenTile = _chosenTile;

                // Sends all the tiles created as a string, stringformat:     (SENDTYPE:ELM1;ELM2;ELM3.....)
                // Returns either true or false based if it was succesful
                if (SendToServer(ServerHandler.ServerState.SendingCLICKPOS, SendSelectedTileAndAgentsPositions(_chosenTile, allAgents)))
                {
                    Debug.Log("Sent Selected tile and agents to server succesfully");
                }
                else
                {
                    Debug.Log("Selected tile and agents were not sent to server");
                }
            }
        }

        // If the python has sent the paths for each agent to UNITY
        if (_server.allAgentsPath.Count != 0)
        {
            // For each of these agent, walk its respective path
            for (int i = 0; i < _server.allAgentsPath.Count; i++)
            {
                // If the agent has paths to take
                if (_server.allAgentsPath[i].Length > 0)
                {
                    Vector3 current = allAgents[i].GetComponent<Transform>().position;
                    int index = _server.allAgentsPath[i].Length - 1;
                    Vector3 target = new Vector3(_server.allAgentsPath[i][index].x, 0, _server.allAgentsPath[i][index].y);



                    // Check again, after potentially removing an element above,
                    // if agent has paths to take
                    if (_server.allAgentsPath[i].Length > 0)
                    {
                        current = allAgents[i].GetComponent<Transform>().position;
                        index = _server.allAgentsPath[i].Length - 1;
                        target = new Vector3(_server.allAgentsPath[i][index].x, 0, _server.allAgentsPath[i][index].y);

                        // Set speed of moving target to be based on the tile it's moving towards
                        float speed = 0f;
                        switch (allTiles[FindElementByCoord(target)].GetComponent<TileScript>().tileType)
                        {
                            case TileScript.ColorTypes.Dirt:
                                speed = Time.deltaTime * (overallSpeed / 4.0f);
                                break;
                            case TileScript.ColorTypes.Grass:
                                speed = Time.deltaTime * (overallSpeed / 8.0f);
                                break;
                            case TileScript.ColorTypes.Gravel:
                                speed = Time.deltaTime * (overallSpeed / 1.0f);
                                break;
                            case TileScript.ColorTypes.Lava:
                                speed = Time.deltaTime * (overallSpeed / 16.0f);
                                break;
                            case TileScript.ColorTypes.Water:
                                speed = Time.deltaTime * (overallSpeed / 12.0f);
                                break;
                        }

                        // Making the agent move towards the next position in its given path
                        allAgents[i].GetComponent<Transform>().position = Vector3.MoveTowards(current, target, speed);

                        // First check if agent has reached next position in its given path
                        if (Vector3.Magnitude(target - current) == 0)
                        {
                            // Removes the last element of the path (due to the agent already reaching this position)
                            Vector2[] placeHolder = new Vector2[_server.allAgentsPath[i].Length - 1];
                            for (int k = 0; k < placeHolder.Length; k++)
                                placeHolder[k] = _server.allAgentsPath[i][k];

                            _server.allAgentsPath[i] = placeHolder;
                        }
                    }

                }
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

            if (i != TileList.Count - 1)
            {
                tempString = string.Concat(tempString, ";");
            }
            returnString = string.Concat(returnString, tempString);
        }
        return returnString;
    }

    string SendSelectedTileAndAgentsPositions(int chosenTile, List<GameObject> agents)
    {
        int[] agentPosition = new int[5];
        for (int i = 0; i < agentPosition.Length; i++)
        {
            Vector3 agentPos = agents[i].GetComponent<Transform>().position;
            agentPosition[i] = FindElementByCoord(agentPos);
        }
        // returnString describes the type of Data sent
        // in this function, 0 is used due to:   0: "Send all tiles" ,  1: "Send selected tile"
        string returnString = "1:";
        // "1:chosenTile"
        returnString = string.Concat(returnString, chosenTile.ToString());
        // "1:chosenTile,"
        returnString = string.Concat(returnString, ",");
        // "1:chosenTile,0;1;2;3;4"
        for (int i = 0; i < agentPosition.Length; i++)
        {
            string tempString = agentPosition[i].ToString();
            if (i != agentPosition.Length - 1)
            {
                tempString = string.Concat(tempString, ";");
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

    public int FindElementByCoord(Vector3 pos)
    {
        return (((int)pos.z) * 40) + ((int)pos.x);
    }

    public Vector2 FindCoordByElement(int element)
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

