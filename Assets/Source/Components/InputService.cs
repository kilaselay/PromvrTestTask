using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PromvrTestTask
{
    public class InputService : MonoBehaviour, IInputService
    {
        public event Action ClickActionButton;
        public event Action ClickResetButton;

        private Vector2 _movement = Vector2.zero;

        public Vector2 Movement => _movement;

        private void Update()
        {
            CheckSpace();
            UpdateWASD();
            CheckBackspace();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckSpace()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ClickActionButton?.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateWASD()
        {
            var currentMovement = Vector2.zero;

            if (Input.GetKey(KeyCode.D))
                currentMovement.x = 1f;
            else if (Input.GetKey(KeyCode.A))
                currentMovement.x = -1f;

            if (Input.GetKey(KeyCode.W))
                currentMovement.y = 1f;
            else if (Input.GetKey(KeyCode.S))
                currentMovement.y = -1f;

            _movement = currentMovement;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckBackspace()
        {
            if(Input.GetKeyDown(KeyCode.Backspace))
                ClickResetButton?.Invoke();
        }
    }
}
