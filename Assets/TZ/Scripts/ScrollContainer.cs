using DG.Tweening;
using UnityEngine;

public class ScrollContainer : MonoBehaviour
{
    private RectTransform containerRectTransform;
    private bool isScaledUp = false;
    
    private Tweener scaleTweener;
    
    public Vector2 AnchoredPosition
    {
        get => containerRectTransform.anchoredPosition;
        set => containerRectTransform.anchoredPosition = value;
    }

    private void OnValidate()
    {
        containerRectTransform = GetComponent<RectTransform>();
    }
    
    private void ScaleUp()
    {
        scaleTweener?.Kill();
        scaleTweener = transform
            .DOScale(2f, 1)
            .SetEase(Ease.OutBack);
    }
    
    private void ScaleDown()
    {
        scaleTweener?.Kill();
        scaleTweener = transform
            .DOScale(1f, 0.5f)
            .SetEase(Ease.InOutSine);
    }

    public float GetHeight() => containerRectTransform.rect.height;
    
    public void PlayWinAnimation()
    {
        if (isScaledUp) return;
        
        ScaleUp();
        isScaledUp = true;
    }

    public void PlayIdleAnimation()
    {
        if (!isScaledUp) return;
        
        ScaleDown();
        isScaledUp = false;
    }
}
