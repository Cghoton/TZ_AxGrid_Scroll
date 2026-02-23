using AxGrid.Base;
using UnityEngine;

public class ScrollView : MonoBehaviourExt
{
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private float scrollSpeed = 200f;
    [SerializeField] private float snapSpeed = 5f;
    
    [SerializeField] private ParticleSystem oneTimeEffect;
    [SerializeField] private GameObject glowingEffect;
    
    private IScrollContainer[] _containers;
    private float _containerHeight;
    private bool _isSnapping = false;
    private bool _isScrolling = false;
    
    [OnStart]
    private void InitializeItems()
    {
        _containers = contentPanel.GetComponentsInChildren<IScrollContainer>();
        
        if (_containers.Length == 0) 
        {
            Debug.LogError("Empty Content Panel");
            return;
        }
        
        _containerHeight = _containers[0].GetHeight();
        PositionContainersInitially();
    }
    
    private void PositionContainersInitially()
    {
        for (var i = 0; i < _containers.Length; i++)
        {
            var yPos = -i * _containerHeight;
            _containers[i].AnchoredPosition = new Vector2(0, yPos);
        }
    }
    
    [OnUpdate]
    private void MoveContainers()
    {
        if (_isSnapping || !_isScrolling) return;

        foreach (var container in _containers)
        {
            var pos = container.AnchoredPosition;
            pos.y -= scrollSpeed * Time.deltaTime;
            
            container.AnchoredPosition = pos;
            if (container.AnchoredPosition.y < -_containerHeight * 2) 
            {
                RepositionItemToTop(container);
            }
        }
    }
    
    [OnUpdate]
    private void TrySnapContainerToCenter()
    {
        if (!_isSnapping) return;

        var targetContainer = FindClosestContainerToCenter();
        if (targetContainer == null) return;

        CalculateContainersPositions(targetContainer);

        if (!IsTargetSnapped(targetContainer)) return;
        
        CompleteSnapping();
        targetContainer.ShowAsSelected();
    }

    private void CalculateContainersPositions(IScrollContainer targetContainer)
    {
        var oldY = targetContainer.AnchoredPosition.y;
        var newY = Mathf.Lerp(oldY, 0, snapSpeed * Time.deltaTime);
        
        targetContainer.AnchoredPosition = new Vector2(targetContainer.AnchoredPosition.x, newY);
    
        var deltaY = newY - oldY;

        foreach (var container in _containers)
        {
            if (container == targetContainer) continue;
            var pos = container.AnchoredPosition;
            pos.y += deltaY;
            container.AnchoredPosition = pos;
        }
    }
    
    private void RepositionItemToTop(IScrollContainer targetContainer)
    {
        var highestY = float.MinValue;
        
        foreach (var otherContainer in _containers)
        {
            if (otherContainer.AnchoredPosition.y > highestY)
                highestY = otherContainer.AnchoredPosition.y;
        }
    
        targetContainer.AnchoredPosition = new Vector2(targetContainer.AnchoredPosition.x, highestY + _containerHeight);
    }
    
    
    private IScrollContainer FindClosestContainerToCenter()
    {
        IScrollContainer targetContainer = null;
        var minDist = float.MaxValue;
        
        foreach (var container in _containers)
        {
            var dist = Mathf.Abs(container.AnchoredPosition.y);
            
            if (!(dist < minDist)) continue;
            
            minDist = dist;
            targetContainer = container;
        }

        return targetContainer;
    }
    
    private void CompleteSnapping()
    {
        _isSnapping = false;
        AdjustContainersPosition();
        
        oneTimeEffect.Play();
        glowingEffect.SetActive(true);
    }

    private void AdjustContainersPosition()
    {
        foreach (var container in _containers)
        {
            var pos = container.AnchoredPosition;
            pos.y = Mathf.Round(pos.y / _containerHeight) * _containerHeight;
            container.AnchoredPosition = pos;
        }
    }

    private static bool IsTargetSnapped(IScrollContainer target)
    {
        return Mathf.Abs(target.AnchoredPosition.y) < 0.1f;
    }

    public void StopScrolling()
    {
        _isSnapping = true;
        _isScrolling = false;
    }

    public void StartScrolling()
    {
        _isSnapping = false;
        _isScrolling = true;
        
        ResetContainerAnimations();
    }

    private void ResetContainerAnimations()
    {
        glowingEffect.SetActive(false);
        
        foreach (var container in _containers)
        {
            container.ShowAsNeutral();
        }
    }
}