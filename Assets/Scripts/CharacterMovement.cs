using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    SpriteRenderer tileColor;

    [Header("State Checks")]
    public bool clicked = false;
    public bool moving = false;

    [Header("Movement")]
    [SerializeField] int movesAvailable = 4;
    [SerializeField] float speed = 5f;

    Vector3 clickedPosition;

    GameObject preventClicking;
    
    PositionController positionController;

    const string canvasName = "Tile Movement Canvas";
    GameObject tileMovementCanvas;

    GameObject buttonMap;
    [SerializeField] GameObject buttonPrefab;

    BoxCollider2D myBodyCollider;

    private void Start()
    {
        preventClicking = GameObject.Find("PreventClicking");
        clickedPosition = gameObject.transform.position;

        tileColor = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();

        positionController = FindObjectOfType<PositionController>();

        tileMovementCanvas = GameObject.Find(canvasName);
        CreateButtonsParent();

        myBodyCollider = gameObject.GetComponent<BoxCollider2D>();

        
    }

    
    private void OnMouseDown()
    {
        //if (buttonMap.transform.childCount > 0) { Destroy(buttonMap); }
        if (myBodyCollider)
        {
            clicked = !clicked;
            positionController.SetCharacter(gameObject.GetComponent<CharacterMovement>());
            
            if (clicked)
            {
                StartCoroutine(InstantiateButtonMap());
            }
            else
            {
                DestroyButtonMap();
            }
        }
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


    //TODO Heavy cleaning and refactoring
    IEnumerator InstantiateButtonMap()
    {
        yield return new WaitForEndOfFrame();
        CreateButtonsParent();

        //starts movesAvailable tiles left of the character, and iterates until movesAvailable tiles right of the character
        for (int xStartPosition = -(movesAvailable); xStartPosition <= movesAvailable; xStartPosition++)
        {
            //TODO add explanation
            for (int yButtonRange = LeftOrRightPlane(xStartPosition, movesAvailable); yButtonRange <= LeftOrRightPlane(xStartPosition, movesAvailable); yButtonRange++)
            {
                //TODO add explanation
                for (int buttonInstance = -yButtonRange; buttonInstance <= yButtonRange; buttonInstance++)
                {
                    if (buttonInstance == 0 && xStartPosition == 0) { continue; }
                    InsantiateButton(buttonMap.transform, buttonPrefab, xStartPosition, buttonInstance);
                }
            }
        }
    }

    private int LeftOrRightPlane(int oldNum, int newNum)
    {
        //if left of the character, instantiate from smallest to greatest <
        if (oldNum < 0)
        {
            return oldNum + newNum;
        }
        //if right of the character, instantiate greatest to smallest >
        else
        {

            return newNum - oldNum;
        }
        //resulting button map shape should look like <>
    }

    void InsantiateButton(Transform buttonMap, GameObject buttonPrefab, int xButtonPosition, int yButtonPosition)
    {
        var button = Instantiate(
        buttonPrefab,
        new Vector3(gameObject.transform.position.x + xButtonPosition, gameObject.transform.position.y + yButtonPosition, 1),
        Quaternion.identity);

        button.transform.SetParent(buttonMap.transform); //change to .transform when normal parent

    }

    void CreateButtonsParent()
    {
        buttonMap = GameObject.Find("Button Map");

        if (!GameObject.Find("Button Map"))
        {
            buttonMap = new GameObject("Button Map");
            //buttonMap.transform.SetParent(tileMovementCanvas.transform);
        }
        buttonMap = GameObject.Find("Button Map");
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
    }

    void Update()
    {
        if (clicked) {
            tileColor.enabled = true;
        }
        else { tileColor.enabled = false; }

        MoveCharacter();

    }
}
