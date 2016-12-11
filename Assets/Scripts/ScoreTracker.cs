using UnityEngine;
using System.Collections;

public class ScoreTracker : MonoBehaviour {

    private static ScoreTracker instance;

    private float cash = 0;
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
	
	}
	
	void Update () {
	
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
