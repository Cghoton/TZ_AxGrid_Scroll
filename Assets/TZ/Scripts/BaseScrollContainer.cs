using UnityEngine;

public abstract class BaseScrollContainer : MonoBehaviour, IScrollContainer
{
    private RectTransform RectTransform;
    
    protected virtual void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }
    
    public Vector2 AnchoredPosition
    {
        get => RectTransform.anchoredPosition;
        set => RectTransform.anchoredPosition = value;
    }
    
    public float GetHeight() => RectTransform.rect.height;
    
    public abstract void ShowAsSelected();
    public abstract void ShowAsNeutral();
}
