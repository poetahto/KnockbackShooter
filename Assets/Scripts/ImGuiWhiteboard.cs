using System;
using System.Collections.Generic;
using UnityEngine;

public class ImGuiWhiteboard : MonoBehaviour
{
    public static ImGuiWhiteboard Instance { get; private set; }

    private readonly List<Action> _drawActions = new();
    private Rect _windowRect = new(20, 20, 120, 50);

    private void Awake()
    {
        Instance = this;
    }

    public void Register(Action action)
    {
        _drawActions.Add(action);
    }

    public void Unregister(Action action)
    {
        _drawActions.Remove(action);
    }

    private void OnGUI()
    {
        _windowRect = GUILayout.Window(0, _windowRect, RenderGUI, "Whiteboard");
    }

    private void RenderGUI(int windowId)
    {
        foreach (Action drawAction in _drawActions)
            drawAction.Invoke();
    }
}