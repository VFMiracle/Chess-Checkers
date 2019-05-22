using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChessAndCheckers
{
    public class Piece
    {
        private Image pieceVis;
        public string type { get; set; }
        public char side { get; }

        private static List<Piece> Pieces = new List<Piece>();
        private static List<Vector> initialPoss = new List<Vector>();

        public Piece() { }

        private Piece(Image img, string t, char s)
        {
            pieceVis = img;
            type = t;
            side = s;
        }

        public static void SetPieces(DependencyObject d)
        {
            if (Pieces.Count() == 0)
            {
                foreach (Image i in FindVisualPieces(d))
                {
                    Pieces.Add(new Piece(i, (string)i.Tag, i.Name.ElementAt(0)));
                    initialPoss.Add(new Vector(i.Margin.Left, i.Margin.Top));
                }
            }
        }

        public static Piece FindPieceAtPos(Vector pos)
        {
            Piece temp = null;
            foreach (Piece piece in Pieces)
            {
                if (piece.pieceVis.Margin.Left == pos.X && piece.pieceVis.Margin.Top == pos.Y)
                    if(piece.pieceVis.Visibility == Visibility.Visible)
                        temp = piece;
            }
            return temp;
        }

        public static Vector FindPiecePosition(Piece p)
        {
            Vector ret = new Vector(p.pieceVis.Margin.Left, p.pieceVis.Margin.Top);
            return ret;
        }

        public static void DisablePieces(List<Piece> pieces)
        {
            foreach(Piece piece in pieces)
                piece.pieceVis.Visibility = Visibility.Hidden;
        }

        public static void MovePiece(Vector pos, Piece piece)
        {
            piece.pieceVis.Margin = new Thickness(pos.X, pos.Y, 0, 0);
        }

        public static Vector FindQtdPieces()
        {
            Vector quantities = new Vector();
            char colorId = ' ';
            foreach(Piece piece in Pieces)
            {
                if (piece.pieceVis.Visibility == Visibility.Visible)
                {
                    colorId = Convert.ToChar(piece.pieceVis.Name.ElementAt(0));
                    if (colorId == 'W') quantities.X++;
                    else quantities.Y++;
                }
            }
            return quantities;
        }

        public static void ResetPieces()
        {
            for(int i = 0; i < Pieces.Count(); i++)
            {
                Pieces.ElementAt(i).pieceVis.Visibility = Visibility.Visible;
                Pieces.ElementAt(i).pieceVis.Margin = 
                    new Thickness(initialPoss.ElementAt(i).X, initialPoss.ElementAt(i).Y, 0, 0);
            }
        }

        public static void ChangePieceImage(string newSource, Piece p)
        {
            p.pieceVis.Source = new BitmapImage(new Uri(newSource));
        }

        public static bool IsTherePieceOfType(string type, char side)
        {
            bool ret = false;
            foreach (Piece piece in Pieces) if (piece.type == type && piece.side == side && 
                    piece.pieceVis.Visibility == Visibility.Visible) ret = true;
            return ret;
        }

        public static Piece VerifyKingState(char side)
        {
            Piece ret = null;
            foreach(Piece piece in Pieces)
            {
                if(piece.side == side && piece.type == "King")
                {
                    List<Vector> enemyPoss = new List<Vector>();
                    if (side == 'W') enemyPoss = FindAllMovePossSide('B');
                    else enemyPoss = FindAllMovePossSide('W');
                    foreach (Vector pos in enemyPoss)
                        if (pos == FindPiecePosition(piece))
                            ret = piece;
                }
            }
            return ret;
        }

        public static List<Vector> FindCheckEscapePoss(char defSide, ref List<Piece> crpChsPieces)
        {
            List<Vector> positions = new List<Vector>();
            foreach (Piece piece in Pieces)
            {
                if (piece.side == defSide && piece.pieceVis.Visibility == Visibility.Visible)
                {
                    Vector orgPos = new Vector(piece.pieceVis.Margin.Left, piece.pieceVis.Margin.Top);
                    List<Vector> auxPositions = MovementTypes.FindMovePoss(piece.side, piece.type, orgPos);
                    if (auxPositions != null) {
                        foreach (Vector pos in auxPositions)
                        {
                            Piece disabledPiece = null;
                            disabledPiece = FindPieceAtPos(pos);
                            if (disabledPiece != null) DisablePieces(new List<Piece>() { disabledPiece });
                            MovePiece(pos, piece);
                            if (!GameManager.IsKingInCheck(defSide))
                            {
                                positions.Add(pos);
                                crpChsPieces.Add(piece);
                            }
                            if (disabledPiece != null) disabledPiece.pieceVis.Visibility = Visibility.Visible;
                            MovePiece(orgPos, piece);
                        }
                    }
                }
            }
            if (positions.Count > 0) return positions;
            else return null;
        }

        public static void IsKingDead(char side)
        {
            Piece kingPiece = null;
            foreach (Piece piece in Pieces)
                if (piece.type == "King" && piece.side == side && piece.pieceVis.Visibility == Visibility.Visible)
                    kingPiece = piece;
            if (kingPiece == null) GameManager.ChessGameEnded(side);
        }

        public static List<Vector> ArePositionsInCheck(List<Vector> positions, char defKingSide)
        {
            List<Vector> auxList = new List<Vector>();
            List<Vector> enemyMovePos = new List<Vector>();
            if (defKingSide == 'W')
                enemyMovePos = FindAllMovePossSide('B', true);
            else
                enemyMovePos = FindAllMovePossSide('W', true);
            foreach (Vector pos in positions)
                foreach (Vector enemyPos in enemyMovePos)
                    if (pos == enemyPos) auxList.Add(pos);
            foreach (Vector pos in auxList)
                positions.Remove(pos);
            return positions;
        }

        public static List<Vector> WouldLeaveKingInCheck(List<Vector> positions, Vector initialPiecePos, char defSide)
        {
            List<Vector> removedPos = new List<Vector>();
            Piece testedPiece = FindPieceAtPos(initialPiecePos);
            Piece enemyPiece;
            foreach (Vector pos in positions)
            {
                enemyPiece = FindPieceAtPos(pos);
                if (enemyPiece != null) DisablePieces(new List<Piece> { enemyPiece });
                MovePiece(pos, testedPiece);
                if (GameManager.IsKingInCheck(defSide)) removedPos.Add(pos);
                MovePiece(initialPiecePos, testedPiece);
                if(enemyPiece != null) enemyPiece.pieceVis.Visibility = Visibility.Visible;
            }
            foreach (Vector pos in removedPos) positions.Remove(pos);
            return positions;
        }

        private static List<Vector> FindAllMovePossSide(char side, bool pawnCapturePos = false)
        {
            List<Vector> positions = new List<Vector>();
            foreach (Piece piece in Pieces)
            {
                if (piece.side == side)
                {
                    if (piece.pieceVis.Visibility == Visibility.Visible)
                    {
                        List<Vector> movePositions = new List<Vector>();
                        /*if (piece.type == "Pawn" && pawnCapturePos)
                            movePositions = MovementTypes.FindMovePoss(side, piece.type, new Vector(
                        piece.pieceVis.Margin.Left, piece.pieceVis.Margin.Top), null, false);
                        else if (piece.type == "King")
                            movePositions = MovementTypes.FindMovePoss(side, piece.type, new Vector(
                        piece.pieceVis.Margin.Left, piece.pieceVis.Margin.Top), null, false);
                        else*/
                            movePositions = MovementTypes.FindMovePoss(side, piece.type, new Vector(
                            piece.pieceVis.Margin.Left, piece.pieceVis.Margin.Top), null, false);
                        if (movePositions != null)
                            positions.AddRange(movePositions);
                    }
                }
            }
            if (positions.Count > 0) return positions;
            else return null;
        }

        private static IEnumerable<Image> FindVisualPieces(DependencyObject depObj)
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is Image)
                    {
                        yield return (Image)child;
                    }

                    foreach (Image childOfChild in FindVisualPieces(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
