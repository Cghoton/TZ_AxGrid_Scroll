using UnityEngine;
using AxGrid.Base;
using AxGrid.Model;

public class ScrollController : MonoBehaviourExt
{
    [SerializeField] private ScrollView scrollView;
    [SerializeField] private bool isScrolling = true;
    
    private AsyncEventManager asyncEventManager;

    private const string StartButtonEnable = "BtnStartButtonEnable";
    private const string StopButtonEnable = "BtnStopButtonEnable";
    
    [OnStart]
    private void Initialize()
    {
        Model.EventManager
            .AddAction("OnStartButtonClick", OnStartButton);
        Model.EventManager
            .AddAction("OnStopButtonClick", OnStopButton);
    }

    private void OnStartButton()
    {
        scrollView.StartScrolling();
        ResolveButtonsWithDelay(StartButtonEnable, StopButtonEnable);
    }

    private void OnStopButton()
    {
        scrollView.StopScrolling();
        ResolveButtonsWithDelay(StopButtonEnable, StartButtonEnable);
    }

    private void ResolveButtonsWithDelay(string btnToDeactivate, string btnToActivate)
    {
        Path.Action(() => Model.Set(btnToDeactivate, false))
            .Wait(1f)
            .Action(() =>
            {
                Model.Set(btnToActivate, true);
            });
    }
}
