using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillMap", menuName = "ScriptableObjects/SkillMap")]
public class SkillMapSO : ScriptableObject
{
    [Header("Skill Node Listesi (TANIM)")]
    [SerializeReference] public List<SkillNodeDef> skillNodes = new(); // STATE burada tutulmaz!
}

[Serializable]
public class SkillNodeDef
{
    public SkillSO skill;
    public int requiredLevel = 1;
    public bool isCenter;
    [SerializeReference] public List<SkillNodeDef> prerequisites = new();
    public Vector2 position;

    [Header("Alternatif Grup (Exclusive Branch)")]
    [Tooltip("Aynı gruptaki skill'lerden yalnızca biri aktif olabilir.")]
    public string exclusiveGroup;

    [Header("Başlangıç Skill'i")]
    [Tooltip("Prerequisite olmadan açılabilir başlangıç noktası.")]
    public bool isStarter = false;
}
