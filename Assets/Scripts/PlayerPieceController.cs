using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPieceController : PieceController {

    public enum PlayerPieceType {
        Normal,
        Destroy,
        Mirror
    }


    [Header("Player Piece Controller Fields")]
    public PlayerPieceType PPieceType;

    public void PlayerIntroAnimationComplete() {

    }

    public void ConvertPlayerPieceType(PlayerPieceType playerPieceType) {
        switch (playerPieceType) {
            case PlayerPieceType.Normal: {
                    PPieceType = playerPieceType;
                    GetComponent<MeshRenderer>().material = MaterialManager.GetMaterial(MaterialManager.MaterialType.Player);
                    break;
                }
            case PlayerPieceType.Destroy: {
                    PPieceType = playerPieceType;
                    GetComponent<MeshRenderer>().material = MaterialManager.GetMaterial(MaterialManager.MaterialType.PlayerDestroy);
                    break;
                }
            case PlayerPieceType.Mirror: {
                    PPieceType = playerPieceType;
                    GetComponent<MeshRenderer>().material = MaterialManager.GetMaterial(MaterialManager.MaterialType.PlayerMirror);
                    break;
                }
            default: {
                    break;
                }
        }
    }

    public override void Obliviate() {
        GetComponent<Collider>().enabled = false;

        for (int i = 0; i < Random.Range(10, 15); i++) {
            GameObject smallPiece = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Rigidbody rb = smallPiece.AddComponent<Rigidbody>();

            smallPiece.transform.position = transform.position + Vector3.forward * Random.Range(-0.5f, 0.5f)
                + Vector3.right * Random.Range(-0.5f, 0.5f)
                + Vector3.up * Random.Range(-0.5f, 0.5f);

            rb.AddExplosionForce(200f, transform.position, 10f);

            if(PType == PieceType.Normal) {
                smallPiece.GetComponent<MeshRenderer>().material = MaterialManager.GetMaterial(PType);
            } else {
                smallPiece.GetComponent<MeshRenderer>().material = MaterialManager.GetMaterial(PPieceType);
            }

            smallPiece.transform.localScale *= transform.localScale.magnitude * Random.Range(0.2f, 0.4f);

            GameController.Instance.StartCoroutine(ShrinkDestroy(smallPiece.transform));

            Destroy(smallPiece, 1.5f);
        }

        Destroy(gameObject);
    }


    private IEnumerator ShrinkDestroy(Transform pieceTransform) {
        yield return new WaitForSeconds(0.9f);

        Vector3 start = pieceTransform.localScale;
        float timer = 0.5f;

        while (timer > 0f) {

            pieceTransform.localScale = Vector3.Lerp(start, Vector3.zero, (0.5f - timer) / 0.5f);

            timer -= Time.deltaTime;

            yield return null;
        }

        pieceTransform.localScale = Vector3.zero;
    }
}
