using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerPrefsManager : MonoBehaviour
{

    public const string MASTER_VOLUME_KEY = "master_volume";
    public const string MASTER_MUTE_KEY = "master_mute";
    public const string DIFFICULTY = "difficulty";
    public const string LEVEL_KEY = "level_unlocked_";

    // Setters
    public static void SetMasterVolume(float volume)
    {
        if (volume >= 0.0f && volume <= 1.0f)
        {
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, volume);
        }
        else
        {
            Debug.LogError(MASTER_VOLUME_KEY + " = " + volume + " out of range");
        }
    }
    public static void SetMasterMute(bool value)
    {
        PlayerPrefs.SetInt(MASTER_VOLUME_KEY, (value)?1:0);
    }

    public static void UnlockLevel(int level)
    {
        if (level <= SceneManager.sceneCountInBuildSettings - 1)
        {
            PlayerPrefs.SetInt(LEVEL_KEY + level.ToString(), level);
        }
        else 
        {
            Debug.LogError(LEVEL_KEY + " = " + level.ToString() + " not within Scene Count in Build Settings");
        }
    }

    public static void SetDifficulty(float difficulty)
    {
        if (difficulty >= 1.0f && difficulty <= 3.0f)
        {
            PlayerPrefs.SetFloat(DIFFICULTY, difficulty);
        }
        else
        {
            Debug.LogError(DIFFICULTY + " must be between 1 and 3");
        }
    }

    // Getters
    public static float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(MASTER_VOLUME_KEY);
    }

    public static bool GetMasterMute()
    {
        return (PlayerPrefs.GetInt(MASTER_MUTE_KEY) == 1)?true:false;
    }

    public static bool IsLevelUnlocked(int level)
    {
        var levelValue = PlayerPrefs.GetInt(LEVEL_KEY + level.ToString());
        var isLevelUnlocked = (levelValue == 1);
        if (level <= SceneManager.sceneCountInBuildSettings - 1)
        {
            return isLevelUnlocked;
        }
        Debug.LogError(levelValue + " not within Scene Count in Build Settings");
        return false;
    }

    public static float GetDifficulty()
    {
        return PlayerPrefs.GetFloat(DIFFICULTY);
    }
}
