using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReserveSlot : MonoBehaviour
{
    [SerializeField] private Card[] cardPrefabs; // Array of card prefabs to choose from

    [SerializeField] private GameObject dropzone;

    private void Awake()
    {
        // Add the starting card to the dropzone slot
        int randomAttribute = Random.Range(0, cardPrefabs.Length);
        int randomPower = Random.Range(1, 6);

        Card startingCard = Instantiate(cardPrefabs[randomAttribute]);
        startingCard.Power = randomPower;

        startingCard.transform.SetParent(dropzone.transform, false); // Parent it to the DropzoneSlot
        startingCard.isReserveCard = true;
    }
}
