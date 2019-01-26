using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour {

    public enum PieceType {
        Player,
        Normal,
        Floor
    }

    public bool HasPiece {
        get {
            return PieceOnTop != null;
        }
    }

    [Header("Piece Controller Fields")]
    public PieceType PType;

    public Vector2Int GridPosition;

    public PieceController PieceBelow, PieceOnTop;

    public void PlaceOnTop(PieceController piece) {
        if (PieceBelow != null) {
            PieceBelow.PieceOnTop = null;
        }

        GridPosition = piece.GridPosition;

        while(piece.PieceOnTop != null) {
            piece = piece.PieceOnTop;
        }

        transform.position = piece.transform.position + Vector3.up;

        piece.PieceOnTop = this;
        PieceBelow = piece;
    }

    public void DestroyAction() {
        Obliviate();

        if (PieceBelow.PType == PieceType.Floor) {
            PieceBelow.PieceOnTop = null;
            return;
        }

        PieceBelow.Obliviate();
    }

    public void ConvertPieceType(PieceType pieceType) {
        switch (pieceType) {
            case PieceType.Normal: {
                    PType = pieceType;
                    GetComponent<MeshRenderer>().material = MaterialManager.GetMaterial(MaterialManager.MaterialType.Floor);
                    break;
                }
            case PieceType.Player: {
                    PType = pieceType;
                    GetComponent<MeshRenderer>().material = MaterialManager.GetMaterial(MaterialManager.MaterialType.Player);
                    break;
                }
            default: {
                    break;
                }
        }
    }

    public virtual void Obliviate() {
        GetComponent<Collider>().enabled = false;

        for (int i = 0; i < Random.Range(10, 15); i++) {
            GameObject smallPiece = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Rigidbody rb = smallPiece.AddComponent<Rigidbody>();

            smallPiece.transform.position = transform.position + Vector3.forward * Random.Range(-0.5f, 0.5f)
                + Vector3.right * Random.Range(-0.5f, 0.5f)
                + Vector3.up * Random.Range(-0.5f, 0.5f);

            rb.AddExplosionForce(200f, transform.position, 10f);

            smallPiece.GetComponent<MeshRenderer>().material = MaterialManager.GetMaterial(PType);

            smallPiece.transform.localScale *= Random.Range(0.2f, 0.4f);

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
