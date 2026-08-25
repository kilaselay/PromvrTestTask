using System;
using UnityEngine;

namespace PromvrTestTask
{
    public interface IInputService
    {
        public event Action ClickActionButton;
        public event Action ClickResetButton;

        public Vector2 Movement { get; }
    }
}
