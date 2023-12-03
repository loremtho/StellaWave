using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarWarsText : MonoBehaviour
{
    public Text starWarsText;
    public float scrollSpeed = 20f;

    void Start()
    {
        StartCoroutine(ScrollText());
    }

    IEnumerator ScrollText()
    {
        // 초기 위치 설정
        RectTransform rectTransform = starWarsText.GetComponent<RectTransform>();
        Vector3 startPosition = rectTransform.localPosition;
        float scrollPosition = 0f;

        // 텍스트를 위로 스크롤하는 루프
        while (scrollPosition < 1f)
        {
            scrollPosition += Time.deltaTime * scrollSpeed / rectTransform.rect.height;

            // 텍스트의 위치 갱신 (위로 이동)
            rectTransform.localPosition = Vector3.Lerp(startPosition, startPosition + Vector3.up * rectTransform.rect.height, scrollPosition);

            yield return null;
        }

       
    }
}
