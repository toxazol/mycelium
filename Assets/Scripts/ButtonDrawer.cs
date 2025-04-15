using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDrawer : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float fillStep = 0.01f;
    [SerializeField] private float moveStep = 0.01f;
    [SerializeField] private bool isShow = false;
    [SerializeField] private bool isHide = false;

    private Button btn;
    private Image img;
    private float step;
    private float timer;
    private float initFill;
    

    void Start()
    {
        btn = GetComponent<Button>();
        img = GetComponent<Image>();
        initFill = img.fillAmount;
    }

    [ContextMenu("Show")]
    public void Show()
    {
        isShow = true;
        isHide = false;
        step = 1/speed;
        timer = 0f;
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        isHide = true;
        isShow = false;
        step = 1/speed;
        timer = 0f;
    }

    void Update()
    {   

        if(!isShow && !isHide) return;
        timer += Time.deltaTime;
        if(timer < step) return;
        
        timer = 0f;
        if(isShow && img.fillAmount + fillStep > 1f) {
            img.fillAmount = 1f;
            isShow = false;
            return;
        }
        if(isHide && img.fillAmount - fillStep < initFill) {
            img.fillAmount = initFill;
            isHide = false;
            return;
        }
        float sign = isShow ? 1 : -1;
        img.fillAmount += fillStep * sign;
        this.transform.Translate(new Vector3(-moveStep * sign, 0f, 0f));
    }
}
