using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDrawer : MonoBehaviour
{
    public float moveStep = 0.001f;
    public bool isShowing = false;
    public bool isHiding = false;
    public RectTransform counter;

    Image img;
    float timer;
    float initFill;
    Vector2 initPos;
    float cliffX;
    float initDist;
    

    void Start()
    {
        img = GetComponent<Image>();
        initFill = img.fillAmount;
        initPos = GetComponent<RectTransform>().anchoredPosition;
        cliffX = counter.anchoredPosition.x;

        initDist = Mathf.Abs(initPos.x - cliffX); 
    }

    [ContextMenu("Show")]
    public void Show()
    {
        isShowing = true;
        isHiding = false;
        timer = 0f;
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        isHiding = true;
        isShowing = false;
        timer = 0f;
    }



    void Update()
    {   

        if(!isShowing && !isHiding) return;
        // dist = init - curProgress*init
        var btnRight = GetComponent<RectTransform>().anchoredPosition.x;

        if(isShowing && btnRight < cliffX) { // && img.fillAmount + fillStep > 1f
            GetComponent<RectTransform>().anchoredPosition.Set(cliffX, initPos.y);
            img.fillAmount = 1f;
            isShowing = false;
            return;
        }
        if(isHiding && btnRight >= initPos.x) { // && img.fillAmount - fillStep < initFill
            GetComponent<RectTransform>().anchoredPosition = initPos;
            img.fillAmount = initFill;
            isHiding = false;
            return;
        }

        var dist = Mathf.Abs(btnRight - cliffX);

        var curProgress = (initDist - dist)/initDist;

        float sign = isShowing ? 1 : -1;
        img.fillAmount = curProgress;
        this.transform.Translate(new Vector3(-moveStep * sign, 0f, 0f));
    }
}
