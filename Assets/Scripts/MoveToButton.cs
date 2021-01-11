using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToButton : MonoBehaviour
{
    [SerializeField] CharacterMovement character;

    public void SetCharacter(CharacterMovement clickedCharacter)
    {
        character = clickedCharacter;
    }

    public void TargetPosition()
    {
        
        character.SetClickedPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3 clickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (transform.position != character.SnapToGrid(clickedPosition)) { character.SetMoving(); }

        StartCoroutine(character.ToggleClicked());
    }

}