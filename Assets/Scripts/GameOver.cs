using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] public static int checkpoint = 1;
    public void LoadGame() {
        SceneManager.LoadScene("Game");
    }
}
