using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseGen : MonoBehaviour
{

    public Vector2 perlinInput;
    public float perlinNoiseVal = 0f;
    public float scale = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        perlinNoiseVal = Mathf.PerlinNoise(perlinInput.x / scale, perlinInput.y / scale);

    }
}
