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

    private MeteorController _meteor;

    private GameObject _currentCiv;

    private bool _playerMoving;
    private bool _gameStarted;

    private void Awake() {
        Instance = this;

        _gridGenerator = GetComponent<GridGenerator>();

        _testGrid = _gridGenerator.GenerateGrid(3, 3);

        ActiveGrid = _testGrid;
    }

    private void Start() {
        //_playerPiece.ConvertPlayerPieceType(PlayerPieceController.PlayerPieceType.Destroy);

        ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(2, 2)));
        ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(2, 2)));
        ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(1, 2)));
        ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(2, 1)));

        PlaceCivsOnPiece(ActiveGrid.Pieces[1,1]);
    }

    private PieceController CreatePieceOnGrid(Vector2Int position) {
        PieceController testPiece = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.FloorPiece), Vector3.zero, Quaternion.identity)
            .GetComponent<PieceController>();

        testPiece.PlaceOnTop(_testGrid.Pieces[position.x, position.y]);

        return testPiece;
    }

    private void Update() {
        int x = (int)Input.GetAxisRaw("Horizontal");
        int z = (int)Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.G)) {
            if(_meteor == null) {
                _meteor = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.Meteor)).GetComponent<MeteorController>();

                StartCoroutine(MeteorPause());
            }
            return;
        }

        if (!_gameStarted)
            return;

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

                //_playerPiece.ConvertPlayerPieceType(PlayerPieceController.PlayerPieceType.Destroy);
            }
        }
    }

    private void PlaceCivsOnPiece(PieceController pieceController) {
        _currentCiv = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.PlayerCivs_Three));

        _currentCiv.transform.position = pieceController.transform.position + _currentCiv.transform.GetChild(0).localScale.x * Vector3.up;

        _currentCiv.transform.position += Vector3.up * 0.9f + Vector3.back * 0.6f + Vector3.left * 0.2f;
    }

    public static void ClearActiveGrid() {
        Instance.ActiveGrid.ClearPlayingPieces();

        Instance.StartCoroutine(Instance.DelayedPlayerStart());

        for (int i = 0; i < Instance._currentCiv.transform.childCount; i++) {
            Instance._currentCiv.transform.GetChild(i).GetComponent<PieceController>().Obliviate();
        }

        Destroy(Instance._currentCiv);
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

    private IEnumerator DelayedPlayerStart() {
        yield return new WaitForSeconds(1f);

        _playerPiece = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.PlayerPiece), Vector3.zero, Quaternion.identity)
    .GetComponent<PlayerPieceController>();

        _playerPiece.PlaceOnTop(ActiveGrid.Pieces[0, 0]);
    }

    private IEnumerator MeteorPause() {
        yield return new WaitForSeconds(0.9f);

        GameObject manualPivot = new GameObject("ManualPivot");

        manualPivot.transform.position = CameraController.Instance.Pivot;

        CameraController.Instance.ManualPivot = manualPivot.transform;

        _meteor.PauseMeteor();

        for (int i = 0; i < Instance._currentCiv.transform.childCount; i++) {
            Instance._currentCiv.transform.GetChild(i).GetComponent<PlayerCivController>().Pause = true;
        }

        while (Vector3.Distance(manualPivot.transform.position, _meteor.transform.position) > 0.1f) {
            manualPivot.transform.position = Vector3.MoveTowards(manualPivot.transform.position, _meteor.transform.position, 0.5f);

            yield return null;
        }

        yield return new WaitForSeconds(2f);

        _meteor.ResumeMeteor();

        Vector3 targetPosition = ActiveGrid.CenterPosition + Vector3.up * 0.8f;

        while (Vector3.Distance(manualPivot.transform.position, targetPosition) > 0.1f) {
            manualPivot.transform.position = Vector3.MoveTowards(manualPivot.transform.position, targetPosition, 0.5f);

            yield return null;
        }

        Destroy(manualPivot);
    }
}
