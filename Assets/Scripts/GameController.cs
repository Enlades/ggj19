using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GameObject ThisIsYouText, ThisIsYouArrow, YourHomeText, YourHomeArrow, MeteorText, MeteorArrow, RebuildText;

    public GameObject FirstLevelPlayer;

    public static GameController Instance;

    public GameObject[] PlayerIntroParticles;

    public GridGenerator.Grid ActiveGrid;

    public int GridLevel;

    private GridGenerator _gridGenerator;

    private PlayerPieceController _playerPiece;

    private PlayerPieceController _playerMirrorPiece;

    private MeteorController _meteor;

    private GameObject _currentCiv;

    private int _cubesUsed;

    private bool _playerMoving;
    private bool _gameStarted;
    private bool _firstPlay;

    private void Awake() {
        Instance = this;

        _firstPlay = true;

        _gridGenerator = GetComponent<GridGenerator>();

        _cubesUsed = 0;

        if (GridLevel == 1) {
            
        } else if (GridLevel == 2) {

        } else if (GridLevel == 3) {

        }
    }

    private void Start() {
        //_playerPiece.ConvertPlayerPieceType(PlayerPieceController.PlayerPieceType.Destroy);

        List<PieceController> solutionPieces = new List<PieceController>();

        if (GridLevel == 1) {
            StartCoroutine(FirstLevelStart());
        } else if (GridLevel == 2) {
            _firstPlay = false;

            ActiveGrid = _gridGenerator.GenerateGrid(4, 4);

            solutionPieces.Add(ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(1, 3))));
            solutionPieces.Add(ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(3, 3))));
            solutionPieces.Add(ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(2, 3))));
            solutionPieces.Add(ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(3, 2))));

            ActiveGrid.GSolution = new GridGenerator.GridSolution(solutionPieces);

            PlaceCivsOnPiece(ActiveGrid.Pieces[1, 1]);

            StartCoroutine(MeteorPause());

            _gameStarted = false;
        } else if (GridLevel == 3) {
            _firstPlay = false;

            ActiveGrid = _gridGenerator.GenerateGrid(5, 5);

            solutionPieces.Add(ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(1, 3))));
            solutionPieces.Add(ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(3, 3))));
            solutionPieces.Add(ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(2, 3))));
            solutionPieces.Add(ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(3, 2))));

            ActiveGrid.GSolution = new GridGenerator.GridSolution(solutionPieces);

            PlaceCivsOnPiece(ActiveGrid.Pieces[1, 1]);

            StartCoroutine(MeteorPause());

            _gameStarted = false;
        }
    }

    private PieceController CreatePieceOnGrid(Vector2Int position) {
        PieceController testPiece = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.FloorPiece), Vector3.zero, Quaternion.identity)
            .GetComponent<PieceController>();

        testPiece.PlaceOnTop(ActiveGrid.Pieces[position.x, position.y]);

        return testPiece;
    }

    private void Update() {
        int x = (int)Input.GetAxisRaw("Horizontal");
        int z = (int)Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.G)) {
            if (_meteor == null) {
                _meteor = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.Meteor)).GetComponent<MeteorController>();

                StartCoroutine(MeteorPause());

                _gameStarted = false;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


        if (!_gameStarted)
            return;

        if (Input.anyKeyDown) {
            if (_playerMoving)
                return;

            if (Mathf.Abs(x) > 0f || Mathf.Abs(z) > 0f) {
                if(_playerPiece.PPieceType == PlayerPieceController.PlayerPieceType.Mirror) {
                    PieceController nextPiece = ActiveGrid.ProcessInput(_playerPiece.GridPosition, new Vector2Int(x, z));

                    if (nextPiece != null) {
                        StartCoroutine(AnimatePlayerMovement(_playerPiece, nextPiece));
                    }

                    nextPiece = ActiveGrid.ProcessInput(_playerMirrorPiece.GridPosition, new Vector2Int(x * -1, z * -1));

                    if (nextPiece != null) {
                        StartCoroutine(AnimatePlayerMovement(_playerMirrorPiece, nextPiece));
                    }


                } else {
                    PieceController nextPiece = ActiveGrid.ProcessInput(_playerPiece.GridPosition, new Vector2Int(x, z));

                    if (nextPiece != null) {
                        StartCoroutine(AnimatePlayerMovement(_playerPiece, nextPiece));
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                if(_playerPiece.PPieceType == PlayerPieceController.PlayerPieceType.Destroy) {
                    PieceController destroyedPiece = _playerPiece.DestroyAction();

                    if (destroyedPiece != null) {
                        ActiveGrid.PlayingPieces.Remove(destroyedPiece);
                    }

                }else if(_playerPiece.PPieceType == PlayerPieceController.PlayerPieceType.Normal) {
                    _playerPiece.ConvertPieceType(PieceController.PieceType.Normal);
                    ActiveGrid.AddPlayingPiece(_playerPiece);
                } else if(_playerPiece.PPieceType == PlayerPieceController.PlayerPieceType.Mirror) {
                    _playerPiece.ConvertPieceType(PieceController.PieceType.Normal);
                    ActiveGrid.AddPlayingPiece(_playerPiece);

                    _playerMirrorPiece.ConvertPieceType(PieceController.PieceType.Normal);
                    ActiveGrid.AddPlayingPiece(_playerMirrorPiece);
                }

                _cubesUsed++;

                if (ActiveGrid.CheckComplete()) {
                    PlaceCivsOnPiece(ActiveGrid.Pieces[1, 1]);

                    _gameStarted = false;
                } else {
                    _playerPiece = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.PlayerPiece), Vector3.zero, Quaternion.identity)
    .GetComponent<PlayerPieceController>();

                    if (GridLevel == 3) {
                        if(_cubesUsed == 1) {
                            _playerPiece.ConvertPlayerPieceType(PlayerPieceController.PlayerPieceType.Destroy);
                        }
                    }

                    _playerPiece.PlaceOnTop(ActiveGrid.Pieces[0, 0]);
                }

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
        if (Instance._playerPiece) {
            Instance._playerPiece.Obliviate();
        }

        Instance.ActiveGrid.ClearPlayingPieces();

        Instance.StartCoroutine(Instance.DelayedPlayerStart());

        if (Instance._currentCiv == null)
            return;

        for (int i = 0; i < Instance._currentCiv.transform.childCount; i++) {
            Instance._currentCiv.transform.GetChild(i).GetComponent<PieceController>().Obliviate();
        }

        Destroy(Instance._currentCiv);
    }

    public static void UnAuthorizedMeteor() {
        if (Instance._meteor == null) {
            Instance._meteor = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.Meteor)).GetComponent<MeteorController>();

            Instance.StartCoroutine(Instance.MeteorPause());

            Instance._gameStarted = false;
        }
    }

    private IEnumerator AnimatePlayerMovement(PlayerPieceController playerPieceController, PieceController pieceController) {
        _playerMoving = true;

        AnimationCurve easeEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        float shrinkTime = 0.09f;
        float expandTime = 0.09f;
        float timer = shrinkTime;

        while (timer > 0f) {

            playerPieceController.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, easeEase.Evaluate((shrinkTime - timer) / shrinkTime));

            timer -= Time.deltaTime;

            yield return null;
        }

        playerPieceController.transform.localScale = Vector3.zero;

        playerPieceController.PlaceOnTop(pieceController);

        timer = expandTime;

        while (timer > 0f) {

            playerPieceController.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, easeEase.Evaluate((expandTime - timer) / expandTime));

            timer -= Time.deltaTime;

            yield return null;
        }

        playerPieceController.transform.localScale = Vector3.one;

        _playerMoving = false;
    }

    private IEnumerator DelayedPlayerStart() {
        yield return new WaitForSeconds(1f);

        _playerPiece = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.PlayerPiece), Vector3.zero, Quaternion.identity)
    .GetComponent<PlayerPieceController>();

        if (GridLevel == 2) {
            _playerPiece.ConvertPlayerPieceType(PlayerPieceController.PlayerPieceType.Destroy);
        } else if(GridLevel == 3) {
            _playerPiece.ConvertPlayerPieceType(PlayerPieceController.PlayerPieceType.Mirror);

            _playerMirrorPiece = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.PlayerPiece), Vector3.zero, Quaternion.identity)
