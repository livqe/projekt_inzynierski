using System.Collections;
using TMPro;
using UnityEngine;

public class CardBoardVisual : MonoBehaviour
{
    public SpriteRenderer artwork;
    public TextMeshPro powerText;
    public TextMeshPro shieldText;

    private CardInstance _myCard;
    private Coroutine currentAnimationRoutine;
    private Vector3 baseScale;
    private Quaternion initialRotation;

    private bool _isDead = false;

    private void Awake()
    {
        if (artwork == null) artwork = GetComponent<SpriteRenderer>();
        initialRotation = transform.localRotation;

        RecalculateBaseScale();
    }

    private void OnDisable()
    {
        ResetTransform();
    }

    public void UpdateVisuals(CardInstance card)
    {
        if (_myCard != null && _myCard != card)
        {
            _myCard.OnPowerChanged -= HandlePowerChange;
            _myCard.OnShieldChanged -= UpdateShieldVisuals;
        }
        
        _myCard = card;
        _isDead = false;

        _myCard.OnPowerChanged -= HandlePowerChange;
        _myCard.OnPowerChanged += HandlePowerChange;

        _myCard.OnShieldChanged -= UpdateShieldVisuals;
        _myCard.OnShieldChanged += UpdateShieldVisuals;

        if (artwork != null)
        {
            artwork.sprite = card.data.artwork;

            RecalculateBaseScale();
        }

        UpdatePowerText();
        UpdateShieldVisuals();

        if (currentAnimationRoutine == null) ResetTransform();
    }

    private void RecalculateBaseScale()
    {
        SpriteScaler scaler = GetComponent<SpriteScaler>();
        if (scaler != null) scaler.AdjustSize();

        baseScale = transform.localScale;

        if (baseScale == Vector3.zero) baseScale = Vector3.one;
    }

    private void UpdatePowerText()
    {
        if (powerText != null && _myCard != null)
        {
            powerText.text = _myCard.currentPower.ToString();

            Color finalColor = Color.black;

            if (_myCard.currentPower > _myCard.basePowerFromEffect) ColorUtility.TryParseHtmlString("#4ACD00", out finalColor);
            else if (_myCard.currentPower < _myCard.basePowerFromEffect) ColorUtility.TryParseHtmlString("#CB0000", out finalColor);
            else ColorUtility.TryParseHtmlString("#333333", out finalColor);

            powerText.color = finalColor;
        }
    }

    private void UpdateShieldVisuals()
    {
        if (shieldText == null) return;

        int currentShield = _myCard.shield;

        if (currentShield > 0)
        {
            shieldText.gameObject.SetActive(true);
            shieldText.text = currentShield.ToString();
        }
        else
        {
            shieldText.gameObject.SetActive(false);
        }
    }

    private void HandlePowerChange(int difference)
    {
        if (!gameObject.activeInHierarchy) return;

        UpdatePowerText();

        bool isDead = _myCard.currentPower < 0 || (_myCard.currentPower == 0 && !_myCard.survivor);
        if (_isDead || isDead)
        {
            if (!_isDead)
            {
                _isDead = true;

                SpawnVFX("damage");
                if (currentAnimationRoutine != null) StopCoroutine(currentAnimationRoutine);
                currentAnimationRoutine = StartCoroutine(AnimateShakeCard());
            }

            return;
        }

        int previousPower = _myCard.currentPower - difference;

        if (previousPower < 0 && difference > 0)
        {
            return;
        }

        if (currentAnimationRoutine != null) StopCoroutine(currentAnimationRoutine);
        ResetTransform();

        if (difference > 0)
        {
            SpawnVFX("buff");
            currentAnimationRoutine = StartCoroutine(AnimateBuffPop());
        }
        else if (difference < 0)
        {
            SpawnVFX("damage");
            currentAnimationRoutine = StartCoroutine(AnimateShakeCard());
        }
    }

    private void ResetTransform()
    {
        transform.localScale = baseScale;
        transform.localRotation = initialRotation;
    }

    private IEnumerator AnimateBuffPop()
    {
        float duration = 0.25f;
        float elapsed = 0f;

        Vector3 targetScale = baseScale * 1.2f;

        while (elapsed < duration / 2)
        {
            float t = elapsed / (duration / 2);
            transform.localScale = Vector3.Lerp(baseScale, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;

        elapsed = 0f;
        while (elapsed < duration / 2)
        {
            float t = elapsed / (duration / 2);
            transform.localScale = Vector3.Lerp(targetScale, baseScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ResetTransform();
    }

    private IEnumerator AnimateShakeCard()
    {
        float duration = 0.35f;
        float elapsed = 0f;
        float angleStrength = 7f;

        while (elapsed < duration)
        {
            float z = Mathf.Sin(elapsed * 50f) * angleStrength;
            transform.localRotation = Quaternion.Euler(0, 0, z);

            angleStrength = Mathf.Lerp(7f, 0f, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        ResetTransform();
    }

    private void SpawnVFX(string type)
    {
        if (VFXManager.Instance == null) return;

        GameObject prefab = (type == "buff") ? VFXManager.Instance.buffPrefab : VFXManager.Instance.damagePrefab;
        AudioClip sound = (type == "buff") ? VFXManager.Instance.buffSound : VFXManager.Instance.damageSound;

        if (prefab != null)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.z = transform.position.z - 2f;

            GameObject vfx = Instantiate(prefab, spawnPos, Quaternion.identity);
            
            if (GameController.Instance.audioSource != null && sound != null)
                GameController.Instance.audioSource.PlayOneShot(sound);

            Destroy(vfx, 2f);
        }
    }

    private void OnDestroy()
    {
        if (_myCard != null)
        {
            _myCard.OnPowerChanged -= HandlePowerChange;
            _myCard.OnShieldChanged -= UpdateShieldVisuals;
        }
    }
}