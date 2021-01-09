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
        StartCoroutine(ToggleClicked());
    }

    IEnumerator ToggleClicked()
    {
        //prevents accidentally calling targetPosition() and OnMouseDown() at the same time
        yield return new WaitForEndOfFrame();
        clicked = !clicked;
    }


    private void TargetPosition()
    {
        //converts where you clicked into transform values
        if (Input.GetMouseButtonDown(0))
        {

            clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //preventClicking.GetComponent<BoxCollider2D>().enabled = true;
            if (transform.position != SnapToGrid(clickedPosition)) { moving = !moving; }
            StartCoroutine(ToggleClicked());
        }
    }

    private void MoveToTargetPosition()
    {
        //TODO if you click on a character while another is moving, the moving character will stop
        foreach (CharacterMovement character in characters)
        {
            //filters out currently clicked on character from the characters array
            if (character.GetComponent<CharacterMovement>() != gameObject.GetComponent<CharacterMovement>())
            {
                //checks to see if another character is on the tile you clicked on
                if (character.transform.position == SnapToGrid(clickedPosition))
                {
                    //resets currently clicked on character, so you won't be able to have two characters selected at once
                    clickedPosition = SnapToGrid(gameObject.transform.position);
                    clicked = !clicked;
                    moving = false;
                }
                else
                {
                    MoveCharacter();
                }
            }
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
            //StartCoroutine(ToggleClicked());
            moving = !moving;
        }
    }

    void Update()
    {
        if (clicked)
        {
            //target something
            bodyColor.color = Color.red;
            TargetPosition();
        }
        else
        {

            bodyColor.color = Color.white;
        }
        if (clickedPosition != SnapToGrid(gameObject.transform.position))
        {
            MoveToTargetPosition();
        }
    }
}
