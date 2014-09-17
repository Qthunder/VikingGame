namespace Viking.Model
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    public interface IBoard
    {
        int BoardDimension { get; set; }

        Dictionary<Point, ISquare> IndexedSquares { get; }

        IPiece King { get; set; }

        ObservableCollection<IPiece> Pieces { get; set; }

        ObservableCollection<ISquare> Squares { get; set; }

        void Initialize();

        void UpdateSquaresIndex();
    }
}
