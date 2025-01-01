using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DeckEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [SerializeField] private VisualTreeAsset listItemPrefab;

    private ListView deckList;
    private PlayerDeckScript deck;

    [MenuItem("Custom/DeckEditor")]
    public static void ShowExample()
    {
        DeckEditor wnd = GetWindow<DeckEditor>();
        wnd.titleContent = new GUIContent("Deck Editor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        deckList = root.Query<ListView>("CardsList");
        listItemPrefab = Resources.Load<VisualTreeAsset>("UI/DeckItem");

        if (!EditorApplication.isPlaying)
        {
            VisualElement label = new Label("Please enter play mode!");
            label.Add(root);
            return;
        }

        deck = FindObjectOfType<PlayerDeckScript>();

        if (deck == null)
        {
            VisualElement label = new Label("No Player deck found!");
            label.Add(root);
            return;
        }

        deckList.makeItem = () =>
        {
            var newListEntry = listItemPrefab.Instantiate();
            var listEntryLogic = new DeckItemController();
            newListEntry.userData = listEntryLogic;
            listEntryLogic.SetVisualElement(newListEntry);
            return newListEntry;
        };
        deckList.bindItem = (e, i) =>
        {
            (e.userData as DeckItemController)?.SetCardData(deck.Deck[i]);
        };
        deckList.itemsSource = deck.Deck;

        Debug.Log("Update");
    }
}

public class DeckItemController
{
    private Label nameLabel;

    public void SetVisualElement(VisualElement visualElement)
    {
        nameLabel = visualElement.Q<Label>();
    }

    public void SetCardData(Card card)
    {
        nameLabel.text = card.name + " - " + card.Power;
    }
}
