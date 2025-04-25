using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public bool isBot;
    public float distanceBetweenCard = 0.5f;
    GameObject[] childObjects;
    public bool isSidePlayer;

    [Header("得分")]
    public int point;

    [Header("雙紅五得分")]
    public bool DoubleRedFive;

    [Header("得分UI")]
    [SerializeField] TextMeshProUGUI textMeshPro;

    private void Update()
    {

        SetPlayerInfo();
    }

    public void ResetPlayerCardPos()
    {
        GetCurrentChild();
        SpreadObjects();
        foreach(GameObject gameObject in childObjects)
        {
            Card card = gameObject.GetComponent<Card>();
            card.AdjustCardScale();
        }
    }


    /// <summary>
    /// 調整牌位置
    /// </summary>
    private void SpreadObjects()
    {
        if (childObjects == null)
            return;
        Vector3 startPosition = this.transform.position;


        //正面玩家
        if(!isSidePlayer)
        {
            if (childObjects.Length % 2 == 0)
            {
                startPosition.x = this.transform.position.x - childObjects.Length / 2 * distanceBetweenCard + distanceBetweenCard / 2;
            }
            else
            {
                startPosition.x = this.transform.position.x - (childObjects.Length - 1) / 2 * distanceBetweenCard;
            }
            for (int i = 0; i < childObjects.Length; i++)
            {
                Vector3 newPosition = new Vector3();
                newPosition = this.transform.position;
                newPosition.x = startPosition.x + i * distanceBetweenCard;
                childObjects[i].transform.position = newPosition;

            }
        }
        else
        {
            if (childObjects.Length % 2 == 0)
            {
                startPosition.z = this.transform.position.z - childObjects.Length / 2 * distanceBetweenCard + distanceBetweenCard / 2;
            }
            else
            {
                startPosition.z = this.transform.position.z - (childObjects.Length - 1) / 2 * distanceBetweenCard;
            }
            for (int i = 0; i < childObjects.Length; i++)
            {
                Vector3 newPosition = new Vector3();
                newPosition = this.transform.position;
                newPosition.z = startPosition.z + i * distanceBetweenCard;
                childObjects[i].transform.position = newPosition;

            }
        }
       
        


    }

    /// <summary>
    /// 取得玩家當前子物件
    /// </summary>
    private void GetCurrentChild()
    {
        childObjects = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            childObjects[i] = transform.GetChild(i).gameObject;
        }

    }

    /// <summary>
    /// 玩家得分刷新
    /// </summary>
    void SetPlayerInfo()
    {
        textMeshPro.text = gameObject.name + " Score: " + point.ToString();
    }


}