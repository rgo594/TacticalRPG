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
   
    private void Start()
    {

    }

    public void SetCharacter(CharacterMovement clickedCharacter)
    {
        if (clickedCharacter != character)
        {
            previousCharacter = character;
        }

        character = clickedCharacter;
    }

    public IEnumerator InstantiateButtonMap()
    {
        yield return new WaitUntil(() => findObstacles == true);
        CreateButtonsParent();

        //starts movesAvailable tiles left of the character, and iterates until movesAvailable tiles right of the character
        for (int xStartPosition = -(character.movesAvailable); xStartPosition <= character.movesAvailable; xStartPosition++)
        {
            for (int buttonInstance = -LeftOrRightPlane(xStartPosition, character.movesAvailable); buttonInstance <= LeftOrRightPlane(xStartPosition, character.movesAvailable); buttonInstance++) //for (int buttonInstance = -yButtonRange; buttonInstance <= yButtonRange; buttonInstance++)
            {
                //TODO currently only works with one obstacle, implement an any statement on line 54
                if(character.transform.GetChild(0).gameObject.GetComponent<DetectObstacles>().obstacles.Count > 0)
                {
                    var detectedObs = character.transform.GetChild(0).gameObject.GetComponent<DetectObstacles>().obstacles;
                    int yMaxTileObsDist = 0;
                    int xMaxTileObsDist = 0;

                    foreach(GameObject obstacle in detectedObs)
                    {
                        yMaxTileObsDist = LeftOrRightPlane((int)obstacle.transform.position.y, (int)character.transform.position.y);
                        xMaxTileObsDist = LeftOrRightPlane((int)obstacle.transform.position.x, (int)character.transform.position.x);
                    }
                    //Debug.Log(yMaxTileObsDist);
                    if (buttonInstance == -yMaxTileObsDist && -xMaxTileObsDist == xStartPosition) 
                    {
                        continue;
                    }
                }

                InsantiateButton(character.buttonMap.transform, buttonPrefab, xStartPosition, buttonInstance);

            }
        }
    }

    public int CalculateDistance(int x1, int x2, int y1, int y2)
    {
        var xDistance = LeftOrRightPlane(x1, x2);
        var yDistance = LeftOrRightPlane(y1, y2);
        return Mathf.Abs(xDistance) + Mathf.Abs(yDistance);
        //return xDistance + yDistance;
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

    private void InsantiateButton(Transform buttonMap, GameObject buttonPrefab, int xButtonPosition, int yButtonPosition)
    {
        var button = Instantiate(
        buttonPrefab,
        new Vector3(character.transform.position.x + xButtonPosition, character.transform.position.y + yButtonPosition, 2),
        Quaternion.identity);

        button.transform.SetParent(buttonMap.transform);

    }

    public void CreateButtonsParent()
    {
        character.buttonMap = new GameObject("Button Map");
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
        //Debug.Log(character.name + detectObstacles);
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
