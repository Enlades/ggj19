using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneController : MonoBehaviour {

    public GameObject UIText;

    private void Awake() {
        UIText.SetActive(true);
    }
}
