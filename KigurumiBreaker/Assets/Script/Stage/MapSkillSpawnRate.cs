using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MapSkillSpawnRate : MonoBehaviour
{
    public enum RewardType
    {
        Skill,      // スキル系（SkillData.SkillElementに準拠）
        Gold,       // ゴールド
        Shop,       // ショップ
        Item,       // アイテム
        None        // なし
    }

    [System.Serializable]
    public struct SkillRate
    {
        public RewardType rewardType;              // 種類（Skill / Gold / Shop / Item）
        public string customName;                  // SkillData以外の識別名（例："HealPotion"）
        public SkillData.SkillElement element;     // スキル属性（SkillTypeのときのみ使用）
        [Range(0, 100)]
        public float probability;                  // 抽選確率
    }

    [Header("報酬出現確率設定（合計100推奨）")]
    public List<SkillRate> skillRates = new List<SkillRate>();

    /// <summary>
    /// 設定された確率に基づいてランダム抽選
    /// </summary>
    public SkillRate GetRandomReward()
    {
        float total = 0;
        foreach (var r in skillRates)
            total += r.probability;

        if (total <= 0)
        {
            Debug.LogWarning("SkillSpawnRate: 合計確率が0です。");
            return default;
        }

        float rand = Random.Range(0, total);
        float cumulative = 0;
        foreach (var r in skillRates)
        {
            cumulative += r.probability;
            if (rand <= cumulative)
                return r;
        }

        return skillRates.Count > 0 ? skillRates[0] : default;
    }
}
