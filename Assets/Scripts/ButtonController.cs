using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Sets what character can be moved and what position its going to move to
public class ButtonController : MonoBehaviour
{
    [SerializeField] CharacterMovement previousCharacter;
    [SerializeField] CharacterMovement character;
    [SerializeField] GameObject buttonPrefab;

    public void SetCharacter(CharacterMovement clickedCharacter)
    {
        if (clickedCharacter != character)
        {
            previousCharacter = character;
        }

        character = clickedCharacter;
    }

    public void InstantiateButtonMap()
    {
        //CreateButtonsParent();

        //starts movesAvailable tiles left of the character, and iterates until movesAvailable tiles right of the character
        for (int xStartPosition = -(character.movesAvailable); xStartPosition <= character.movesAvailable; xStartPosition++)
        {
            for (int buttonInstance = -LeftOrRightPlane(xStartPosition, character.movesAvailable); buttonInstance <= LeftOrRightPlane(xStartPosition, character.movesAvailable); buttonInstance++) //for (int buttonInstance = -yButtonRange; buttonInstance <= yButtonRange; buttonInstance++)
            {
                if (buttonInstance == 0 && xStartPosition == 0) { continue; }
                InsantiateButton(character.buttonMap.transform, buttonPrefab, xStartPosition, buttonInstance);
            }
        }
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

    void InsantiateButton(Transform buttonMap, GameObject buttonPrefab, int xButtonPosition, int yButtonPosition)
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

    private void OnMouseDown()
    {
        Debug.Log("works");
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