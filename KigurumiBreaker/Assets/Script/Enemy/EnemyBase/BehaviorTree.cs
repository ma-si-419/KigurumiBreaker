using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ビヘイビアツリーのノードの基底クラス
public abstract class BhaiviorTree
{
    // ノードを実行する抽象メソッド
    public abstract bool Tick();
}

// 行動ノード
public class ActionNode : BhaiviorTree
{
    // 行動を実行する関数
    private System.Func<bool> _action;
    // コンストラクタで行動関数を受け取る
    public ActionNode(System.Func<bool> action) => _action = action;

    // 行動関数を実行して結果を返す
    public override bool Tick() => _action();
}

// 条件ノード
public class ConditionNode : BhaiviorTree
{
    // 条件を判断する関数
    private System.Func<bool> _condition;
    // コンストラクタで条件関数を受け取る
    public ConditionNode(System.Func<bool> condition) => _condition = condition;

    // 条件関数を実行して結果を返す
    public override bool Tick() => _condition();
}

// シークエンスノード(AND連結)
public class SequenceNode : BhaiviorTree
{
    // 子ノードのリスト
    private List<BhaiviorTree> _chidren;
    // コンストラクタで子ノードを受け取る
    public SequenceNode(params BhaiviorTree[] children) => _chidren = new List<BhaiviorTree>(children);

    // 全ての子ノードを順に実行し、1つでも失敗したら失敗を返す
    public override bool Tick()
    {
        foreach (var child in _chidren)
        {
            if (!child.Tick()) return false;
        }
        return true;
    }
}

// セレクタノード(OR連結)
public class SelectorNode : BhaiviorTree
{
    // 子ノードのリスト
    private List<BhaiviorTree> _chidren;
    // コンストラクタで子ノードを受け取る
    public SelectorNode(params BhaiviorTree[] children) => _chidren = new List<BhaiviorTree>(children);

    // 子ノードを順に実行し、1つでも成功したら成功を返す
    public override bool Tick()
    {
        foreach (var child in _chidren)
        {
            if (child.Tick()) return true;
        }
        return false;
    }
}