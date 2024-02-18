using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //make the object renderer disactivate on start
        gameObject.GetComponent<Renderer>().enabled = false;
    }
}
