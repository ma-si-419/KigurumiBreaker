using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackCreateData : MonoBehaviour
{
    [SerializeField] private string PrefabName = "NewSkillAttack";

    [SerializeField] private float Damage = 10.0f;

    [SerializeField] private int LifeTime = 10;
    
    [SerializeField] private GameObject EffectPrefab;

    public string prefabName => PrefabName;

    public int lifeTime => LifeTime;
    public float damage => Damage;
    public GameObject effectPrefab => EffectPrefab;
}
