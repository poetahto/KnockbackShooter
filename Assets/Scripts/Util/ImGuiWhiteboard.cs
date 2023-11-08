using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class ImGuiWhiteboard : MonoBehaviour
    {
        public static ImGuiWhiteboard Instance { get; private set; }

        private readonly List<Action> _drawActions = new();

        private void Awake()
        {
            Instance = this;
        }

        public IDisposable Register(Action action)
        {
            _drawActions.Add(action);
            return new DisposableAction(() => Unregister(action));
        }

        public void Unregister(Action action)
        {
            _drawActions.Remove(action);
        }

        private Rect _windowRect = new(10, 10, 120, 50);

        private void OnGUI()
        {
            _windowRect = GUILayout.Window(0, _windowRect, DrawWindow, "Whiteboard");
        }

        private void DrawWindow(int windowId)
        {
            GUILayout.FlexibleSpace();
        
            foreach (Action drawAction in _drawActions)
                drawAction.Invoke();
        
            GUILayout.FlexibleSpace();
            GUI.DragWindow();
        }
    }
}