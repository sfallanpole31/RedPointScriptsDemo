using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

public class DeckManager : MonoBehaviour
{
    [Header("牌組物件")]
    public GameObject deckGameObject;
    [Header("展示牌物件")]
    public GameObject ShowingCardGameObject;
    [Header("使用過牌物件")]
    public GameObject UsedCardGameObject;
    [Header("資訊顯示Text")]
    public GameObject InfoText;
    [Header("重來按鈕")]
    public GameObject RestartButton;

    public GameObject[] Players = new GameObject[4]; // 玩家位置


    public Card[] publicCards; //公共牌組
    public Card[] showingCards; //可吃牌

    [Header("當前牌數")]
    public int CurrentCardCount = 4;

    public Transform centerPoint; // 牌組的中心點
    public float radius = 2f; // 半徑，用於調整牌距離中心的遠近
    public GameObject targetPosition; // 卡牌要飛到的空中位置
                                      // 用於檢查是否已經在進行 EndGame 協程
    private bool isEndingGame = false;
    public TextMeshProUGUI textMeshPro;

    CardMatcher cardMatcher;

    private void Start()
    {
        textMeshPro = InfoText.GetComponent<TextMeshProUGUI>();
        cardMatcher = GetComponent<CardMatcher>();
        SendCard(6, Players[0]);
        SendCard(6, Players[1]);
        SendCard(6, Players[2]);
        SendCard(6, Players[3]);
        UpdatePublicCards(CurrentCardCount);
        Players[0].GetComponent<PlayerController>().ResetPlayerCardPos();
        Players[1].GetComponent<PlayerController>().ResetPlayerCardPos();
        Players[2].GetComponent<PlayerController>().ResetPlayerCardPos();
        Players[3].GetComponent<PlayerController>().ResetPlayerCardPos();

    }


    private void Update()
    {

        // 確保在場景中有 GameObject 和 PlayerController
        if (publicCards.Count() == 0)
        {
            // 檢查是否已經在進行 EndGame 協程以避免重複觸發
            if (!isEndingGame)
            {
                StartCoroutine(EndGame());
                isEndingGame = true; // 防止協程重複啟動
            }
        }
    }
    /// <summary>
    /// 結束遊戲
    /// </summary>
    private IEnumerator EndGame()
    {

        //檢測有雙紅5 需做加減分
        bool hasDoubleRedFive = false; // 用來記錄是否有任何玩家的 DoubleRedFive 為 true
        // 遍歷所有玩家，檢查 DoubleRedFive 並加分
        foreach (GameObject player in Players)
        {
            PlayerController controller = player.GetComponent<PlayerController>();

            if (controller != null && controller.DoubleRedFive)
            {
                controller.point += 60;
                hasDoubleRedFive = true; // 有玩家的 DoubleRedFive 為 true
            }
        }
        // 如果有玩家的 DoubleRedFive 為 true，才進行其他玩家的 -20 處理
        if (hasDoubleRedFive)
        {
            foreach (GameObject player in Players)
            {
                PlayerController controller = player.GetComponent<PlayerController>();

                if (controller != null && !controller.DoubleRedFive)
                {
                    controller.point -= 20;
                }
            }
        }

        //顯示 哪位玩家獲勝

        GameObject highestPointPlayer = Players
                                        .OrderByDescending(player => player.GetComponent<PlayerController>().point)
                                        .FirstOrDefault();

        if(Players[0]!= highestPointPlayer)
        {
            textMeshPro.text = "You lose!!";
        }
        else
        {
            textMeshPro.text = "You Win!!";
        }


        yield return new WaitForSeconds(2f);


        //跳出button再來一局
        RestartButton.SetActive(true);
    }

    // 重新開始遊戲的方法
    public void RestartGame()
    {
        // 獲取當前場景名稱
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 重新加載當前場景
        SceneManager.LoadScene(currentSceneName);
    }

