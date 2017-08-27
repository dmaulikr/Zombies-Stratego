﻿
using System.Collections.Generic;
using UnityEngine;

public enum GameMode { Edit, SinglePlayer, Multiplayer }

public class Globals : Singleton<Globals> {

    public const int TOTAL_MONEY = 100;
    public const int MAX_ZOMBIES_FOR_PLAYER = 36; //9 rows on 4 columns

    public Dictionary<string, GameObject> unityObjects;

    public Dictionary<string, GameObject> UnityObjects { get { return unityObjects; } }

    private void Start() {
        unityObjects = new Dictionary<string, GameObject>();
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("UnityObject");
        foreach(var ob in gameObjects) {
            unityObjects.Add(ob.name, ob);
        }
    }

}