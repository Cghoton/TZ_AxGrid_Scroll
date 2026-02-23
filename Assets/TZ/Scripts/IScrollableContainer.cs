using UnityEngine;

public interface IScrollContainer 
{
    Vector2 AnchoredPosition { get; set; }
    float GetHeight();
    void ShowAsSelected();
    void ShowAsNeutral();
}
