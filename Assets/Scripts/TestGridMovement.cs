﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;

    public LayerMask whatStopsMovement;

    // Start is called before the first frame update
    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position,moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.5f)
        {
            if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1)
            {
                Vector3 horizontalInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);

                if (!Physics2D.OverlapCircle(horizontalInput, 2f, whatStopsMovement))
                {
                    movePoint.position += horizontalInput;
                }
            }
            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1)
            {
                Vector3 verticalInput = new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);

                if (!Physics2D.OverlapCircle(verticalInput, 2f, whatStopsMovement))
                {
                    movePoint.position += verticalInput;
                }
            }
        }
    }
}
