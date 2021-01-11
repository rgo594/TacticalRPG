using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    MoveToButton moveButtonObj;
    // Start is called before the first frame update

    void Start()
    {
        moveButtonObj = FindObjectOfType<MoveToButton>();
    }

    public void FindTarget()
    {
        moveButtonObj.TargetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        moveButtonObj = FindObjectOfType<MoveToButton>();
        Debug.Log(moveButtonObj);
    }
}
