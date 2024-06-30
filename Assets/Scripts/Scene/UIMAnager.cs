using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    private float fill = 0;
    private float _amountOfCapsules;
    [SerializeField] private Image _progressBar;
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private TextMeshProUGUI nextLevel;
    public void Init()
    {
        _amountOfCapsules = CapsuleManager.Instance._capsulesAmount;
        currentLevel.text = SceneManager.GetActiveScene().buildIndex.ToString();
        nextLevel.text = (SceneManager.GetActiveScene().buildIndex+1).ToString();
    }

    private void Update()
    {
        if (CapsuleManager.Instance._capsulesCounter>0)
        {
            fill = CapsuleManager.Instance._capsulesCounter / _amountOfCapsules;
        }
        _progressBar.fillAmount = fill;
    }

    public void StartGame()
    {
        Brusher.AnimationNow = false;
        CapsuleManager.Instance.RecalcTargetCapsules();
        Brusher.isRotate = true;
        CapsuleManager.Instance.isStarted = true;
        _startPanel.SetActive(false);
    }

    public void Reload(){
        _startPanel.SetActive(true);
    }   

}
