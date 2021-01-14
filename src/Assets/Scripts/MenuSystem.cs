using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour
{
    public Text levelText;

    private void Start()
    {
        levelText.text = "1";
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Level");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("GameMenu");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void ChangeLevel(float sliderValue)
    {
        Game.startingLevel = (int)sliderValue;
        levelText.text = sliderValue.ToString();
    }
}
