using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class GrowVine : MonoBehaviour
{
    private Material mat;
    public float TimeToGrow = 3;
    private readonly float refreshRate = 0.05f;
    public float MinGrow = 0f;
    public float MaxGrow = 0.97f;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    public void Grow()
    {
        mat.DOFloat(MaxGrow, "_Grow", TimeToGrow).SetEase(Ease.OutSine);
        // StartCoroutine(GrowRoutine());
    }

    public void Shrink()
    {
        mat.DOFloat(MinGrow, "_Grow", TimeToGrow).SetEase(Ease.OutSine);
        // StartCoroutine(ShrinkRoutine());
    }

    // private IEnumerator GrowRoutine()
    // {
    //     float growValue = mat.GetFloat("_Grow");
    //     if (!fullyGrown)
    //     {
    //         while (growValue < MaxGrow)
    //         {
    //             growValue += 1 / (TimeToGrow / refreshRate);
    //             mat.SetFloat("_Grow", growValue);

    //             yield return new WaitForSeconds(refreshRate);
    //         }
    //     }
    // }

    // private IEnumerator ShrinkRoutine()
    // {
    //     float growValue = mat.GetFloat("_Grow");
    //     while (growValue > MinGrow)
    //     {
    //         growValue -= 1 / (TimeToGrow / refreshRate);
    //         mat.SetFloat("_Grow", growValue);

    //         yield return new WaitForSeconds(refreshRate);
    //     }
    // }

}
