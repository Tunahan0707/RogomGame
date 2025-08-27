using UnityEngine;

public class MarketRoom : MonoBehaviour
{
    void Awake()
    {
        GameSceneManager.OnContinueButtonClicked += ContinueButtonClicked;
    }
    private void ContinueButtonClicked()
    {
        if (RandomRoomSelector.selectedRoom == RoomType.Market)
        {
            // Implement market room logic here
        }
    }
}
