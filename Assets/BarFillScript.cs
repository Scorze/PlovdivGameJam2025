using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BarFillScript : MonoBehaviour
{
    public Gradient gradient;

    void Start()
    {
        gradient = GetComponentInParent<SimpleHPBarScript>().gradient;
    }

    // Update is called once per frame
    void Update()
    {
        gradient = GetComponentInParent<SimpleHPBarScript>().gradient;
    }
}