    /// <summary>
    /// 發牌
    /// </summary>
    /// <param name="publicCards">公共排組</param>
    /// <param name="numberOfCards">幾張牌</param>
    /// <param name="player">哪個玩家</param>
    private void SendCard(int numberOfCards, GameObject player)
    {
        publicCards = deckGameObject.GetComponentsInChildren<Card>();
        // 需要保证 publicCards 数组中有足够的卡牌
        if (publicCards.Length < numberOfCards)
        {
            Debug.LogWarning("Not enough cards to distribute.");
            return;
        }

        // 选择的卡牌数组
        Card[] selectedCards = new Card[numberOfCards];
        int selectedCount = 0;

        // 随机选择卡牌
        while (selectedCount < numberOfCards)
        {
            int randomIndex = Random.Range(0, publicCards.Length);

            // 选择的卡牌
            Card card = publicCards[randomIndex];

            // 将选中的卡牌添加到选中数组中
            selectedCards[selectedCount] = card;
            selectedCount++;

            publicCards[randomIndex].gameObject.transform.SetParent(ShowingCardGameObject.transform);//更換父物件
            publicCards = deckGameObject.GetComponentsInChildren<Card>();
        }

        // 将选中的卡牌分配给玩家
        foreach (Card card in selectedCards)
        {
            if (card != null) // 可能出现 null
            {
                Transform targetPosition = player.transform;

                card.transform.position = targetPosition.position;
                card.transform.rotation = targetPosition.rotation;

                card.Owner = player;
                card.transform.SetParent(player.transform);
            }
        }

    }

    public Card DrawNewCard()
    {
        // 從公共牌抽一張出來
        publicCards = deckGameObject.GetComponentsInChildren<Card>();
        int randomIndex = Random.Range(0, publicCards.Length);
        // 选择的卡牌
        Card card = publicCards[randomIndex];

        // 放在空中動畫，飛在空中停三秒
        StartCoroutine(MoveCard(card.gameObject,targetPosition, 1.0f));
        print("抽牌抽出" + card.ToString());
        return card;

    }


    public void UpdatePublicCards(int numberOfCards)
    {
        publicCards = deckGameObject.GetComponentsInChildren<Card>();
        showingCards = ShowingCardGameObject.GetComponentsInChildren<Card>();


        // 选择的卡牌数组
        while (showingCards.Length != numberOfCards)
        {
            int randomIndex = Random.Range(0, publicCards.Length);
            // 选择的卡牌
            Card card = publicCards[randomIndex];

            // 将已选择的卡牌从 publicCards 中移除
            publicCards[randomIndex].gameObject.transform.SetParent(ShowingCardGameObject.transform);//更換父物件
            publicCards = deckGameObject.GetComponentsInChildren<Card>();
            showingCards = ShowingCardGameObject.GetComponentsInChildren<Card>();
        }



        int cardCount = showingCards.Length;
        // 計算每張牌應該放置的位置
        for (int i = 0; i < cardCount; i++)
        {
            // 動態計算每張牌的角度
            float angle = 360f / cardCount * i;

            // 將角度轉換為弧度，以便計算 X 和 Y 坐標
            float radians = angle * Mathf.Deg2Rad;

            // 計算牌的位置，基於圓形佈局
            Vector3 cardPosition = new Vector3(
                centerPoint.position.x + Mathf.Cos(radians) * radius,
                centerPoint.position.y,
                centerPoint.position.z + Mathf.Sin(radians) * radius
            );

            // 設定每張卡牌的位置和旋轉
            showingCards[i].transform.position = cardPosition;
            showingCards[i].transform.LookAt(centerPoint); // 使牌面朝向中心點
        }

    }

    /// <summary>
    ///  選擇可匹配的得分最高的牌
    /// </summary>
    /// <returns></returns>
    public Card GetHighestScoringMatch(Card selectedOwnCard)
    {
        showingCards = ShowingCardGameObject.GetComponentsInChildren<Card>();
        Card[] publicCards = showingCards;
        Card bestMatch = null;

        foreach (Card card in publicCards)
        {
            if (cardMatcher.AreCardsMatch(selectedOwnCard, card))
            {
                if (bestMatch == null || card.Point > bestMatch.Point)
                {
                    bestMatch = card;
                }

                if(selectedOwnCard.Point ==5 & card.Point == 5)
                {
                    selectedOwnCard.Owner.GetComponent<PlayerController>().DoubleRedFive = true;
                }
            }
        }
        return bestMatch; // 如果沒有匹配的牌，bestMatch 會是 null
    }


    public IEnumerator MoveCard(GameObject startPosition, GameObject endPosition, float moveDuration)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = startPosition.transform.position;
        Vector3 targetPosition = endPosition.transform.position;

        while (elapsedTime < moveDuration)
        {
            // 逐渐更新 elapsedTime
            elapsedTime += Time.deltaTime;

            // 使用 Lerp 计算新的位置
            startPosition.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / moveDuration);

            // 等待下一帧继续
            yield return null;
        }
        // 确保最后的位置准确
        startPosition.transform.position = targetPosition;
    }


}
