using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] public static int score;
    // Start is called before the first frame update
    void Start()
    {
        if ( SceneManager.GetActiveScene().name == "Congratulations") {
            foreach ( Text element in FindObjectsOfType<Text>()) {
                if ( element.CompareTag("Shredder")) {
                    element.text = "Score: " + score;
                }
            }
        }
    }

}
