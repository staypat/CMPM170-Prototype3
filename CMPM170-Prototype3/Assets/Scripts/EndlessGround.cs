using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessGround : MonoBehaviour
{
    public float tileLength = 5f;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z + tileLength < player.position.z) {
            // Reposition the tile to the front
            transform.position += Vector3.forward * tileLength * 2;
        }
    }
}
