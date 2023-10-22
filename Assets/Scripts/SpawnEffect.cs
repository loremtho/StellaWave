using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    [Header("렌더러")]
    [SerializeField] private Renderer[] Headrenderers;
    [SerializeField] private Renderer[] Bodyrenderers;

    [Header("메테리얼")]

    [SerializeField] private Material[] HeadOrgmat; 
    [SerializeField] private Material[] BodyOrgmat; 
    [SerializeField] private Material materialDissolve;

    [Header("오브젝트 껏다키기")]
    [SerializeField] private GameObject Eyes; 
    [SerializeField] private  float fadeTime = 2f;
    // Start is called before the first frame update
    void Start()
    {
        Bodyrenderers[0].material = materialDissolve;
        Dofade(0,1,fadeTime);
        Bodyrenderers[1].material = materialDissolve;
        Dofade(0,1,fadeTime);
        Bodyrenderers[2].material = materialDissolve;
        Dofade(0,1,fadeTime);


        Headrenderers[0].material = materialDissolve;
        Dofade(0,1,fadeTime);
        Headrenderers[1].material = materialDissolve;
        Dofade(0,1,fadeTime);
        Headrenderers[2].material = materialDissolve;
        Dofade(0,1,fadeTime);
        Headrenderers[3].material = materialDissolve;
        Dofade(0,1,fadeTime);

        this.Eyes.SetActive(false);

    }


    void Dofade ( float start, float dest, float time)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", start, "to", dest, "time", time, "onupdatetarget", gameObject
                    , "onupdate", "TweenOnUpdate", "oncomplete", "TweenOnComplete", "essetype", iTween.EaseType.easeInCubic));
    }

    /*void TweenOnUpdate(float value)
    {
        GetComponent<Renderer>().material.SetFloat("_Split_Value", value);
    }*/
    void TweenOnComplete()
    {
        Bodyrenderers[0].material = BodyOrgmat[0];
        Bodyrenderers[1].material = BodyOrgmat[1];
        Bodyrenderers[2].material = BodyOrgmat[2];

        Headrenderers[0].material = HeadOrgmat[0];
        Headrenderers[1].material = HeadOrgmat[1];
        Headrenderers[2].material = HeadOrgmat[2];
        Headrenderers[3].material = HeadOrgmat[3];
        
        this.Eyes.SetActive(true);
    }
}
