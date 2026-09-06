using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;


public class Npc : MonoBehaviour, IPointerClickHandler
{
    private SpineAnimSwap spineAnimSwap;
    private Enums.NpcState _state;
    private CapsuleCollider2D _collider;
    private Enums.ZombieState _zombieState;
    private int _health;
    
    public CapsuleCollider2D Collider{get{return _collider;}}
    
    public Enums.ZombieState ZombieState{get{return _zombieState;}}
    private void Awake()
    {
        spineAnimSwap = GetComponent<SpineAnimSwap>();
        _collider = GetComponent<CapsuleCollider2D>();
    }
    
    //todo: 랜덤한 스프라이트 적용
    public void Init(Enums.NpcState state)
    {
        _collider.enabled = false;
        _state = state;
        switch (_state)
        {
            case Enums.NpcState.Normal:
                gameObject.tag = Consts.NORMAL;
                CharacterManager.instance.InitializeCharacterToNormal(spineAnimSwap);
                break;
            case Enums.NpcState.Suspicious:
                gameObject.tag = Consts.SUSPIC;
                CharacterManager.instance.InitializeCharacterToSuspic(spineAnimSwap);
                break;
            case Enums.NpcState.Zombie:
                Debug.Log("좀비다!!!!");
                gameObject.tag = Consts.ZOMBIE;
                _collider.enabled = true;
                _health = 1;
                RandomZombie();
                break;
        }
    }
    //단계에 따라 랜덤하게 좀비 종류 지정
    private void RandomZombie()
    {
        if (GameManager.Instance.canZombie3Spawn)
        {
            var r = Random.Range(0, 6);
            switch (r)
            {
                case < 3:
                    _zombieState = Enums.ZombieState.Normal;
                    CharacterManager.instance.InitializeCharacterToNormalZombie(spineAnimSwap);
                    break;
                case < 5:
                    _zombieState = Enums.ZombieState.Jump;
                    CharacterManager.instance.InitializeCharacterToJumpZombie(spineAnimSwap);
                    break;
                default:
                    _zombieState = Enums.ZombieState.Tank;
                    CharacterManager.instance.InitializeCharacterToTankZombie(spineAnimSwap);
                    _health = 3;
                    break;
            }
        }
        else if (GameManager.Instance.canZombie2Spawn)
        {
            var r = Random.Range(0, 5);
            switch (r)
            {
                case < 3:
                    CharacterManager.instance.InitializeCharacterToNormalZombie(spineAnimSwap);
                    _zombieState = Enums.ZombieState.Normal;
                    break;

                case < 5:
                    CharacterManager.instance.InitializeCharacterToJumpZombie(spineAnimSwap);
                    _zombieState = Enums.ZombieState.Jump;
                    break;
            }
        }
        else
        {
            _zombieState = Enums.ZombieState.Normal;
            CharacterManager.instance.InitializeCharacterToNormalZombie(spineAnimSwap);
        }
    }
    //좀비 터치 시
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.NpcList[0] != this) return;
        _health--;
        if (_health > 0)return;
        GameManager.Instance.OnClickZombie();
    }
}
