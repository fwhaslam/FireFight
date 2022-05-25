using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameScript : MonoBehaviour
{

    Vector3 startLoc;
    Vector3 startScale;
    Quaternion startRotate;

    float offset;

    // Start is called before the first frame update
    void Start() {

        offset = UnityEngine.Random.Range(0f,1f);

        startScale = transform.localScale;
        startRotate = transform.localRotation;

        var pos = transform.localPosition;
        startLoc = new Vector3( pos.x, pos.y - startScale.y, pos.z );
    }

    // Update is called once per frame
    void Update() {

        var time = ( Time.time + offset ) * 2f;

        // bounce up/down
        var scaleSin = (float)Math.Sin( ( time ) * Math.PI  );
        var sy = startScale.y * ( 1f + scaleSin*0.2f );
        transform.localScale = new Vector3( startScale.x, sy, startScale.z );

        // lean right/left at different rate
        var sin = (float)Math.Sin( ( time/2f ) * Math.PI  );
        transform.localRotation = Quaternion.Euler( 45, 30 * sin, 0 );
    }
}
