using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class _DebugText : MonoBehaviour {
    public static _DebugText instance;
    public Text tt;
	// Use this for initialization

        void Awake()
    {
        instance = this;
    }
	void Start () {
        tt = this.GetComponent<Text>();
	}
	
	// Update is called once per frame
	public void De(string content)
    {
        tt.text = content;
    }
}
