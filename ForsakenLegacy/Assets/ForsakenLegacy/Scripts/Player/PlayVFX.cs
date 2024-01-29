using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayVFX : MonoBehaviour
{
    public VisualEffect slashOne;
    public VisualEffect slashTwo;
    public VisualEffect slashThree;
    
    void PlaySlashOne()
    {
        slashOne.Play();
    }
    void PlaySlashTwo()
    {
        slashTwo.Play();
    }
    void PlaySlashThree()
    {
        slashThree.Play();
    }
}
