using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    [SerializeField] int currentPlayerIndex = 0; // ��e�^�X�����a����
    //private bool hasPlayedCard = false; // ��e���a�O�_�w�X�P
    //private bool hasDrawnCard = false;  // ��e���a�O�_�w��P

    DeckManager deckManager;
    CardSelector cardSelector;

    private void Start()
    {
        deckManager = this.gameObject.GetComponent<DeckManager>();
        deckManager.Players[currentPlayerIndex].GetComponent<CardSelector>().isActivePlayer = true;
        cardSelector = deckManager.Players[currentPlayerIndex].GetComponent<CardSelector>();
    }

    void Update()
    {

            cardSelector = deckManager.Players[currentPlayerIndex].GetComponent<CardSelector>();
            if (cardSelector.IsAction)
            {
                EndTurn();
                if(cardSelector.gameObject.GetComponent<PlayerController>().isBot==true)
                {
                    cardSelector.selectedOwnCard = null;
                    cardSelector.selectedPublicCard = null;
                    cardSelector.gameObject.GetComponent<AIController>().isChoosingCard = false;
                    cardSelector.gameObject.GetComponent<AIController>().isThrowing = false;
                }
            }
        
    }

    
    private void EndTurn()
    {
        // ������e���a�^�X�ý���U�@�쪱�a
        cardSelector.IsAction = false;
        cardSelector.isActivePlayer = false;
        //hasPlayedCard = false;
        //hasDrawnCard = false;
        currentPlayerIndex = (currentPlayerIndex + 1) % 4;
        Debug.Log("���쪱�a�G" + currentPlayerIndex);
        deckManager.Players[currentPlayerIndex].GetComponent<CardSelector>().isActivePlayer = true;
    }


}

