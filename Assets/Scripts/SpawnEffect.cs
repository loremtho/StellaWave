using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private Material materialOrg;
    [SerializeField] private Material materialPhase;
    [SerializeField] private Material materialDissolve;
    [SerializeField] private  float fadeTime = 2f;
    // Start is called before the first frame update
    void Start()
    {
        renderer.material = materialDissolve;
        Dofade(0,1,fadeTime);
    }


    void Dofade ( float start, float dest, float time)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", start, "to", dest, "time", time, "onupdatetarget", gameObject
                    , "onupdate", "TweenOnUpdate", "oncomplete", "TweenOnComplete", "essetype", iTween.EaseType.easeInCubic));
    }

    void TweenOnUpdate(float value)
    {
        renderer.material.SetFloat("_Split_Value", value);
    }
    void TweenOnComplete()
    {
        renderer.material = materialOrg;
    }
}
