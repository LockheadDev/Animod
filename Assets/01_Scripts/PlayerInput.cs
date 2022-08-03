using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{ 
    private bool menuEnabled = false;
    [SerializeField]
    private GameObject Canvas;

    private void Start()
    {
        Canvas.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) toggleMenu();
    }
    
    void toggleMenu()
    {
        menuEnabled = !menuEnabled;
        Canvas.SetActive(menuEnabled);
    }
}
