using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UIElements;
using TMPro;

public class BarkManager : MonoBehaviour
{
    public BarkScriptableObject barkData;
    public TMP_Text barkText;
    public Canvas barkCanvas;

    private float panelMoveDuration;

    public void TriggerRandomBark()
    {
        if (barkData == null || barkData.barks.Length == 0) return;
        
        StartCoroutine(TriggerBarkCoroutine());
    }

    private IEnumerator TriggerBarkCoroutine()
    {
        int randomIndex = Random.Range(0, barkData.barks.Length);
        BarkScriptableObject.Bark selectedBark = barkData.barks[randomIndex];
        barkText.text = selectedBark.text;

        yield return new WaitForSeconds(panelMoveDuration);
    }
}
