using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    CardSelector cardSelector;
    DeckManager deckManager;

    public bool isChoosingCard = false; // �ΨӰl�ܬO�_���b��P
    public bool isThrowing = false;     // �ΨӰl�ܬO�_���b��P

    private void Start()
    {
        cardSelector = gameObject.GetComponent<CardSelector>();
        deckManager = cardSelector.deckManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (cardSelector.isActivePlayer && cardSelector.selectedOwnCard == null && !isChoosingCard ) // ��e���a��ʡA�B�|����P�M��P
        {
            
            ChooseCard(); // �u����@����P
            if (cardSelector.selectedOwnCard == null)
                isChoosingCard = false;
            else
                isChoosingCard = true;
        }

        // �p�G�w�g��ܤF�P�A�}�l��P
        if (isChoosingCard && cardSelector.selectedOwnCard != null && !isThrowing)
        {
            isThrowing = true; // �}�l��P
            StartCoroutine(Move()); // ��P�L�{
            print("AI��P�F " + cardSelector.selectedOwnCard.ToString() );
        }

    }

    IEnumerator Move()
    {
        StartCoroutine(cardSelector.ThrowCard());
        yield return new WaitForSeconds(3.0f);

        // ������P�᭫�m���A
        //isThrowing = false;
        //isChoosingCard = false; // ���m��P���A�A�H�K�U�@�^�X�i�H�A����P
    }


    /// <summary>
    /// ��̲ܳŦX�޿誺�P
    /// </summary>
    void ChooseCard()
    {
        Card[] cardsYouOwn = cardSelector.gameObject.GetComponentsInChildren<Card>();
        Card[] showingCards = deckManager.ShowingCardGameObject.GetComponentsInChildren<Card>();


        Card bestCard1 = null;
        Card bestCard2 = null;
        int highestPoints = 0;

        // �M���A�֦����P
        foreach (var card1 in cardsYouOwn)
        {
            // �M���i�ܪ��P
            foreach (var card2 in showingCards)
            {
                // �ˬd��i�P�� Rank ����
                if (card1.Rank < 10 && card2.Rank < 10)
                {
                    // �p�G��i�P�� Rank �p�� 10�A���̪� Rank �ۥ[���� 10
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
                    // �p�G��i�P�� Rank �j�󵥩� 10�A���̪� Rank �����۵�
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

        // �p�G�S�����ŦX���󪺵P�A��� Rank �M Point �̧C���P
        if (bestCard1 == null)
        {
            Card lowestCard = cardsYouOwn[0];
            foreach (var card in cardsYouOwn)
            {
                // ��� Point�APoint ��C���u��
                if (card.Point < lowestCard.Point)
                {
                    lowestCard = card;
                }
                // �p�G Point �ۦP�A�h��� Rank
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
