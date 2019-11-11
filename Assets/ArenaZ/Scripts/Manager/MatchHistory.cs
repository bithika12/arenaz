using UnityEngine;
using UnityEngine.UI;
using ArenaZ.Manager;
using ArenaZ.Screens;

[RequireComponent(typeof(UIScreen))]
public class MatchHistory : MonoBehaviour
{
    [SerializeField] private Button closeButton;

    void Start()
    {
        GettingButtonReferences();
    }

    private void OnDestroy()
    {
        ReleaseButtonReferences();
    }

    private void GettingButtonReferences()
    {
        closeButton.onClick.AddListener(OnClickClose);
    }

    private void ReleaseButtonReferences()
    {
        closeButton.onClick.RemoveListener(OnClickClose);
    }

    private void OnClickClose()
    {
        UIManager.Instance.HideScreen(Page.PlayerMatchHistory.ToString());
    }
}
