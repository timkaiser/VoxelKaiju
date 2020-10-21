using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Source: parts copied from: https://answers.unity.com/questions/609526/time-since-object-was-instantiated.html
public class Parts_Despawn : MonoBehaviour
{
    // Update is called once per frame
    private float initializationTime;

    void Start() {
        //Debug.Log("Initialized");
        initializationTime = Time.timeSinceLevelLoad;
    }
    void Update() {
        //Debug.Log(initializationTime + " | " + Config.GetFloat(Config.Type.Gameplay, "BrokenPartsDespawnTime") + " | " + Time.timeSinceLevelLoad);
        if (initializationTime + Config.GetFloat(Config.Type.Gameplay, "BrokenPartsDespawnTime") - 1 <= Time.timeSinceLevelLoad) {
            Renderer[] rend = this.gameObject.GetComponentsInChildren<Renderer>();
            foreach(Renderer r in rend) {
                r.enabled = !r.enabled;
            }

        }

        if (initializationTime + Config.GetFloat(Config.Type.Gameplay, "BrokenPartsDespawnTime") <= Time.timeSinceLevelLoad) {
            Destroy(this.gameObject);
        }


    }
}