.GetComponent<PlayerPieceController>();

            _playerMirrorPiece.ConvertPlayerPieceType(PlayerPieceController.PlayerPieceType.Mirror);

            _playerMirrorPiece.PlaceOnTop(ActiveGrid.Pieces[4, 4]);
        }

        _playerPiece.PlaceOnTop(ActiveGrid.Pieces[0, 0]);
    }

    private IEnumerator MeteorPause() {

        if(GridLevel != 1) {

            yield return new WaitForSeconds(3f);

            _meteor = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.Meteor)).GetComponent<MeteorController>();
        }

        yield return new WaitForSeconds(0.9f);

        GameObject manualPivot = new GameObject("ManualPivot");

        if (_firstPlay) {

            manualPivot.transform.position = CameraController.Instance.Pivot;

            CameraController.Instance.ManualPivot = manualPivot.transform;

            _meteor.PauseMeteor();

            MeteorArrow.SetActive(true);
            MeteorText.SetActive(true);

            for (int i = 0; i < Instance._currentCiv.transform.childCount; i++) {
                Instance._currentCiv.transform.GetChild(i).GetComponent<PlayerCivController>().Pause = true;
            }

            while (Vector3.Distance(manualPivot.transform.position, _meteor.transform.position) > 0.1f) {
                manualPivot.transform.position = Vector3.MoveTowards(manualPivot.transform.position, _meteor.transform.position, 0.5f);

                yield return null;
            }

        }

        yield return new WaitForSeconds(0.5f);

        if (GridLevel == 2) {
            CreatePieceOnGrid(new Vector2Int(2, 2)).ConvertPieceType(PieceController.PieceType.MeteorResidue);
        }

        yield return new WaitForSeconds(1f);

        if (_firstPlay) {
            MeteorArrow.GetComponent<Animator>().SetTrigger("Switch");
            MeteorText.GetComponent<Animator>().SetTrigger("Switch");

            yield return new WaitForSeconds(0.5f);

            MeteorArrow.SetActive(false);
            MeteorText.SetActive(false);

            _meteor.ResumeMeteor();

            Vector3 targetPosition = ActiveGrid.CenterPosition + Vector3.up * 0.8f;

            while (Vector3.Distance(manualPivot.transform.position, targetPosition) > 0.1f) {
                manualPivot.transform.position = Vector3.MoveTowards(manualPivot.transform.position, targetPosition, 0.5f);

                yield return null;
            }

            Destroy(manualPivot);
        }

        if (_firstPlay) {
            RebuildText.SetActive(true);

            yield return new WaitForSeconds(0.9f);
        }

        _gameStarted = true;

        _firstPlay = false;

        yield return new WaitForSeconds(2f);

        if (_firstPlay) {
            RebuildText.GetComponent<Animator>().SetTrigger("Switch");
        } else {
            Destroy(manualPivot);
        }

        yield return new WaitForSeconds(0.5f);

        if (_firstPlay) {
            RebuildText.SetActive(false);
        }
    }

    private IEnumerator FirstLevelStart() {

        yield return new WaitForSeconds(0.5f);

        FirstLevelPlayer.SetActive(true);

        ThisIsYouArrow.SetActive(true);
        ThisIsYouText.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        FirstLevelPlayer.GetComponent<PieceController>().Obliviate();

        ThisIsYouArrow.GetComponent<Animator>().SetTrigger("Switch");
        ThisIsYouText.GetComponent<Animator>().SetTrigger("Switch");

        yield return new WaitForSeconds(0.5f);

        ThisIsYouArrow.SetActive(false);
        ThisIsYouText.SetActive(false);

        yield return new WaitForSeconds(0.8f);

        YourHomeArrow.SetActive(true);
        YourHomeText.SetActive(true);

        List<PieceController> solutionPieces = new List<PieceController>();

        ActiveGrid = _gridGenerator.GenerateGrid(3, 3);

        solutionPieces.Add(ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(2, 2))));
        solutionPieces.Add(ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(2, 2))));
        solutionPieces.Add(ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(1, 2))));
        solutionPieces.Add(ActiveGrid.AddPlayingPiece(CreatePieceOnGrid(new Vector2Int(2, 1))));

        ActiveGrid.GSolution = new GridGenerator.GridSolution(solutionPieces);

        PlaceCivsOnPiece(ActiveGrid.Pieces[1, 1]);

        yield return new WaitForSeconds(1f);

        YourHomeArrow.GetComponent<Animator>().SetTrigger("Switch");
        YourHomeText.GetComponent<Animator>().SetTrigger("Switch");

        yield return new WaitForSeconds(0.5f);

        YourHomeArrow.SetActive(false);
        YourHomeText.SetActive(false);

        _meteor = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.Meteor)).GetComponent<MeteorController>();

        StartCoroutine(MeteorPause());
    }
}
