using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using System.Windows.Input;

namespace ChessAndCheckers
{
    public class GameManager
    {
        private static IEnumerable<TextBlock> textBlocks;
        private static Piece pawnInPlace = null;

        private static bool gameEnded = false;

        public static bool GameEnded
        {
            get { return gameEnded; }
        }

        public static void SetTextBlocks(DependencyObject d)
        {
            textBlocks = FindLabels(d);
        }

        public static void WriteMessage(string textBlockName, string message)
        {
            foreach (TextBlock textBlock in textBlocks)
                if (textBlock.Name == textBlockName) textBlock.Text = message;
        }

        public static void ClearLabels()
        {
            foreach (TextBlock txt in textBlocks)
                if(txt.Text != "Restart Game" && txt.Text != "Go to Main Menu" && txt.Text != "Quit Game")
                    txt.Text = "";
        }

        public static void EndTurn(char s)
        {
            SelectSquare.ChangeActvPlayer();
            if (!HasCheckersGameEnded())
            {
                if (s == 'W') WriteMessage("warningText", "It's player 2 turn.");
                else WriteMessage("warningText", "It's player 1 turn.");
            }
            else SelectSquare.DeactivateAllPlayers();
        }

        public static void ResetGame()
        {
            Piece.ResetPieces();
            SelectSquare.ResetPlayers();
            gameEnded = false;
        }

        public static void TurnToDama(Piece ctrldPiece)
        {
            if (ctrldPiece.side == 'W' && Piece.FindPiecePosition(ctrldPiece).Y == 30)
                ctrldPiece.type = "Dama";
            else if (Piece.FindPiecePosition(ctrldPiece).Y == 555)
                ctrldPiece.type = "Dama";
        }

        public static void PawnInPlace(Piece ctrldPiece)
        {
            if ((ctrldPiece.side == 'W' && Piece.FindPiecePosition(ctrldPiece).Y == 30) ||
                Piece.FindPiecePosition(ctrldPiece).Y == 555)
            {
                string text = "R- Rook \n K- Knight \n B- Bishop \n Q- Queen";
                WriteMessage("topText", text);
                pawnInPlace = ctrldPiece;
            }
        }

        public static bool ChangePawn(Key k)
        {
            if(pawnInPlace != null)
            {
                bool pieceChanged = false;
                switch (k)
                {
                    case Key.R:
                        pawnInPlace.type = "Rook";
                        if(pawnInPlace.side == 'B')
                            Piece.ChangePieceImage("C:\\Users\\Casa\\Source\\ImageAssets\\Torre_Negra.png", 
                                pawnInPlace);
                        else
                            Piece.ChangePieceImage("C:\\Users\\Casa\\Source\\ImageAssets\\Torre_Branca.png", 
                                pawnInPlace);
                        pieceChanged = true;
                        break;
                    case Key.B:
                        pawnInPlace.type = "Bishop";
                        if (pawnInPlace.side == 'B')
                            Piece.ChangePieceImage("C:\\Users\\Casa\\Source\\ImageAssets\\Bispo_Negro.png",
                                pawnInPlace);
                        else
                            Piece.ChangePieceImage("C:\\Users\\Casa\\Source\\ImageAssets\\Bispo_Branco.png",
                                pawnInPlace);
                        pieceChanged = true;
                        break;
                    case Key.K:
                        pawnInPlace.type = "Knight";
                        if (pawnInPlace.side == 'B')
                            Piece.ChangePieceImage("C:\\Users\\Casa\\Source\\ImageAssets\\Cavalo_Negro.png",
                                pawnInPlace);
                        else
                            Piece.ChangePieceImage("C:\\Users\\Casa\\Source\\ImageAssets\\Cavalo_Branco.png",
                                pawnInPlace);
                        pieceChanged = true;
                        break;
                    case Key.Q:
                        if (Piece.IsTherePieceOfType("Queen", pawnInPlace.side) == false)
                        {
                            pawnInPlace.type = "Queen";
                            if (pawnInPlace.side == 'B')
                                Piece.ChangePieceImage("C:\\Users\\Casa\\Source\\ImageAssets\\Rainha_Negra.png",
                                    pawnInPlace);
                            else
                                Piece.ChangePieceImage("C:\\Users\\Casa\\Source\\ImageAssets\\Rainha_Branca.png",
                                    pawnInPlace);
                            pieceChanged = true;
                        }
                        else WriteMessage("warningText", "You already have a queen in your side. Please choose " +
                            "another piece type.");
                        break;
                    }
                if(pieceChanged)
                    pawnInPlace = null;
                return pieceChanged;
            }
            return true;
        }

        public static bool IsKingInCheck(char side) //true - Is in check, false - is safe.
        {
            bool ret = false;
            Piece kingPiece = Piece.VerifyKingState(side);
            if (kingPiece != null)
            {
                Vector kingPos = Piece.FindPiecePosition(kingPiece);
                Table.MarkWarningSection(kingPos);
                ret = true;
            }
            else Piece.IsKingDead(side);
            return ret;
        }

        public static List<Vector> FreeKingFromCheck(char defSide, ref List<Piece> crpChessPieces)
        {
            List<Vector> retPositions = Piece.FindCheckEscapePoss(defSide, ref crpChessPieces);
            if (retPositions == null)
                ChessGameEnded(defSide);
            return retPositions;
        }

        public static void ChessGameEnded(char loseSide)
        {
            if (loseSide == 'W') WriteMessage("warningText", "Player 2 won the game.");
            else WriteMessage("warningText", "Player 1 won the game.");
            SelectSquare.DeactivateAllPlayers();
            gameEnded = true;
        }

        private static bool HasCheckersGameEnded()
        {
            gameEnded = false;
            Vector qtds = Piece.FindQtdPieces();
            if (qtds.X == 0 || qtds.Y == 0)
            {
                gameEnded = true;
                if (qtds.Y == 0) WriteMessage("topText", "Player 1 won the game!");
                else if (qtds.X == 0) WriteMessage("topText", "Player 2 won the game!");
            }
            return gameEnded;
        }

        private static IEnumerable<TextBlock> FindLabels(DependencyObject depObj)
        {
            if(depObj != null)
            {
                for(int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child is TextBlock) yield return (TextBlock)child;

                    foreach (TextBlock childOfChild in FindLabels(child)) yield return childOfChild;
                }
            }
        }
    }
}
