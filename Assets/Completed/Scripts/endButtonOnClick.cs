using UnityEngine;
using System.Collections;
using Completed;

public class endButtonOnClick : MonoBehaviour {

    public GameManager gm;

	// Use this for initialization
	void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();
	}
	
    public void endturn()
    {
        gm.endTurn();
    }

	// Update is called once per frame
	void Update () {
	
	}
}
