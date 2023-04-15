using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TerrainUtils;

public class GetSetHeightMap : MonoBehaviour
{
    
    Terrain terrain;
    [SerializeField]
    Texture2D heightMapOne;
    [SerializeField]
    Texture2D heightMapTwo;
    [SerializeField]
    ComputeShader computeShader;
    float[,] heightData;
    // Start is called before the first frame update
    void Start()
    {
        terrain = GetComponent<Terrain>();
        SetTerrain(heightMapOne);
    }
    
    void SetTerrain(Texture2D heightMap)
    {
        heightData = new float[terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution];
        //terrain.terrainData.baseMapResolution = 1024;
        for (int i = 0; i < terrain.terrainData.heightmapResolution; i++)
        {
            for (int j = 0; j < terrain.terrainData.heightmapResolution; j++)
            {
                heightData[i, j] = heightMap.GetPixel(i, j).r;
            }
        }
        terrain.terrainData.SetHeights(0, 0, heightData);
    }

    void ModifyHeightMap(Texture2D heightMap)
    {
        Texture2D tempTex = new Texture2D(heightMap.width, heightMap.height, heightMap.graphicsFormat, UnityEngine.Experimental.Rendering.TextureCreationFlags.MipChain);
        CommandBuffer cmd = CommandBufferPool.Get();
        var mainKernel = computeShader.FindKernel("TerrainGen");
        computeShader.GetKernelThreadGroupSizes(mainKernel, out uint xGroupSize, out uint yGroupSize, out uint _zGroupSize);
        cmd.Blit(heightMap, tempTex);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            SetTerrain(heightMapTwo);
        }
    }
}
