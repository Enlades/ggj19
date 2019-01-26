using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour {

    public enum PrefabType {
        FloorPiece,
        PlayerPiece,
        PlayerDestroyPiece,
        PlayerMirrorPiece,
        Meteor,
        PlayerCivs_Three,
        MeteorResidue,
    }

    public static PrefabManager Instance;

    private GameObject _floorPiece, _playerPiece, _playerDestroyPiece, _meteor, _playerCivsThree, _meteorResidue, _playerMirrorPiece;

    private void Awake() {
        Instance = this;

        _floorPiece = Resources.Load("Prefabs/FloorPiece")as GameObject;
        _playerPiece = Resources.Load("Prefabs/PlayerPiece") as GameObject;
        _playerDestroyPiece = Resources.Load("Prefabs/PlayerDestroyPiece") as GameObject;
        _meteor = Resources.Load("Prefabs/Meteor") as GameObject;
        _playerCivsThree = Resources.Load("Prefabs/PlayerCiv_Three") as GameObject;
        _meteorResidue = Resources.Load("Prefabs/MeteorResidue") as GameObject;
        _playerMirrorPiece = Resources.Load("Prefabs/PlayerMirrorPiece") as GameObject;
    }

    public static GameObject GetPrefab(PrefabType prefabType) {
        switch (prefabType) {
            case PrefabType.FloorPiece: {
                    return Instance._floorPiece;
                }
            case PrefabType.PlayerPiece: {
                    return Instance._playerPiece;
                }
            case PrefabType.PlayerDestroyPiece: {
                    return Instance._playerDestroyPiece;
                }
            case PrefabType.Meteor: {
                    return Instance._meteor;
                }
            case PrefabType.PlayerCivs_Three: {
                    return Instance._playerCivsThree;
                }
            case PrefabType.MeteorResidue: {
                    return Instance._meteorResidue;
                }
            case PrefabType.PlayerMirrorPiece: {
                    return Instance._playerMirrorPiece;
                }
            default: {
                    return null;
                }
        }
    }
}
