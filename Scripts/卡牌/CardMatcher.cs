using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMatcher : MonoBehaviour
{

    // 检查两张卡片是否符合吃牌逻辑
    public bool AreCardsMatch(Card playCard, Card publicCard)
    {
        if (playCard == null | publicCard == null)
            return false;

        if(IsSmallPoint(playCard.Rank))//數字為1~9
        {
            if (playCard.Rank + publicCard.Rank == 10)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else //數字10~13
        {
            if (playCard.Rank == publicCard.Rank)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }



    public bool IsSmallPoint(int point)
    {
        return point <= 9;
    }

}
