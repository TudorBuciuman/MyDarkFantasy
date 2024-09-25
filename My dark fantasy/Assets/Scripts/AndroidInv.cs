using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AndroidInv : MonoBehaviour, IPointerClickHandler
{
    public Toolbar t;
    public byte id;
    public void OnPointerClick(PointerEventData eventData)
    {
        t.Changesloth(id);
    }
}
