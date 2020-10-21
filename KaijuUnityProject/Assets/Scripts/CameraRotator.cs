using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    //rotation
    public float lerpSpeed = 1;
    public float angle = 50;
    private float lerpValue = 0;

    Quaternion start, goal;

    // Start is called before the first frame update
    void Start() {
        //rotation
        goal = this.transform.rotation;
        this.transform.Rotate(new Vector3(0, angle, 0));
        start = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        lerpValue += lerpSpeed * Time.deltaTime / 100;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, goal, lerpValue);
        if (Mathf.Abs(Quaternion.Angle(this.transform.rotation, goal)) <= 0.5) {
            lerpValue = 0;
            var tmp = goal;
            goal = start;
            start = tmp;
        }
    }
}
