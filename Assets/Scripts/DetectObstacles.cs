using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectObstacles : MonoBehaviour
{
    public List<GameObject> obstacles;
    CharacterMovement characterMovement;
    ButtonController buttonController;

    private void Start()
    {
        characterMovement = gameObject.GetComponentInParent(typeof(CharacterMovement)) as CharacterMovement;
        buttonController = FindObjectOfType<ButtonController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Obstacles>())
        {
            obstacles.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach(GameObject obstacle in obstacles)
        {
            if (collision.gameObject == obstacle)
            {
                obstacles.Remove(obstacle);
            }
        }
    }
}
