using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]

public class OhNoCar : MonoBehaviour{

    SphereCollider sphereCollider;

    [SerializeField] GameObject ragdoll;

    Vector3 location = Vector3.zero;

    Quaternion posi = Quaternion.identity;

    void Start(){
        
        location = transform.position;

    }

    void Update(){

    }

    private void OnCollisionEnter(Collision collision){

        this.gameObject.SetActive(false);

        Instantiate(ragdoll, location, posi);

    }

}
