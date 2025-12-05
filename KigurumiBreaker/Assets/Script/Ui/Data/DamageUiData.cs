using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/DamageUiData")]
public class DamageUiData : ScriptableObject
{
    [Header("生存時間")]
    [SerializeField] private int LifeTime;

    [Header("最初の大きさ")]
    [SerializeField] private float StartScale;

    [Header("最後の大きさ")]
    [SerializeField] private float EndScale;


    // 読み取り専用プロパティ
    public int lifeTime => LifeTime;
    public float startScale => StartScale;
    public float endScale => EndScale;
}
