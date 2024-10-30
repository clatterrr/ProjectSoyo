using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GifReader : MonoBehaviour
{
    private Texture2D[] gifFrames; // Frames of the GIF as separate textures
    public float frameDelay = 0.1f; // Time delay between frames

    private Material gifMaterial;
    private Texture2DArray textureArray;
    private int frameIndex = 0;

    void Start()
    {

    }

    private void FixedUpdate()
    {
    }
}
