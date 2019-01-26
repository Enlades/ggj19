using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour {

    private Vector3 _pieceOffset = new Vector3(1.2f, 0f, 1.2f);

    public Grid GenerateGrid(int width, int height) {
        PieceController[,] pieces = new PieceController[width, height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                pieces[i, j] = Instantiate(PrefabManager.GetPrefab(PrefabManager.PrefabType.FloorPiece)
                    , Vector3.right * (i * _pieceOffset.x) + Vector3.forward * (j * _pieceOffset.z)
                    , Quaternion.identity).GetComponent<PieceController>();
            }
        }

        return new Grid(pieces);
    }

    public class Grid {
        public int Width {
            get {
                return Pieces.GetLength(0);
            }
        }
        public int Height {
            get {
                return Pieces.GetLength(1);
            }
        }

        public GameObject GridParent;

        public Transform transform {
            get {
                return GridParent.transform;
            }
        }

        public Vector3 CenterPosition {
            get {
                Vector3 result = Vector3.zero;

                for (int i = 0; i < Width; i++) {
                    for (int j = 0; j < Height; j++) {
                        result += Pieces[i, j].transform.position;
                    }
                }

                result /= Width * Height;

                return result;
            }
        }

        public PieceController[,] Pieces;

        public List<PieceController> PlayingPieces;

        public GridSolution GSolution;

        public Grid(PieceController[,] pieces) {
            Pieces = pieces;

            GridParent = new GameObject("GridParent");
            GridParent.transform.position = (Pieces[0, 0].transform.position + Pieces[Width - 1, Height - 1].transform.position) / 2f;

            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    Pieces[i, j].transform.SetParent(GridParent.transform);

                    Pieces[i, j].GridPosition = new Vector2Int(i, j);

                    Pieces[i, j].PType = PieceController.PieceType.Floor;
                }
            }

            PlayingPieces = new List<PieceController>();
        }

        public PieceController ProcessInput(Vector2Int position, Vector2Int direction) {
            if(position.x + direction.x >= Width || position.x + direction.x < 0 
                || position.y + direction.y >= Height || position.y + direction.y < 0) {
                return null;
            }

            return Pieces[position.x + direction.x, position.y + direction.y];
        }

        public void ClearPlayingPieces() {
            for(int i = 0; i < PlayingPieces.Count; i++) {
                PlayingPieces[i].Obliviate();
            }

            PlayingPieces.Clear();
        }

        public PieceController AddPlayingPiece(PieceController pieceController) {
            PlayingPieces.Add(pieceController);

            return pieceController;
        }

        public bool CheckComplete() {
            return GridSolution.CheckSolution(this, GSolution);
        }
    }

    public class GridSolution {

        private List<PieceController.PieceAbsolutePosition> _pieceAbsolutePositions;

        public GridSolution(List<PieceController> playingPieces) {
            _pieceAbsolutePositions = new List<PieceController.PieceAbsolutePosition>();
            
            for(int i = 0; i < playingPieces.Count; i++) {
                _pieceAbsolutePositions.Add(playingPieces[i].PAbsolutePosition);
            }
        }

        public static bool CheckSolution(Grid grid, GridSolution gridSolution) {
            if (grid.PlayingPieces.Count != gridSolution._pieceAbsolutePositions.Count) {
                return false;
            }

            for(int i = 0; i < grid.PlayingPieces.Count; i++) {
                bool contains = false;
                for(int j = 0; j < gridSolution._pieceAbsolutePositions.Count; j++) {
                    if (gridSolution._pieceAbsolutePositions[j].Compare(grid.PlayingPieces[i].PAbsolutePosition)) {
                        contains = true;
                    }
                }

                if (!contains)
                    return false;
            }

            return true;
        }
    }
}
