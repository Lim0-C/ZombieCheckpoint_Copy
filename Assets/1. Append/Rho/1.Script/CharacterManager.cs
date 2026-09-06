using System;
using UnityEngine;
using UnityEngine.XR;
using static Enums;
using Random = UnityEngine.Random;

public enum PersonType
{
    Normal,
    Suspic,
    Zombie
}

public enum Hair
{
    Hair_Normal_1,
    Hair_Normal_2,
    Hair_Normal_3,
    Hair_Normal_4,
    Hair_Normal_5,
    Hair_Normal_6,
    Hair_Normal_7,
    Hair_Normal_8,
    Hair_Zombie_1,
    Hair_Zombie_2,
    Hair_Zombie_3
}

public enum Mask
{
    Mask_1,
    Mask_2
}
public enum Mouth
{
    Mouth_Normal_1,
    Mouth_Normal_2,
    Mouth_Suspic_1,
}
public enum Deco
{
    Deco_Normal_1,
    Deco_Normal_2,
    Deco_Normal_3,
    Deco_Suspic_1
}
public enum R_Hand_Attack
{
    R_Hand_Attack_1,
    R_Hand_Attack_2,
    R_Hand_Attack_3
}

public enum L_Hand_Attack
{
    L_Hand_Attack_1,
    L_Hand_Attack_2,
    L_Hand_Attack_3
}

public enum Eye
{
    Eye_Close,
    Eye_Normal,
    Eye_Suspic_1,
    Eye_Suspic_2
}

public enum R_Arm
{
    R_Arm_Normal_1,
    R_Arm_Normal_2,
    R_Arm_Normal_3,
    R_Arm_Normal_4,
    R_Arm_Zombie_1_Idle,
    R_Arm_Zombie_1_Attack,
    R_Arm_Zombie_2_Idle,
    R_Arm_Zombie_2_Attack,
    R_Arm_Zombie_3_Idle,
    R_Arm_Zombie_3_Attack
}

public enum L_Arm
{
    L_Arm_Normal_1,
    L_Arm_Normal_2,
    L_Arm_Normal_3,
    L_Arm_Normal_4,
    L_Arm_Zombie_1_Idle,
    L_Arm_Zombie_1_Attack,
    L_Arm_Zombie_2_Idle,
    L_Arm_Zombie_2_Attack,
    L_Arm_Zombie_3_Idle,
    L_Arm_Zombie_3_Attack
}

public enum Body
{
    Body_Normal_1,
    Body_Normal_2,
    Body_Normal_3,
    Body_Normal_4,
    Body_Zombie_1,
    Body_Zombie_2,
    Body_Zombie_3
}

