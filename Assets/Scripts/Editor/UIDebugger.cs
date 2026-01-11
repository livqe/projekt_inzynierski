using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UIDebugger : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current == null) return; 

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = mousePos
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log($"--- Mysz: {pointerData.position} ---");

            if (results.Count == 0)
            {
                Debug.LogWarning("Brak celu UI");
            }
            else
            {
                foreach (var result in results)
                {
                    Debug.Log($"Trafiono w UI: {result.gameObject.name} | Depth: {result.depth} | Layer: {LayerMask.LayerToName(result.gameObject.layer)}");
                }
            }
        }
    }
}