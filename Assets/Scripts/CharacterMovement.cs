using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    int instanceCounter;
    SpriteRenderer tileColor;

    [Header("State Checks")]
    bool clicked = false;
    bool moving = false;

    Vector3 clickedPosition;
    [SerializeField] float speed = 5f;
 

    [SerializeField] CharacterMovement[] characters;
    GameObject preventClicking;
    
    PositionController positionController;

    const string canvasName = "Tile Movement Canvas";
    GameObject tileMovementCanvas;

    GameObject buttonMap;
    [SerializeField] GameObject buttonPrefab;

    BoxCollider2D myBodyCollider;

    FindPositionController testCollider;

    private void Start()
    {
        preventClicking = GameObject.Find("PreventClicking");
        clickedPosition = gameObject.transform.position;
        tileColor = gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
        characters = FindObjectsOfType<CharacterMovement>();
        positionController = FindObjectOfType<PositionController>();

        tileMovementCanvas = GameObject.Find(canvasName);


        CreateButtonsParent();

        myBodyCollider = gameObject.GetComponent<BoxCollider2D>();

        testCollider = FindObjectOfType<FindPositionController>();
    }

    private void OnMouseDown()
    {
        Destroy(GameObject.Find("Button Map"));
        clicked = !clicked;
        positionController.SetCharacter(gameObject.GetComponent<CharacterMovement>());
        StartCoroutine(InstantiateButtonMap());
    }


    //TODO Heavy cleaning and refactoring
    IEnumerator InstantiateButtonMap()
    {
        yield return new WaitForEndOfFrame();
        CreateButtonsParent();
        int straightTilesLength = 4;

        for (int tile = -straightTilesLength; tile <= straightTilesLength; tile++)
        {
            if (tile == 0) { continue; }
            //creates horizontal buttons
            InsantiateButtons(buttonMap, buttonPrefab, tile, 0);
            //creates vertical buttons
            InsantiateButtons(buttonMap, buttonPrefab, 0, tile);
        }
        for (int tileInstances = -(straightTilesLength - 1); tileInstances < straightTilesLength; tileInstances++)
        {
          for (int incLength = ReturnGreater(tileInstances, straightTilesLength); incLength <= ReturnGreater(tileInstances, straightTilesLength); incLength++)
            { 
              for (int i = -incLength; i <= incLength; i++)
                {
                    if (i == 0|| tileInstances == 0) { continue; }
                    InsantiateButtons(buttonMap, buttonPrefab, tileInstances, i);
                    instanceCounter++;
                    Debug.Log(instanceCounter);
                }
            }
        }
    }

    private int ReturnGreater(int oldNum, int newNum)
    {
        if (oldNum < 0)
        {
            return oldNum + newNum;
        }
        else
        {
            return newNum - oldNum;
        }
    }

    void InsantiateButtons(GameObject buttonMap, GameObject buttonPrefab, int xButtonPosition, int yButtonPosition)
    { 
        var button = Instantiate(
        buttonPrefab,
        new Vector3(gameObject.transform.position.x + xButtonPosition, gameObject.transform.position.y + yButtonPosition),
        Quaternion.identity);

        button.transform.SetParent(buttonMap.transform);
    }

    void CreateButtonsParent()
    {
        buttonMap = GameObject.Find("Button Map");

        if (!GameObject.Find("Button Map"))
        {
            buttonMap = new GameObject("Button Map");
            buttonMap.transform.SetParent(tileMovementCanvas.transform);
        }
    }

    public IEnumerator ToggleClicked()
    {
        clicked = true;
        yield return new WaitUntil(() => moving == true);
        clicked = false;
    }

    public Vector3 SnapToGrid(Vector3 rawWorldPos)
    {
        float newX = Mathf.RoundToInt(rawWorldPos.x);
        float newY = Mathf.RoundToInt(rawWorldPos.y);
        return new Vector3(newX, newY);
    }

    public void SetMoving()
    {
        moving = !moving;
    }

    public void SetClickedPosition(Vector3 position)
    {
        clickedPosition = position;
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
                    //Destroy(GameObject.Find("Button Map"));
                    clickedPosition = SnapToGrid(gameObject.transform.position);
                    clicked = !clicked;
                    moving = false;
                    preventClicking.GetComponent<BoxCollider2D>().enabled = false;
                    //Destroy(GameObject.Find("Button Map"));
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
        if (clicked) { tileColor.enabled = true; } else { tileColor.enabled = false; }
        MoveToTargetPosition();

        if(myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Obstacles")))
        {
            Debug.Log("works");
        }

/*        if (testCollider.IsTouchingLayers(LayerMask.GetMask("Characters")))
        {

        }*/
    }
}

