using System.Collections;
using UnityEngine;
using Spine.Unity;
using Unity.VisualScripting;
using Spine;


public class SpineAnimSwap : MonoBehaviour
{
    private SkeletonAnimation skel; // 재생 중인 Skeleton과 AnimationState가 들어있다, 런타임 인스턴스
    private const string NPC_SKIN = "Skin";
    private const string ANIMATION_IDLE = "Idle_Normal";
    private const string ANIMATION_ATTACK = "Attack";

    void OnEnable()
    {
        skel = GetComponent<SkeletonAnimation>();
        skel.Initialize(false); // 이미 초기화돼 있으면 내부에서 무시됨
    }

    IEnumerator SwapToIdle()
    {
        float randomNum = Random.Range(0, 0.5f);
        yield return new WaitForSeconds(randomNum);
        skel.AnimationState.SetAnimation(0, ANIMATION_IDLE, true); // 트랙0에 walk 루프
    }

    IEnumerator SwapToAttack()
    {
        yield return null;
        skel.AnimationState.SetAnimation(0, ANIMATION_ATTACK, true); // 트랙0에 walk 루프
    }

    public void SetAnimationToAttack(Enums.ZombieState zombieState)
    {
        switch (zombieState)
        {
            case Enums.ZombieState.Normal:
                StartCoroutine(SwapToAttack());
                break;
            case Enums.ZombieState.Jump:
                StartCoroutine(SwapToAttack());
                break;
            case Enums.ZombieState.Tank:
                StartCoroutine(SwapToAttack());
                break;
        }
    }

    public void SetAnimationToIdle()
    {
        StartCoroutine(SwapToIdle());
    }
    
    /// slotName: "hair" 처럼 슬롯 이름
    /// keyName : "hair" 처럼 플레이스홀더(attachment key) 이름 (슬롯 안에 있는 것들)
    /// fromSkin: "Hair_A", "Hair_B"처럼, 그 플레이스홀더를 가진 스킨 이름, (Body)
    public void SetPlaceholderFromSkin(string slotName, string keyName)
    {
        if (string.IsNullOrEmpty(keyName))
        {
            skel.Skeleton.SetAttachment(slotName, null); // 룰북 자체를 비움(다시 지정 전까지 계속 숨김)
            skel.AnimationState.Apply(skel.Skeleton);
            skel.LateUpdate();
            return;
        }

        SkeletonData data = skel.Skeleton.Data;
        //설계도(어떤 부품과 애니메이션이 있는지 정의)
        //정의 정보 (Spine에서 Export된 JSON/Skel.bytes/Atlas 내용을 읽어서 구조화한 데이터베이스)
        //→ 실행 도중 변하지 않음.
        //Bones: 모든 뼈(BoneData)의 리스트
        //Slots: 슬롯(SlotData) 리스트
        //Skins: 스킨(Skin) 리스트
        //Animations: 애니메이션(Animation) 리스트
        //Events: 이벤트 정의
        //FindSkin(string name)
        //FindSlot(string name)
        //FindAnimation(string name)
        //런타임 스켈레톤 인스턴스(skel.Skeleton)는 이 설계도를 참조해 움직인다.

        // (2) 스킨 찾기
        Skin skin = data.FindSkin(NPC_SKIN);

        // (3) 슬롯과 인덱스
        SlotData slotData = data.FindSlot(slotName);
        int slotIndex = slotData.Index;

        // (4) 스킨에서 해당 키(플레이스홀더) 첨부물 가져오기
        Attachment att = skin.GetAttachment(slotIndex, keyName);

        // ★ 어태치먼트를 바꿀 때 이 방식을 사용해보기!
        // (5) 슬롯 인스턴스를 직접 찾아 꽂기  ← 인덱서 대신 이 방식 사용
        Slot slot = skel.Skeleton.FindSlot(slotName);
        slot.Attachment = att; // 순간 반영

        // (6) 이번 프레임 즉시 반영
        skel.AnimationState.Apply(skel.Skeleton);
        skel.LateUpdate();
    }
}
