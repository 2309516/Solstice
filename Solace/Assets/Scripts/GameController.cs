using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    void Start()
{

    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
}

void Update()
{

    if (Input.GetKeyDown(KeyCode.Escape))
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
}
