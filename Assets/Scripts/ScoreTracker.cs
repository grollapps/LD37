using UnityEngine;
using System.Collections;

public class ScoreTracker : MonoBehaviour {

    [SerializeField]
    private TextMesh expTextValue;

    [SerializeField]
    private TextMesh cashTextValue;

    private static ScoreTracker instance;

    private float lastUpdateTime = 0;
    private float updateInterval = 0.3f;

    private float cash = 1000;
    private float exp = 0;


    void Awake() {
        instance = this;
    }

    public static ScoreTracker getInstance() {
        if (instance == null) {
            Debug.LogError("No ScoreTracker instance. Make sure this script is on a GameObject in the scene");
        }
        return instance;
    }

	void Start () {
        updateTexts();
	}

    private void updateTexts() {
        expTextValue.text = exp.ToString("F0");
        cashTextValue.text = "$"+cash.ToString("F2");
    }

	void Update () {
        if (Time.time - lastUpdateTime > updateInterval) {
            lastUpdateTime = Time.time;
            updateTexts();
        }
	}

    public void addCashAndExp(float cashVal, float expVal) {
        addCash(cashVal);
        addExp(expVal);
    }

    public void addCash(float value) {
        cash += value;
    }

    public void addExp(float exp) {
        this.exp += exp;
    }

    public float getCash() {
        return cash;
    }

    public float getExp() {
        return exp;
    }
}
