using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCardUIValueManager : MonoBehaviour
{
    [SerializeField] public TMP_Text _abilityNameText;
    [SerializeField] private TMP_Text _abilityDescriptionText;
    [SerializeField] private Image _circleProgressImage;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] public RectTransform _shakeTarget;
    private bool _isShaking = false;
    private float _timer = 0;
    private float _totalTimeAvailable = 60;
    private bool _hasExpired = false;

    public void SetCardValue(string abilityName, string abilityDescription)
    {
        _abilityNameText.text = abilityName;
        _abilityDescriptionText.text = abilityDescription;
    }

    private void Start()
    {
        _totalTimeAvailable = Random.Range(6, 13) * 5; //always divisble by 5 cuz it looks better than ex. 53s
        _timer = _totalTimeAvailable;

        PlayPopupAnimation();
    }

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            _timer = Mathf.Max(_timer, 0); // Clamp to 0

            // Update circle fill
            _circleProgressImage.fillAmount = 1 - (_timer / _totalTimeAvailable);

            if (_timerText != null)
            {
                // Format as MM:SS
                int minutes = Mathf.FloorToInt(_timer / 60f);
                int seconds = Mathf.FloorToInt(_timer % 60f);
                _timerText.text = $"{minutes:00}:{seconds:00}";
            }

            if (_timer <= 10f && !_isShaking)
            {
                _isShaking = true;
                StartShakeEffect();
            }
        }

        if (_timer <= 0f && !_hasExpired)
        {
            _hasExpired = true;

            _shakeTarget.DOKill(); // Stop all tweens
            _shakeTarget.localRotation = Quaternion.identity; // Reset rotation
            _shakeTarget.anchoredPosition = Vector2.zero; // (optional) Reset position if ever moved

            GameManager.Instance.GenerateNewTriggerWord(_abilityNameText.text);
        }
    }

    public void PlayDisappearAnimation(System.Action onComplete = null)
    {
        _shakeTarget.DOKill(); // Stop any current tweens

        Sequence seq = DOTween.Sequence();
        seq.Append(_shakeTarget.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0.2f).SetEase(Ease.InBack));

        seq.OnComplete(() =>
        {
            onComplete?.Invoke(); // Destroy, disable, or recycle the card
        });
    }

    public void PlayPopupAnimation()
    {
        _shakeTarget.DOKill(); // Ensure clean state
        _shakeTarget.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        _shakeTarget
            .DOScale(Vector3.one, 0.4f)
            .SetEase(Ease.OutBack);
    }

    private void StartShakeEffect()
    {
        _shakeTarget.DOKill(); // Kill any previous tweens

        _shakeTarget.localRotation = Quaternion.identity;

        _shakeTarget
            .DORotate(new Vector3(0, 0, 3f), 0.15f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
