using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDrawer : MonoBehaviour
{
    public float speed = 1f;
    public bool isShowing = false;
    public bool isHiding = false;
    public RectTransform counter;
    public float curProgress = 0;

    Image img;
    Vector2 initPos;
    float cliffX;
    float initDist;
    float initFill;
    RectTransform rect;
    

    void Start()
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        initFill = img.fillAmount;
        initPos = rect.anchoredPosition;
        cliffX = counter.anchoredPosition.x;
        initDist = Mathf.Abs(initPos.x - cliffX); 
    }

    [ContextMenu("Show")]
    public void Show()
    {
        isShowing = true;
        isHiding = false;
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        isHiding = true;
        isShowing = false;
    }

    void Update()
    {   

        if(!isShowing && !isHiding) return;

        float sign = isShowing ? 1 : -1;
        curProgress += sign * speed * Time.deltaTime;

        if(isShowing && curProgress > 1f) { 
            curProgress = 1f;
            img.fillAmount = 1f;
            rect.anchoredPosition.Set(cliffX, initPos.y);
            isShowing = false;
            return;
        }
        if(isHiding && curProgress < 0f) { 
            curProgress = 0f;
            img.fillAmount = initFill;
            rect.anchoredPosition = initPos;
            isHiding = false;
            return;
        }
        
        img.fillAmount = curProgress * (1f - initFill) + initFill;
        float dist = (1 - curProgress) * initDist;
        float btnRight = cliffX + dist;

        var tmp = rect.anchoredPosition;
        rect.anchoredPosition = new Vector2(btnRight, tmp.y);
    }
}