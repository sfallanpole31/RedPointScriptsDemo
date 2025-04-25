using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    [SerializeField] int currentPlayerIndex = 0; // 當前回合的玩家索引
    //private bool hasPlayedCard = false; // 當前玩家是否已出牌
    //private bool hasDrawnCard = false;  // 當前玩家是否已抽牌

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
        // 結束當前玩家回合並輪到下一位玩家
        cardSelector.IsAction = false;
        cardSelector.isActivePlayer = false;
        //hasPlayedCard = false;
        //hasDrawnCard = false;
        currentPlayerIndex = (currentPlayerIndex + 1) % 4;
        Debug.Log("輪到玩家：" + currentPlayerIndex);
        deckManager.Players[currentPlayerIndex].GetComponent<CardSelector>().isActivePlayer = true;
    }


}

