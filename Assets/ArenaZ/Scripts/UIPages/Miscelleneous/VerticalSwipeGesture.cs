using UnityEngine;
using System;
using UnityEngine.UI.Extensions;
using ArenaZ.Manager;
using UnityEngine.EventSystems;

[RequireComponent(typeof(HorizontalScrollSnap))]
public class VerticalSwipeGesture : MonoBehaviour,IBeginDragHandler,IEndDragHandler
{
    private Vector2 firstTouchPos;
    private Vector2 lastTouchPos;
    private float dragDistance;
    private HorizontalScrollSnap horizontalScrollSnap;

    private void Start()
    {
        dragDistance = Screen.height * 8 / 100; //dragDistance is 8% height of the screen
        horizontalScrollSnap = GetComponent<HorizontalScrollSnap>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        firstTouchPos = eventData.position;
        lastTouchPos = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        lastTouchPos = eventData.position;
        onVerticalSwipe();
    }

    private void onVerticalSwipe()
    {
        if (!horizontalScrollSnap.PageIsChanging)
        {
            if (Mathf.Abs(lastTouchPos.y - firstTouchPos.y) > dragDistance)
            {
                if (lastTouchPos.y > firstTouchPos.y)
                {
                    Debug.Log("Up Swipe");
                    UIManager.Instance.swipeUp?.Invoke(true);
                }
                else
                {
                    Debug.Log("Down Swipe");
                    UIManager.Instance.swipeUp?.Invoke(false);
                }
            }
        }
        firstTouchPos = Vector2.zero;
        lastTouchPos = Vector2.zero;
    }
}
