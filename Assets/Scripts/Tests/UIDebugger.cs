using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UIDebugger : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = mousePos
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log($"--- KLIKNIÊCIE W MIEJSCU {mousePos} ---");

            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    Debug.Log($"Trafiono w UI: {result.gameObject.name} (Tag: {result.gameObject.tag})");
                }
            }
            else
            {
                Debug.Log("UI nie wykry³o ¿adnego obiektu (Raycast przebi³ siê na wylot).");
            }
        }
    }
}