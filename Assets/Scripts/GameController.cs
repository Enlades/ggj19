using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController Instance;

    public GameObject[] PlayerIntroParticles;

    public GridGenerator.Grid ActiveGrid;

    private GridGenerator _gridGenerator;

    private GridGenerator.Grid _testGrid;

    private GridGenerator.Grid[] _levelGrids;

    private PlayerPieceController _playerPiece;

    private bool _playerMoving;

    private void Awake() {
        Instance = this;

        _gridGenerator = GetComponent<GridGenerator>();

        _testGrid = _gridGenerator.GenerateGrid(3, 3);

        ActiveGrid = _testGrid;
    }

    private void Start() {
        _playerPiece = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.PlayerPiece), Vector3.zero, Quaternion.identity)
            .GetComponent<PlayerPieceController>();

        _playerPiece.PlaceOnTop(_testGrid.Pieces[0, 0]);

        _playerPiece.ConvertPlayerPieceType(PlayerPieceController.PlayerPieceType.Destroy);

        CreatePieceOnGrid(new Vector2Int(2, 2));
        CreatePieceOnGrid(new Vector2Int(2, 2));
        //CreatePieceOnGrid(new Vector2Int(3, 2));
        CreatePieceOnGrid(new Vector2Int(1, 2));
        CreatePieceOnGrid(new Vector2Int(2, 1));
        //CreatePieceOnGrid(new Vector2Int(2, 3));
    }

    private void CreatePieceOnGrid(Vector2Int position) {
        PieceController testPiece = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.FloorPiece), Vector3.zero, Quaternion.identity)
            .GetComponent<PieceController>();

        testPiece.PlaceOnTop(_testGrid.Pieces[position.x, position.y]);
    }

    private void Update() {
        int x = (int)Input.GetAxisRaw("Horizontal");
        int z = (int)Input.GetAxisRaw("Vertical");

        if (Input.anyKeyDown) {
            if (_playerMoving)
                return;

            if (Mathf.Abs(x) > 0f || Mathf.Abs(z) > 0f) {
                PieceController nextPiece = _testGrid.ProcessInput(_playerPiece.GridPosition, new Vector2Int(x, z));

                if (nextPiece != null) {
                    StartCoroutine(AnimatePlayerMovement(nextPiece));
                }
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                if(_playerPiece.PPieceType == PlayerPieceController.PlayerPieceType.Destroy) {
                    _playerPiece.DestroyAction();
                }else if(_playerPiece.PPieceType == PlayerPieceController.PlayerPieceType.Normal) {
                    _playerPiece.ConvertPieceType(PieceController.PieceType.Normal);
                }

                _playerPiece = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.PlayerPiece), Vector3.zero, Quaternion.identity)
                    .GetComponent<PlayerPieceController>();

                _playerPiece.PlaceOnTop(_testGrid.Pieces[0, 0]);

                _playerPiece.ConvertPlayerPieceType(PlayerPieceController.PlayerPieceType.Destroy);
            }
        }
    }

    private IEnumerator AnimatePlayerMovement(PieceController pieceController) {
        _playerMoving = true;

        AnimationCurve easeEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        float shrinkTime = 0.09f;
        float expandTime = 0.09f;
        float timer = shrinkTime;

        while (timer > 0f) {

            _playerPiece.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, easeEase.Evaluate((shrinkTime - timer) / shrinkTime));

            timer -= Time.deltaTime;

            yield return null;
        }

        _playerPiece.transform.localScale = Vector3.zero;

        _playerPiece.PlaceOnTop(pieceController);

        timer = expandTime;

        while (timer > 0f) {

            _playerPiece.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, easeEase.Evaluate((expandTime - timer) / expandTime));

            timer -= Time.deltaTime;

            yield return null;
        }

        _playerPiece.transform.localScale = Vector3.one;

        _playerMoving = false;
    }
}
