
using UnityEngine;
using System.Collections.Generic;
using Ninsar.Showcase.MatrixPeek.Core.Scriptable;

namespace Ninsar.Showcase.MatrixPeek.Core
{
    internal sealed class GridView : MonoBehaviour
    {
        public int сlampedGridSize { get; private set; }

        private readonly List<CellView> _cellViews = new();

        private GridViewConfigSO _config;

        private void Awake() => _config = GridViewConfigSO.Load();

        public void Initialize(GridModel model, int requestedSize)
        {
            сlampedGridSize = Mathf.Clamp(requestedSize, 1, Mathf.Min(model.rows, model.cols));
            for (var i = 0; i < сlampedGridSize * сlampedGridSize; i++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = transform;
                cube.transform.localScale = Vector3.one * 0.9f;
                _cellViews.Add(new CellView(cube));
            }
        }

        public void RenderGrid(int[,] data)
        {
            var index = 0;
            for (var y = 0; y < сlampedGridSize; y++)
            {
                for (var x = 0; x < сlampedGridSize; x++)
                {
                    var digit = data[x, y];

                    var renderX = _config.flipHorizontal ? сlampedGridSize - 1 - x : x;
                    var renderY = _config.flipVertical ? сlampedGridSize - 1 - y : y;

                    var halfSize = (сlampedGridSize - 1) / 2f;
                    var localPos = new Vector3((renderX - halfSize) * _config.cellSpacing, 0f, (renderY - halfSize) * _config.cellSpacing) + _config.originOffset;

                    var cellView = _cellViews[index];
                    cellView.transform.localPosition = localPos;
                    cellView.renderer.material.color = _config.palette[Mathf.Clamp(digit - 1, 0, _config.palette.Length - 1)];

                    index++;
                }
            }
        }

        private readonly struct CellView
        {
            public readonly Transform transform;
            public readonly Renderer renderer;

            public CellView(GameObject gameObject)
            {
                transform = gameObject.transform;
                renderer = gameObject.GetComponent<Renderer>();
            }
        }
    }
}