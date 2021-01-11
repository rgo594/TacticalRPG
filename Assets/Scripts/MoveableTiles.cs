using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableTiles : MonoBehaviour
{
    Vector3 originalPosition;
    [SerializeField] float tilesMoved = 0;

    private void Start()
    {
        originalPosition = gameObject.transform.position;
    }

    public float OriginalOrNewPosition(float newPos, float originalPos)
    {
        if (Mathf.Abs(newPos) > Mathf.Abs(originalPos))
        {
            return Mathf.Abs(newPos) - Mathf.Abs(originalPos);
        }
        else
        {
            return Mathf.Abs(originalPos) - Mathf.Abs(newPos);
        }
    }

    private void Update()
    {

        if (originalPosition != gameObject.transform.position)
        {

/*            while (tilesMoved < OriginalOrNewPosition(gameObject.transform.position.y, originalPosition.y))
            {

                tilesMoved++;
            }*/

            /*while (tilesMoved < OriginalOrNewPosition(gameObject.transform.position.y, originalPosition.y) && gameObject.transform.position.y != originalPosition.y)
            {
                tilesMoved++;
                }
                else
                {
                    tilesMoved++;
                }
                
            }*/
        }
    }
}


/*            Debug.Log("new transform is " + gameObject.transform.position.x);
            Debug.Log("og transform is " + originalPosition.x);
            Debug.Log(gameObject.transform.position.x - originalPosition.x);*/