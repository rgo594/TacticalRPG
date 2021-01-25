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
        //var detectedObs = character.transform.GetChild(0).gameObject.GetComponent<DetectObstacles>().obstacles;

        List<Obstacles> detectedObstacles;

        List<Obstacles> obstacleRow;

        //When obstacles are blocking, the button instance range will start at this number
        int negRevisedButtonInstanceStartPosition;

        int xObstacleCharacterDistance;
        bool obstaclesPresent;
        bool obstacleRowBlockingPath = false;

        SetObstacleVariables(out detectedObstacles, out negRevisedButtonInstanceStartPosition, out xObstacleCharacterDistance, out obstaclesPresent);
        CreateButtonsParent();

        if(detectedObstacles.Any(obstacle => obstacle.transform.position.x == character.transform.position.x)) 
        { obstacleRowBlockingPath = true; } 
        else
        { obstacleRowBlockingPath = false; }

        //starts movesAvailable buttons left of the character, and iterates until movesAvailable buttons right of the character
        for (int xStartPosition = -(character.movesAvailable); xStartPosition <= character.movesAvailable; xStartPosition++)
        {
            if (obstaclesPresent && obstacleRowBlockingPath)
            {
                //The difference between xStartPosition and xObstacleCharacterDistance offsets the xStartPosition x value to line up with the starting obstacle's x value in the obstacle row
                if (xStartPosition + xObstacleCharacterDistance > detectedObstacles[4].transform.position.x - 1)
                {
                    //The revisedButtonInstanceStartPosition, is calculated by subtracting movesAvailable(Maximum buttonInstance value) by the distance between the starting obstacle and the characters x values
                    //line 129 then stops instantiation if the current buttonInstance is less than or equal to the -revisedButtonInstanceStartPosition (if the obstacle row was above the character it would be greater than and positive revisedButtonInstanceStartPosition)
                    //then for each subsequent x position along the obstacle row, buttonInstance starts 1 greater than the previous button column 
                    //Example: distance between character and obstacle = 2; movesAvailable = 6; revisedButtonInstanceStartPosition = 6 - 2; buttonInstance = -6; if (buttonInstance <= -revisedButtonInstanceStartPosition) stop button instantiation
                    negRevisedButtonInstanceStartPosition--;
                }
            }

            //for each iteration of the first for loop, start a second for loop which instantiates buttons in a vertical column starting at the negative difference between the xStartPosition and movesAvailable and finishes iteration at the positive difference
            //(Example: xStart = -2, movesAvailable = 4 ; 4 + -2 = 2 ; instantiate five buttons starting at the -2 y position and -2 x position and finish iterating until the 2 y position.)
            for (int buttonInstance = -LeftOrRightPlane(xStartPosition, character.movesAvailable); buttonInstance <= LeftOrRightPlane(xStartPosition, character.movesAvailable); buttonInstance++)
            {
                if (obstaclesPresent)
                {
                    var detectedObs = character.transform.GetChild(0).gameObject.GetComponent<DetectObstacles>().obstacles;

                    List<Vector3> maxButtonDist = new List<Vector3>();
                    bool singleObstacleButtonAbortions = false;

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
                    singleObstacleButtonAbortions = SingleObstacleButtonAbortion(buttonInstance, detectedObs, singleObstacleButtonAbortions, xStartPosition);

                    if (singleObstacleButtonAbortions)
                    {
                        singleObstacleButtonAbortions = false;
                        continue;
                    }

                    if (buttonInstance <= -negRevisedButtonInstanceStartPosition && obstacleRowBlockingPath && negRevisedButtonInstanceStartPosition > 0)
                    {
                        continue;
                    }
                }

                if (xStartPosition == 0 && buttonInstance == 0)
                {
                    continue;
                }

                InsantiateButton(character.buttonMap.transform, buttonPrefab, xStartPosition, buttonInstance);
            }
        }
    }

    private void SetObstacleVariables(out List<Obstacles> detectedObstacles, out int negRevisedButtonInstanceStartPosition, out int xObstacleCharacterDistance, out bool obstaclesPresent)
    {
        if (character.transform.GetChild(0).gameObject.GetComponent<DetectObstacles>().obstacles.Count > 0)
        {
            obstaclesPresent = true;
            detectedObstacles = character.transform.GetChild(0).gameObject.GetComponent<DetectObstacles>().obstacles;

            xObstacleCharacterDistance = (int)Vector3.Distance
                (new Vector3(character.transform.position.x, 0),
                new Vector3(detectedObstacles[4].transform.position.x, 0));

            
            negRevisedButtonInstanceStartPosition = character.movesAvailable - xObstacleCharacterDistance  ;
        }
        else
        {
            obstaclesPresent = false;
            detectedObstacles = new List<Obstacles>();

            xObstacleCharacterDistance = 0;
            negRevisedButtonInstanceStartPosition = 0;
        }
    }

    private bool SingleObstacleButtonAbortion(int buttonInstance, List<Obstacles> detectedObs, bool skipMaxTile, int xStartPosition)
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