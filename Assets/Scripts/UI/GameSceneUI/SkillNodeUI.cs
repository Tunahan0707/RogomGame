using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Refs")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button unlockButton;
    [SerializeField] private Image background;
    [SerializeField] private Image blackOverlay; // level yetmiyorsa/kapalıysa

    private SkillNodeDef nodeDef;
    private SkillMapUIManager mapUI;

    public SkillSO Skill => nodeDef != null ? nodeDef.skill : null;

    public void Setup(SkillNodeDef def, SkillMapUIManager ui)
    {
        nodeDef = def;
        mapUI = ui;

        if (Skill == null)
        {
            iconImage.enabled = false;
            nameText.text = "Boş";
            unlockButton.interactable = false;
            UpdateVisual();
            return;
        }

        iconImage.enabled = true;
        iconImage.sprite = Skill.icon;
        nameText.text = Skill.skillName;

        unlockButton.onClick.RemoveAllListeners();
        unlockButton.onClick.AddListener(() =>
        {
            if (SkillMapManager.Instance.TryUnlock(Skill))
                UpdateVisual();
        });

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        var mgr = SkillMapManager.Instance;
        if (mgr == null || Skill == null)
        {
            unlockButton.interactable = false;
            if (blackOverlay) blackOverlay.enabled = true;
            return;
        }

        bool unlocked = mgr.IsUnlocked(Skill);
        bool blocked  = mgr.IsBlocked(Skill);
        bool eligible = mgr.CanUnlock(Skill);

        if (unlocked)
        {
            background.color = Skill.uiColor;
            unlockButton.interactable = false;
            if (blackOverlay) blackOverlay.enabled = false;
        }
        else if (blocked)
        {
            background.color = Color.gray;
            unlockButton.interactable = false;
            if (blackOverlay) blackOverlay.enabled = true;
        }
        else if (!eligible)
        {
            background.color = Color.gray;
            unlockButton.interactable = false;

            int level = XPManager.Instance != null ? XPManager.Instance.GetPlayerLevel() : 1;
            bool levelYetmiyor = level < (nodeDef != null ? nodeDef.requiredLevel : int.MaxValue);
            if (blackOverlay) blackOverlay.enabled = levelYetmiyor;
        }
        else
        {
            background.color = Color.white;
            unlockButton.interactable = true;
            if (blackOverlay) blackOverlay.enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var mgr = SkillMapManager.Instance;
        if (mgr == null || Skill == null) return;

        int level = XPManager.Instance != null ? XPManager.Instance.GetPlayerLevel() : 1;
        bool levelOK = nodeDef != null && level >= nodeDef.requiredLevel;
        bool blocked = mgr.IsBlocked(Skill);

        string text = (levelOK && !blocked)
            ? $"{Skill.skillName}\n{Skill.description}\nMaliyet: {Skill.cost} PUAN"
            : string.Empty;

        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Show(text, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Hide();
    }
}
