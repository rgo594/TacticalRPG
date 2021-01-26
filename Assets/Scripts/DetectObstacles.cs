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

    bool sortingFinished = false;
    bool obstacleRowPresent = false;
    bool obstacleRowPresentLeft = false;

    IEnumerable<Obstacles> query;

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
            obstacles.Add(collision.GetComponent<Obstacles>());
            obstacleRowPresent = false;
            obstacleRowPresentLeft = false;
        }
        query = obstacles.OrderBy(obstacle => obstacle.transform.position.x);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (Obstacles obstacle in obstacles)
        {
            if (collision.GetComponent<Obstacles>() == obstacle)
            {
                obstacles.Remove(obstacle);
            }
        }
    }

    private void SortObs()
    {
        foreach (Obstacles obstacle in query)
        {
            sortedObstacles.Add(obstacle);
        }
        sortingFinished = true;
    }

    private void Update()
    {
        if (obstacles.Count > 0)
        {
            foreach (Obstacles obstacle in obstacles)
            {
                if (obstacle.transform.position.x == gameObject.transform.parent.transform.position.x)
                {
                    if(centerObstacle)
                    {
                        if(centerObstacle.transform.position.x != gameObject.transform.parent.transform.position.x)
                        {
                            obstacleRowblockage.Clear();
                            obstacleRowPresent = false;
                            obstacleRowPresentLeft = false;
                            centerObstacle = obstacle.GetComponent<Obstacles>();
                        }
                    }
                    else
                    {
                        centerObstacle = obstacle.GetComponent<Obstacles>();
                    }
                }
            }
            //need to clear out obstacleRows if not on an obstacle
        }
        if (sortingFinished == false && obstacles.Count > 0)
        {
            SortObs();
        }

        if (centerObstacle && obstacleRowPresent == false)
        {
            for (int i = 0; i < sortedObstacles.Count; i++)
            { 
                if ((int)sortedObstacles[i + (int)centerObstacle.transform.position.x].transform.position.x == (int)centerObstacle.transform.position.x + i)
                {
                    obstacleRowblockage.Add(sortedObstacles[i + (int)centerObstacle.transform.position.x]);
                }
                else
                {
                    break;
                }
                obstacleRowPresent = true;
            }

        }
        if (centerObstacle && obstacleRowPresentLeft == false)
        {
            for (int i = 0; i < sortedObstacles.Count; i++)
            {
                if ((int)sortedObstacles[(int)centerObstacle.transform.position.x - i].transform.position.x == (int)centerObstacle.transform.position.x)
                {
                    continue;
                }
                else if ((int)sortedObstacles[(int)centerObstacle.transform.position.x - i].transform.position.x == (int)centerObstacle.transform.position.x - i)
                {
                    obstacleRowblockage.Add(sortedObstacles[(int)centerObstacle.transform.position.x - i]);
                }
                else
                {
                    break;
                }
                obstacleRowPresentLeft = true;
            }
        }
    }
}
