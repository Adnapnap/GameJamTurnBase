using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    // 每次玩家移动需更新
    [HideInInspector]
    public int2 currPlayerPos;
  
    [HideInInspector]
    public bool isJumping;          // 玩家跳跃的状态
    [HideInInspector]
    public bool isUnderProtected;   // 护盾开启的状态

    public SpriteRenderer PlayerSprite;
    public bool IsLastJump;

    private PlayerInputType preInputType;
    private int healMax;

    private int damageAmount;
    private bool shouldDamage;
    private bool isPlayerA;
    
    public void Setup(int2 playerPos)
    {
        healMax = 4;
        currPlayerPos = playerPos;
        // 初始化时，重置到默认状态
        isJumping = false;
        isUnderProtected = false;
        isPlayerA = GameManagerSingleton.Instance.IsPlayerA;
    }

    public void Reset()
    {
        int2 startPos = GameManagerSingleton.Instance.PlayerStartPos;
        if (!currPlayerPos.Equals(startPos))
        {
            TileManagerSingleton.Instance.MoveObjectToTile(startPos, gameObject);
        }
        Setup(startPos);
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="dir"></param>
    public Task DoMove(int2 dir)
    {
        int2 newPos = dir + currPlayerPos;
        if (!CheckLimit(newPos)) return null;
        currPlayerPos = newPos;
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger(dir.Equals(new int2(-1, 0)) ? "MoveLeftTrigger" : "MoveRightTrigger");
        return TileManagerSingleton.Instance.MoveObjectToTile(currPlayerPos, gameObject);
    }
    

    /// <returns>
    /// 玩家位置
    /// </returns>
    public int2 GetPlayerPos()
    {
        return currPlayerPos;
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    /// todo remake
    public async void DoJump()
    {
        // 跳跃动画
        isJumping = true;
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger("JumpTrigger");
        await DoMove(new int2(0, 1));
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger("JumpIdleTrigger");

        IsLastJump = true;
    }
    

    public void HandleLastJump()
    {
        if (!IsLastJump) return;
        DoMove(new int2(0, -1));
        IsLastJump = false;
    }

    /// <summary>
    /// 处理横向轻攻击
    /// </summary>
    /// <param name="attackPosList"></param>
    public Task DoHorizontalAttack(int damage = 2)
    {
        // 正常攻击
        List<int2> attackList = new List<int2>();
        for (int i = -2; i < 3; i++)
        {
            int2 newPos = new int2(currPlayerPos.x + i, currPlayerPos.y);
            if (!CheckLimit(newPos)) continue;
            attackList.Add(newPos);
        }
        GameManagerSingleton.Instance.Boss.CheckForDamage(2, attackList);
        
        return HandleAnimation( "SwordTrigger","playerASword","playerBSword");
    }

    private Task HandleAnimation(string triggerName, string animationNameA, string animationNameB)
    {
        var tcs = new TaskCompletionSource<bool>();
        StartCoroutine( PlayAnimationAndWait(triggerName,isPlayerA?animationNameA:animationNameB, tcs) );
        return tcs.Task;
    }
    
    IEnumerator PlayAnimationAndWait(string triggerName,string animationName, TaskCompletionSource<bool> tcs)
    {
        var animator = GameManagerSingleton.Instance.PlayerAnimator;
        animator.SetTrigger(triggerName);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        tcs.SetResult(true);
    }
    

    /// <summary>
    /// 处理十字重攻击
    /// </summary>
    /// <param name="attackPosList"></param>
    public Task DoCrossAttack(int damage = 4)
    {
        List<int2> attackList = new List<int2>();
        
        int2 pos = new int2(currPlayerPos.x, currPlayerPos.y + 1);
        if (CheckLimit(pos)) attackList.Add(pos);
        pos = new int2(currPlayerPos.x - 1, currPlayerPos.y);
        if (CheckLimit(pos)) attackList.Add(pos);
        pos = new int2(currPlayerPos.x + 1, currPlayerPos.y);
        if (CheckLimit(pos)) attackList.Add(pos);
        attackList.Add(currPlayerPos);
        
        GameManagerSingleton.Instance.Boss.CheckForDamage(damage, attackList);

        return HandleAnimation("HammerTrigger", "playerAHammer", "playerBHammer");
    }

    /// <summary>
    /// 加血
    /// </summary>
    public Task DoHeal()
    {
        healMax--;
        if (healMax <= 0) return null;
        GameManagerSingleton.Instance.PlayerHp_UI.OnGetRecovery(2);
        return HandleAnimation("HealTrigger", "playerAHeal", "playerBHeal");
    }

    /// <summary>
    /// 护盾
    /// </summary>
    public Task DoProtected()
    {
        isUnderProtected = true;
        return HandleAnimation("ShieldTrigger", "playerAShield", "playerBShield");
    }
    
    public void ResetPlayerState()
    {
        isJumping = false;
        isUnderProtected = false;
        // (状态对应的动画，也一并重置)
    }
    

    public void CheckForDamage(List<int2> attackList,int value)
    {
        foreach (var pos in attackList)
        {
            TileManagerSingleton.Instance.ChangeTileColorPlayer(pos);
            if (currPlayerPos.Equals(pos)) OnTakeDamage(value);
        }
    }

    public void CheckForDamageLaser(List<int2> attackList,int value)
    {
        foreach (var pos in attackList)
        {
            if (!currPlayerPos.Equals(pos)) continue;
            GameManagerSingleton.Instance.PlayerHp_UI.OnTakeDamage(value);
            TakeDamageAnimation();
        }
    }

    private void OnTakeDamage(int _damage)
    {
        if (isJumping) return;

        if (isUnderProtected) _damage /= 2;
        
        // Anim

        // VFX

        // Audio
        shouldDamage = true;
        damageAmount = _damage;
    }
    
    private void TakeDamageAnimation()
    {
        GameManagerSingleton.Instance.PlayerAnimator.SetTrigger("DamageTrigger");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Boss") && shouldDamage)
        {
            GameManagerSingleton.Instance.PlayerHp_UI.OnTakeDamage(damageAmount);
            TakeDamageAnimation();
        }
        shouldDamage = false;
        damageAmount = 0;
    }
    

    private bool CheckLimit(int2 targetPos)
    {
        return targetPos.x >= 0 && targetPos.x < GameManagerSingleton.Instance.Width && targetPos.y >= 0 && targetPos.y < GameManagerSingleton.Instance.Height;
    }

}

public class PlayerInputType
{
    public const int MOVEA = 0;
    public const int MOVED = 1;
    public const int ATTACK1 = 2;
    public const int ATTACK2 = 3;
    public const int DEFENSE = 4;
    public const int HEAL = 5;
    public const int JUMP = 6;
}