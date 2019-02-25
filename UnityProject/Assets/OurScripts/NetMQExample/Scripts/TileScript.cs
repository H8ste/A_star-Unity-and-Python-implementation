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
        Water
    }
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
                    rend.material.color = new Color(0f / 255f, 128f / 255f, 28f / 255f);
                    tyleCost = 10; break;
                }
            case ColorTypes.Dirt:
                {
                    rend.material.color = new Color(120f / 255f, 72f / 255f, 0f / 255f);
                    tyleCost = 5; break;
                }
            case ColorTypes.Gravel:
                {
                    rend.material.color = new Color(86f / 255f, 82f / 255f, 87f / 255f);
                    tyleCost = 1; break;
                }
            case ColorTypes.Lava:
                {
                    rend.material.color = new Color(81f / 255f, 6f / 255f, 13f / 255f);
                    tyleCost = 10000; break;
                }
            case ColorTypes.Water:
                {
                    rend.material.color = new Color(12f / 255f, 77f / 255f, 105f / 255f);
                    tyleCost = 20; break;
                }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}