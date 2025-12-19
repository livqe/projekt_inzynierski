using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursor; // kursor.png
    [SerializeField] private Texture2D clickCursor;   // kursor2.png
    [SerializeField] private Vector2 hotspot = Vector2.zero;

    private bool isClicking = false;

    private void Start()
    {
        // ustaw domyślny kursor
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }

    private void Update()
    {
        // przytrzymanie lewego przycisku myszy
        if (Input.GetMouseButton(0))
        {
            if (!isClicking) // zmiana tylko raz na początek przytrzymania
            {
                Cursor.SetCursor(clickCursor, hotspot, CursorMode.Auto);
                isClicking = true;
            }
        }
        else
        {
            if (isClicking) // wraca do domyślnego kursora po puszczeniu
            {
                Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
                isClicking = false;
            }
        }
    }
}
