using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gamecontroller : MonoBehaviour
{
    public GameObject tileRef;
    private List<GameObject> allTiles = new List<GameObject>();


    //Sending DATA to PYTHON <-> Receiving DATA from PYTHON
    private enum ServerState
    {
        Sending,
        Receiving
    }


    //Struct used to send only the server-required KNOWLEDGE
    private struct tileSEND
    {
        public int x, y, tileCost;
    }


    void Start()
    {

        for (int x = 0; x < 40; x++)
            for (int z = 0; z < 25; z++)
            {
                allTiles.Add(Instantiate(tileRef, new Vector3(x, 0, z), Quaternion.identity));
                allTiles[allTiles.Count - 1].GetComponent<TileScript>().tileType = (TileScript.ColorTypes)Random.Range(0, 4);
            }


        StartCoroutine("SendTiles");
    }


    IEnumerator SendTiles()
    {
        int index = 0;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log("Frame is over");
            if (++index == 3) { yield break; }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            if (Physics.Raycast(ray))
            {
                Vector3 postion = ray.origin;
                Debug.Log((int)postion.x + ", " + (int)postion.y + ", " + (int)postion.z + "       :       " + postion.x + ", " + postion.y + ", " + postion.z);
            }

        }
        
    }
}