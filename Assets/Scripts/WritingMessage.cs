using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WritingMessage : MonoBehaviour
{
    [SerializeField] [Range(0, 100)] int yOffset;

    void Start()
    {
        StartCoroutine(MoveDots());
    }

    IEnumerator MoveDots()
    {
        Image[] dots = this.GetComponentsInChildren<Image>();
        foreach (Image item in dots)
        {
            item.rectTransform.LeanMoveLocalY(item.rectTransform.localPosition.y + yOffset, .2f).setEaseInOutExpo().setLoopPingPong();
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
}
