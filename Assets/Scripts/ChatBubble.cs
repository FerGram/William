using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] float height = 2f;
    [SerializeField] float width = 4f;
    private Image bgImage;
    private TextMeshPro textMeshPro;

    void Awake()
    {
        bgImage = transform.Find("Image").GetComponent<Image>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
    }

    public void Setup(string text)
    {
        textMeshPro.SetText(text);
        textMeshPro.ForceMeshUpdate();
        Vector2 textSize = textMeshPro.GetRenderedValues(false);

        Vector2 padding = new Vector2(width, height);
        bgImage.rectTransform.sizeDelta = textSize + padding;
    }
}
