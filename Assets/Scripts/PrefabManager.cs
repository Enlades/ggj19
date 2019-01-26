﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour {

    public enum PrefabType {
        FloorPiece,
        PlayerPiece,
        PlayerDestroyPiece
    }

    public static PrefabManager Instance;

    private GameObject _floorPiece, _playerPiece, _playerDestroyPiece;

    private void Awake() {
        Instance = this;

        _floorPiece = Resources.Load("Prefabs/FloorPiece")as GameObject;
        _playerPiece = Resources.Load("Prefabs/PlayerPiece") as GameObject;
        _playerDestroyPiece = Resources.Load("Prefabs/PlayerDestroyPiece") as GameObject;
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
            default: {
                    return null;
                }
        }
    }
}