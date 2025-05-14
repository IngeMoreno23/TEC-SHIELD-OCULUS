using UnityEngine;
using UnityEngine.UIElements;

public class Game1Panel : MonoBehaviour
{
    [SerializeField]
    private UIDocument m_UIDocument;

    [SerializeField]
    private GameManager1 m_GameManager;

    [SerializeField]
    private VisualTreeAsset m_Game1PanelAsset;


    private void OnEnable()
    {
        var root = m_UIDocument.rootVisualElement;

        var button = root.Q<Button>("start-button");

        button?.RegisterCallback<ClickEvent>(StartGame);
    }

    private void StartGame(ClickEvent evt)
    {
        Debug.Log("Start Game button clicked");
        if (m_GameManager == null) return;
        if (!m_GameManager.AbleToStart) return;

        StartCoroutine(m_GameManager.StartGame());

        var root = m_UIDocument.rootVisualElement;
        var rootContainer = root.Q<VisualElement>("root-container");

        rootContainer.Clear();

        var game1Panel = m_Game1PanelAsset.CloneTree();
        game1Panel.style.flexGrow = 1;
        game1Panel.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
        game1Panel.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        rootContainer.Add(game1Panel);
    }


}