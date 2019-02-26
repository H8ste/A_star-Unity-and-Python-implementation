using UnityEngine;
using System.Collections;

public class TileScript : MonoBehaviour
{
    [HideInInspector]

    public enum ColorTypes
    {
        Grass,
        Dirt,
        Gravel,
        Lava,
        Water,
        Selected
    }
    public Material Water, Lava, Grass, Selected, Gravel, Dirt;
    public ColorTypes tileType = 0;
    public int tyleCost = 0;


    // Use this for initialization
    void Start()
    {
        ;

        // rend.material.shader = Shader.Find("_Color");


    }
    
    public void CalculateTypeCost()
    {
        Renderer rend = GetComponent<Renderer>();
        switch (tileType)
        {
            case ColorTypes.Grass:
                {
                    rend.material = Grass;
                    tyleCost = 10; break;
                }
            case ColorTypes.Dirt:
                {
                    rend.material = Dirt;
                    tyleCost = 5; break;
                }
            case ColorTypes.Gravel:
                {
                    rend.material = Gravel;
                    tyleCost = 1; break;
                }
            case ColorTypes.Lava:
                {
                    rend.material = Lava;
                    tyleCost = 10000; break;
                }
            case ColorTypes.Water:
                {
                    rend.material = Water;
                    tyleCost = 20; break;
                }
            case ColorTypes.Selected:
                {
                    rend.material = Selected; break;
                }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}