using UnityEngine;


[CreateAssetMenu(fileName = "NewSkill", menuName = "ScriptableObjects/Skill")]
public class SkillSO : ScriptableObject
{
    [Header("Skill Info")]
    public string skillName;
    [TextArea(2, 4)] public string description;


    [Header("Visuals")]
    public Sprite icon;
    public Color uiColor = Color.white;
    [Header("Gameplay Data")]
    public string skillID; // benzersiz id
    public int cost = 1; // PUAN maliyeti
    public SkillType type = SkillType.Passive; // Pasif/aktif ayrımı
    public int manaCost = 0; // Sadece aktiflerde anlamlı
    [Tooltip("Skill açıldığında eklenecek gizli lanet yüzdesi")] public int curseGain = 5;


    private void OnValidate()
    {
        if (string.IsNullOrEmpty(skillID))
        {
            skillID = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
public enum SkillType { Passive, Active }