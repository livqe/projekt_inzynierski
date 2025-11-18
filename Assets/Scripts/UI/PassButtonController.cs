using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Configuration")]
    [SerializeField] private float holdTime = 3.0f;

    [Header("Visual (Optional)")]
    [SerializeField] private Image fillImage;

    private bool isHeld = false;
    private float timer = 0f;
    private bool hasPassedRound = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;
        timer = 0f;
        hasPassedRound = false;
        Debug.Log("Przytrzymywanie przycisku...");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHeld = false;

        if (fillImage != null) fillImage.fillAmount = 0;

        if (!hasPassedRound)
        {
            Debug.Log("Krótkie klikniêcie: Koniec tury.");
            GameController.Instance.EndPlayerTurn();
        }
    }

    private void Update()
    {
        if (isHeld && !hasPassedRound)
        {
            timer += Time.deltaTime;

            if (fillImage != null)
                fillImage.fillAmount = timer / holdTime;

            if (timer >= holdTime)
            {
                Debug.Log("Przytrzymano 3s: Pas rundy");
                GameController.Instance.PlayerPassRound();
                hasPassedRound = true;
                isHeld = false;
            }
        }
    }
}
