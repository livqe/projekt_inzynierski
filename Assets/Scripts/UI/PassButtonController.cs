using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PassButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Configuration")]
    [SerializeField] private float holdTime = 3.0f;

    [Header("Visual")]
    [SerializeField] private Image fillImage;

    private CanvasGroup canvasGroup;

    private bool isHeld = false;
    private float timer = 0f;
    private bool hasPassedRound = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetInteractable(bool isInteractable)
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        if (isInteractable)
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0.5f;
            canvasGroup.blocksRaycasts = false;

            isHeld = false;
            timer = 0f;
            if (fillImage != null) fillImage.fillAmount = 0;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GameController.Instance.isPlayerTurn) return;
        if (GameController.Instance.currentState == GameState.WaitingForTarget) return;

        isHeld = true;
        timer = 0f;
        hasPassedRound = false;
        Debug.Log("Przytrzymywanie przycisku...");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameController.Instance.currentState == GameState.WaitingForTarget) return;
        if (!isHeld) return;

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

                hasPassedRound = true;
                isHeld = false;
                if (fillImage != null) fillImage.fillAmount = 0;

                GameController.Instance.PlayerPassRound();
            }
        }
    }
}
