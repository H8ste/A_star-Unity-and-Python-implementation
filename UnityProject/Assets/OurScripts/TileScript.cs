using UnityEngine;
using System.Collections;

public class TileScript : MonoBehaviour
{
    [HideInInspector]
    public int tileType;

    private enum ColorTypes
    {
        Grass,
        Dirt,
        Gravel,
        Lava,
        Water
    }

    // Use this for initialization
    void Start()
    {
        
        Renderer rend = GetComponent<Renderer>();
        
        // rend.material.shader = Shader.Find("_Color");
        rend.material.color = Color.black;
    }

    // Update is called once per frame
    void Update()
    {

    }
}