
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Ninsar.Showcase.MatrixPeek.Core
{
    [RequireComponent(typeof(Canvas))]
    internal sealed class GridOverlayUI : MonoBehaviour
    {
        [field: Header("Settings")] 
        [field: SerializeField] private int _cellSize = 36;
        [field: SerializeField] private Color _textColor = Color.white;
        [field: SerializeField] private Color _bgNormal = new Color(0, 0, 0, 0.35f);
        [field: SerializeField] private Color _bgHighlight = new Color(1f, 0.85f, 0.2f, 0.75f);

        [field: Header("References")] 
        [field: SerializeField] private GridController _controller;

        private readonly List<Cell> _cells = new();

        private RectTransform _rootPanel;
        private GridLayoutGroup _grid;

        private bool _isVisible;
        private int cachedRows;
        private int cachedCols;

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
            var panelGO = new GameObject("Grid (Overlay Panel)", typeof(RectTransform), typeof(Image));
            _rootPanel = (RectTransform)panelGO.transform;
            _rootPanel.SetParent(transform, false);
            _rootPanel.anchorMin = new Vector2(1, 1);
            _rootPanel.anchorMax = new Vector2(1, 1);
            _rootPanel.pivot = new Vector2(1, 1);
            _rootPanel.anchoredPosition = new Vector2(-16, -16);
            _rootPanel.sizeDelta = new Vector2(400, 400);

            if (_rootPanel.TryGetComponent(out Image background))
            {
                background.color = new Color(0f, 0f, 0f, 0.5f);
            }

            var gridGO = new GameObject("Grid", typeof(RectTransform), typeof(GridLayoutGroup));
            var rectTransform = (RectTransform)gridGO.transform;
            rectTransform.SetParent(_rootPanel, false);
            rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(_cellSize * 20, _cellSize * 12);

            _grid = gridGO.GetComponent<GridLayoutGroup>();
            _grid.cellSize = new Vector2(_cellSize, _cellSize);
            _grid.spacing = new Vector2(2, 2);
            _grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            _grid.startAxis = GridLayoutGroup.Axis.Horizontal;

            FitPanelToGrid();
        }

        private void FitPanelToGrid()
        {
            var cols = _controller?.cols ?? 0;
            var rows = _controller?.rows ?? 0;

            if (cols == 0 || rows == 0)
            {
                return;
            }

            var spacing = _grid.spacing;
            var cell = _grid.cellSize;

            var width = cols * cell.x + (cols - 1) * spacing.x;
            var height = rows * cell.y + (rows - 1) * spacing.y;

            var grt = (RectTransform)_grid.transform;
            grt.anchorMin = grt.anchorMax = new Vector2(0, 1);
            grt.pivot = new Vector2(0, 1);
            grt.anchoredPosition = Vector2.zero;
            grt.sizeDelta = new Vector2(width, height);
            _grid.childAlignment = TextAnchor.UpperLeft;
            _grid.startCorner = GridLayoutGroup.Corner.UpperLeft;

            const float pad = 8f;
            _rootPanel.pivot = new Vector2(1, 1);
            _rootPanel.sizeDelta = new Vector2(width + pad * 2, height + pad * 2);
            ((RectTransform)_grid.transform).anchoredPosition = new Vector2(pad, -pad);
        }

        private void Rebuild()
        {
            if (_controller == null || _controller.lines == null)
            {
                return;
            }

            var rows = _controller.rows;
            var cols = _controller.cols;
            if (rows <= 0 || cols <= 0)
            {
                return;
            }

            if (rows == cachedRows && cols == cachedCols && _cells.Count == rows * cols)
            {
                return;
            }

            cachedRows = rows;
            cachedCols = cols;

            var need = rows * cols;
            var have = _cells.Count;

            for (var i = have - 1; i >= need; i--)
            {
                Destroy(_cells[i].labelText.gameObject.transform.parent.gameObject);
                _cells.RemoveAt(i);
            }

            for (var i = have; i < need; i++)
            {
                var cellGO = new GameObject($"cell_{i}", typeof(RectTransform), typeof(Image));
                var rt = (RectTransform)cellGO.transform;
                rt.SetParent(_grid.transform, false);

                var backgroundImage = cellGO.GetComponent<Image>();
                backgroundImage.color = _bgNormal;

                var textGO = new GameObject("label", typeof(RectTransform), typeof(TextMeshProUGUI));
                var trt = (RectTransform)textGO.transform;
                trt.SetParent(cellGO.transform, false);
                trt.anchorMin = trt.anchorMax = new Vector2(0.5f, 0.5f);
                trt.pivot = new Vector2(0.5f, 0.5f);
                trt.sizeDelta = Vector2.zero;

                var labelText = textGO.GetComponent<TextMeshProUGUI>();
                labelText.alignment = TextAlignmentOptions.Center;
                labelText.fontSize = Mathf.RoundToInt(_cellSize * 0.5f);
                labelText.color = _textColor;

                _cells.Add(new Cell { backgroundImage = backgroundImage, labelText = labelText });
            }

            _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _grid.constraintCount = cols;

            FitPanelToGrid();
        }

        private void Refresh()
        {
            if (!_isVisible)
            {
                return;
            }

            if (_controller == null || _controller.lines == null)
            {
                return;
            }

            Rebuild();

            var rows = _controller.rows;
            var cols = _controller.cols;
            var pos = _controller.windowPos;

            bool InWindow(int x, int y)
            {
                var dx = (x - pos.x + cols) % cols;
                var dy = (y - pos.y + rows) % rows;
                return dx >= 0 && dx < 3 && dy >= 0 && dy < 3;
            }

            var index = 0;
            for (var y = 0; y < rows; y++)
            {
                var line = _controller.lines[y];
                for (var x = 0; x < cols; x++)
                {
                    var ch = line[x];
                    var cell = _cells[index];
                    cell.labelText.text = ch.ToString();
                    cell.backgroundImage.color = InWindow(x, y) ? _bgHighlight : _bgNormal;

                    index++;
                }
            }
        }

        private struct Cell
        {
            public Image backgroundImage;
            public TextMeshProUGUI labelText;
        }
    }
}