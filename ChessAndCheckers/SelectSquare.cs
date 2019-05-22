using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace ChessAndCheckers
{
    public class SelectSquare
    {
        private Rectangle playerVis;
        private readonly char side;

        private static bool obligatoryMove = false;
        private static int prmtdPossIndex = 0;
        private static Piece ctrldPiece = null;
        private static List<Vector> permitedPoss = null;
        private static SelectSquare actvPlayer;
        private static List<SelectSquare> players = new List<SelectSquare>();
        private static List<Piece> gonePieces = new List<Piece>();
        private static List<Piece> correspChessPieces = new List<Piece>();

        private SelectSquare(Rectangle r, char c)
        {
            playerVis = r;
            side = c;
        }

        public static void SetPlayers(DependencyObject d)
        {
            if (players.Count() == 0)
            {
                int i = 0;
                foreach (Rectangle rect in FindVisPlayers(d))
                {
                    char c = ' ';
                    if (i % 2 == 0)
                    {
                        c = 'W';
                        i++;
                    }
                    else c = 'B';
                    players.Add(new SelectSquare(rect, c));
                }
                actvPlayer = players.ElementAt(0);
            }
        }

        public static void Movement(KeyEventArgs k)
        {
            if (actvPlayer != null)
            {
                if (permitedPoss == null)
                {
                    switch (k.Key)
                    {
                        case Key.W:
                            IncrementMargin(actvPlayer.playerVis, 0, -75);
                            if (actvPlayer.playerVis.Margin.Top < 30) actvPlayer.playerVis.Margin =
                                     new Thickness(actvPlayer.playerVis.Margin.Left, 555, 0, 0);
                            GameManager.ClearLabels();
                            break;
                        case Key.A:
                            IncrementMargin(actvPlayer.playerVis, -75, 0);
                            if (actvPlayer.playerVis.Margin.Left < 188) actvPlayer.playerVis.Margin =
                                     new Thickness(713, actvPlayer.playerVis.Margin.Top, 0, 0);
                            GameManager.ClearLabels();
                            break;
                        case Key.S:
                            IncrementMargin(actvPlayer.playerVis, 0, 75);
                            if (actvPlayer.playerVis.Margin.Top > 555) actvPlayer.playerVis.Margin =
                                     new Thickness(actvPlayer.playerVis.Margin.Left, 30, 0, 0);
                            GameManager.ClearLabels();
                            break;
                        case Key.D:
                            IncrementMargin(actvPlayer.playerVis, 75, 0);
                            if (actvPlayer.playerVis.Margin.Left > 713) actvPlayer.playerVis.Margin =
                                     new Thickness(188, actvPlayer.playerVis.Margin.Top, 0, 0);
                            GameManager.ClearLabels();
                            break;
                    }
                }
                else
                {
                    if (actvPlayer != null)
                    {
                        switch (k.Key)
                        {
                            case Key.D:
                                prmtdPossIndex--;
                                GameManager.ClearLabels();
                                break;
                            case Key.A:
                                prmtdPossIndex++;
                                GameManager.ClearLabels();
                                break;
                        }
                    }
                    if (prmtdPossIndex >= permitedPoss.Count) prmtdPossIndex = 0;
                    else if (prmtdPossIndex < 0) prmtdPossIndex = permitedPoss.Count - 1;

                    actvPlayer.playerVis.Margin = new Thickness(permitedPoss.ElementAt(prmtdPossIndex).X,
                        permitedPoss.ElementAt(prmtdPossIndex).Y, 0, 0);

                    if (correspChessPieces.Count > 0)
                    {
                        Table.ResetCalloutSection();
                        Table.CalloutSection(Piece.FindPiecePosition(correspChessPieces.ElementAt(prmtdPossIndex)));
                        ctrldPiece = correspChessPieces.ElementAt(prmtdPossIndex);
                    }
                }
            }
        }

        public static void SelectSection()
        {
            if (ctrldPiece == null)
            {
                ctrldPiece = Piece.FindPieceAtPos(
                    new Vector(actvPlayer.playerVis.Margin.Left, actvPlayer.playerVis.Margin.Top));
                if (ctrldPiece != null && actvPlayer.side == ctrldPiece.side)
                {
                    permitedPoss = MovementTypes.FindMovePoss(ctrldPiece.side, ctrldPiece.type,
                        new Vector(actvPlayer.playerVis.Margin.Left, actvPlayer.playerVis.Margin.Top));
                }
                if (permitedPoss != null)
                {
                    actvPlayer.playerVis.Margin = new Thickness(permitedPoss[0].X, permitedPoss[0].Y, 0, 0);
                    Table.HighLightSections(permitedPoss);
                }
                else DropPiece();
            }
            else MoveControlledPiece();
        }

        public static void DropPiece()
        {
            if (!obligatoryMove)
            {
                ctrldPiece = null;
                permitedPoss = null;
                correspChessPieces = new List<Piece>();
                Table.ResetTable();
                Table.ResetCalloutSection();
                Table.ResetWarningSection();
                prmtdPossIndex = 0;
            }
        }

        public static void ChangeActvPlayer()
        {
            if (actvPlayer.playerVis.Name == players.ElementAt(0).playerVis.Name)
                actvPlayer = players.ElementAt(1);
            else actvPlayer = players.ElementAt(0);
        }

        public static void DeactivateAllPlayers()
        {
            actvPlayer = null;
        }

        public static void ResetPlayers()
        {
            actvPlayer = players.ElementAt(0);
            DropPiece();
        }

        private static void MoveControlledPiece()
        {
            bool atePiece = false;

            if (ctrldPiece.type == "Checker" &&
                Math.Abs(actvPlayer.playerVis.Margin.Left - Piece.FindPiecePosition(ctrldPiece).X) > 75)
            {
                Vector piecePos = Piece.FindPiecePosition(ctrldPiece);
                Vector temp = new Vector((actvPlayer.playerVis.Margin.Left - piecePos.X) / 2,
                    (actvPlayer.playerVis.Margin.Top - piecePos.Y) / 2);
                Vector vec = new Vector(temp.X + piecePos.X, temp.Y + piecePos.Y);
                gonePieces.Add(Piece.FindPieceAtPos(vec));
                atePiece = true;
            }
            else if (ctrldPiece.type == "Dama")
            {
                Vector direction = new Vector(actvPlayer.playerVis.Margin.Left - Piece.FindPiecePosition(ctrldPiece).X,
                    actvPlayer.playerVis.Margin.Top - Piece.FindPiecePosition(ctrldPiece).Y);
                int x = 0;
                int y = 0;
                if (direction.X > 0) x = 75;
                else x = -75;
                if (direction.Y > 0) y = 75;
                else y = -75;
                Vector posModifier = new Vector(x, y);
                bool quitLoop = false;

                while (!quitLoop)
                {
                    Vector searchedPos = Piece.FindPiecePosition(ctrldPiece) + posModifier;
                    if (Piece.FindPieceAtPos(searchedPos) != null)
                    {
                        if (gonePieces.Count > 0)
                        {
                            bool pieceCounted = false;
                            Piece searchedPiece = Piece.FindPieceAtPos(searchedPos);
                            foreach (Piece p in gonePieces)
                                if (p == searchedPiece) pieceCounted = true;
                            if (!pieceCounted)
                            {
                                gonePieces.Add(Piece.FindPieceAtPos(searchedPos));
                                atePiece = true;
                                quitLoop = true;
                            }
                        }
                        else
                        {
                            gonePieces.Add(Piece.FindPieceAtPos(searchedPos));
                            atePiece = true;
                            quitLoop = true;
                        }
                    }
                    else if (searchedPos == new Vector(actvPlayer.playerVis.Margin.Left,
                        actvPlayer.playerVis.Margin.Top)) quitLoop = true;
                    else if (searchedPos.X > 713 || searchedPos.X < 188) quitLoop = true;

                    posModifier += new Vector(x, y);
                }
            }
            else
                if (Piece.FindPieceAtPos(new Vector(actvPlayer.playerVis.Margin.Left,
                    actvPlayer.playerVis.Margin.Top)) != null)
                    gonePieces.Add(Piece.FindPieceAtPos(new Vector(actvPlayer.playerVis.Margin.Left, 
                        actvPlayer.playerVis.Margin.Top)));

            Piece.MovePiece(new Vector(actvPlayer.playerVis.Margin.Left, actvPlayer.playerVis.Margin.Top), ctrldPiece);

            if (atePiece)
            {
                List<Vector> gonePiecesPos = new List<Vector>();
                foreach (Piece p in gonePieces) gonePiecesPos.Add(Piece.FindPiecePosition(p));
                permitedPoss = MovementTypes.FindMovePoss(ctrldPiece.side, ctrldPiece.type,
                    new Vector(actvPlayer.playerVis.Margin.Left, actvPlayer.playerVis.Margin.Top), gonePiecesPos);
            }
            else permitedPoss = null;

            if (permitedPoss == null)
            {
                bool kingIsChecked = false;
                Piece.DisablePieces(gonePieces);
                if (ctrldPiece.type == "Checker") GameManager.TurnToDama(ctrldPiece);
                else if(ctrldPiece.type != "Dama")
                {
                    if (ctrldPiece.type == "Pawn") GameManager.PawnInPlace(ctrldPiece);
                    if (actvPlayer.side == 'W') kingIsChecked = GameManager.IsKingInCheck('B');
                    else kingIsChecked = GameManager.IsKingInCheck('W');
                }
                obligatoryMove = false;
                DropPiece();
                gonePieces = new List<Piece>();
                if(actvPlayer != null) GameManager.EndTurn(actvPlayer.side);
                if (kingIsChecked)
                {
                    obligatoryMove = true;
                    permitedPoss = GameManager.FreeKingFromCheck(actvPlayer.side, ref correspChessPieces);
                    if (permitedPoss != null)
                    {
                        actvPlayer.playerVis.Margin = new Thickness(permitedPoss.ElementAt(0).X,
                            permitedPoss.ElementAt(0).Y, 0, 0);
                        ctrldPiece = correspChessPieces.ElementAt(0);
                        Table.CalloutSection(Piece.FindPiecePosition(correspChessPieces.ElementAt(0)));
                        Table.HighLightSections(permitedPoss);
                    }
                }
            }
            else
            {
                Table.ResetTable();
                actvPlayer.playerVis.Margin = new Thickness(permitedPoss[0].X, permitedPoss[0].Y, 0, 0);
                obligatoryMove = true;
                Table.HighLightSections(permitedPoss);
            }
            prmtdPossIndex = 0;
        }

        private static IEnumerable<Rectangle> FindVisPlayers(DependencyObject depObj)
        {
            if(depObj != null)
            {
                for(int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if(child != null && child is Rectangle)
                    {
                        yield return (Rectangle)child;
                    }

                    foreach(Rectangle childOfChild in FindVisPlayers(child))
                    {
                        if ((string)childOfChild.Tag == "SlctSqr")
                            yield return childOfChild;
                    }
                }
            }
        }

        private static void IncrementMargin(Rectangle rect, int left, int top)
        {
            rect.Margin = new Thickness(rect.Margin.Left + left, rect.Margin.Top + top, 0, 0);
        }
    }
}
