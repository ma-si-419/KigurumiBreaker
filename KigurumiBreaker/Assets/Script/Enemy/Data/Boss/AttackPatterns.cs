using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BossAttackPatterns")]
public class BossAttackPatterns : ScriptableObject
{
    public List<AttackPatterns> attackPatternsList;
}

[System.Serializable]
public class AttackPatterns
{
    enum AttackType
    {
        Melee,
        Ranged,
        Area,
        Special
    }

    [Header("UŒ‚ƒpƒ^[ƒ“‚Ì–¼‘O")]
    [SerializeField] private string patternName;
    [Header("‚Ç‚Ì‚­‚ç‚¢‚Ì‹——£‚Åg‚¤‚©")]
    [SerializeField] private float range;
    [Header("UŒ‚‚ÌƒN[ƒ‹ƒ_ƒEƒ“")]
    [SerializeField] private float cooldown;
    [Header("UŒ‚‚Ìí—Ş")]
    [SerializeField] private AttackType attackType;

    //ÀÛ‚ÌUŒ‚‚ğŒÄ‚Ô
    public bool Execute(BossEnemy boss)
    {
        //‚±‚±‚ÅUŒ‚‚Ìí—Ş‚É‰‚¶‚ÄUŒ‚‚ğÀs‚·‚é
        switch (attackType)
        {
            case AttackType.Melee:
                //boss.MeleeAttack();
                boss.ChangeState(new BossAttackType2State(boss));   //‹ßÚUŒ‚ó‘Ô‚É•ÏX
                break;

            case AttackType.Ranged:
                //boss.RangedAttack();
                boss.ChangeState(new BossAttackType2State(boss));   //‹ßÚUŒ‚ó‘Ô‚É•ÏX
                break;

            case AttackType.Area:
                //boss.AreaAttack();
                boss.ChangeState(new BossAttackType2State(boss));   //‹ßÚUŒ‚ó‘Ô‚É•ÏX
                break;

            case AttackType.Special:
                //boss.SpecialAttack();
                boss.ChangeState(new BossAttackType2State(boss));   //‹ßÚUŒ‚ó‘Ô‚É•ÏX
                break;
        }

        return true;
    }

}
