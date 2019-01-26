using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour {

    public enum MaterialType {
        Player,
        PlayerDestroy,
        Floor
    }

    public static MaterialManager Instance;

    private Material _playerMat, _playerDestroyMat, _floorMat;

    private void Awake() {
        _playerMat = Resources.Load("Materials/PlayerMat") as Material;
        _floorMat = Resources.Load("Materials/FloorMat") as Material;
        _playerDestroyMat = Resources.Load("Materials/PlayerDestroyMat") as Material;

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
            default: {
                    return null;
                }
        }
    }
}
