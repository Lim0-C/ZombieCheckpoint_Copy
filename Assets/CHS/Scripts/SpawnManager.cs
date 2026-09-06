using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs (UI Image + NPC)")]
    public NPC citizenPrefab;
    public NPC suspectPrefab;
    public NPC zombiePrefab;

    [Header("Lane Parent")]
    public RectTransform lane;

    // 간단 랜덤 스폰
    public NPC SpawnRandom()
    {
        int r = Random.Range(0, 100);
        NPC prefab;
        if (r < 45)       prefab = citizenPrefab; // 45%
        else if (r < 80)  prefab = suspectPrefab; // 35%
        else              prefab = zombiePrefab;  // 20%

        var npc = Instantiate(prefab, lane);
        npc.SetupAtCenter(lane);
        return npc;
    }
}