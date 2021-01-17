using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    [SerializeField] bool left = false;
    [SerializeField] bool right = false;

    GameObject myObj;
    ButtonController buttonController;

    CharacterMovement character;

    // Start is called before the first frame update
    void Start()
    {
        myObj = gameObject;
        buttonController = FindObjectOfType<ButtonController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*    private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Button>())
            {
                var obstacleCollider = myObj.GetComponent<BoxCollider2D>();
                obstacleCollider.size = new Vector3(1, 6, 2);
                Destroy(collision.gameObject);
            }
        }*/

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
    }

/*    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Button>())
        {
            Destroy(collision.gameObject);
        }
    }*/

    IEnumerator DestroyButtons(GameObject button)
    {
        yield return new WaitForEndOfFrame();
        Destroy(button);
    }

}
