using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DitherToShowPlayer : MonoBehaviour
{
    private Material _material;
    private float _ditherValue;
    public bool Dither = true;
    private bool _dithering;

    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponent<Renderer>().material;
        _ditherValue = _material.GetFloat("_DitherThreshold");
    }

    private void Update() {
        if (Dither && !_dithering)
        {
            //Call DitherMat Coroutine
            StartCoroutine("DitherMat");
        }
        else if (!Dither && _dithering)
        {
            StartCoroutine("DitherMatBack");
        }
    }

    private IEnumerator DitherMatBack()
    {
        _dithering = false;
        float time = 0;
        while (time < 0.3f)
        {
            _ditherValue = Mathf.Lerp(0.3f, 1, time / 0.3f);
            _material.SetFloat("_DitherThreshold", _ditherValue);
            time += Time.deltaTime;
            yield return null;
        }
        _ditherValue = 1;
        _material.SetFloat("_DitherThreshold", _ditherValue);
    }

    private IEnumerator DitherMat()
    {
        _dithering = true;
        float time = 0;
        while (time < 0.3f)
        {
            _ditherValue = Mathf.Lerp(1, 0.3f, time / 0.3f);
            _material.SetFloat("_DitherThreshold", _ditherValue);
            time += Time.deltaTime;
            yield return null;
        }
        _ditherValue = 0.3f;
        _material.SetFloat("_DitherThreshold", _ditherValue);
    }
}
