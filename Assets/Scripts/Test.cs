using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.FixedPoint;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var a = (FP)3.5f;
        var b = (FP)2.1f;
        Debug.Log(a + b);
        Debug.Log(a - b);
        Debug.Log(a * b);
        Debug.Log(a / b);
    }
}
