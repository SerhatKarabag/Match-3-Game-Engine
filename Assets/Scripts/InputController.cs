using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : AbstractClass
{
    public static bool touching;
#if UNITY_ANDROID
    void Update()
    {
        if (BoardController.instance.ReadyToGetInput() && Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began) // A finger touched the screen
            {
                SetFirstDrop(); // Set FirstDrop if the touched object has a collider
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && touching) // A finger moved on the screen
            {
                SetSecondDrop(); // Set Second drop 
            }
        }
    }
    private void SetFirstDrop()
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        Vector2 touchPos = new Vector2(wp.x, wp.y);
        Collider2D collider = Physics2D.OverlapPoint(touchPos);
        if (collider != null)
        {
            BoardController.instance.SelectFirstDrop(collider);
            touching = true;
        }
    }
    private void SetSecondDrop()
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        Vector2 touchCurrentWorldPosition = new Vector2(wp.x, wp.y);
        Collider2D collider = Physics2D.OverlapPoint(touchCurrentWorldPosition);
        if (collider != null)
        {
            if (collider.GetComponent<Drop>() != BoardController.instance.TouchStartDrop)
            {
                BoardController.instance.SelectSecondDrop(collider);
                touching = false;
            }
        }
    }
#endif

    //#if UNITY_EDITOR

    //    void Update()
    //    {
    //        if (BoardController.instance.ReadyToGetInput() && Input.GetMouseButtonDown(0))
    //        {
    //            SetFirstDrop(); // Set FirstDrop if the touched object has a collider
    //        }
    //        else if (Input.GetMouseButton(0) && touching) // touching will bee true when first drop selected
    //        {
    //            SetSecondDrop();
    //        }
    //    }
    //    private void SetFirstDrop()
    //    {
    //        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector2 touchPos = new Vector2(wp.x, wp.y);
    //        Collider2D collider = Physics2D.OverlapPoint(touchPos);
    //        if (collider != null)
    //        {
    //            BoardController.instance.SelectFirstDrop(collider);
    //            touching = true;
    //        }
    //    }
    //    private void SetSecondDrop()
    //    {
    //        Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector2 touchCurrentWorldPosition = new Vector2(wp.x, wp.y);
    //        Collider2D collider = Physics2D.OverlapPoint(touchCurrentWorldPosition);
    //        if (collider != null)
    //        {
    //            if (collider.GetComponent<Drop>() != BoardController.instance.TouchStartDrop)
    //            {
    //                BoardController.instance.SelectSecondDrop(collider);
    //                touching = false;
    //            }
    //        }
    //    }
    //#endif
}

