
using UnityEngine;

namespace Ninsar.Showcase.MatrixPeek.Core.Scriptable
{
    [CreateAssetMenu(fileName = _fileName, menuName = "Game/Core/Grid Controller Config")]
    internal sealed class GridControllerConfigSO : ScriptableObject
    {
        [field: Header("General Settings")]
        [field: SerializeField] public string gridDataFileName { get; private set; }
        [field: SerializeField, Min(1)] public int gridSize { get; private set; }

        private const string _fileName = "GridControllerConfig";

        public static GridControllerConfigSO Load() => Resources.Load<GridControllerConfigSO>(_fileName);
    }
}