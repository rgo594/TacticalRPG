using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Vector3 clickedPosition;
    SpriteRenderer bodyColor;

    [SerializeField] GameObject quick;

    [SerializeField] float speed = 5f;

    [Header("State Checks")]
    public bool clicked = false;
    public bool moving = false;

    [SerializeField] CharacterMovement[] characters;


    private void Start()
    {
        clickedPosition = gameObject.transform.position;
        bodyColor = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        characters = FindObjectsOfType<CharacterMovement>();
    }

    private void OnMouseDown()
    {
        StartCoroutine(ToggleClicked());
    }

    IEnumerator ToggleClicked()
    {
        //prevents accidentally calling targetPosition() and OnMouseDown() at the same time
        yield return new WaitForEndOfFrame();
        clicked = !clicked;
    }

    private void MoveToTargetPosition()
    {
        //converts where you clicked into transform values
        if (Input.GetMouseButtonDown(0))
        {
            clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (transform.position != SnapToGrid(clickedPosition)){ moving = !moving; }
        }

        if (transform.position != SnapToGrid(clickedPosition))
        {
            //moves player towards the y value of where you clicked
            gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3
                (gameObject.transform.position.x, Mathf.RoundToInt(clickedPosition.y)), speed * Time.deltaTime);

            //moves player to x value of where you clicked, after it finishes moving to the y value
            if (gameObject.transform.position.y == Mathf.RoundToInt(clickedPosition.y))
            {
                gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3
                    (Mathf.RoundToInt(clickedPosition.x), gameObject.transform.position.y), speed * Time.deltaTime);
            }

            if (moving)
            {
                ResetCharacter();
            }
        }
    }

    private Vector3 SnapToGrid(Vector3 rawWorldPos)
    {
        float newX = Mathf.RoundToInt(rawWorldPos.x);
        float newY = Mathf.RoundToInt(rawWorldPos.y);
        return new Vector3(newX, newY);
    }

    private void ResetCharacter()
    {
        //Resets character to preclicked state
        if (Mathf.RoundToInt(clickedPosition.x) == gameObject.transform.position.x &&
                        Mathf.RoundToInt(clickedPosition.y) == gameObject.transform.position.y)
        {
            StartCoroutine(ToggleClicked());
            moving = !moving;
        }
    }

    void Update()
    {
        if (clicked)
        {
            bodyColor.color = Color.red;
            MoveToTargetPosition();
        }
        else
        {
            bodyColor.color = Color.white;
        }
    }
}
