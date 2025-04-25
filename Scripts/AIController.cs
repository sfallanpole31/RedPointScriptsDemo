using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    CardSelector cardSelector;
    DeckManager deckManager;

    public bool isChoosingCard = false; // 用來追蹤是否正在選牌
    public bool isThrowing = false;     // 用來追蹤是否正在丟牌

    private void Start()
    {
        cardSelector = gameObject.GetComponent<CardSelector>();
        deckManager = cardSelector.deckManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (cardSelector.isActivePlayer && cardSelector.selectedOwnCard == null && !isChoosingCard ) // 當前玩家行動，且尚未選牌和丟牌
        {
            
            ChooseCard(); // 只執行一次選牌
            if (cardSelector.selectedOwnCard == null)
                isChoosingCard = false;
            else
                isChoosingCard = true;
        }

        // 如果已經選擇了牌，開始丟牌
        if (isChoosingCard && cardSelector.selectedOwnCard != null && !isThrowing)
        {
            isThrowing = true; // 開始丟牌
            StartCoroutine(Move()); // 丟牌過程
            print("AI丟牌了 " + cardSelector.selectedOwnCard.ToString() );
        }

    }

    IEnumerator Move()
    {
        StartCoroutine(cardSelector.ThrowCard());
        yield return new WaitForSeconds(3.0f);

        // 完成丟牌後重置狀態
        //isThrowing = false;
        //isChoosingCard = false; // 重置選牌狀態，以便下一回合可以再次選牌
    }


    /// <summary>
    /// 選擇最符合邏輯的牌
    /// </summary>
    void ChooseCard()
    {
        Card[] cardsYouOwn = cardSelector.gameObject.GetComponentsInChildren<Card>();
        Card[] showingCards = deckManager.ShowingCardGameObject.GetComponentsInChildren<Card>();


        Card bestCard1 = null;
        Card bestCard2 = null;
        int highestPoints = 0;

        // 遍歷你擁有的牌
        foreach (var card1 in cardsYouOwn)
        {
            // 遍歷展示的牌
            foreach (var card2 in showingCards)
            {
                // 檢查兩張牌的 Rank 條件
                if (card1.Rank < 10 && card2.Rank < 10)
                {
                    // 如果兩張牌的 Rank 小於 10，它們的 Rank 相加應為 10
                    if (card1.Rank + card2.Rank == 10)
                    {
                        int totalPoints = card1.Point + card2.Point;
                        if (totalPoints > highestPoints)
                        {
                            highestPoints = totalPoints;
                            bestCard1 = card1;
                            bestCard2 = card2;
                        }
                    }
                }
                else if (card1.Rank >= 10 && card2.Rank >= 10)
                {
                    // 如果兩張牌的 Rank 大於等於 10，它們的 Rank 必須相等
                    if (card1.Rank == card2.Rank)
                    {
                        int totalPoints = card1.Point + card2.Point;
                        if (totalPoints > highestPoints)
                        {
                            highestPoints = totalPoints;
                            bestCard1 = card1;
                            bestCard2 = card2;
                        }
                    }
                }
            }
        }

        // 如果沒有選到符合條件的牌，選擇 Rank 和 Point 最低的牌
        if (bestCard1 == null)
        {
            Card lowestCard = cardsYouOwn[0];
            foreach (var card in cardsYouOwn)
            {
                // 比較 Point，Point 更低的優先
                if (card.Point < lowestCard.Point)
                {
                    lowestCard = card;
                }
                // 如果 Point 相同，則比較 Rank
                else if (card.Point == lowestCard.Point && card.Rank < lowestCard.Rank)
                {
                    lowestCard = card;
                }
            }
            bestCard1 = lowestCard;
        }

        cardSelector.selectedOwnCard = bestCard1;

    }
}
