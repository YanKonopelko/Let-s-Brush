using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class UIMAnager : MonoBehaviour
{
    private float fill = 0;
    private float _amountOfCapsules;
    [SerializeField] private Image _progressBar;
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private TextMeshProUGUI nextLevel;
    private void Start()
    {
        _amountOfCapsules = GetComponent<ScenesManager>()._capsulesAmount;
        currentLevel.text = SceneManager.GetActiveScene().buildIndex.ToString();
        nextLevel.text = (SceneManager.GetActiveScene().buildIndex+1).ToString();
    }

    private void Update()
    {
        if (GetComponent<ScenesManager>()._capsulesCounter>0)
        {
            fill = GetComponent<ScenesManager>()._capsulesCounter / _amountOfCapsules;
        }
        _progressBar.fillAmount = fill;
    }

    public void StartGame()
    {
        Brusher.AnimationNow = false;
        GameObject.Find("Brusher").GetComponent<BrusherStartChanger>().GetDown();
        Destroy(_startPanel);
    }
}
