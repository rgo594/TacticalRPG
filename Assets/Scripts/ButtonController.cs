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
    //DetectObstacles detectObstacles;
    bool findObstacles = true;

    public List<Obstacles> detectObstacles;
    //bool hasElements = detectObstacles.Any();

    private void Start()
    {
        detectObstacles = new List<Obstacles>(FindObjectsOfType<Obstacles>());
        //bool hasElements = detectObstacles.Any(i => i.name == "somename"));
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
        //currently always true until obstacle logic is implemented
        yield return new WaitUntil(() => findObstacles == true);
        CreateButtonsParent();

        //starts movesAvailable tiles left of the character, and iterates until movesAvailable tiles right of the character
        for (int xStartPosition = -(character.movesAvailable); xStartPosition <= character.movesAvailable; xStartPosition++)
        {
            for (int buttonInstance = -LeftOrRightPlane(xStartPosition, character.movesAvailable); buttonInstance <= LeftOrRightPlane(xStartPosition, character.movesAvailable); buttonInstance++) //for (int buttonInstance = -yButtonRange; buttonInstance <= yButtonRange; buttonInstance++)
            {
                //if (buttonInstance == 0 && xStartPosition == 0) { continue; }
                //detectObstacles.Any(i => character.transform.position.x - i.transform.position.x == 0)


                if (detectObstacles.Any(obstacle => xStartPosition == obstacle.transform.position.x 
                    && character.transform.position.x == obstacle.transform.position.x 
                    && ObstacleCondition((int)obstacle.transform.position.y, LeftOrRightPlane(xStartPosition, character.movesAvailable)) == buttonInstance))
                { continue; }

                InsantiateButton(character.buttonMap.transform, buttonPrefab, xStartPosition, buttonInstance);

            }
        }
    }
    //ObstacleCondition((int)i.transform.position.y, LeftOrRightPlane(xStartPosition, character.movesAvailable)) == buttonInstance)
    private int ObstacleCondition(int yObstacle, int yMax)
    {
        Debug.Log("ychar - yobs is less than moves available " + (Mathf.Abs(character.transform.position.y) - Mathf.Abs(yObstacle) > character.movesAvailable));
        Debug.Log(yObstacle >= 0 && character.transform.position.y < 0);
        if (Mathf.Abs(character.transform.position.y) - Mathf.Abs(yObstacle) < character.movesAvailable)
        {
            if (yObstacle >= 0 && character.transform.position.y < 0)
            {
            Debug.Log("above or equal" + (yMax - yObstacle));
                return yMax - yObstacle;
}
           else
            {
            Debug.Log("below " + (yMax + yObstacle));
            return yObstacle - yMax;
            }
        }
        else { return 0;  }
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

/*if (detectObstacles.Any(i => character.transform.position.x - i.transform.position.x == 0))
{

}*/

/*if (detectObstacles.Any(i => character.transform.position.x - i.transform.position.x == 0))
{
    int yeet = 500;
    for (int i = 0; i < detectObstacles.Count; i++)
    {
        if (detectObstacles[i].transform.position.y + 1 == buttonInstance) { yeet = buttonInstance; }
    }

    if (yeet == buttonInstance) { continue; }
    InsantiateButton(character.buttonMap.transform, buttonPrefab, xStartPosition, buttonInstance);
}
else
{
    InsantiateButton(character.buttonMap.transform, buttonPrefab, xStartPosition, buttonInstance);
}*/

/*bool coo = false;

for (int obstacle = 0; obstacle < detectObstacles.Count; obstacle++)
{
    if (xStartPosition == detectObstacles[obstacle].transform.position.x && ObstacleCondition((int)detectObstacles[obstacle].transform.position.y, LeftOrRightPlane(xStartPosition, character.movesAvailable)) == buttonInstance)
    {
        coo = true;
    }
    else
    {
        coo = false;
    }
}

if (coo) { continue; }*/