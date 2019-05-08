using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

    [Range(-5, 5)]
    public float offsetMultiplier;
    Mesh mesh;
    MeshFilter meshF;
    MeshCollider meshC;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    public bool animate;

    public int xSize;
    public int zSize;
    [Range(0, 1)]
    public float noiseCeiling;
    [Range(0, 1)]
    public float noiseFloor;

    public float noiseSmooth;
    public float noiseIntensity;

    public int terrainScale = 1;
    [SerializeField] bool drawGizmos;
    float offset = 0;

    int frames;

    // Use this for initialization
    void Start()
    {
        if (offsetMultiplier == 0) offsetMultiplier = 1;
        if (noiseCeiling <= 0) noiseCeiling = 1;
        mesh = new Mesh();
        CreateShape();
        meshF = GetComponent<MeshFilter>();
        meshF.mesh = mesh;
        //meshC = GetComponent<MeshCollider>();
        //meshC.sharedMesh = mesh;


    }
    void FixedUpdate()
    {

        if (animate)
        {

            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {

                    float y = Mathf.PerlinNoise(x * noiseSmooth + offset, z * noiseSmooth + offset) * noiseIntensity;
                    if (y < noiseFloor * noiseIntensity) y = noiseFloor;
                    if (y > noiseCeiling * noiseIntensity) y = noiseCeiling;

                    vertices[i] = new Vector3(vertices[i].x, y, vertices[i].z);

                    i++;
                }
            }
            offset += 0.01f * offsetMultiplier;

            meshF.mesh.vertices = vertices;
            meshF.mesh.RecalculateNormals();

            frames++;

            if (frames == 20)
            {
                //meshC.sharedMesh = null;
                //meshC.sharedMesh = meshF.mesh;
                frames = 0;
            }

        }
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                int x2 = x * terrainScale;
                int z2 = z * terrainScale;

                float y = Mathf.PerlinNoise(x * noiseSmooth, z * noiseSmooth) * noiseIntensity;
                if (y < noiseFloor * noiseIntensity) y = noiseFloor;
                if (y > noiseCeiling * noiseIntensity) y = noiseCeiling;

                vertices[i] = new Vector3(x2, y, z2);

                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {

                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        uvs = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
                i++;
            }
        }

        UpdateMesh();

    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

    }

    private void OnDrawGizmos()
    {
        if (vertices == null || !drawGizmos) return;


        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}