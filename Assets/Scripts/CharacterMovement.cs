using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Vector3 clickedPosition;
    float speed = 5f;
    public bool clicked = false;
    public bool moving = false;

    private void Start()
    {
        clickedPosition = gameObject.transform.position;
    }

    private void OnMouseDown()
    {
        var bodyColor = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (bodyColor.color != Color.red)
        {
            bodyColor.color = Color.red;
        }
        else
        {
            bodyColor.color = Color.white;
        }

        StartCoroutine(ToggleClicked());
    }

    IEnumerator ToggleClicked()
    {
        //prevents accidentally calling targetPosition() and OnMouseDown() at the same time
        yield return new WaitForEndOfFrame();
        clicked = !clicked;
    }

    IEnumerator waitClick()
    {
        //prevents accidentally calling targetPosition() and OnMouseDown() at the same time
        clicked = !clicked;
        yield return new WaitForFixedUpdate();
    }

    private void targetPosition()
    {
        //converts where you clicked into transform values
        if (Input.GetMouseButtonDown(0))
        {
            clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            moving = true;

        }

        //moves player towards the y value of where you clicked
        gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3
            (gameObject.transform.position.x, Mathf.RoundToInt(clickedPosition.y)), speed * Time.deltaTime);

        //moves player to x value of where you clicked, after it finishes moving to the y value
        if(Mathf.RoundToInt(gameObject.transform.position.y) == Mathf.RoundToInt(clickedPosition.y))
        {
            gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3
                (Mathf.RoundToInt(clickedPosition.x), gameObject.transform.position.y), speed * Time.deltaTime);
        }
        if(moving)
        {
            if(Mathf.RoundToInt(clickedPosition.x) == Mathf.RoundToInt(gameObject.transform.position.x) && Mathf.RoundToInt(clickedPosition.y) == Mathf.RoundToInt(gameObject.transform.position.y))
            {
                Debug.Log("works");
            }
        }
    }

    IEnumerator Example()
    {
        //Debug.Log("Waiting for prince/princess to rescue me...");
        yield return new WaitUntil(() => moving == true);
        Debug.Log("Finally I have been rescued!");
    }


    void Update()
    {
        if (clicked)
        {
            targetPosition();
        }
    }
}
