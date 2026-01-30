using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrikTrackerBinder : MonoBehaviour
{

    [Header("Input Transforms")]
    public Transform headInput;
    public Transform lefthandInput;
    public Transform righthandInput;
    public Transform waistInput;
    public Transform leftfootInput;
    public Transform rightfootInput;

    [Header("VRIK Targets")]
    public Transform headTarget;
    public Transform lefthandTarget;
    public Transform righthandTarget;
    public Transform waistTarget;
    public Transform leftfootTarget;
    public Transform rightfootTarget;

    // Update is called once per frame
    void Update()
    {
        Copy(headInput, headTarget);
        Copy(lefthandInput, lefthandTarget);
        Copy(righthandInput, righthandTarget);
        Copy(waistInput, waistTarget);
        Copy(leftfootInput, leftfootTarget);
        Copy(rightfootInput, rightfootTarget);
    }

    void Copy(Transform src, Transform dst)
    {
        if (src == null || dst == null) return;
        dst.position = src.position;
        dst.rotation = src.rotation;
    }
}
