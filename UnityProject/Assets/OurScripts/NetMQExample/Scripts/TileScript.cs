using UnityEngine;
using System.Collections;

public class TileScript : MonoBehaviour
{
    [HideInInspector]
    Renderer _RendTexture;
    Material preMaterial;
    public enum ColorTypes
    {
        Grass,
        Dirt,
        Gravel,
        Lava,
        Water,
    }
    public Material Water, Lava, Grass, Selected, Gravel, Dirt;

    public bool _Selected = false;
    public ColorTypes tileType = 0;
    public int tyleCost = 0;


    // Use this for initialization
    void Start()
    {

        _RendTexture = GetComponent<Renderer>();
        // rend.material.shader = Shader.Find("_Color");


    }
    
    public void CalculateTypeCost()
    {
        _RendTexture = GetComponent<Renderer>();
        switch (tileType)
        {
            case ColorTypes.Grass:
                {
                    _RendTexture.material = Grass;
                    tyleCost = 10; break;
                }
            case ColorTypes.Dirt:
                {
                    _RendTexture.material = Dirt;
                    tyleCost = 5; break;
                }
            case ColorTypes.Gravel:
                {
                    _RendTexture.material = Gravel;
                    tyleCost = 1; break;
                }
            case ColorTypes.Lava:
                {
                    _RendTexture.material = Lava;
                    tyleCost = 10000; break;
                }
            case ColorTypes.Water:
                {
                    _RendTexture.material = Water;
                    tyleCost = 20; break;
                }
        }
    }

    public void Select()
    {
        _RendTexture = GetComponent<Renderer>();
        preMaterial = _RendTexture.material;
        _RendTexture.material = Selected;
    }

      public void deSelect()
    {
        //Only runs after select() has been run ones as to instantiate preMaterial
        _RendTexture = GetComponent<Renderer>();
        _RendTexture.material = preMaterial;
    }


    // Update is called once per frame
    void Update()
    {
       
    }
}