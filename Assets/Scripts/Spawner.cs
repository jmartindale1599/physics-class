using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Spawner : MonoBehaviour{

    [SerializeField] GameObject obj;

    void Start(){
    
    }

    void Update(){

        if(Input.GetKeyDown(KeyCode.Space)) Instantiate(obj, GetComponentInChildren<Transform>().position, GetComponentInChildren<Transform>().rotation);
        
        //if(Input.GetKey(KeyCode.Space)) Instantiate(obj, GetComponentInChildren<Transform>().position, GetComponentInChildren<Transform>().rotation);

    }

}
