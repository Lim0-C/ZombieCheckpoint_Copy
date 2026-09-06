using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ZombieEventController : MonoBehaviour
{
    [SerializeField] private Image panel;
    
    private Coroutine _currentCoroutine;
    private GameObject zombieObject;
    private SpineAnimSwap animationSwap;

    public void ZombieEvent(Enums.ZombieState zombieState, GameObject _zombieObject)
    {
        zombieObject = _zombieObject;
        animationSwap = zombieObject.GetComponent<SpineAnimSwap>();
        _currentCoroutine = StartCoroutine(zombieState == Enums.ZombieState.Jump ? WaitForJumpZombie() : WaitForZombie(zombieState));
    }

    IEnumerator WaitForZombie(Enums.ZombieState zombieState)
    {
        float t = 0;
        while (t < 1f)
        {
            t+=Time.deltaTime;
            yield return null;
        }
        zombieObject.GetComponent<Npc>().Collider.enabled = false;
        switch (zombieState)
        {
            case Enums.ZombieState.Normal:
                CharacterManager.instance.SetCharacterAttackZombieToNormalZombie(animationSwap);
                SoundManager.Instance.PlayOneShot(Enums.SfxClips.NormalZombieAttack); // 좀비의 공격
                break;
            case Enums.ZombieState.Tank:
                CharacterManager.instance.SetCharacterAttackZombieToTankZombie(animationSwap);
                SoundManager.Instance.PlayOneShot(Enums.SfxClips.TankZombieAttack); // 좀비의 공격
                break;
            default:
                break;
        }
        var n = zombieState == Enums.ZombieState.Normal ? 4f : 5f;
        GameManager.Instance.MissZombie(zombieState, n);
        GameManager.Instance.HitEvent();
    }

    IEnumerator WaitForJumpZombie()
    {
        float t = 0;
        while (t < 0.5f)
        {
            t+=Time.deltaTime;
            yield return null;
        }
        zombieObject.GetComponent<Npc>().Collider.enabled = false;
        CharacterManager.instance.SetCharacterAttackZombieToJumpZombie(animationSwap);
        GameManager.Instance.MissZombie(Enums.ZombieState.Jump);
        GameManager.Instance.HitEvent();
        SoundManager.Instance.PlayOneShot(Enums.SfxClips.JumpZombieAttack); // 좀비의 공격
    }

    public void StopZombieEvent()
    {
        if (_currentCoroutine == null)return;
        StopCoroutine(_currentCoroutine);
        _currentCoroutine = null;
    }
}
