using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCivController : PlayerPieceController {

    [Header("Player Civ Controller Fields")]
    public AnimationCurve JumpCurve;

    public bool Pause;

    protected override void Awake() {
        base.Awake();

        StartCoroutine(Dance());
    }

    private IEnumerator Dance() {

        yield return new WaitForSeconds(0.6f);

        while (gameObject) {

            yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));

            float baseTimer = Random.Range(0.2f, 0.4f);
            float timer = baseTimer;

            Vector3 startPosition = transform.position;
            Vector3 targetHeight = Vector3.zero;

            int side = (int)Mathf.Pow(-1, Random.Range(0, 2));

            while (timer > 0f) {

                while (Pause) {
                    yield return null;
                }

                transform.Rotate(Vector3.up, 3f * side);

                targetHeight = Vector3.up * JumpCurve.Evaluate((baseTimer - timer) / baseTimer);

                transform.position = startPosition + targetHeight;

                timer -= Time.deltaTime;

                yield return null;
            }

            transform.position = startPosition;
        }
    }
}
