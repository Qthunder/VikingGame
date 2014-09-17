namespace Viking.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using Viking.Common;

    public class Board : BasePropertyChanged, IBoard
    {
        private IPiece activePiece;

        private int boardDimension = 9;

        private IPiece king;

        private ObservableCollection<IPiece> pieces;

        private ObservableCollection<ISquare> squares;

        public IPiece ActivePiece
        {
            get
            {
                return this.activePiece;
            }

            set
            {
                if (value != activePiece)
                {
                    activePiece = value;
                    RaisePropertyChanged(() => ActivePiece);
                    if (ActivePiece != null)
                    {
                        SetValidMoves(ActivePiece);
                    }
                    else
                    {
                        ResetValidMoves();
                    }
                }
            }
        }

        public int BoardDimension
        {
            get
            {
                return this.boardDimension;
            }

            set
            {
                if (value != boardDimension)
                {
                    boardDimension = value;
                    RaisePropertyChanged(() => BoardDimension);
                }
            }
        }

        public Dictionary<Point, ISquare> IndexedSquares { get; private set; }

        public IPiece King
        {
            get
            {
                return this.king;
            }

            set
            {
                if (value != king)
                {
                    king = value;
                    RaisePropertyChanged(() => King);
                }
            }
        }

        public ObservableCollection<IPiece> Pieces
        {
            get
            {
                return this.pieces;
            }

            set
            {
                if (value != pieces)
                {
                    pieces = value;
                    RaisePropertyChanged(() => Pieces);
                }
            }
        }

        public ObservableCollection<ISquare> Squares
        {
            get
            {
                return this.squares;
            }

            set
            {
                if (value != squares)
                {
                    squares = value;
                    RaisePropertyChanged(() => Squares);
                    UpdateSquaresIndex();
                }
            }
        }

        public static void CheckCapture(IBoard board, ISquare targetSquare)
        {
            CheckCaptureHorizontal(board, targetSquare, Square.NeighborDirection.Right);
            CheckCaptureHorizontal(board, targetSquare, Square.NeighborDirection.Left);
            CheckCaptureVertical(board, targetSquare, Square.NeighborDirection.Top);
            CheckCaptureVertical(board, targetSquare, Square.NeighborDirection.Bottom);
        }

        public static bool CheckWin(IBoard board, Player currentPlayer)
        {
            bool retVal = false;
            if (currentPlayer == Player.Defender)
            {
                int numAttackers = board.Pieces.Count(delegate(IPiece piece)
                {
                    return piece.Player == Player.Attacker;
                });

                retVal = numAttackers < 3;

                if (!retVal)
                {
                    int nullSides = board.King.Location.Neighbors.Count(delegate(KeyValuePair<Square.NeighborDirection, ISquare> square)
                    {
                        return square.Value == null;
                    });

                    retVal = nullSides == 2;
                }
            }
            else
            {
                int capturedSides = board.King.Location.Neighbors.Count(delegate(KeyValuePair<Square.NeighborDirection, ISquare> square)
                {
                    return square.Value == null || (square.Value.Occupant != null && square.Value.Occupant.Player == Player.Attacker);
                });

                retVal = capturedSides == 4;
            }

            if (!retVal)
            {
                List<IPiece> opposingPieces = ListMoveablePieces(board, currentPlayer == Player.Defender ? Player.Attacker : Player.Defender);
                retVal = !GetValidMovesExist(opposingPieces);
            }

            return retVal;
        }

        public static bool GetValidMovesExist(List<IPiece> piecesToCheck)
        {
            bool retVal = false;
            for (int i = 0; i < piecesToCheck.Count; i++)
            {
                retVal = ListValidMoves(piecesToCheck[i], piecesToCheck[i].Location, Square.NeighborDirection.Left).Count > 0
                        || ListValidMoves(piecesToCheck[i], piecesToCheck[i].Location, Square.NeighborDirection.Top).Count > 0
                        || ListValidMoves(piecesToCheck[i], piecesToCheck[i].Location, Square.NeighborDirection.Right).Count > 0
                        || ListValidMoves(piecesToCheck[i], piecesToCheck[i].Location, Square.NeighborDirection.Bottom).Count > 0;

                if (retVal)
                {
                    break;
                }
            }

            return retVal;
        }

        public static List<IPiece> ListMoveablePieces(IBoard board, Player player)
        {
            List<IPiece> moveablePieces = new List<IPiece>();
            for (int i = 0; i < board.Pieces.Count; i++)
            {
                if (board.Pieces[i].Player == player)
                {
                    moveablePieces.Add(board.Pieces[i]);
                }
            }

            return moveablePieces;
        }

        public static List<Tuple<IPiece, ISquare>> ListValidMoves(IPiece piece, ISquare origin, Square.NeighborDirection direction)
        {
            List<Tuple<IPiece, ISquare>> validMoves = new List<Tuple<IPiece, ISquare>>();
            ISquare nextSquare = origin.Neighbors[direction];
            while (nextSquare != null && !nextSquare.IsOccupied)
            {
                if (piece.IsKing || !nextSquare.IsRestricted)
                {
                    validMoves.Add(new Tuple<IPiece, ISquare>(piece, nextSquare));
                }

                nextSquare = nextSquare.Neighbors[direction];
            }

            return validMoves;
        }

        public static List<Tuple<IPiece, ISquare>> ListValidMoves(List<IPiece> piecesToCheck)
        {
            List<Tuple<IPiece, ISquare>> retVal = new List<Tuple<IPiece, ISquare>>();
            for (int i = 0; i < piecesToCheck.Count; i++)
            {
                retVal.AddRange(ListValidMoves(piecesToCheck[i], piecesToCheck[i].Location, Square.NeighborDirection.Left));
                retVal.AddRange(ListValidMoves(piecesToCheck[i], piecesToCheck[i].Location, Square.NeighborDirection.Top));
                retVal.AddRange(ListValidMoves(piecesToCheck[i], piecesToCheck[i].Location, Square.NeighborDirection.Right));
                retVal.AddRange(ListValidMoves(piecesToCheck[i], piecesToCheck[i].Location, Square.NeighborDirection.Bottom));
            }

            return retVal;
        }

        public static void MakeMove(IBoard board, IPiece piece, ISquare targetSquare)
        {
            piece.Location.Occupant = null;
            piece.Location = targetSquare;
            targetSquare.Occupant = piece;
            CheckCapture(board, targetSquare);
        }

        public void Initialize()
        {
            Squares = null;
            Pieces = null;
            List<Square> newBoard = new List<Square>();
            List<Piece> newPieces = new List<Piece>();
            List<Square> previousRow = new List<Square>();
            List<Square> currentRow = new List<Square>();
            for (int rowIndex = 0; rowIndex < BoardDimension; rowIndex++)
            {
                previousRow = currentRow;
                currentRow = new List<Square>();
                for (int colIndex = 0; colIndex < BoardDimension; colIndex++)
                {
                    Square newSquare = new Square();
                    if (currentRow.Count > 0)
                    {
                        newSquare.Left = currentRow[colIndex - 1];
                        currentRow[colIndex - 1].Right = newSquare;
                    }

                    if (previousRow.Count > 0)
                    {
                        newSquare.Top = previousRow[colIndex];
                        previousRow[colIndex].Bottom = newSquare;
                    }

                    newSquare.Row = rowIndex;
                    newSquare.Col = colIndex;

                    int center = BoardDimension / 2;
                    int third = BoardDimension / 3;
                    if (rowIndex == center && colIndex == center)
                    {
                        newSquare.IsRestricted = true;
                    }

                    if (rowIndex == center || colIndex == center
                        || ((colIndex == 0 || colIndex == BoardDimension - 1) && rowIndex >= third && rowIndex < BoardDimension - third)
                        || ((rowIndex == 0 || rowIndex == BoardDimension - 1) && colIndex >= third && colIndex < BoardDimension - third))
                    {
                        newSquare.IsStartPosition = true;
                    }

                    if (newSquare.IsStartPosition)
                    {
                        Piece newPiece = new Piece();
                        newSquare.Occupant = newPiece;
                        newPiece.Location = newSquare;
                        if (newSquare.IsRestricted)
                        {
                            newPiece.IsKing = true;
                            newPiece.Player = Player.Defender;
                            King = newPiece;
                        }
                        else if ((newSquare.Col == center && (rowIndex >= third - 1 && rowIndex < BoardDimension - third + 1))
                                    || (newSquare.Row == center && (colIndex >= third - 1 && colIndex < BoardDimension - third + 1)))
                        {
                            newPiece.Player = Player.Defender;
                        }
                        else
                        {
                            newPiece.Player = Player.Attacker;
                        }

                        newPieces.Add(newPiece);
                    }

                    currentRow.Add(newSquare);
                    newBoard.Add(newSquare);
                }
            }

            Squares = new ObservableCollection<ISquare>(newBoard);
            Pieces = new ObservableCollection<IPiece>(newPieces);
        }

        public void ResetMoveablePieces()
        {
            for (int i = 0; i < Pieces.Count; i++)
            {
                ((Piece)Pieces[i]).IsMoveable = false;
            }
        }

        public void ResetValidMoves()
        {
            for (int i = 0; i < Squares.Count; i++)
            {
                Squares[i].IsValidMove = false;
            }
        }

        public void SetMoveablePieces(Player player)
        {
            for (int i = 0; i < Pieces.Count; i++)
            {
                if (Pieces[i].Player == player)
                {
                    ((Piece)Pieces[i]).IsMoveable = true;
                }
                else
                {
                    ((Piece)Pieces[i]).IsMoveable = false;
                }
            }
        }

        public void SetValidMoves(IPiece currentPiece)
        {
            ResetValidMoves();
            ResetMoveablePieces();
            ActivePiece = currentPiece;
            (ActivePiece as Piece).IsMoveable = true;
            ISquare indexSquare = currentPiece.Location.Left;
            while (indexSquare != null && !indexSquare.IsOccupied)
            {
                if (currentPiece.IsKing || !indexSquare.IsRestricted)
                {
                    indexSquare.IsValidMove = true;
                }

                indexSquare = indexSquare.Left;
            }

            indexSquare = currentPiece.Location.Top;
            while (indexSquare != null && !indexSquare.IsOccupied)
            {
                if (currentPiece.IsKing || !indexSquare.IsRestricted)
                {
                    indexSquare.IsValidMove = true;
                }

                indexSquare = indexSquare.Top;
            }

            indexSquare = currentPiece.Location.Right;
            while (indexSquare != null && !indexSquare.IsOccupied)
            {
                if (currentPiece.IsKing || !indexSquare.IsRestricted)
                {
                    indexSquare.IsValidMove = true;
                }

                indexSquare = indexSquare.Right; ;
            }

            indexSquare = currentPiece.Location.Bottom;
            while (indexSquare != null && !indexSquare.IsOccupied)
            {
                if (currentPiece.IsKing || !indexSquare.IsRestricted)
                {
                    indexSquare.IsValidMove = true;
                }

                indexSquare = indexSquare.Bottom;
            }
        }

        public void UpdateSquaresIndex()
        {
            IndexedSquares = new Dictionary<Point, ISquare>();
            for (int i = 0; i < Squares.Count; i++)
            {
                IndexedSquares.Add(new Point(Squares[i].Row, Squares[i].Col), Squares[i]);
            }
        }

        private static void CheckCaptureHorizontal(IBoard board, ISquare originSquare, Square.NeighborDirection direction)
        {
            if (direction == Square.NeighborDirection.Top || direction == Square.NeighborDirection.Bottom)
            {
                throw new ArgumentException();
            }

            ISquare targetSquare = originSquare.Neighbors[direction];
            if (targetSquare != null && targetSquare.IsOccupied && targetSquare.Occupant.Player != originSquare.Occupant.Player && !targetSquare.Occupant.IsKing)
            {
                IPiece deadPiece = null;
                ISquare oppositeSquare = targetSquare.Neighbors[direction];
                if (oppositeSquare != null && oppositeSquare.IsOccupied && oppositeSquare.Occupant.Player == originSquare.Occupant.Player)
                {
                    deadPiece = targetSquare.Occupant;
                }
                else if (oppositeSquare == null && (targetSquare.Top == null || targetSquare.Bottom == null))
                {
                    if ((targetSquare.Top == null && targetSquare.Bottom.Occupant != null && targetSquare.Bottom.Occupant.Player == originSquare.Occupant.Player)
                        || (targetSquare.Bottom == null && targetSquare.Top.Occupant != null && targetSquare.Top.Occupant.Player == originSquare.Occupant.Player))
                    {
                        deadPiece = targetSquare.Occupant;
                    }
                }

                if (deadPiece != null)
                {
                    deadPiece.Location = null;
                    targetSquare.Occupant = null;
                    board.Pieces.Remove(deadPiece);
                }
            }
        }

        private static void CheckCaptureVertical(IBoard board, ISquare originSquare, Square.NeighborDirection direction)
        {
            if (direction == Square.NeighborDirection.Left || direction == Square.NeighborDirection.Right)
            {
                throw new ArgumentException();
            }

            ISquare targetSquare = originSquare.Neighbors[direction];
            if (targetSquare != null && targetSquare.IsOccupied && targetSquare.Occupant.Player != originSquare.Occupant.Player && !targetSquare.Occupant.IsKing)
            {
                IPiece deadPiece = null;
                ISquare oppositeSquare = targetSquare.Neighbors[direction];
                if (oppositeSquare != null && oppositeSquare.IsOccupied && oppositeSquare.Occupant.Player == originSquare.Occupant.Player)
                {
                    deadPiece = targetSquare.Occupant;
                }
                else if (oppositeSquare == null && (targetSquare.Right == null || targetSquare.Left == null))
                {
                    if ((targetSquare.Right == null && targetSquare.Left.Occupant != null && targetSquare.Left.Occupant.Player == originSquare.Occupant.Player)
                        || (targetSquare.Left == null && targetSquare.Right.Occupant != null && targetSquare.Right.Occupant.Player == originSquare.Occupant.Player))
                    {
                        deadPiece = targetSquare.Occupant;
                    }
                }

                if (deadPiece != null)
                {
                    deadPiece.Location = null;
                    targetSquare.Occupant = null;
                    board.Pieces.Remove(deadPiece);
                }
            }
        }
    }
}
