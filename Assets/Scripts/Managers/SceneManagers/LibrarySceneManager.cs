using UnityEngine;

public class LibrarySceneManager : MonoBehaviour
{
    private LibrarySceneUI librarySceneUI => LibrarySceneUI.Instance;
    [SerializeField] private CardsDataBase cardsDatabase;

    private void Start()
    {
        foreach (var card in cardsDatabase.cards)
        {
            librarySceneUI.SpawnCard(card);
        }
    }
}
