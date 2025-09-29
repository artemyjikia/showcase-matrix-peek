
using UnityEngine;

namespace Ninsar.Showcase.MatrixPeek.Core.Scriptable
{
    [CreateAssetMenu(fileName = "UIStyleConfig", menuName = "Game/Core/UI Style Config")]
    internal sealed class UIStyleConfigSO : ScriptableObject
    {
        [field: Header("Layout Settings")]
        [field: SerializeField] public int cellSize { get; private set; } = 36;
        [field: SerializeField] public Vector2 cellSpacing { get; private set; } = new(2, 2);
        [field: SerializeField] public float panelPadding { get; private set; } = 8f;

        [field: Header("Colors Settings")]
        [field: SerializeField] public Color textColor { get; private set; } = Color.white;
        [field: SerializeField] public Color panelBackgroundColor { get; private set; } = new(0, 0, 0, 0.5f);
        [field: SerializeField] public Color cellNormalColor { get; private set; } = new(0, 0, 0, 0.35f);
        [field: SerializeField] public Color cellHighlightColor { get; private set; } = new(1f, 0.85f, 0.2f, 0.75f);
    }
}