using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText; // tooltip yerine prefab içi açıklama
    [SerializeField] private Button unlockButton;
    [SerializeField] private Image background;
    [SerializeField] private Image blackOverlay;

    private SkillNodeDef nodeDef;
    private SkillMapUIManager mapUI;
    private Color defaultColor;

    public SkillSO Skill => nodeDef != null ? nodeDef.skill : null;

    public void Setup(SkillNodeDef def, SkillMapUIManager ui)
    {
        nodeDef = def;
        mapUI = ui;

        descriptionText.gameObject.SetActive(false); // başlangıçta gizli
        if (Skill == null)
        {
            nameText.text = "Boş";
            iconImage.enabled = false;
            unlockButton.interactable = false;
            return;
        }

        nameText.text = Skill.skillName;
        iconImage.sprite = Skill.icon;
        defaultColor = background.color;

        unlockButton.onClick.RemoveAllListeners();
        unlockButton.onClick.AddListener(() =>
        {
            if (SkillMapManager.Instance.TryUnlock(Skill))
                UpdateVisual();
        });

        UpdateVisual();
    }

    public SkillNodeDef GetNodeDef() => nodeDef;

    public void UpdateVisual()
    {
        var mgr = SkillMapManager.Instance;
        if (mgr == null || Skill == null) return;

        bool unlocked = mgr.IsUnlocked(Skill);
        bool blocked = mgr.IsBlocked(Skill);
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
            if (blackOverlay) blackOverlay.enabled = true;
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
        if (Skill == null) return;

        descriptionText.text = Skill.description;
        descriptionText.gameObject.SetActive(true);

        mapUI.HighlightConnectionsForSkill(Skill); // 🔥 çizgileri parlat
        if (!string.IsNullOrEmpty(nodeDef.exclusiveGroup))
            mapUI.HighlightExclusiveGroup(nodeDef.exclusiveGroup, this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionText.gameObject.SetActive(false);

        mapUI.ResetAllHighlights(); // 🔥 çizgileri eski hale döndür
        mapUI.ResetExclusiveHighlights();
    }


    public string GetExclusiveGroup() => nodeDef != null ? nodeDef.exclusiveGroup : string.Empty;
}
