
using UnityEngine;
using System;

namespace Ninsar.Showcase.MatrixPeek.Controls
{
    internal sealed class GridInput : MonoBehaviour
    {
        public event Action<Vector2Int> onMoveEvent;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                onMoveEvent?.Invoke(new Vector2Int(-1, 0));
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                onMoveEvent?.Invoke(new Vector2Int(+1, 0));
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                onMoveEvent?.Invoke(new Vector2Int(0, -1));
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                onMoveEvent?.Invoke(new Vector2Int(0, +1));
            }
        }
    }
}