using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText;
    public Color notSelectedColor = Color.black;
    public Color selectedColor = Color.white;
    void Start()
    {
        if(buttonText != null)
        {
            buttonText.color = notSelectedColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(buttonText != null)
        {
            buttonText.color = selectedColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(buttonText != null)
        {
            buttonText.color = notSelectedColor;
        }
    }
}
