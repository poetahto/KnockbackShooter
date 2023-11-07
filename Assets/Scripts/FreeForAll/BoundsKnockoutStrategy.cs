#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif
using UnityEngine;

public static class Transform1D
{
    public static float SmoothStart2(float t) => t * t;
    public static float SmoothStart3(float t) => t * t * t;
    public static float SmoothStart4(float t) => t * t * t * t;
    public static float SmoothStart5(float t) => t * t * t * t * t;
    public static float SmoothStart6(float t) => t * t * t * t * t * t;
}

namespace FreeForAll
{
    public class BoundsKnockoutStrategy : KnockoutStrategy
    {
        [SerializeField] 
        private Bounds bounds;

        public override void OnLogic()
        {
            foreach (GameObject target in Manager.Targets)
            {
                if (!bounds.Contains(target.transform.position))
                {
                    Manager.OnKnockout(new KnockoutManager.KnockoutData
                    {
                        Object = target
                    });
                }
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(BoundsKnockoutStrategy))]
        private class BoundsStrategyEditor : Editor
        {
            private readonly BoxBoundsHandle _handle = new();
            
            private void OnSceneGUI()
            {
                var t = (BoundsKnockoutStrategy) target;

                using var _ = new Handles.DrawingScope(Color.red);
                
                // Draw the handle for editing bounds
                t.bounds.center = t.transform.position;
                _handle.size = t.bounds.size;
                _handle.center = t.bounds.center;
                // _handle.wireframeColor = Color.red;
                // _handle.handleColor = Color.red;
                EditorGUI.BeginChangeCheck();
                _handle.DrawHandle();
                
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(t, "Change Bounds");
                    t.bounds.size = _handle.size;
                    t.bounds.center = _handle.center;
                }

                // Draw views for each player
                if (t.Manager != null)
                {
                    foreach (GameObject obj in t.Manager.Targets)
                    {
                        var objPos = obj.transform.position;
                        var farAway = (objPos - t.bounds.center).normalized * (t.bounds.extents.magnitude);
                        t.bounds.IntersectRay(new Ray(farAway, (objPos - farAway).normalized), out float distance);
                        farAway += (objPos - farAway).normalized * distance;
                        var closest = t.bounds.ClosestPoint(farAway);
                        if (obj.TryGetComponent(out MeshRenderer mr))
                            objPos = mr.bounds.ClosestPoint(closest);
                        var mid = ((closest - objPos) * 0.5f) + objPos;
                        Handles.DrawLine(objPos, closest);
                        Handles.DrawWireCube(objPos, Vector3.one * 0.25f);
                        Handles.DrawWireCube(closest, Vector3.one * 0.25f);
                        float percent = Transform1D.SmoothStart2(1 - Mathf.Clamp01((closest - objPos).magnitude / (closest - t.bounds.center).magnitude));
                        Handles.Label(mid, $"Danger: {percent}");
                    }
                }
            }
        }
#endif
    }
}