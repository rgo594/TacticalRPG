using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DetectObstacles : MonoBehaviour
{
    public List<Obstacles> obstacles;
    public List<Obstacles> sortedObstacles;
    public List<Obstacles> obstacleRowblockage;
    public Obstacles centerObstacle;
    bool sortingFinished = true;
    bool coo = true;

    IEnumerable<Obstacles> query;

    CharacterMovement characterMovement;
    ButtonController buttonController;

    private void Start()
    {
        characterMovement = gameObject.GetComponentInParent(typeof(CharacterMovement)) as CharacterMovement;
        buttonController = FindObjectOfType<ButtonController>();
        //Debug.Log(gameObject.transform.parent.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Obstacles>())
        {
            obstacles.Add(collision.GetComponent<Obstacles>());
            query = obstacles.OrderBy(obstacle => obstacle.transform.position.x);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach(Obstacles obstacle in obstacles)
        {
            if (collision.GetComponent<Obstacles>() == obstacle)
            {
                obstacles.Remove(obstacle);
            }
        }
        sortingFinished = false;
    }

    private void SortObs()
    {
        foreach (Obstacles obstacle in query)
        {
            sortedObstacles.Add(obstacle);
        }
        //yield return new WaitForEndOfFrame();
        //need to reset somewhere
        sortingFinished = false;
    }

    private void Update()
    {
        if (obstacles.Count > 0)
        {

            foreach (Obstacles obstacle in obstacles)
            {
                if (obstacle.transform.position.x == gameObject.transform.parent.transform.position.x)
                {
                    centerObstacle = obstacle.GetComponent<Obstacles>();
                }
            }

        }
        if(sortingFinished && obstacles.Count > 0)
        {
            SortObs();
        }

        if (centerObstacle && coo)
        {
            for (int i = 0; i < sortedObstacles.Count; i++)
            {
                //Debug.Log($"{i}: ob: {(int)obstacles[i].transform.position.x} centerObs: {(int)centerObstacle.transform.position.x + i} {(int)obstacles[i].transform.position.x == (int)centerObstacle.transform.position.x + i}");
                if ((int)sortedObstacles[i].transform.position.x == (int)centerObstacle.transform.position.x + i)
                {
                    obstacleRowblockage.Add(sortedObstacles[i]);
                }
                //need to reset somewhere
                coo = false;
            }

        }

    }
}
