using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour {

    public enum MaterialType {
        Player,
        PlayerDestroy,
        PlayerMirror,
        Floor,
        MeteorResidue,
    }

    public static MaterialManager Instance;

    private Material _playerMat, _playerDestroyMat, _floorMat, _meteorResidueMat, _playerMirrorMat;

    private void Awake() {
        _playerMat = Resources.Load("Materials/PlayerMat") as Material;
        _floorMat = Resources.Load("Materials/FloorMat") as Material;
        _playerDestroyMat = Resources.Load("Materials/PlayerDestroyMat") as Material;
        _meteorResidueMat = Resources.Load("Materials/MeteorResidueMat") as Material;
        _playerMirrorMat = Resources.Load("Materials/PlayerMirrorMat") as Material;

        Instance = this;
    }

    public static Material GetMaterial(MaterialType materialType) {
        switch (materialType) {
            case MaterialType.Floor: {
                    return Instance._floorMat;
                }
            case MaterialType.Player: {
                    return Instance._playerMat;
                }
            case MaterialType.PlayerDestroy: {
                    return Instance._playerDestroyMat;
                }
            case MaterialType.MeteorResidue: {
                    return Instance._meteorResidueMat;
                }
            case MaterialType.PlayerMirror: {
                    return Instance._playerMirrorMat;
                }
            default: {
                    return null;
                }
        }
    }

    public static Material GetMaterial(PlayerPieceController.PlayerPieceType playerPieceType) {
        switch (playerPieceType) {
            case PlayerPieceController.PlayerPieceType.Destroy: {
                    return Instance._playerDestroyMat;
                }
            case PlayerPieceController.PlayerPieceType.Normal: {
                    return Instance._playerMat;
                }
            case PlayerPieceController.PlayerPieceType.Mirror: {
                    return Instance._playerMirrorMat;
                }
            default: {
                    return null;
                }
        }
    }

    public static Material GetMaterial(PieceController.PieceType pieceType) {
        switch (pieceType) {
            case PieceController.PieceType.Normal: {
                    return Instance._floorMat;
                }
            case PieceController.PieceType.Player: {
                    return Instance._playerMat;
                }
            case PieceController.PieceType.Floor: {
                    return Instance._floorMat;
                }
            case PieceController.PieceType.MeteorResidue: {
                    return Instance._meteorResidueMat;
                }
            default: {
                    return null;
                }
        }
    }
}
