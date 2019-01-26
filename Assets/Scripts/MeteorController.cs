using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour {

    private bool _isPaused;
    private bool _jobDone;

    private void Awake() {
        _jobDone = false;

        transform.position = new Vector3(-7.73f, 13.75f, 0.7f);
        transform.eulerAngles = new Vector3(-88f, -712f, -56f);
    }

    public void PauseMeteor() {
        _isPaused = true;

        GetComponent<ParticleSystem>().Pause();
    }

    public void ResumeMeteor() {
        _isPaused = false;

        GetComponent<ParticleSystem>().Play();
    }

    private void Update() {
        if (_jobDone || _isPaused)
            return;

        transform.position = Vector3.MoveTowards(transform.position, GameController.Instance.ActiveGrid.CenterPosition, 15f * Time.deltaTime);

        if(Vector3.Distance(transform.position, GameController.Instance.ActiveGrid.CenterPosition) < 0.1f) {
            Destroy(gameObject);

            for(int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).GetComponent<PieceController>().Obliviate();
            }

            CameraController.ShakeCamera();

            GameController.ClearActiveGrid();

            _jobDone = true;
        }
    }
}
