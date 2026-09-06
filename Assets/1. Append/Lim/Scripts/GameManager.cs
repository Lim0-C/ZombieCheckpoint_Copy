using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class GameManager : Singleton<GameManager>
{
    [SerializeField]private GameObject npcPrefab;
    [SerializeField]private Slider gaugeSlider;
    [SerializeField]private ZombieEventController zombieEventController;
    [SerializeField]private UiController uiController;
    
    public float maxGauge;
    public float successDecision;
    public float failDecision;
    public float successKillZombie;
    public float failKillZombie;
    
    private float _currentGauge;
    private List<Npc> _npcList;
    private float _gaugeReduction;         //게이지 감소 계수
    private Vector3 _initCameraPosition;
    
    public bool canZombie2Spawn;
    public bool canZombie3Spawn;
    
    private Coroutine gaugeCoroutine;
    private Coroutine zombie2Coroutine;
    private Coroutine zombie3Coroutine;
    
    public List<Npc> NpcList{get{return _npcList;}}

    // 카메라 이동
    private float npcGapY = 1f;
    private float npcGapZ = 1f;
    private Vector3 cameraDestination;

    //좀비 퇴치시 동작
    public void OnClickZombie()
    {
        zombieEventController.StopZombieEvent();
        var npc = _npcList[0];
        Debug.Log($"좀비 퇴치");
        SoundManager.Instance.PlayOneShot(Enums.SfxClips.ShootZombie); // 좀비 퇴치
        AddGauge(successKillZombie);
        _npcList.Remove(npc);
        CheckFrontZombie();
        NpcObjectPool.Instance.ReturnObject(npc);
        RespawnNpc();
        
        StopCoroutine(MoveCamera());
        StartCoroutine(MoveCamera());       // 기존 카메라 무빙을 중단하고 새로 시작
    }

    public void HitEvent()
    {
        uiController.HitEvent();
    }

    public void MissZombie(Enums.ZombieState state, float? disGauge = null)
    {
        var npc = _npcList[0];
        if (state != Enums.ZombieState.Jump && disGauge != null)
        {
            SoundManager.Instance.PlayOneShot(Enums.SfxClips.PlayerHit); // 플레이어 피격
            DisGauge(disGauge.Value);
        }
        
        _npcList.Remove(npc);
        CheckFrontZombie();
        StartCoroutine(DelayReturnObject(npc));
        RespawnNpc();
        
        StopCoroutine(MoveCamera());
        StartCoroutine(MoveCamera());       // 기존 카메라 무빙을 중단하고 새로 시작
    }

    IEnumerator DelayReturnObject(Npc npc)
    {
        yield return new WaitForSeconds(1f);
        NpcObjectPool.Instance.ReturnObject(npc);
    }

    public void OnClickIsolation()
    {
        zombieEventController.StopZombieEvent();
        Decision(Consts.SUSPIC);
    }

    public void OnClickAccept()
    {
        zombieEventController.StopZombieEvent();
        Decision(Consts.NORMAL);
    }

    void RespawnNpc()
    {
        var npc = NpcObjectPool.Instance.GetObject();
        var r = Random.Range(0, 10);
        switch (r)
        {
            case < 5:
                npc.Init(Enums.NpcState.Normal);
                break;
            case < 9:
                npc.Init(Enums.NpcState.Suspicious);
                break;
            default:
                npc.Init(Enums.NpcState.Zombie);
                break;
        }

        npc.transform.position = _npcList[_npcList.Count - 1].transform.position + new Vector3(0, npcGapY, npcGapZ);
        _npcList.Add(npc);
    }

    //좀비가 맨앞에 있는지 확인하고 있으면 좀비 이벤트 실행
    void CheckFrontZombie()
    {
        if (_npcList[0].CompareTag(Consts.ZOMBIE))
        {
            GameObject npcObject = _npcList[0].gameObject;
            zombieEventController.ZombieEvent(_npcList[0].ZombieState, npcObject);
        }
    }
    
    void Decision(string tag)
    {
        var npc = _npcList[0];
        if (npc.CompareTag(tag))
        {
			AddGauge(successDecision);
            RespawnNpc();
        }
        else
        {
            SoundManager.Instance.PlayOneShot(Enums.SfxClips.SortFailed); // 분류 실패
            var n = npc.CompareTag(Consts.ZOMBIE)? failKillZombie : failDecision;
            DisGauge(n);
            RespawnNpc();
        }
        _npcList.Remove(npc);
        CheckFrontZombie();
        // npc 분류 방향을 위한 로직
        bool goLeft = false;

        if (tag == Consts.SUSPIC)
        {
            goLeft = true;
        }

        StopCoroutine("MoveCamera");    
        StartCoroutine("MoveCamera");       // 기존 카메라 무빙을 중단하고 새로 시작
        StartCoroutine(SwipeNpc(goLeft, npc)); // npc 분류 이후 처리 메서드 시작
    }
    
    void AddGauge(float gauge)
    {
        _currentGauge = Mathf.Clamp(_currentGauge + gauge, 0, maxGauge);
    }

    void DisGauge(float gauge)
    {
        _currentGauge = Mathf.Clamp(_currentGauge - gauge, 0, maxGauge);
    }
    
    IEnumerator GaugeAutoDown()
    {
        float t = 0;
        while (_currentGauge > 0)
        {
            t += Time.deltaTime;
            _currentGauge -= _gaugeReduction * Time.deltaTime;
            gaugeSlider.value = _currentGauge / maxGauge;

            if (t > 10f)
            {
                Debug.Log("속도 업");
                _gaugeReduction *= 1.5f;
                t = 0;
            }
            yield return null;
        }
        gaugeSlider.value = _currentGauge / maxGauge;
        GameEnd();
    }

    IEnumerator Zombie2SpawnTimer()
    {
        float t = 0;
        while (t < 10f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        canZombie2Spawn = true;
        Debug.Log("점프 좀비 스폰 가능");
        zombie3Coroutine = StartCoroutine(Zombie3SpawnTimer());
    }

    IEnumerator Zombie3SpawnTimer()
    {
        float t = 0;
        while (t < 20f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        canZombie3Spawn = true;
        Debug.Log("탱크 좀비 스폰 가능");
    }

    void GameEnd()
    {
        Debug.Log("게임 오버");
    }

    IEnumerator MoveCamera()
    {
        // 카메라 이동(npc 대기열 이동 효과) 메서드
        float elapsedTime = 0f;
        Vector3 startPosition = Camera.main.transform.position;
        cameraDestination = cameraDestination + new Vector3(0, npcGapY, npcGapZ);

        while (elapsedTime < 1f)
        {
            Camera.main.transform.position = Vector3.Lerp(startPosition, cameraDestination, elapsedTime);
            elapsedTime += Time.deltaTime * 5.0f;
            yield return null;
        }

        Camera.main.transform.position = cameraDestination;

        yield break;
    }

    IEnumerator SwipeNpc(bool swipeLeft, Npc npc)
    {
        // 분류된 npc 좌우 이동 효과 메서드
        float elapsedTime = 0f;
        Vector3 startPosition = npc.gameObject.transform.position;
        Vector3 destinationPosition = startPosition + new Vector3(10f, 0, 0);

        if (swipeLeft == true)
        {
            destinationPosition.x *= -1f;
        }

        Npc frontNpc = npc;

        while (elapsedTime < 1f)
        {
            frontNpc.transform.position = Vector3.Lerp(startPosition, destinationPosition, elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 이동 후 카메라 시야에서 사라지면 오브젝트 풀에 반환
        NpcObjectPool.Instance.ReturnObject(frontNpc);

        yield break;
    }

    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void Init()
    {
        //수치 초기화
        _currentGauge = maxGauge;
        canZombie2Spawn = false;
        canZombie3Spawn = false;
        _gaugeReduction = 1f;

        Camera.main.transform.position = _initCameraPosition;
        
        for (int i = 0; i < 10; i++)
        {
            var npc = NpcObjectPool.Instance.GetObject();
            var r = Random.Range(0, 10);
            switch (r)
            {
                case < 5:
                    npc.Init(Enums.NpcState.Normal);
                    break;
                case < 9:
                    npc.Init(Enums.NpcState.Suspicious);
                    break;
                default:
                    npc.Init(Enums.NpcState.Zombie);
                    break;
            }
            npc.transform.position = new Vector3(0, i * npcGapY, i * npcGapZ);
            _npcList.Add(npc);
        }

        cameraDestination = Camera.main.transform.position;
        
        gaugeCoroutine = StartCoroutine(GaugeAutoDown());
        zombie2Coroutine = StartCoroutine(Zombie2SpawnTimer());
    }
    private void Awake()
    {
        base.Awake();
        _npcList = new List<Npc>();
        _initCameraPosition = Camera.main.transform.position;
    }

    void Start()
    {
        Init();
    }

    //키보드 사용을 위한 임시 코드
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnClickIsolation();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            OnClickAccept();
        }
    }
}
