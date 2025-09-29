
using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Ninsar.Showcase.MatrixPeek.Controls;
using Random = UnityEngine.Random;

namespace Ninsar.Showcase.MatrixPeek.Core
{
    internal sealed class GridController : MonoBehaviour
    {
        [field: Header("Grid Layout")]
        [field: SerializeField, Min(0.01f)] private float _cellSpacing = 1.0f;
        [field: SerializeField] private Vector3 _originOffset = Vector3.zero;

        [field: Space(10)]
        [field: SerializeField] private bool _flipHorizontal;
        [field: SerializeField] private bool _flipVertical;

        [field: Header("References")]
        [field: SerializeField] private GridInput _input;

        private readonly Color[] _palette =
        {
            Color.red,
            Color.yellow,
            Color.blue,
            new(0.5f, 0f, 0.5f)
        };

        public List<string> lines { get; private set; }

        public int rows { get; private set; }
        public int cols { get; private set; }

        private int _gridSize = 9;

        private Vector2Int _windowPos;

        private readonly List<GameObject> _cubes = new();

        public Vector2Int windowPos => _windowPos;

        public event Action onWindowChangedEvent;

        private void Start()
        {
            LoadFile();
            InitCubes();
            RenderWindow();
        }

        private void OnEnable() => _input.onMoveEvent += OnMove;

        private void OnDisable() => _input.onMoveEvent -= OnMove;

        private void LoadFile()
        {
            var path = Path.Combine(Application.streamingAssetsPath, "grid.txt");
            if (!File.Exists(path))
            {
                Debug.LogError($"File not found: {path}");
                return;
            }

            lines = new List<string>(File.ReadAllLines(path));
            rows = lines.Count;
            cols = lines[0].Length;

            _windowPos = new Vector2Int(
                Random.Range(0, cols),
                Random.Range(0, rows)
            );
        }

        private void InitCubes()
        {
            for (var i = 0; i < _gridSize; i++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = transform;
                cube.transform.localScale = Vector3.one * 0.9f;
                _cubes.Add(cube);
            }
        }

        private void RenderWindow()
        {
            var index = 0;
            for (var y = 0; y < 3; y++)
            {
                for (var x = 0; x < 3; x++)
                {
                    var srcX = (_windowPos.x + x) % cols;
                    var srcY = (_windowPos.y + y) % rows;

                    var ch = lines[srcY][srcX];
                    var digit = ch - '0';

                    int rx = x, ry = y;

                    if (_flipHorizontal)
                    {
                        rx = 2 - rx;
                    }

                    if (_flipVertical)
                    {
                        ry = 2 - ry;
                    }

                    var localPos = new Vector3((rx - 1) * _cellSpacing, 0f, (ry - 1) * _cellSpacing) + _originOffset;

                    _cubes[index].transform.localPosition = localPos;
                    var rend = _cubes[index].GetComponent<Renderer>();
                    rend.material.color = _palette[Mathf.Clamp(digit - 1, 0, 3)];

                    index++;
                }
            }

            onWindowChangedEvent?.Invoke();
        }

        private void OnMove(Vector2Int delta)
        {
            _windowPos.x = (_windowPos.x + delta.x % cols + cols) % cols;
            _windowPos.y = (_windowPos.y + delta.y % rows + rows) % rows;

            RenderWindow();
        }
    }
}