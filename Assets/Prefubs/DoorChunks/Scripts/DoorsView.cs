using TMPro;
using UnityEngine;

public class DoorsView : MonoBehaviour
{
    [Header("Renderers")]
    [SerializeField] private SpriteRenderer _leftRenderer;
    [SerializeField] private SpriteRenderer _rightRenderer;
    [Header("Labels")]
    [SerializeField] private TMP_Text _leftLabel;
    [SerializeField] private TMP_Text _rightLabel;

    public void Apply(DoorPairConfig cfg)
    {
        ApplyOne(_leftRenderer, _leftLabel, cfg.left, cfg);
        ApplyOne(_rightRenderer, _rightLabel, cfg.right, cfg);
    }

    private void ApplyOne(SpriteRenderer rend, TMP_Text txt, DoorPairConfig.Side side, DoorPairConfig cfg)
    {
        if(!rend || !txt) return;
        bool isBonus = DoorPairConfig.IsBonus(side.type);
        rend.color = isBonus ? cfg.bonusColor : cfg.penaltyColor;
        txt.text = string.IsNullOrWhiteSpace(side.labelOverride)
            ? DoorPairConfig.FormatLabel(side.type, Mathf.Abs(side.value))
            : side.labelOverride;
    }
}