using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public IntVariable currentLevel;
    public void LoadScene(int level)
    {
        currentLevel.Value = level;
        SceneManager.LoadScene("Level"+level);
    }

    public void LoadNextScene()
    {
        currentLevel.Value++;
        if (currentLevel.Value > 3)
        {
            SceneManager.LoadScene("StartScreen");
        }
        else
        {
            SceneManager.LoadScene("Level" + currentLevel.Value);
        }
    }
}
