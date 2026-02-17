using AxGrid.Base;
using UnityEngine;

public class ScrollView : MonoBehaviourExt
{
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private float scrollSpeed = 200f;
    [SerializeField] private float snapSpeed = 5f;
    
    [SerializeField] private ParticleSystem oneTimeEffect;
    [SerializeField] private GameObject glowingEffect;
    
    private ScrollContainer[] containers;
    private float containerHeight;
    
    private bool isSnapping = false;
    private bool isScrolling = false;
    
    [OnStart]
    private void InitializeItems()
    {
        containers = contentPanel.GetComponentsInChildren<ScrollContainer>();
        
        if (containers.Length == 0) 
        {
            Debug.LogError("Empty Content Panel");
            return;
        }
        
        containerHeight = containers[0].GetHeight();
        PositionContainersInitially();
    }
    
    private void PositionContainersInitially()
    {
        for (var i = 0; i < containers.Length; i++)
        {
            var yPos = -i * containerHeight;
            containers[i].AnchoredPosition = new Vector2(0, yPos);
        }
    }
    
    [OnUpdate]
    private void MoveContainers()
    {
        if (isSnapping || !isScrolling) return;

        foreach (var item in containers)
        {
            var pos = item.AnchoredPosition;
            pos.y -= scrollSpeed * Time.deltaTime;
            
            item.AnchoredPosition = pos;
            if (item.AnchoredPosition.y < -containerHeight * 2) 
            {
                RepositionItemToTop(item);
            }
        }
    }
    
    [OnUpdate]
    private void TrySnapContainerToCenter()
    {
        if (!isSnapping) return;

        var targetContainer = FindClosestContainerToCenter();
        if (targetContainer == null) return;

        CalculateContainersPositions(targetContainer);

        if (!IsTargetSnapped(targetContainer)) return;
        
        CompleteSnapping();
        targetContainer.PlayWinAnimation();
    }

    private void CalculateContainersPositions(ScrollContainer target)
    {
        var oldY = target.AnchoredPosition.y;
        var newY = Mathf.Lerp(oldY, 0, snapSpeed * Time.deltaTime);
        
        target.AnchoredPosition = new Vector2(target.AnchoredPosition.x, newY);
    
        var deltaY = newY - oldY;

        foreach (var container in containers)
        {
            if (container == target) continue;
            var pos = container.AnchoredPosition;
            pos.y += deltaY;
            container.AnchoredPosition = pos;
        }
    }
    
    private void RepositionItemToTop(ScrollContainer item)
    {
        var highestY = float.MinValue;
        
        foreach (var otherItem in containers)
        {
            if (otherItem.AnchoredPosition.y > highestY)
                highestY = otherItem.AnchoredPosition.y;
        }
    
        item.AnchoredPosition = new Vector2(item.AnchoredPosition.x, highestY + containerHeight);
    }
    
    
    private ScrollContainer FindClosestContainerToCenter()
    {
        ScrollContainer target = null;
        var minDist = float.MaxValue;
        
        foreach (var item in containers)
        {
            var dist = Mathf.Abs(item.AnchoredPosition.y);
            
            if (!(dist < minDist)) continue;
            
            minDist = dist;
            target = item;
        }

        return target;
    }
    
    private void CompleteSnapping()
    {
        isSnapping = false;
        AdjustContainersPosition();
        
        oneTimeEffect.Play();
        glowingEffect.SetActive(true);
    }

    private void AdjustContainersPosition()
    {
        foreach (var container in containers)
        {
            var pos = container.AnchoredPosition;
            pos.y = Mathf.Round(pos.y / containerHeight) * containerHeight;
            container.AnchoredPosition = pos;
        }
    }

    private static bool IsTargetSnapped(ScrollContainer target)
    {
        return Mathf.Abs(target.AnchoredPosition.y) < 0.1f;
    }

    public void StopScrolling()
    {
        isSnapping = true;
        isScrolling = false;
    }

    public void StartScrolling()
    {
        isSnapping = false;
        isScrolling = true;
        
        ResetContainerAnimations();
    }

    private void ResetContainerAnimations()
    {
        glowingEffect.SetActive(false);
        
        foreach (var container in containers)
        {
            container.PlayIdleAnimation();
        }
    }
}