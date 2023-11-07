#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif
using UnityEngine;

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
                        // Goal: find out how far away the player is from the edge of the box.
                        // It's made complicated since we are inside of it.
                        // First, we trace a line from the center towards the target, and try to snap it to the edge of the bounds if we overshot.
                        // Then, adjust the player's position if they have mesh bounds.
                        // Finally, get the midpoint for drawing text.
                        
                        Vector3 objPos = obj.transform.position;
                        Vector3 farAway = (objPos - t.bounds.center).normalized * (t.bounds.extents.magnitude);
                        t.bounds.IntersectRay(new Ray(farAway, (objPos - farAway).normalized), out float distance);
                        farAway += (objPos - farAway).normalized * distance;
                        Vector3 closest = t.bounds.ClosestPoint(farAway);
                        if (obj.TryGetComponent(out MeshRenderer mr))
                            objPos = mr.bounds.ClosestPoint(closest);
                        Vector3 mid = ((closest - objPos) * 0.5f) + objPos;
                        
                        Handles.DrawLine(objPos, closest);
                        Handles.DrawWireCube(objPos, Vector3.one * 0.25f);
                        Handles.DrawWireCube(closest, Vector3.one * 0.25f);
                        // todo: this danger percent is pretty useful for gameplay; make it not editor-only
                        float percent = Transform1D.SmoothStart2(1 - Mathf.Clamp01((closest - objPos).magnitude / (closest - t.bounds.center).magnitude));
                        Handles.Label(mid, $"Danger: {percent}");
                    }
                }
            }
        }
#endif
    }
}