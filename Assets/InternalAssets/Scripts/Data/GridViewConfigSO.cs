
using UnityEngine;

namespace Ninsar.Showcase.MatrixPeek.Core.Scriptable
{
    [CreateAssetMenu(fileName = _fileName, menuName = "Game/Core/Grid View Config")]
    internal sealed class GridViewConfigSO : ScriptableObject
    {
        [field: Header("Layout Settings")]
        [field: SerializeField, Min(0.01f)] public float cellSpacing { get; private set; } = 1.0f;
        [field: SerializeField] public Vector3 originOffset { get; private set; } = Vector3.zero;

        [field: Header("Appearance Settings")]
        [field: SerializeField] public bool flipHorizontal { get; private set; }
        [field: SerializeField] public bool flipVertical { get; private set; }

        [field: Header("Colors Settings")]
        [field: SerializeField] public Color[] palette { get; private set; } =
        {
            Color.red,
            Color.yellow,
            Color.blue,
            new(0.5f, 0f, 0.5f)
        };

        private const string _fileName = "GridViewConfig";

        public static GridViewConfigSO Load() => Resources.Load<GridViewConfigSO>(_fileName);
    }
}