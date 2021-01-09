using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private Vector3 clickedPosition;
    SpriteRenderer bodyColor;

    [SerializeField] float speed = 5f;

    [Header("State Checks")]
    public bool clicked = false;
    public bool moving = false;

    [SerializeField] CharacterMovement[] characters;
    GameObject preventClicking;


    private void Start()
    {
        Debug.Log(GameObject.Find("PreventClicking"));

        preventClicking = GameObject.Find("PreventClicking");

        clickedPosition = gameObject.transform.position;
        bodyColor = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        characters = FindObjectsOfType<CharacterMovement>();
    }

    private void OnMouseDown()
    {
        clicked = !clicked;
    }

    IEnumerator ToggleClicked()
    {
        clicked = true;
        yield return new WaitUntil(() => moving == true);
        clicked = false;
    }

    private Vector3 SnapToGrid(Vector3 rawWorldPos)
    {
        float newX = Mathf.RoundToInt(rawWorldPos.x);
        float newY = Mathf.RoundToInt(rawWorldPos.y);
        return new Vector3(newX, newY);
    }

    private void TargetPosition()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //converts where you clicked into transform values
            clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (transform.position != SnapToGrid(clickedPosition)) { moving = !moving; }

            StartCoroutine(ToggleClicked());
        }
    }

    private void MoveCharacter()
    {
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

            //if the character is in the moving state while on the same tile as the clicked position turn off moving state
            if (moving)
            {
            preventClicking.GetComponent<BoxCollider2D>().enabled = true;
                if (SnapToGrid(clickedPosition) == gameObject.transform.position)
                {
                    preventClicking.GetComponent<BoxCollider2D>().enabled = false;
                    moving = !moving;
                }
            }
        }
    }

    private void MoveToTargetPosition()
    {
        foreach (CharacterMovement character in characters)
        {
           
            if (character.name != gameObject.name)
            {
                //checks to see if another character is on the tile you clicked on
                if (character.transform.position == SnapToGrid(clickedPosition))
                {
                    //resets currently clicked on character, so you won't be able to have two characters selected at once
                    clickedPosition = SnapToGrid(gameObject.transform.position);
                    clicked = !clicked;
                    moving = false;
                    preventClicking.GetComponent<BoxCollider2D>().enabled = false;
                }
                else
                {
                    MoveCharacter();
                }
            }
        }
    }

    void Update()
    {
        if (clicked)
        {
            bodyColor.color = Color.red;
            TargetPosition();
        }
        else
        {
            bodyColor.color = Color.white;
        }
        MoveToTargetPosition();
    }
}