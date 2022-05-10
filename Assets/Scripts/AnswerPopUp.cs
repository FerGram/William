using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerPopUp : MonoBehaviour
{
    private bool displayed = false;

    public void UpdateAnswersDisplay()
    {
        RectTransform thisTransform = this.GetComponent<RectTransform>();
        
        if (!displayed && !LeanTween.isTweening(thisTransform))
        {
            displayed = true;
            thisTransform.LeanMoveLocalY(thisTransform.localPosition.y + 500, .5f).setEaseInOutQuad();
        }
        else if (!LeanTween.isTweening(thisTransform))
        {
            displayed = false;
            thisTransform.LeanMoveLocalY(thisTransform.localPosition.y - 500, .5f).setEaseInOutQuad();
        }
    }
}
