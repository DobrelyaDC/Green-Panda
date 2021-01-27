using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IWindows
{
    void Init(OfficeData data);
}

public class UIController : OfficeController
{
    private const float _timeToLoadingBonus = 3f;
    private const int _countBonusInGame = 3;

    [System.Serializable]
    public class Bonus
    {
        public Sprite imageBonus;
        public int textBonus;
    }

    [SerializeField] private List<Bonus> bonus;

    #region UI, Windows
    [SerializeField] private TextMeshProUGUI _worldTimeText;
    [SerializeField] private Image _loader;
    [SerializeField] private GameObject _breakfastButton;
    [SerializeField] private GameObject _breakfastParent;

    [SerializeField] private Image _imageBonus;
    [SerializeField] private TextMeshProUGUI _textBonus;
    [SerializeField] private GameObject _bonus;
    #endregion

    private Vector3 _offset = new Vector3(12f, 16f, 0f);
    private bool _isLoading;

    private GameObject _currentButton;

    private int _countActiveBonus;

    private void Awake()
    {
        onBreakfast += BreakfastStarted;
        onReturnToWork += BreakfastEnded;
    }

    private void Update()
    {
        ViewWorldTime();
        SetAmountLoader();
    }
    /// <summary>
    /// typeBonus 0 = coin; typeBonus 1 = energy;
    /// </summary>
    public void ActiveBonus(bool active, int typeBonus, int count)
    {
        _bonus.SetActive(active);
        if (!active)
            return;
        _imageBonus.sprite = bonus[typeBonus].imageBonus;
        _textBonus.text = count.ToString();
        StartCoroutine(OnDisableButtonDelay(_bonus));
    }

    private void ViewWorldTime()
    {
        _worldTimeText.text = FormatTime(_officeData.WorldTime);
    }

    private void SetAmountLoader()
    {
        if (_isLoading)
        {
            _loader.fillAmount += Time.deltaTime / _timeToLoadingBonus;
            if(_loader.fillAmount == 1)
            {
                _isLoading = false;
                _loader.fillAmount = 0;
                _currentButton.SetActive(false);
            }
        }
    }

    public void BreakfastStarted(GameObject employee)
    {
        _breakfastButton.SetActive(true);
        _countActiveBonus = _countBonusInGame;

        for (int i = 0; i > _breakfastParent.transform.childCount; i++)
        {
            _breakfastParent.transform.GetChild(i).gameObject.SetActive(true);
            _breakfastParent.transform.GetChild(i).GetComponent<Button>().interactable = true;
        }
    }

    public void BreakfastEnded(GameObject employee)
    {
        _breakfastButton.SetActive(false);

        _breakfastParent.SetActive(false);
    }

    public void ButtonsForLoader(GameObject button)
    {
        if (!_isLoading)
        {
            _currentButton = button;
            _currentButton.GetComponent<Button>().interactable = false;
            _isLoading = true;
            _loader.transform.position = _currentButton.transform.position + _offset;
        }
    }

    private IEnumerator OnDisableButtonDelay(GameObject bonus)
    {
        yield return new WaitForSeconds(_timeToLoadingBonus);
        bonus.SetActive(false);
    }

    private void OnDisable()
    {
        onBreakfast -= BreakfastStarted;
        onReturnToWork -= BreakfastEnded;
    }
}
