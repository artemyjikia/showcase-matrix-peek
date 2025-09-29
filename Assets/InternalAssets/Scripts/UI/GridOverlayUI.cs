
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Ninsar.Showcase.MatrixPeek.Core.Scriptable;

namespace Ninsar.Showcase.MatrixPeek.Core
{
    [RequireComponent(typeof(Canvas))]
    internal sealed class GridOverlayUI : MonoBehaviour
    {
        [field: Header("Settings")] 
        [field: SerializeField] private UIStyleConfigSO _config;

        [field: Header("References")] 
        [field: SerializeField] private GridController _controller;

        private readonly List<GridUIBuilder.CellElements> _cells = new();

        private RectTransform _rootPanel;
        private GridLayoutGroup _gridContainer;

        private bool _isVisible;

        private int _cachedRows;
        private int _cachedCols;

        private void Awake()
        {
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            if (!GetComponent<GraphicRaycaster>())
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }

            BuildUI();
            SetVisible(false);
        }

        private void OnEnable() => _controller.onWindowChangedEvent += Refresh;

        private void OnDisable() => _controller.onWindowChangedEvent -= Refresh;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                SetVisible(!_isVisible);
            }
        }

        public void SetVisible(bool visible)
        {
            _isVisible = visible;
            _rootPanel.gameObject.SetActive(visible);

            if (visible)
            {
                Refresh();
            }
        }

        private void BuildUI()
        {
            _rootPanel = GridUIBuilder.BuildRootPanel(transform);
            _gridContainer = GridUIBuilder.BuildGridContainer(_rootPanel, _config.cellSize, _config.cellSpacing);

            FitPanelToGrid();
        }

        private void Rebuild()
        {
            if (_controller == null || _controller.model == null)
            {
                return;
            }

            var rows = _controller.model.rows;
            var cols = _controller.model.cols;
            if (rows <= 0 || cols <= 0)
            {
                return;
            }

            if (rows == _cachedRows && cols == _cachedCols && _cells.Count == rows * cols)
            {
                return;
            }

            _cachedRows = rows;
            _cachedCols = cols;

            var need = rows * cols;
            var have = _cells.Count;

            for (var i = have - 1; i >= need; i--)
            {
                Destroy(_cells[i].labelText.gameObject.transform.parent.gameObject);
                _cells.RemoveAt(i);
            }

            for (var i = have; i < need; i++)
            {
                var cellElements = GridUIBuilder.BuildCell(_gridContainer.transform, i, _config.cellSize, _config.panelBackgroundColor, _config.textColor);
                _cells.Add(new GridUIBuilder.CellElements { backgroundImage = cellElements.backgroundImage, labelText = cellElements.labelText });
            }

            _gridContainer.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridContainer.constraintCount = cols;

            FitPanelToGrid();
        }

        private void Refresh()
        {
            if (!_isVisible)
            {
                return;
            }

            if (_controller == null || _controller.model == null)
            {
                return;
            }

            Rebuild();

            var rows = _controller.model.rows;
            var cols = _controller.model.cols;
            var pos = _controller.viewOrigin;

            var gridSize = _controller.view.сlampedGridSize;

            bool InWindow(int x, int y)
            {
                var dx = (x - pos.x + cols) % cols;
                var dy = (y - pos.y + rows) % rows;
                return dx >= 0 && dx < gridSize && dy >= 0 && dy < gridSize;
            }

            var index = 0;
            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    var value = _controller.model.GetValueAt(x, y);
                    var cell = _cells[index];
                    cell.labelText.text = value.ToString();
                    cell.backgroundImage.color = InWindow(x, y) ? _config.cellHighlightColor : _config.cellNormalColor;

                    index++;
                }
            }
        }

        private void FitPanelToGrid()
        {
            if (_controller == null || _controller.model == null)
            {
                return;
            }

            var cols = _controller.model.cols;
            var rows = _controller.model.rows;

            var spacing = _gridContainer.spacing;
            var cell = _gridContainer.cellSize;

            var width = cols * cell.x + (cols - 1) * spacing.x;
            var height = rows * cell.y + (rows - 1) * spacing.y;

            var gridRectTransform = (RectTransform)_gridContainer.transform;
            gridRectTransform.anchorMin = gridRectTransform.anchorMax = new Vector2(0, 1);
            gridRectTransform.pivot = new Vector2(0, 1);
            gridRectTransform.anchoredPosition = Vector2.zero;
            gridRectTransform.sizeDelta = new Vector2(width, height);
            _gridContainer.childAlignment = TextAnchor.UpperLeft;
            _gridContainer.startCorner = GridLayoutGroup.Corner.UpperLeft;

            var padding = _config.panelPadding;
            _rootPanel.pivot = new Vector2(1, 1);
            _rootPanel.sizeDelta = new Vector2(width + padding * 2, height + padding * 2);
            ((RectTransform)_gridContainer.transform).anchoredPosition = new Vector2(padding, -padding);
        }
    }
}