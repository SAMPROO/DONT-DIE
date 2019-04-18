using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivingBoard : MonoBehaviour
{
    private Vector3[] verticies;
    
    // Start is called before the first frame update
    void Start()
    {
        verticies = GetComponent<MeshFilter>().mesh.vertices;
        
        for (int i = 0; i < verticies.Length; i++)
        {
            verticies[i].y = 100;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
