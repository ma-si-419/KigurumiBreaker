using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerStateDataList")]
public class PlayerStateDataList : ScriptableObject
{
    [Header("Stateごとの情報")]
    [SerializeField] private List<PlayerStateData> _stateDataList = new List<PlayerStateData>();

    public List<PlayerStateData> StateDataList => _stateDataList;

#if UNITY_EDITOR
    // StateTypeに合わせてリストを自動調整
    private void OnValidate()
    {
        var enumValues = (PlayerState.StateKind[])Enum.GetValues(typeof(PlayerState.StateKind));

        // 追加されていないStateを補完
        foreach (var value in enumValues)
        {
            if (!_stateDataList.Exists(s => s.stateKind == value))
            {
                // Stateの情報を設定
                PlayerStateData stateData = new PlayerStateData();
                stateData.SetStateKind(value);

                // リストに追加
                _stateDataList.Add(stateData);
            }
        }

        // Enumから削除されたStateを削除
        _stateDataList.RemoveAll(s => Array.IndexOf(enumValues, s.stateKind) < 0);

        // ソート（Enum順で見やすく）
        _stateDataList.Sort((a, b) => a.stateKind.CompareTo(b.stateKind));

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}

[Serializable]
public class PlayerStateData
{
    [Header("どのStateか")]
    [HideInInspector][SerializeField] PlayerState.StateKind StateKind;

    [Header("回避できるかどうか")]
    [SerializeField] private bool AbleToDodge;

    [Header("攻撃可能かどうか")]
    [SerializeField] private bool AbleToAttack;

    [Header("特殊攻撃可能かどうか")]
    [SerializeField] private bool AbleToSpecialAttack;

    // Editorから書き込めるように
    public void SetStateKind(PlayerState.StateKind kind)
    {
        StateKind = kind;
    }

    // 読み取り専用
    public PlayerState.StateKind stateKind => StateKind;
    public bool ableToDodge => AbleToDodge;
    public bool ableToAttack => AbleToAttack;
    public bool ableToSpecialAttack => AbleToSpecialAttack;
}
