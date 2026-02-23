using DG.Tweening;
using UnityEngine;

public class ScaleScrollContainer : BaseScrollContainer
{
    [Header("Scale properties")]
    [SerializeField] private float selectedScale = 2f;
    [SerializeField] private float selectedDuration = 1f;
    [SerializeField] private float neutralScale = 1f;
    [SerializeField] private float neutralDuration = 0.5f;
    
    private bool _isSelected = false;
    
    private Tweener _currentTween;
    
    private void ScaleUp()
    {
        _currentTween?.Kill();
        _currentTween = transform
            .DOScale(selectedScale, selectedDuration)
            .SetEase(Ease.OutBack);
    }
    
    private void ScaleDown()
    {
        _currentTween?.Kill();
        _currentTween = transform
            .DOScale(neutralScale, neutralDuration)
            .SetEase(Ease.InOutSine);
    }
    
    public override void ShowAsSelected()
    {
        if (_isSelected) return;
        
        ScaleUp();
        _isSelected = true;
    }

    public override void ShowAsNeutral()
    {
        if (!_isSelected) return;
        
        ScaleDown();
        _isSelected = false;
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
    }
}

