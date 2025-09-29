
using UnityEngine;
using System;
using Ninsar.Showcase.MatrixPeek.Controls;
using Ninsar.Showcase.MatrixPeek.Core.Scriptable;
using Random = UnityEngine.Random;

namespace Ninsar.Showcase.MatrixPeek.Core
{
    [RequireComponent(typeof(GridInput))]
    [RequireComponent(typeof(GridView))]
    internal sealed class GridController : MonoBehaviour
    {
        public event Action onWindowChangedEvent;

        public GridModel model { get; private set; }
        public GridView view { get; private set; }

        public Vector2Int viewOrigin { get; private set; }

        private GridControllerConfigSO _config;
        private GridInput _input;

        private void Awake()
        {
            _config = GridControllerConfigSO.Load();

            view = GetComponent<GridView>();
            _input = GetComponent<GridInput>();
        }

        private void Start()
        {
            model = new GridModel(_config.gridDataFileName);

            view.Initialize(model, _config.gridSize);
            viewOrigin = new Vector2Int(Random.Range(0, model.cols), Random.Range(0, model.rows));

            Render();
        }

        private void OnEnable() => _input.onMoveEvent += OnMove;
        private void OnDisable() => _input.onMoveEvent -= OnMove;

        private void OnMove(Vector2Int delta)
        {
            var newX = (viewOrigin.x + delta.x % model.cols + model.cols) % model.cols;
            var newY = (viewOrigin.y + delta.y % model.rows + model.rows) % model.rows;
            viewOrigin = new Vector2Int(newX, newY);

            Render();
        }

        private void Render()
        {
            var subGridData = model.GetSubGrid(viewOrigin.x, viewOrigin.y, _config.gridSize);
            view.RenderGrid(subGridData);

            onWindowChangedEvent?.Invoke();
        }
    }
}