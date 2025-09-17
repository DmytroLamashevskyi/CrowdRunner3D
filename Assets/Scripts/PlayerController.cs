using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float _moveSpeed = 3f;

    [Header("Control")]
    [SerializeField] 
    private float _slideSpeed = 1.0f;


    private Vector3 _clickedScreenPosition = Vector3.zero;
    private Vector3 _clickedPlayerPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveForward();
        ManageControl();
    }

    private void ManageControl()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _clickedScreenPosition = Input.mousePosition;
            _clickedPlayerPosition = transform.position;
        }
        else if(Input.GetMouseButton(0))
        {
            float xScreenDifference = Input.mousePosition.x - _clickedScreenPosition.x;
            xScreenDifference /= Screen.width;
            xScreenDifference *= _slideSpeed;

            Vector3 position = transform.position;
            position.x = _clickedPlayerPosition.x + xScreenDifference;
            transform.position = position;
        }
    }

    public void MoveForward()
    {
        transform.position += Vector3.forward * Time.deltaTime * _moveSpeed;
    }
}
