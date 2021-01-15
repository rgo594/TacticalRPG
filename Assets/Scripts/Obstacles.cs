using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    [SerializeField] bool left = false;
    [SerializeField] bool right = false;

    GameObject myObj;

    // Start is called before the first frame update
    void Start()
    {
        myObj = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Obstacles>())
        {
            if (collision.transform.position.x == (myObj.transform.position.x - 1))
            {
                left = true;
            }

            else if (collision.transform.position.x == (myObj.transform.position.x + 1))
            {
                right = true;
            }
        }

        if(collision.GetComponent<Button>())
        {
            var obstacleCollider = myObj.GetComponent<BoxCollider2D>();
            obstacleCollider.size = new Vector3(1, 6, 2);
            Destroy(collision.gameObject);
            //StartCoroutine(DestroyButtons(collision.gameObject));
/*            if (collision.transform.position == myObj.transform.position)
            {
                var obstacleCollider = myObj.GetComponent<BoxCollider2D>();
                obstacleCollider.size = new Vector3(1, 6, 2);
                StartCoroutine(DestroyButtons(collision.gameObject));
                Debug.Log("works");
                
            }*/
        }
    }

    IEnumerator DestroyButtons(GameObject button)
    {
        yield return new WaitForEndOfFrame();
        Destroy(button);
    }

}
