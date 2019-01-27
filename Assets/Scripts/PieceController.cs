using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour {

    public enum PieceType {
        Player,
        Normal,
        Floor,
        MeteorResidue
    }

    public bool HasPiece {
        get {
            return PieceOnTop != null;
        }
    }

    [Header("Piece Controller Fields")]
    public PieceType PType;

    public Vector2Int GridPosition;

    public int Level {
        get {
            int counter = 0;
            PieceController pieceControllerRef = PieceBelow;

            while(pieceControllerRef != null) {
                counter++;
                pieceControllerRef = pieceControllerRef.PieceBelow;
            }

            return counter;
        }
    }

    public PieceAbsolutePosition PAbsolutePosition {
        get {
            return new PieceAbsolutePosition(GridPosition, Level);
        }
    }

    public PieceController PieceBelow, PieceOnTop;

    public bool ControlsInverted;

    private Vector3 _targetScale;

    protected virtual void Awake() {
        _targetScale = transform.localScale;

        transform.localScale = Vector3.zero;

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn() {

        AnimationCurve easeEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        float baseTimer = 0.5f;
        float timer = baseTimer;

        while(timer > 0f) {

            transform.localScale = Vector3.Lerp(Vector3.zero, _targetScale, easeEase.Evaluate((baseTimer - timer) / baseTimer));

            timer -= Time.deltaTime;

            yield return null;
        }

        transform.localScale = _targetScale;
    }

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

    public PieceController DestroyAction() {
        Obliviate();

        if (PieceBelow.PType == PieceType.Floor) {
            PieceBelow.PieceOnTop = null;
            return null;
        }

        PieceBelow.Obliviate();

        return PieceBelow;
    }

    public void ConvertPieceType(PieceType pieceType) {
        switch (pieceType) {
            case PieceType.Normal: {

                    PType = pieceType;
                    GetComponent<MeshRenderer>().material = MaterialManager.GetMaterial(MaterialManager.MaterialType.Floor);

                    if (Level >= 5) {
                        GameController.UnAuthorizedMeteor();
                    }
                    break;
                }
            case PieceType.Player: {
                    PType = pieceType;
                    GetComponent<MeshRenderer>().material = MaterialManager.GetMaterial(MaterialManager.MaterialType.Player);
                    break;
                }
            case PieceType.MeteorResidue: {
                    PType = pieceType;
                    GetComponent<MeshRenderer>().material = MaterialManager.GetMaterial(MaterialManager.MaterialType.MeteorResidue);
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

        if (pieceTransform)
            pieceTransform.localScale = Vector3.zero;
    }

    public class PieceAbsolutePosition {
        public Vector2Int position;
        public int Level;

        public PieceAbsolutePosition(Vector2Int vector2Int, int level) {
            position = vector2Int;
            Level = level;
        }

        public bool Compare(PieceAbsolutePosition pieceAbsolutePosition) {
            return Level == pieceAbsolutePosition.Level
                        && position == pieceAbsolutePosition.position;
        }
    }
}
