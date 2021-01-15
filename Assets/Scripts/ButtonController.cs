using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Sets what character can be moved and what position its going to move to
public class ButtonController : MonoBehaviour
{
    [SerializeField] CharacterMovement previousCharacter;
    [SerializeField] CharacterMovement character;

    public void SetCharacter(CharacterMovement clickedCharacter)
    {
        if(clickedCharacter != character)
        { 
            previousCharacter = character; 
        }
        
        character = clickedCharacter;
    }

    public void TargetPosition()
    {
        character.SetClickedPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3 clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (transform.position != character.SnapToGrid(clickedPosition)) { character.SetMoving(); }

        StartCoroutine(character.ToggleClicked());
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