public enum R_Leg
{
    R_Leg_Normal_1,
    R_Leg_Normal_2,
    R_Leg_Normal_3,
    R_Leg_Zombie_1_Idle,
    R_Leg_Zombie_1_Attack,
    R_Leg_Zombie_2_Idle,
    R_Leg_Zombie_2_Attack,
    R_Leg_Zombie_3_Idle,
    R_Leg_Zombie_3_Attack
}
public enum L_Leg
{
    L_Leg_Normal_1,
    L_Leg_Normal_2,
    L_Leg_Normal_3,
    L_Leg_Zombie_1_Idle,
    L_Leg_Zombie_1_Attack,
    L_Leg_Zombie_2_Idle,
    L_Leg_Zombie_2_Attack,
    L_Leg_Zombie_3_Idle,
    L_Leg_Zombie_3_Attack
}

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    public void Awake()
    {
        instance = this;
    }

    public void InitializeCharacterToNormal(SpineAnimSwap character)
    {
        character.SetPlaceholderFromSkin("Eye_Black", "Eye_Black");

        character.SetPlaceholderFromSkin("R_Hand_Attack", null);
        character.SetPlaceholderFromSkin("L_Hand_Attack", null);

        //머리
        int tempToInt = Random.Range((int)Hair.Hair_Normal_1, (int)Hair.Hair_Normal_8 + 1);
        string tempToStr = ((Hair)tempToInt).ToString();
        character.SetPlaceholderFromSkin("Hair", tempToStr);

        //상반신
        tempToInt = Random.Range((int)Body.Body_Normal_1, (int)Body.Body_Normal_4 + 1);
        tempToStr = ((Body)tempToInt).ToString();
        character.SetPlaceholderFromSkin( "Body",tempToStr);

        tempToStr = ((R_Arm)tempToInt).ToString();
        character.SetPlaceholderFromSkin( "R_Arm",tempToStr);

        tempToStr = ((L_Arm)tempToInt).ToString();
        character.SetPlaceholderFromSkin( "L_Arm",tempToStr);

        //하반신
        tempToInt = Random.Range((int)R_Leg.R_Leg_Normal_1, (int)R_Leg.R_Leg_Normal_3 + 1);
        tempToStr = ((R_Leg)tempToInt).ToString();
        character.SetPlaceholderFromSkin("R_Leg", tempToStr);
        tempToStr = ((L_Leg)tempToInt).ToString();
        character.SetPlaceholderFromSkin("L_Leg", tempToStr);

        //마스크
        tempToInt = Random.Range((int)Mask.Mask_1, (int)Mask.Mask_2 + 5); //한 수 더해서 없는 경우도 생성
        if (tempToInt <= (int)Mask.Mask_2)
        {
            tempToStr = ((Mask)tempToInt).ToString();
            character.SetPlaceholderFromSkin("Mask", tempToStr);
        }
        else
        {
            character.SetPlaceholderFromSkin("Mask", null);
        }

        //데코
        tempToInt = Random.Range((int)Deco.Deco_Normal_1, (int)Deco.Deco_Normal_2 + 2); //한 수 더해서 없는 경우도 생성
        if (tempToInt <= (int)Deco.Deco_Normal_2)
        {
            tempToStr = ((Deco)tempToInt).ToString();
            character.SetPlaceholderFromSkin("Deco", tempToStr);
        }
        else
        {
            character.SetPlaceholderFromSkin("Deco", null);
        }

        //눈
        
        tempToStr = Eye.Eye_Normal.ToString();
        character.SetPlaceholderFromSkin("Eye", tempToStr);

        //입
        tempToInt = Random.Range((int)Mouth.Mouth_Normal_1, (int)Mouth.Mouth_Normal_2 + 1);
        tempToStr = ((Mouth)tempToInt).ToString();
        character.SetPlaceholderFromSkin("Mouth", tempToStr);

        //모션
        character.SetAnimationToIdle();
    }
    public void InitializeCharacterToSuspic(SpineAnimSwap character)
    {
        character.SetPlaceholderFromSkin("Eye_Black", "Eye_Black");
        character.SetPlaceholderFromSkin("R_Hand_Attack", null);
        character.SetPlaceholderFromSkin("L_Hand_Attack", null);

        //머리
        int tempToInt = Random.Range((int)Hair.Hair_Normal_1, (int)Hair.Hair_Normal_8 + 1);
        string tempToStr = ((Hair)tempToInt).ToString();
        character.SetPlaceholderFromSkin("Hair", tempToStr);

        //상반신
        tempToInt = Random.Range((int)Body.Body_Normal_1, (int)Body.Body_Normal_4 + 1);
        tempToStr = ((Body)tempToInt).ToString();
        character.SetPlaceholderFromSkin("Body", tempToStr);

        tempToStr = ((R_Arm)tempToInt).ToString();
        character.SetPlaceholderFromSkin("R_Arm", tempToStr);

        tempToStr = ((L_Arm)tempToInt).ToString();
        character.SetPlaceholderFromSkin("L_Arm", tempToStr);

        //하반신
        tempToInt = Random.Range((int)R_Leg.R_Leg_Normal_1, (int)R_Leg.R_Leg_Normal_3 + 1);
        tempToStr = ((R_Leg)tempToInt).ToString();
        character.SetPlaceholderFromSkin("R_Leg", tempToStr);
        tempToStr = ((L_Leg)tempToInt).ToString();
        character.SetPlaceholderFromSkin("L_Leg", tempToStr);

        //마스크
        tempToInt = Random.Range((int)Mask.Mask_1, (int)Mask.Mask_2 + 5); //한 수 더해서 없는 경우도 생성
        if (tempToInt <= (int)Mask.Mask_2)
        {
            tempToStr = ((Mask)tempToInt).ToString();
            character.SetPlaceholderFromSkin("Mask", tempToStr);
        }
        else
        {
            character.SetPlaceholderFromSkin("Mask", null);
        }

        bool[] suspicTrue = new bool[2];
        //눈
        tempToInt = Random.Range((int)Eye.Eye_Normal, (int)Eye.Eye_Suspic_2 + 1);
        tempToStr = ((Eye)tempToInt).ToString();
        character.SetPlaceholderFromSkin("Eye", tempToStr);
        suspicTrue[0] = tempToInt == (int)Eye.Eye_Normal ? false : true;

        //입
        tempToInt = Random.Range((int)Mouth.Mouth_Normal_1, (int)Mouth.Mouth_Suspic_1 + 1);
        tempToStr = ((Mouth)tempToInt).ToString();
        character.SetPlaceholderFromSkin("Mouth", tempToStr);
        suspicTrue[1] = tempToInt == (int)Mouth.Mouth_Suspic_1 ? true : false;

        //데코

        bool isSuspic = false;

        for (int i = 0; i < suspicTrue.Length; i++)
        {
            if (suspicTrue[i] == true)
            {
                isSuspic = true;
                break;
            }
        }

        tempToInt = Random.Range((int)Deco.Deco_Normal_1, (int)Deco.Deco_Suspic_1 + 5); //한 수 더해서 없는 경우도 생성
        if (!isSuspic)
        {
            tempToStr = Deco.Deco_Suspic_1.ToString();
            character.SetPlaceholderFromSkin("Deco", tempToStr);
        }
        else if (tempToInt <= (int)Deco.Deco_Suspic_1)
        {
            tempToStr = ((Deco)tempToInt).ToString();
            character.SetPlaceholderFromSkin("Deco", tempToStr);
        }
        else
        {
            character.SetPlaceholderFromSkin("Deco", null);
        }

        //모션
        character.SetAnimationToIdle();
    }
    public void InitializeCharacterToNormalZombie(SpineAnimSwap character)
    {
        character.SetPlaceholderFromSkin("R_Hand_Attack", null);
        character.SetPlaceholderFromSkin("L_Hand_Attack", null);

        //머리
        string tempToStr = Hair.Hair_Zombie_1.ToString();
        character.SetPlaceholderFromSkin("Hair", tempToStr);

        //상반신
        tempToStr = Body.Body_Zombie_1.ToString();
        character.SetPlaceholderFromSkin("Body", tempToStr);

        tempToStr = R_Arm.R_Arm_Zombie_1_Idle.ToString();
        character.SetPlaceholderFromSkin("R_Arm", tempToStr);

        tempToStr = L_Arm.L_Arm_Zombie_1_Idle.ToString();
        character.SetPlaceholderFromSkin("L_Arm", tempToStr);

        //하반신
        tempToStr = R_Leg.R_Leg_Zombie_1_Idle.ToString();
        character.SetPlaceholderFromSkin("R_Leg", tempToStr);
        tempToStr = L_Leg.L_Leg_Zombie_1_Idle.ToString();
        character.SetPlaceholderFromSkin("L_Leg", tempToStr);

        //눈
        tempToStr = (Eye.Eye_Normal.ToString());
        character.SetPlaceholderFromSkin("Eye", tempToStr);

        character.SetPlaceholderFromSkin("Eye_Black", null);
        character.SetPlaceholderFromSkin("Deco", null);
        character.SetPlaceholderFromSkin("Mouth", null);
        character.SetPlaceholderFromSkin("Mask", null);
        //모션
        character.SetAnimationToIdle();
    }
    public void InitializeCharacterToJumpZombie(SpineAnimSwap character)
    {
        character.SetPlaceholderFromSkin("R_Hand_Attack", null);
        character.SetPlaceholderFromSkin("L_Hand_Attack", null);

        //머리
        string tempToStr = Hair.Hair_Zombie_2.ToString();
        character.SetPlaceholderFromSkin("Hair", tempToStr);

        //상반신
        tempToStr = Body.Body_Zombie_2.ToString();
        character.SetPlaceholderFromSkin("Body", tempToStr);

        tempToStr = R_Arm.R_Arm_Zombie_2_Idle.ToString();
        character.SetPlaceholderFromSkin("R_Arm", tempToStr);

        tempToStr = L_Arm.L_Arm_Zombie_2_Idle.ToString();
        character.SetPlaceholderFromSkin("L_Arm", tempToStr);

        //하반신
        tempToStr = R_Leg.R_Leg_Zombie_2_Idle.ToString();
        character.SetPlaceholderFromSkin("R_Leg", tempToStr);
        tempToStr = L_Leg.L_Leg_Zombie_2_Idle.ToString();
        character.SetPlaceholderFromSkin("L_Leg", tempToStr);

        character.SetPlaceholderFromSkin("Eye_Black", null);
        character.SetPlaceholderFromSkin("Deco", null);
        character.SetPlaceholderFromSkin("Mouth", null);
        character.SetPlaceholderFromSkin("Mask", null);

        //눈
        tempToStr = Eye.Eye_Suspic_1.ToString();
        character.SetPlaceholderFromSkin("Eye", tempToStr);

        //모션
        character.SetAnimationToIdle();
    }
    public void InitializeCharacterToTankZombie(SpineAnimSwap character)
    {
        character.SetPlaceholderFromSkin("R_Hand_Attack", null);
        character.SetPlaceholderFromSkin("L_Hand_Attack", null);

        //머리
        string tempToStr = Hair.Hair_Zombie_3.ToString();
        character.SetPlaceholderFromSkin("Hair", tempToStr);

        //상반신
        tempToStr = Body.Body_Zombie_3.ToString();
        character.SetPlaceholderFromSkin("Body", tempToStr);

        tempToStr = R_Arm.R_Arm_Zombie_3_Idle.ToString();
        character.SetPlaceholderFromSkin("R_Arm", tempToStr);

        tempToStr = L_Arm.L_Arm_Zombie_3_Idle.ToString();
        character.SetPlaceholderFromSkin("L_Arm", tempToStr);

        //하반신
        tempToStr = R_Leg.R_Leg_Zombie_3_Idle.ToString();
        character.SetPlaceholderFromSkin("R_Leg", tempToStr);
        tempToStr = L_Leg.L_Leg_Zombie_3_Idle.ToString();
        character.SetPlaceholderFromSkin("L_Leg", tempToStr);

        character.SetPlaceholderFromSkin("Eye_Black", null);
        character.SetPlaceholderFromSkin("Deco", null);
        character.SetPlaceholderFromSkin("Mouth", null);
        character.SetPlaceholderFromSkin("Mask", null);

        //눈
        tempToStr = Eye.Eye_Suspic_2.ToString();
        character.SetPlaceholderFromSkin("Eye", tempToStr);

        //모션
        character.SetAnimationToIdle();
    }

    public void SetCharacterAttackZombieToNormalZombie(SpineAnimSwap character)
    {
        //머리
        string tempToStr = Hair.Hair_Zombie_1.ToString();
        character.SetPlaceholderFromSkin("Hair", tempToStr);

        //상반신
        tempToStr = Body.Body_Zombie_1.ToString();
        character.SetPlaceholderFromSkin("Body", tempToStr);

        tempToStr = R_Arm.R_Arm_Zombie_1_Attack.ToString();
        character.SetPlaceholderFromSkin("R_Arm", tempToStr);

        tempToStr = L_Arm.L_Arm_Zombie_1_Attack.ToString();
        character.SetPlaceholderFromSkin("L_Arm", tempToStr);

        //손
        tempToStr = R_Hand_Attack.R_Hand_Attack_1.ToString();
        character.SetPlaceholderFromSkin("R_Hand_Attack", tempToStr);

        tempToStr = L_Hand_Attack.L_Hand_Attack_1.ToString();
        character.SetPlaceholderFromSkin("L_Hand_Attack", tempToStr);

        //하반신
        tempToStr = R_Leg.R_Leg_Zombie_1_Attack.ToString();
        character.SetPlaceholderFromSkin("R_Leg", tempToStr);
        tempToStr = L_Leg.L_Leg_Zombie_1_Attack.ToString();
        character.SetPlaceholderFromSkin("L_Leg", tempToStr);

        character.SetPlaceholderFromSkin("Eye_Black", null);
        character.SetPlaceholderFromSkin("Deco", null);
        character.SetPlaceholderFromSkin("Mouth", null);
        character.SetPlaceholderFromSkin("Mask", null);

        //눈
        tempToStr = Eye.Eye_Normal.ToString();
        character.SetPlaceholderFromSkin("Eye", tempToStr);

        //모션
        character.SetAnimationToAttack(Enums.ZombieState.Tank);
    }

    public void SetCharacterAttackZombieToJumpZombie(SpineAnimSwap character)
    {
        //머리
        string tempToStr = Hair.Hair_Zombie_2.ToString();
        character.SetPlaceholderFromSkin("Hair", tempToStr);

        //상반신
        tempToStr = Body.Body_Zombie_2.ToString();
        character.SetPlaceholderFromSkin("Body", tempToStr);

        tempToStr = R_Arm.R_Arm_Zombie_2_Attack.ToString();
        character.SetPlaceholderFromSkin("R_Arm", tempToStr);

        tempToStr = L_Arm.L_Arm_Zombie_2_Attack.ToString();
        character.SetPlaceholderFromSkin("L_Arm", tempToStr);

        //손
        tempToStr = R_Hand_Attack.R_Hand_Attack_2.ToString();
        character.SetPlaceholderFromSkin("R_Hand_Attack", tempToStr);

        tempToStr = L_Hand_Attack.L_Hand_Attack_2.ToString();
        character.SetPlaceholderFromSkin("L_Hand_Attack", tempToStr);

        //하반신
        tempToStr = R_Leg.R_Leg_Zombie_2_Attack.ToString();
        character.SetPlaceholderFromSkin("R_Leg", tempToStr);
        tempToStr = L_Leg.L_Leg_Zombie_2_Attack.ToString();
        character.SetPlaceholderFromSkin("L_Leg", tempToStr);

        character.SetPlaceholderFromSkin("Eye_Black", null);
        character.SetPlaceholderFromSkin("Deco", null);
        character.SetPlaceholderFromSkin("Mouth", null);
        character.SetPlaceholderFromSkin("Mask", null);

        //눈
        tempToStr = Eye.Eye_Suspic_1.ToString();
        character.SetPlaceholderFromSkin("Eye", tempToStr);

        //모션
        character.SetAnimationToAttack(Enums.ZombieState.Jump);
    }

    public void SetCharacterAttackZombieToTankZombie(SpineAnimSwap character)
    {
        //머리
        string tempToStr = Hair.Hair_Zombie_3.ToString();
        character.SetPlaceholderFromSkin("Hair", tempToStr);

        //상반신
        tempToStr = Body.Body_Zombie_3.ToString();
        character.SetPlaceholderFromSkin("Body", tempToStr);

        tempToStr = R_Arm.R_Arm_Zombie_3_Attack.ToString();
        character.SetPlaceholderFromSkin("R_Arm", tempToStr);

        tempToStr = L_Arm.L_Arm_Zombie_3_Attack.ToString();
        character.SetPlaceholderFromSkin("L_Arm", tempToStr);

        //손
        tempToStr = R_Hand_Attack.R_Hand_Attack_3.ToString();
        character.SetPlaceholderFromSkin("R_Hand_Attack", tempToStr);

        tempToStr = L_Hand_Attack.L_Hand_Attack_3.ToString();
        character.SetPlaceholderFromSkin("L_Hand_Attack", tempToStr);

        //하반신
        tempToStr = R_Leg.R_Leg_Zombie_3_Attack.ToString();
        character.SetPlaceholderFromSkin("R_Leg", tempToStr);
        tempToStr = L_Leg.L_Leg_Zombie_3_Attack.ToString();
        character.SetPlaceholderFromSkin("L_Leg", tempToStr);

        character.SetPlaceholderFromSkin("Eye_Black", null);
        character.SetPlaceholderFromSkin("Deco", null);
        character.SetPlaceholderFromSkin("Mouth", null);
        character.SetPlaceholderFromSkin("Mask", null);

        //눈
        tempToStr = Eye.Eye_Suspic_2.ToString();
        character.SetPlaceholderFromSkin("Eye", tempToStr);

        //모션
        character.SetAnimationToAttack(Enums.ZombieState.Tank);
    }
}
