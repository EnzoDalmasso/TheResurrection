using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltoRegulable : MonoBehaviour
{

    
    private bool enSuelo;
    private bool saltar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            saltar = true;
        }
        //enSuelo=Physics2D.OverlapBox()
    }
}
