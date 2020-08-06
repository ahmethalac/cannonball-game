using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    Image timerBar;
    [SerializeField] float timeLeft;
    [SerializeField] bool initiate;
    [SerializeField] bool done;
    [SerializeField] float maxTime;
    // Start is called before the first frame update
    void Start()
    {
        done = false;
        timerBar = GetComponent<Image>();
        timerBar.fillAmount = 0;
        maxTime = FindObjectOfType<Game>().GetBarSecond();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0 && initiate) {
            timeLeft -= Time.deltaTime;
            timerBar.fillAmount = timeLeft / maxTime;
        }
    }

    public void Initiate() {
        initiate = true;
        timeLeft = maxTime;
        if (!done) {
            timerBar.fillAmount = 1;
            done = true;
        }
    }

    public void Stop() {
        initiate = false;
        timerBar.fillAmount = 0;
        done = false;
    }
}
