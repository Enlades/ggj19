using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public static CameraController Instance;

    public Transform CameraLeftmost, CameraRightmost;

    public Transform ManualPivot;

    public Vector3 Pivot {
        get {
            if (GameController.Instance == null || GameController.Instance.ActiveGrid == null)
                return Vector3.zero;

            if (ManualPivot != null)
                return ManualPivot.position;

            return GameController.Instance.ActiveGrid.CenterPosition + Vector3.up * 0.8f;
        }
    }

    private Vector3 _startPosition;

    private void Awake() {
        Instance = this;

        _startPosition = transform.position;
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.P)) {
            ShakeCamera();
        }

        if (Input.GetKey(KeyCode.E)) {
            transform.position = Vector3.Lerp(transform.position, CameraRightmost.position, Time.deltaTime * 6f);
        } else if (Input.GetKey(KeyCode.Q)) {
            transform.position = Vector3.Lerp(transform.position, CameraLeftmost.position, Time.deltaTime * 6f);
        } else {
            transform.position = Vector3.Lerp(transform.position, _startPosition, Time.deltaTime * 6f);
        }

        transform.LookAt(Pivot);
    }

    public static void ShakeCamera() {
        Instance.StartCoroutine(Instance.CameraShake());
    }

    private IEnumerator CameraShake() {
        for(int i = 0; i < 20; i++) {
            float timer = Random.Range(0.04f, 0.06f);
            float timerBase = timer;

            while(timer > 0f) {
                transform.position = Vector3.Lerp(transform.position
                    , transform.position + transform.up * Random.Range(-0.4f, 0.4f) + transform.right * Random.Range(-0.4f, 0.4f), (timerBase - timer) / timerBase);

                timer -= Time.deltaTime;

                yield return null;
            }
        }
    }
}
