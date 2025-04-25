using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : MonoBehaviour
{
    [Header("選中的手牌")]
    public Card selectedOwnCard;
    [Header("選中的公共牌")]
    public Card selectedPublicCard;
    [Header("用過的卡牌放置物件")]
    public GameObject UsedCard;

    #region 選牌時放大往上移
    //紀錄原始位置與大小
    private Dictionary<Card, (Vector3 originalScale, Vector3 originalPosition)> cardStates
           = new Dictionary<Card, (Vector3 originalScale, Vector3 originalPosition)>();

    [Header("放大倍率")]
    public float scaleFactor = 1.3f;

    [Header("牌間距偏移輛")]
    public float yOffset = 3f;
    #endregion

    CardMatcher cardMatcher;
    public DeckManager deckManager;

    public bool IsAction; //是否已經操作
    public bool isActivePlayer = false; // 是否為當前玩家的回合

    private void Start()
    {
        cardMatcher = deckManager.GetComponent<CardMatcher>();
    }

    void Update()
    {
        if (this.gameObject.GetComponent<PlayerController>().isBot == true)
            return;

        if (!isActivePlayer) return; // 只有當前玩家才能行動


        // 檢查是否按下滑鼠左鍵
        if (Input.GetMouseButtonDown(0))
        {
            // 創建射線從滑鼠位置到場景中
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 繪制射線以便於在Scene視圖中查看
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.green, 2f);

            // 執行射線檢測
            if (Physics.Raycast(ray, out hit))
            {

                // 繪制從相機到檢測到物體的線
                Debug.DrawLine(ray.origin, hit.point, Color.red, 2f);


                if (hit.transform.gameObject.GetComponent<Card>() == null)
                    return;

                //點擊第二次丟牌
                if (selectedOwnCard == hit.transform.gameObject.GetComponent<Card>())
                {
                    StartCoroutine(ThrowCard());
                    return;
                }

                // 檢測選牌
                if (hit.transform.gameObject.GetComponent<Card>().Owner == this.gameObject)
                {
                    if (selectedOwnCard != null & hit.transform.gameObject.GetComponent<Card>() != selectedOwnCard)
                    {

                        UnSelectEffect(selectedOwnCard);
                    }
                    // 執行選擇牌的邏輯
                    SelectCard(hit.transform.gameObject);
                    SelectEffect(hit.transform.gameObject.GetComponent<Card>());
                }

            }
        }
    }


    void SelectCard(GameObject gameObject)
    {
        // 在這裡處理選擇牌的邏輯
        Debug.Log("選牌: " + gameObject.name);
        // 例如，改變牌的外觀或記錄選中狀態
        selectedOwnCard = gameObject.GetComponent<Card>();
    }



    public IEnumerator ThrowCard()
    {
        print("丟牌!!" + selectedOwnCard.ToString());
        selectedOwnCard.Owner.GetComponent<CardSelector>().isActivePlayer = false;
        GameObject resetOwner = selectedOwnCard.Owner;
        selectedOwnCard.transform.localEulerAngles = new Vector3(0, 0.0f, 0.0f);


        //判斷桌上是否有符合可吃的牌 
        Card bestCard = deckManager.GetHighestScoringMatch(selectedOwnCard);
        if (bestCard == null)
        {
            //若沒有 丟上公共排區
            print("牌不匹配!! 丟至公共牌區 " + selectedOwnCard);
            UnSelectEffect(selectedOwnCard);

            //將丟出的牌移至公共排區
            StartCoroutine(deckManager.MoveCard(selectedOwnCard.gameObject, deckManager.gameObject, 0.7f));
            yield return new WaitForSeconds(1f);
            selectedOwnCard.Owner = null;
            selectedOwnCard.gameObject.transform.SetParent(deckManager.ShowingCardGameObject.transform);
            deckManager.UpdatePublicCards(++deckManager.CurrentCardCount);
            selectedOwnCard = null;
            resetOwner.GetComponent<PlayerController>().ResetPlayerCardPos();

            //執行抽牌
            selectedOwnCard = deckManager.DrawNewCard();
            selectedOwnCard.Owner = this.gameObject;
            yield return new WaitForSeconds(3.0f);

            //判斷新抽的牌與桌上的牌 有無匹配
            bestCard = deckManager.GetHighestScoringMatch(selectedOwnCard);
            //有 執行吃牌 
            if (bestCard != null)
            {
                //若有 執行手牌移到可吃牌的動畫
                selectedPublicCard = bestCard;
                StartCoroutine(deckManager.MoveCard(selectedOwnCard.gameObject, selectedPublicCard.gameObject, 0.7f));
                StartCoroutine(GetScore());
                yield return new WaitForSeconds(1f);

                //轉移到使用過的牌區
                selectedOwnCard.gameObject.transform.SetParent(UsedCard.transform);
                selectedPublicCard.Owner = this.gameObject;
                selectedPublicCard.gameObject.transform.SetParent(UsedCard.transform);

                yield return null;

                // 將 position 設置為零
                selectedOwnCard.gameObject.transform.localPosition = Vector3.zero;
                selectedPublicCard.gameObject.transform.localPosition = Vector3.zero;

                //更新公共牌數量
                deckManager.CurrentCardCount -= 1;
                deckManager.UpdatePublicCards(deckManager.CurrentCardCount);
                print("得分!" + selectedOwnCard.ToString() + "+" + selectedPublicCard.ToString());
            }
            else
            {
                //若沒有 執行牌到公共牌區的動畫
                StartCoroutine(deckManager.MoveCard(selectedOwnCard.gameObject, deckManager.gameObject, 0.7f));
                yield return new WaitForSeconds(1f);

                //將丟出的牌移至公共排區
                selectedOwnCard.Owner = null;
                selectedOwnCard.gameObject.transform.SetParent(deckManager.ShowingCardGameObject.transform);
                deckManager.UpdatePublicCards(++deckManager.CurrentCardCount);
                selectedOwnCard = null;
            }
            IsAction = true;
            yield break; // 使用yield break結束協程
        }
        else
        {
            //若有 執行手牌移到可吃牌的動畫
            UnSelectEffect(selectedOwnCard);
            selectedPublicCard = bestCard;
            StartCoroutine(deckManager.MoveCard(selectedOwnCard.gameObject, selectedPublicCard.gameObject, 1.0f));
            StartCoroutine(GetScore());
            yield return new WaitForSeconds(1f);


            //轉移到使用過的牌區
            selectedOwnCard.gameObject.transform.SetParent(UsedCard.transform);
            selectedPublicCard.Owner = this.gameObject;
            selectedPublicCard.gameObject.transform.SetParent(UsedCard.transform);

            yield return null;

            // 將 position 設置為零
            selectedOwnCard.gameObject.transform.localPosition = Vector3.zero;
            selectedPublicCard.gameObject.transform.localPosition = Vector3.zero;

            //更新公共牌數量
            deckManager.CurrentCardCount -= 1;
            deckManager.UpdatePublicCards(deckManager.CurrentCardCount);
            print("得分!" + selectedOwnCard.ToString() + "+" + selectedPublicCard.ToString());

            resetOwner.GetComponent<PlayerController>().ResetPlayerCardPos();

            //執行抽牌
            selectedOwnCard = deckManager.DrawNewCard();
            selectedOwnCard.Owner = this.gameObject;
            yield return new WaitForSeconds(3.0f);

            //判斷新抽的牌與桌上的牌 有無匹配
            bestCard = deckManager.GetHighestScoringMatch(selectedOwnCard);
            //有 執行吃牌 
            if (bestCard != null)
            {
                //若有 執行手牌移到可吃牌的動畫
                selectedPublicCard = bestCard;
                StartCoroutine(deckManager.MoveCard(selectedOwnCard.gameObject, selectedPublicCard.gameObject, 0.7f));
                StartCoroutine(GetScore());
                yield return new WaitForSeconds(1f);

                //轉移到使用過的牌區
                selectedOwnCard.gameObject.transform.SetParent(UsedCard.transform);
                selectedPublicCard.Owner = this.gameObject;
                selectedPublicCard.gameObject.transform.SetParent(UsedCard.transform);

                yield return null;

                // 將 position 設置為零
                selectedOwnCard.gameObject.transform.localPosition = Vector3.zero;
                selectedPublicCard.gameObject.transform.localPosition = Vector3.zero;

                //更新公共牌數量
                deckManager.CurrentCardCount -= 1;
                deckManager.UpdatePublicCards(deckManager.CurrentCardCount);
                print("得分!" + selectedOwnCard.ToString() + "+" + selectedPublicCard.ToString());
            }
            else
            {
                StartCoroutine(deckManager.MoveCard(selectedOwnCard.gameObject, deckManager.gameObject, 0.7f));
                yield return new WaitForSeconds(1f);

                //將丟出的牌移至公共排區
                selectedOwnCard.Owner = null;
                selectedOwnCard.gameObject.transform.SetParent(deckManager.ShowingCardGameObject.transform);
                // 等待一幀
                yield return null;
                deckManager.UpdatePublicCards(++deckManager.CurrentCardCount);
                selectedOwnCard = null;
            }

            IsAction = true;

            yield break; // 使用yield break結束協程
        }
    }

    /// <summary>
    /// 得分
    /// </summary>
    private IEnumerator GetScore()
    {
        int GetPoint = selectedOwnCard.Point + selectedPublicCard.Point;
        selectedOwnCard.Owner.GetComponent<PlayerController>().point += GetPoint;
        if (selectedOwnCard.Point == 5 & selectedPublicCard.Point == 5)
            deckManager.textMeshPro.text = "double red 5 ! +70";
        else
            deckManager.textMeshPro.text = "+" + GetPoint.ToString();
        yield return new WaitForSeconds(1f);
        deckManager.textMeshPro.text = null;
    }

    void SelectEffect(Card card)
    {
        // 如果卡片已经存在于字典中，则返回
        if (cardStates.ContainsKey(card))
        {
            return;
        }

        // 存储原始状态
        var originalScale = card.transform.localScale;
        var originalPosition = card.transform.position;
        cardStates[card] = (originalScale, originalPosition);

        // 选择特效、放大
        var cardComponent = card.GetComponent<Card>();
        if (cardComponent != null)
        {
            cardComponent.ParticleSystem.SetActive(true);
        }

        // 放大物件
        card.transform.localScale = originalScale * scaleFactor;
        // 在 Y 轴上增加
        card.transform.localPosition = new Vector3(card.transform.localPosition.x, card.transform.localPosition.y + 0.06f, card.transform.localPosition.z);

    }

    void UnSelectEffect(Card card)
    {
        if (cardStates.TryGetValue(card, out var state))
        {
            // 恢复原始状态
            card.transform.localScale = state.originalScale;
            card.transform.position = state.originalPosition;

            // 关闭特效
            var cardComponent = card.GetComponent<Card>();
            if (cardComponent != null)
            {
                cardComponent.ParticleSystem.SetActive(false);
            }

            // 从字典中移除卡片
            cardStates.Remove(card);
        }
    }

    //void OnCardDrawn(Card card)
    //{
    //    selectedOwnCard = card;
    //    card.Owner = this.gameObject;
    //}

    //IEnumerator MoveCard(GameObject selectOwnCard,GameObject selectPublicCard,float moveDuration)
    //{
    //    float elapsedTime = 0f;
    //    Vector3 startingPosition = selectOwnCard.transform.position;
    //    Vector3 targetPosition = selectPublicCard.transform.position;

    //    while (elapsedTime < moveDuration)
    //    {
    //        // 逐渐更新 elapsedTime
    //        elapsedTime += Time.deltaTime;

    //        // 使用 Lerp 计算新的位置
    //        selectedOwnCard.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / moveDuration);

    //        // 等待下一帧继续
    //        yield return null;
    //    }
    //    // 确保最后的位置准确
    //    selectedOwnCard.transform.position = targetPosition;
    //}

}

