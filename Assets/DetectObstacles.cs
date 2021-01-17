using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectObstacles : MonoBehaviour
{
    public Obstacles[] obstacles;

    private void Start()
    {
        obstacles = FindObjectsOfType<Obstacles>();
    }
}
