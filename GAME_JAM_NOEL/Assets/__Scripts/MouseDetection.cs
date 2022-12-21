using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDetection : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    
    private Camera mainCamera;
    [SerializeField]
    private Vector3 mousePos;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Set Cursor Sprite 
        //TODO if GameplayManager.StateGame == InGame : Modifier le curseur seulement en jeu 
        //Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, mainCamera.nearClipPlane));
        mousePos.z = 0f;
    }
    
    public Vector2 MousePos => new Vector2(mousePos.x,mousePos.y);
}
