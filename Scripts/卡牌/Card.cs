using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Suit
{
    Hearts,   // 紅心
    Diamonds, // 方塊
    Clubs,    // 梅花
    Spades    // 黑桃
}

public class Card : MonoBehaviour
{
    [Header("數字")]
    public int Rank;

    [Header("分數")]
    public int Point;

    [Header("持有者")]
    public GameObject Owner;

    [Header("選中特效")]
    public GameObject ParticleSystem;

    bool isTurn;

    public void AdjustCardScale()
    {

        if (Owner == null)
        {
            return;
        }

        // 检查牌的持有者的 tag 是否是 "Player"
        if (Owner.tag == "Player")
        {
            // 增加 180 度到当前的 x 轴旋转
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        }
        else
        {
                transform.localEulerAngles = new Vector3(180f, 0.0f, 0.0f);

        }
    }

}
