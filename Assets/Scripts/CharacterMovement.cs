using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("State Checks")]
    public bool clicked = false;
    public bool moving = false;

    [Header("Movement")]
    [SerializeField] public int movesAvailable = 4;
    [SerializeField] float speed = 5f;

    public GameObject buttonMap;
    ButtonController buttonController;

    Vector3 clickedPosition;
    GameObject preventClicking;

    SpriteRenderer tileColor;
    BoxCollider2D boxCollider;

  

    private void Start()
    {
        preventClicking = GameObject.Find("PreventClicking");
        clickedPosition = gameObject.transform.position;

        tileColor = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();

        buttonController = FindObjectOfType<ButtonController>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    
    private void OnMouseDown()
    {

        if (boxCollider.GetType() == typeof(BoxCollider2D))
        {
            clicked = !clicked;
            buttonController.SetCharacter(gameObject.GetComponent<CharacterMovement>());

            if (clicked)
            {
                //buttonController.CreateButtonsParent();
                StartCoroutine(buttonController.InstantiateButtonMap());
            }
            else
            {
                buttonController.DestroyButtonMap();
            }
        }
        else if (boxCollider.GetType() == typeof(CircleCollider2D)) { Debug.Log("fuck unity"); }
    }

    public Vector3 SnapToGrid(Vector3 rawWorldPos)
    {
        float newX = Mathf.RoundToInt(rawWorldPos.x);
        float newY = Mathf.RoundToInt(rawWorldPos.y);
        return new Vector3(newX, newY);
    }

    public IEnumerator ToggleClicked()
    {
        clicked = true;
        yield return new WaitUntil(() => moving == true);
        clicked = false;
    }

    public void SetMoving()
    {
        moving = !moving;
    }

    public void SetClicked(bool newClicked)
    {
        clicked = newClicked;
    }
    public void SetClickedPosition(Vector3 position)
    {
        clickedPosition = position;
    }

    public void DestroyButtonMap()
    {
        Destroy(buttonMap);
    }

    IEnumerator MoveCharacter()
    {
        yield return new WaitForEndOfFrame();
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
            Destroy(GameObject.Find("Button Map"));
            //prevent clicking other characters while one is in the moving state
            preventClicking.GetComponent<BoxCollider2D>().enabled = true;
            if (SnapToGrid(clickedPosition) == gameObject.transform.position)
            {
                preventClicking.GetComponent<BoxCollider2D>().enabled = false;
                moving = !moving;
                
            }
        }
    }

    void Update()
    {
        //if (clicked) { tileColor.enabled = true; } else { tileColor.enabled = false; }
        if (transform.position != SnapToGrid(clickedPosition)) { StartCoroutine(MoveCharacter()); }

    }
}