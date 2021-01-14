using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
An object reference is needed to run an onClick function from a script, 
so in order to use the TargetPosition() function (so the character will move to the targeted button), 
you need to pass in the position controller object.
the position controller needs a game object to know which character is targeting a position.
The position controller is given the character game object when you click on a character.
The same click event instantiates the buttons the character can move to.
So passing in the position controller prefab to the buttons onClick throws a null reference error when the button instantiates
because its looking for a clicked character after the event occured.
Even instantiating a position controller doesn't work.

The button instantiation and the character setting of the position controller object, need to happen at different times
to use the position controller script independently of this one.

this script is to look for the existing position controller in the scene which has a character set.
This script is tied to the button prefab which gets loaded into its own onclick.
 */

public class FindPositionController : MonoBehaviour
{
    PositionController positionController;
    Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        positionController = FindObjectOfType<PositionController>();
        originalColor = gameObject.GetComponent<SpriteRenderer>().color;
    }

    public void FindTarget()
    {
        positionController.TargetPosition();
    }

    public void OnMouseDown()
    {
        positionController.TargetPosition();
    }

    public void OnMouseOver()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void OnMouseExit()
    {
        gameObject.GetComponent<SpriteRenderer>().color = originalColor;
    }

    // Update is called once per frame
    void Update()
    {
        positionController = FindObjectOfType<PositionController>();
    }
}
