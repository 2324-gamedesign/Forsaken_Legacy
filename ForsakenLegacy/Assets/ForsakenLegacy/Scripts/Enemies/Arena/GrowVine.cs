using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class GrowVine : MonoBehaviour
{
    private Material mat;
    private float timeToGrow = 3;
    private float refreshRate = 0.05f;
    private float minGrow = 0.6f;
    private float maxGrow = 0.97f;
    private bool fullyGrown = false;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    public void Grow()
    {
        StartCoroutine(GrowRoutine());
    }

    public void Shrink()
    {
        StartCoroutine(ShrinkRoutine());
    }

    private IEnumerator GrowRoutine()
    {
        float growValue = mat.GetFloat("_Grow");
        if (!fullyGrown)
        {
            while (growValue < maxGrow)
            {
                growValue += 1 / (timeToGrow / refreshRate);
                mat.SetFloat("_Grow", growValue);

                yield return new WaitForSeconds(refreshRate);
            }
        }
    }

    private IEnumerator ShrinkRoutine()
    {
        float growValue = mat.GetFloat("_Grow");
        while (growValue > minGrow)
        {
            growValue -= 1 / (timeToGrow / refreshRate);
            mat.SetFloat("_Grow", growValue);

            yield return new WaitForSeconds(refreshRate);
        }
    }

}
