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

/*    Obstacles leftObs;
    Obstacles rightObs;*/

    int leftOfLeft = 0;

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

    private Obstacles LeftObs()
    {
        var detectedObs = character.transform.GetChild(0).gameObject.GetComponent<DetectObstacles>().obstacles;

        foreach (Obstacles obstacle in detectedObs)
        {
            if (obstacle.left != true && obstacle.right == true)
            {
                return obstacle;
            }
        }
        return new Obstacles();

    }   
    private Obstacles RightObs()
    {
        var detectedObs = character.transform.GetChild(0).gameObject.GetComponent<DetectObstacles>().obstacles;

        foreach (Obstacles obstacle in detectedObs)
        {
            if (obstacle.left == true && obstacle.right != true)
            {
                return obstacle;
            }
        }
        return new Obstacles();
    }

    public IEnumerator InstantiateButtonMap()
    {
        Obstacles leftObs = LeftObs();
        Obstacles rightObs = RightObs();

        yield return new WaitUntil(() => findObstacles == true);
        CreateButtonsParent();

        //starts movesAvailable buttons left of the character, and iterates until movesAvailable buttons right of the character
        for (int xStartPosition = -(character.movesAvailable); xStartPosition <= character.movesAvailable; xStartPosition++)
        {
            int lastButtonInstance = -LeftOrRightPlane(xStartPosition, character.movesAvailable);

            //for each iteration of the first for loop, start a second for loop which instantiates buttons in a vertical column starting at the negative difference between the xStartPosition and movesAvailable and finishes iteration at the positive difference
            //(Example: xStart = -2, movesAvailable = 4 ; 4 + -2 = 2 ; instantiate five buttons starting at the -2 y position and -2 x position and finish iterating until the 2 y position.)
            for (int buttonInstance = -LeftOrRightPlane(xStartPosition, character.movesAvailable); buttonInstance <= LeftOrRightPlane(xStartPosition, character.movesAvailable); buttonInstance++)
            {
                if (character.transform.GetChild(0).gameObject.GetComponent<DetectObstacles>().obstacles.Count > 0)
                {
                    var detectedObs = character.transform.GetChild(0).gameObject.GetComponent<DetectObstacles>().obstacles;

                    List<Vector3> maxButtonDist = new List<Vector3>();
                    bool skipMaxButton = false;

                    foreach (Obstacles obstacle in detectedObs)
                    {
                        maxButtonDist.Add(new Vector3(obstacle.transform.position.x - character.transform.position.x, obstacle.transform.position.y - character.transform.position.y, 2));
                    }


                    //don't instantiate buttons that have the same position as obstacles
                    if (maxButtonDist.Any(maxDist => maxDist == new Vector3(xStartPosition, buttonInstance, 2)))
                    {
                        continue;
                    }

                    //if an obstacle is on the same x or y coordinate as the character, don't instantiate the farthest button away on the opposite coordinate (same x, y button column has one less, same y, x row has one less button)
                    skipMaxButton = CheckYObstaclePosition(buttonInstance, detectedObs, skipMaxButton, xStartPosition);

                    if (skipMaxButton)
                    {
                        skipMaxButton = false;
                        continue;
                    }



                    bool mur = false;

                    for (int i = buttonInstance; i < lastButtonInstance + 2; i++)
                    {
                        if (xStartPosition > -1 && lastButtonInstance < i)
                        {
                            buttonInstance = lastButtonInstance + 2;
                            //mur = true;
                            //Debug.Log(lastButtonInstance);
                        }
                        else
                        {
                            mur = false;
                        }
                    }

                    if (mur)
                    {
                        continue;
                    }

                    //int coo;

                    //int coo = something(leftObs, rightObs, xStartPosition, buttonInstance);
                    /*
                                        if (buttonInstance > coo)
                                        {
                                            continue;
                                        }*/

                }
                if (xStartPosition == 0 && buttonInstance == 0)
                {
                    continue;
                }

                InsantiateButton(character.buttonMap.transform, buttonPrefab, xStartPosition, buttonInstance);
            }
        }
    }

    private bool CheckYObstaclePosition(int buttonInstance, List<Obstacles> detectedObs, bool skipMaxTile, int xStartPosition)
    {
        foreach (Obstacles obstacle in detectedObs)
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
            else if (character.transform.position.y == obstacle.transform.position.y)
            {
                if (character.transform.position.x > obstacle.transform.position.x)
                {
                    if (xStartPosition == -character.movesAvailable)
                    {
                        skipMaxTile = true;
                    }
                }
                else
                {
                    if (xStartPosition == character.movesAvailable)
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



/*bool mur = false;

for (int i = -4; i < 0; i++)
{
    if (xStartPosition > -1 && buttonInstance < i)
    {
        mur = true;
    }
    else
    {
        mur = false;
    }
}

if (mur)
{
    continue;
}


private int something(Obstacles leftObs, Obstacles rightObs, int xStartPosition, int buttonInstance)
{
    if (leftObs.transform.position.x - character.transform.position.x > character.transform.position.x - rightObs.transform.position.x)
    {
        if (xStartPosition == leftObs.transform.position.x - 1)
        {
            return buttonInstance;
        }
    }
    else
    {
        Debug.Log("right");
    }

    return 1000;
}*/