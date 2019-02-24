using UnityEngine;
using System.Collections;

public class Gamecontroller : MonoBehaviour {
	public GameObject tileRef;
	private ArrayList allTiles = new ArrayList();

	// Use this for initialization
	void Start () {
		for (int x = 0; x < 100; x++)
		{
			for (int z = 0; z < 100; z++)
			{
				
				allTiles.Add(Instantiate(tileRef,new Vector3(x,0,z), Quaternion.identity));
				allTiles.get(allTiles.LastIndexOf());
				
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}