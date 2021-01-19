using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Sets what character can be moved and what position its going to move to
public class ButtonController : MonoBehaviour
{
    [SerializeField] CharacterMovement previousCharacter;
    [SerializeField] CharacterMovement character;
    [SerializeField] GameObject buttonPrefab;
    bool findObstacles = true;

    public bool inRangeOfObs = false;

    public void SetCharacter(CharacterMovement clickedCharacter)
    {
        if (clickedCharacter != character)
        {
            previousCharacter = character;
        }

        character = clickedCharacter;
    }

    private int LeftOrRightPlane(int oldNum, int newNum)
    {
        //if left of the character, instantiate smallest to greatest <
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

    public void CreateButtonsParent()
    {
        character.buttonMap = new GameObject("Button Map");
    }

    private void InsantiateButton(Transform buttonMap, GameObject buttonPrefab, int xButtonPosition, int yButtonPosition)
    {
        var button = Instantiate(
        buttonPrefab,
        new Vector3(character.transform.position.x + xButtonPosition, character.transform.position.y + yButtonPosition, 2),
        Quaternion.identity);

        button.transform.SetParent(buttonMap.transform);

    }

    public IEnumerator InstantiateButtonMap()
    {
        yield return new WaitUntil(() => findObstacles == true);
        CreateButtonsParent();

        //starts movesAvailable buttons left of the character, and iterates until movesAvailable buttons right of the character
        for (int xStartPosition = -(character.movesAvailable); xStartPosition <= character.movesAvailable; xStartPosition++)
        {
            //for each iteration of the first for loop, start a second for loop which instantiates buttons in a vertical column starting at the negative difference between the xStartPosition and movesAvailable and finishes iteration at the positive difference
            //(Example: xStart = -2, movesAvailable = 4 ; 4 + -2 = 2 ; instantiate five buttons starting at the -2 y position and -2 x position and finish iterating until the 2 y position.)
            for (int buttonInstance = -LeftOrRightPlane(xStartPosition, character.movesAvailable); buttonInstance <= LeftOrRightPlane(xStartPosition, character.movesAvailable); buttonInstance++)
            {
                if(character.transform.GetChild(0).gameObject.GetComponent<DetectObstacles>().obstacles.Count > 0)
                {
                    var detectedObs = character.transform.GetChild(0).gameObject.GetComponent<DetectObstacles>().obstacles;
                    List<Vector3> maxButtonDist = new List<Vector3>();
                    bool skipMaxButton = false;

                    foreach (GameObject obstacle in detectedObs)
                    {
                        maxButtonDist.Add(new Vector3(obstacle.transform.position.x - character.transform.position.x, obstacle.transform.position.y - character.transform.position.y, 2));
                    }


                    //don't instantiate buttons that have the same position as obstacles
                    if (maxButtonDist.Any(maxDist => maxDist == new Vector3(xStartPosition, buttonInstance, 2)))
                    {
                        continue;
                    }

                    //if an obstacle is on the same x as the character, don't instantiate the farthest button away
                    skipMaxButton = CheckYObstaclePosition(buttonInstance, detectedObs, skipMaxButton);

                    if (skipMaxButton)
                    {
                        skipMaxButton = false;
                        continue;
                    }
                }
                InsantiateButton(character.buttonMap.transform, buttonPrefab, xStartPosition, buttonInstance);
            }
        }
    }

    private bool CheckYObstaclePosition(int buttonInstance, List<GameObject> detectedObs, bool skipMaxTile)
    {
        foreach (GameObject obstacle in detectedObs)
        {
            if (character.transform.position.x == obstacle.transform.position.x)
            {
                if (character.transform.position.y > obstacle.transform.position.y)
                {
                    if (buttonInstance == -character.movesAvailable)
                    {
                        skipMaxTile = true;
                    }
                }
                else
                {
                    if (buttonInstance == character.movesAvailable)
                    {
                        skipMaxTile = true;
                    }
                }
            }
        }

        return skipMaxTile;
    }

    public int CalculateDistance(int x1, int x2, int y1, int y2)
    {
        var xDistance = LeftOrRightPlane(x1, x2);
        var yDistance = LeftOrRightPlane(y1, y2);
        return Mathf.Abs(xDistance) + Mathf.Abs(yDistance);
    }

    public void TargetPosition()
    {
        character.SetClickedPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3 clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (transform.position != character.SnapToGrid(clickedPosition)) { character.SetMoving(); }

        StartCoroutine(character.ToggleClicked());
    }

    public void DestroyButtonMap()
    {
        Destroy(character.buttonMap);
    }

    private void Update()
    {
        if (character && previousCharacter)
        {
            if (character.clicked == true)
            {
                previousCharacter.DestroyButtonMap();
                previousCharacter.SetClicked(false);
            }
        }
    }

}
