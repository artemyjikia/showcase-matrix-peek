
using UnityEngine;
using System;
using System.Linq;

namespace Ninsar.Showcase.MatrixPeek.Core
{
    internal sealed class GridModel
    {
        public int rows { get; }
        public int cols { get; set; }

        private readonly int[,] _gridData;

        public GridModel(string fileName)
        {
            var textAsset = Resources.Load<TextAsset>(fileName);
            if (textAsset == null)
            {
                Debug.LogError($"[GridModel] Failed to load file from Resources: '{fileName}'!");
                return;
            }

            if (string.IsNullOrWhiteSpace(textAsset.text))
            {
                Debug.LogError($"[GridModel] Grid file '{fileName}' is empty or contains only whitespace!");
                return;
            }

            var lines = textAsset.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
            {
                Debug.LogError($"[GridModel] File '{fileName}' does not contain any valid data rows!");
                return;
            }

            rows = lines.Length;
            cols = lines[0].Length;

            if (lines.Any(line => line.Length != cols))
            {
                Debug.LogError($"[GridModel] Parsing error in '{fileName}': All rows must have the same length. Expected length: {cols}.");

                rows = 0;
                cols = 0;

                return;
            }

            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    var character = lines[y][x];
                    if (!char.IsDigit(character))
                    {
                        Debug.LogError($"[GridModel] Parsing error in '{fileName}': Invalid character '{character}' found at line {y + 1}, column {x + 1}.");

                        rows = 0;
                        cols = 0;

                        return;
                    }
                }
            }

            _gridData = new int[cols, rows];

            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    _gridData[x, y] = lines[y][x] - '0';
                }
            }

            Debug.Log($"[GridModel] Grid '{fileName}' loaded successfully. Size: {cols}x{rows}.");
        }

        public int GetValueAt(int x, int y)
        {
            return x < 0 || x >= cols || y < 0 || y >= rows ? 0 : _gridData[x, y];
        }

        public int[,] GetSubGrid(int startX, int startY, int size)
        {
            var subGrid = new int[size, size];
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var srcX = (startX + x + cols) % cols;
                    var srcY = (startY + y + rows) % rows;
                    subGrid[x, y] = _gridData[srcX, srcY];
                }
            }

            return subGrid;
        }
    }
}