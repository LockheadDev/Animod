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
        if (Input.GetKeyDown(KeyCode.Alpha1)) SceneChanger.Instance.LoadScene(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SceneChanger.Instance.LoadScene(2);

    }
    
    void toggleMenu()
    {
        
        menuEnabled = !menuEnabled;
        Canvas.SetActive(menuEnabled);
    }
}
