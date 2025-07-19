using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public Vector2 TouchDist;
    [HideInInspector] public Vector2 PointerOld;
    [HideInInspector] protected int FingerId = -1; 
    [HideInInspector] public bool Pressed;

    void Update()
    {
        if (Pressed)
        {
            bool found = false;
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                if (t.fingerId == FingerId)
                {
                    TouchDist = t.position - PointerOld;
                    PointerOld = t.position;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                TouchDist = Vector2.zero;
            }
        }
        else
        {
            TouchDist = Vector2.zero;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;

        float closestDistance = float.MaxValue;
        int bestFingerId = -1;

        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            float dist = Vector2.Distance(touch.position, eventData.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                bestFingerId = touch.fingerId;
            }
        }

        FingerId = bestFingerId;
        PointerOld = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
        FingerId = -1;
    }
}
