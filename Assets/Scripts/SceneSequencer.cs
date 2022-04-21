using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Transform to Singleton

public class SceneSequencer : MonoBehaviour
{
    
    private enum enVPosition { Center, Up, Down };
    private enum enHPosition { Center, Right, Left};
    public enum enTPostition { CenterCenter, UpCenter, DownCenter, CenterRight, CenterLeft, UpRight, UpLeft, DownRight, DownLeft};

    private enVPosition vPosition = enVPosition.Center;
    private enHPosition hPosition = enHPosition.Center;
    private enTPostition tPostition = enTPostition.CenterCenter;

    private EffectsPool effPool;
    
    //--SPAWN CONFIGS--
    [System.Serializable]
    public class SpawnConfig
    {
        public  string name;
        public int id;
        public Transform transform;
        public EffectData effectData;
        public enTPostition enPosition;
        
    }


    //--INSPECTOR--
    public List<SpawnConfig> spawnConfigs = new List<SpawnConfig>(9);

    //Create Singleton
    public static SceneSequencer Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    

    private void Update()
    {
        DetectKeys();
        tPostition = GetTotalPosition(hPosition, vPosition);
        print(tPostition);
        if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.RightControl))
        {
            print("Creating effect...");
            SpawnInPosition(tPostition);
        }

    }
    public void EnableAnimation(int posID) //TODO with gizmos number call the animations
    {
        foreach (SpawnConfig item in spawnConfigs)
        {
            if (item.id == posID) effPool.ActivateEffect(item.effectData, item.transform.position);
        }
    }
    private void Start()
    {
        effPool = FindObjectOfType<EffectsPool>();
        for (int i = 0; i < effPool.Quantity; i++)
        {
            SpawnInPosition(enTPostition.CenterCenter);
        }
    }

    private void SpawnInPosition(enTPostition tPostition)
    {
        foreach (SpawnConfig item in spawnConfigs)
        {
            if(tPostition == item.enPosition)
            {
                print("enter");
                effPool.ActivateEffect(item.effectData, item.transform.position);
            }
                
        }
    }

    private enTPostition GetTotalPosition(enHPosition hpos, enVPosition vpos)
    {

        switch (vpos)
        {
            case enVPosition.Center:
                switch (hpos)
                {
                    case enHPosition.Center:
                        return enTPostition.CenterCenter;
                    case enHPosition.Right:
                        return enTPostition.CenterRight;
                    case enHPosition.Left:
                        return enTPostition.CenterLeft;
                }
                break;
            case enVPosition.Up:
                switch (hpos)
                {
                    case enHPosition.Center:
                        return enTPostition.UpCenter;
                    case enHPosition.Right:
                        return enTPostition.UpRight;
                    case enHPosition.Left:
                        return enTPostition.UpLeft;
                }
                break;
            case enVPosition.Down:
                switch (hpos)
                {
                    case enHPosition.Center:
                        return enTPostition.DownCenter;
                    case enHPosition.Right:
                        return enTPostition.DownRight;
                    case enHPosition.Left:
                        return enTPostition.DownLeft;
                }
                break;
            default:
                return enTPostition.CenterCenter;
        }
        return enTPostition.CenterCenter;
    }
    private void DetectKeys()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) vPosition = enVPosition.Up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) vPosition = enVPosition.Down;
        if (Input.GetKeyDown(KeyCode.RightArrow)) hPosition = enHPosition.Right;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) hPosition = enHPosition.Left;

        if (Input.GetKeyUp(KeyCode.UpArrow)) vPosition = enVPosition.Center;
        if (Input.GetKeyUp(KeyCode.DownArrow)) vPosition = enVPosition.Center;
        if (Input.GetKeyUp(KeyCode.RightArrow)) hPosition = enHPosition.Center;
        if (Input.GetKeyUp(KeyCode.LeftArrow)) hPosition = enHPosition.Center;
    }


}
