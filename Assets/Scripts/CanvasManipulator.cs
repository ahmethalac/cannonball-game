using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManipulator : MonoBehaviour
{
    [SerializeField] Font font;
    [SerializeField] List<Breakable> breakables;
    [SerializeField] List<GameObject> texts;
    [SerializeField] GameObject countdown;
    // Start is called before the first frame update
    void Start()
    {
        breakables = new List<Breakable>();
        texts = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0 ; i < breakables.Count ; i++){
            texts[i].transform.position = breakables[i].transform.position;
            texts[i].GetComponent<Text>().text = breakables[i].GetBreakCount().ToString();
        }
    }

    public void CreateText(Breakable obstacle, bool notVisible) {
        GameObject go = new GameObject(obstacle.name + " Text");
        go.transform.SetParent(this.transform);

        go.AddComponent(typeof(Text));
        go.GetComponent<Text>().text = obstacle.GetBreakCount().ToString();
        go.GetComponent<Text>().font = font;
        go.GetComponent<Text>().fontSize = 50;
        go.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        go.GetComponent<Text>().fontStyle = FontStyle.Bold;

        go.GetComponent<RectTransform>().localScale = Vector3.one;

        go.transform.rotation = obstacle.gameObject.transform.rotation;

        go.AddComponent(typeof(Canvas));
        go.GetComponent<Canvas>().overrideSorting = true;
        go.GetComponent<Canvas>().sortingLayerName = "Text";
        if (notVisible) {
            go.transform.localScale = Vector3.zero;
        }
        breakables.Add(obstacle);
        texts.Add(go);
    }

    public void CreateScore() {
        GameObject go = new GameObject("Score");
        go.transform.SetParent(transform);

        go.AddComponent(typeof(Text));
        go.GetComponent<Text>().text = Score.score.ToString();
        go.GetComponent<Text>().font = font;
        go.GetComponent<Text>().fontSize = 50;
        go.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        go.GetComponent<Text>().fontStyle = FontStyle.Bold;

        go.transform.position = Vector3.zero;

    }

    public void CreateText(string text) {
        GameObject go = new GameObject("Checkpoint");
        go.transform.SetParent(transform);
        go.AddComponent(typeof(Text));
        go.GetComponent<Text>().text = text;
        go.GetComponent<Text>().font = font;
        go.GetComponent<Text>().fontSize = 50;
        go.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        go.GetComponent<Text>().fontStyle = FontStyle.Bold;

        go.GetComponent<RectTransform>().localScale = Vector3.one;
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(500,100);

        go.transform.position = new Vector2(0,5);

        go.AddComponent(typeof(Canvas));
        go.GetComponent<Canvas>().overrideSorting = true;
        go.GetComponent<Canvas>().sortingLayerName = "Text";

        Destroy(go,2f);
    }
    public void CreateCountDown(Vector3 position, int count) {
        GameObject go = new GameObject("Countdown");
        go.transform.SetParent(transform);

        go.AddComponent(typeof(Text));
        go.GetComponent<Text>().text = "Ahmet";
        go.GetComponent<Text>().font = font;
        go.GetComponent<Text>().fontSize = 50;
        go.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        go.GetComponent<Text>().fontStyle = FontStyle.Bold;

        go.GetComponent<RectTransform>().localScale = Vector3.one;
        go.transform.position = position;
        countdown = go;
        texts.Add(go);
        StartCoroutine(CountDown(count));
    }

    private IEnumerator CountDown(int k) {
        while(k > 0 && countdown != null) {
            countdown.GetComponent<Text>().text = k.ToString();
            k--;
            yield return new WaitForSeconds(1f);
        }
        texts.Remove(countdown);
        Destroy(countdown);
        countdown = null;
    }
    public void RemoveObstacle(Breakable obstacle) {
        int index = breakables.IndexOf(obstacle);
        Debug.Log(index);
        breakables.Remove(obstacle);
        Destroy(texts[index]);
        texts.RemoveAt(index);
    }

    public void NewLevel() {
        StopAllCoroutines();
        texts.Remove(countdown);
        Destroy(countdown);
        countdown = null;
    }
}
