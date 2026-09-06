
public static class Enums
{
    //사운드
    public enum SfxClips
    {
        GameEnd,
        SortSuccess,
        SortFailed,
        NormalZombieAttack,
        JumpZombieAttack,
        TankZombieAttack,
        ShootZombie,
        PlayerHit,
    }

    //npc가 될수 있는 상태들
    public enum NpcState
    {
        Normal,
        Suspicious,
        Zombie
    }

    public enum ZombieState
    {
        Normal,
        Jump,
        Tank
    }
}
