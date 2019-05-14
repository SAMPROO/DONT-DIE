using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    CanvasGroup cg;
    private float fadeSpeed = 1;
    private float fadeDelay = 2;
    private float currentTime;
    private float currentAlpha;

    private void Start()
    {
        cg = GetComponent<CanvasGroup>();
        currentAlpha = cg.alpha;
    }

    void Update()
    {
        if(currentTime<0 && currentAlpha>0)
        {
            currentAlpha -= fadeSpeed * Time.deltaTime;
            cg.alpha = currentAlpha;
        }
        if(currentTime>0)
        {
            currentTime -= 1 * Time.deltaTime;
        }
    }

    public void DoEffect()
    {
        cg.alpha = 1;
        currentTime = fadeDelay;
        currentAlpha = 1;
    }
}
