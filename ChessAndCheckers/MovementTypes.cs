using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChessAndCheckers
{
    public class MovementTypes
    {
        public static List<Vector> FindMovePoss(char side, string type, Vector initialPos, 
            List<Vector> blockedPoss = null, bool auxBool = true)
        {
            switch (type)
            {
                case "Checker":
                    if (blockedPoss == null) return CheckerMovement(side, initialPos);
                    else return CheckerChainMovement(side, initialPos, blockedPoss);
                case "Dama":
                    if (blockedPoss == null) return DamaMovement(side, initialPos);
                    else return DamaChainMovement(side, initialPos, blockedPoss);
                case "Pawn":
                    return PawnMovement(side, initialPos, !auxBool, auxBool);
                case "Rook":
                    return RookMovement(side, initialPos, auxBool);
                case "Knight":
                    return KnightMovement(side, initialPos, auxBool);
                case "Bishop":
                    return BishopMovement(side, initialPos, auxBool);
                case "Queen":
                    return QueenMovement(side, initialPos, auxBool);
                case "King":
                    return KingMovement(side, initialPos, auxBool);
                default:
                    return null;
            }
        }

        private static List<Vector> CheckerMovement(char s, Vector iniPos)
        {
            List<Vector> positions = new List<Vector>();
            Vector posModifier = new Vector();

            if (s == 'B') posModifier = new Vector(75, 75);
            else posModifier = new Vector(75, -75);

            for (int i = 0; i < 2; i++)
            {
                Vector searchedPos = new Vector(iniPos.X + posModifier.X, iniPos.Y + posModifier.Y);

                if (searchedPos.X >= 188 && searchedPos.X <= 713 && searchedPos.Y >= 30 && searchedPos.Y <= 555)
                {
                    Piece pieceAtDest = Piece.FindPieceAtPos(searchedPos);
                    if (pieceAtDest != null)
                    {
                        if (pieceAtDest.side != s)
                        {
                            searchedPos = new Vector(searchedPos.X + posModifier.X, searchedPos.Y + posModifier.Y);
                            if (searchedPos.X >= 188 && searchedPos.X <= 713 && searchedPos.Y >= 30 && 
                                searchedPos.Y <= 555)
                                if (Piece.FindPieceAtPos(searchedPos) == null)
                                    positions.Add(new Vector(searchedPos.X, searchedPos.Y));
                        }
                    }
                }

                posModifier = new Vector(posModifier.X * -1, posModifier.Y);
            }

            if (positions.Count == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector searchedPos = new Vector(iniPos.X + posModifier.X, iniPos.Y + posModifier.Y);
                    if (searchedPos.X >= 188 && searchedPos.X <= 713 && searchedPos.Y >= 30 && searchedPos.Y <= 555)
                    {
                        if (Piece.FindPieceAtPos(searchedPos) == null)
                            positions.Add(searchedPos);
                    }
                    posModifier = new Vector(posModifier.X * -1, posModifier.Y);
                }
            }

            if (positions.Count > 0) return positions;
            else return null;
        }

        private static List<Vector> CheckerChainMovement(char s, Vector iniPos, List<Vector> blkPos)
        {
            List<Vector> positions = new List<Vector>();
            Vector posModifier = new Vector(75, 75);

            for (int i = 0; i < 4; i++)
            {
                Vector searchedPos = new Vector(iniPos.X + (posModifier.X * 2), iniPos.Y + (posModifier.Y * 2));

                if (searchedPos.X >= 188 && searchedPos.X <= 713 && searchedPos.Y >= 30 && searchedPos.Y <= 555)
                {
                    Piece pieceAtDest = Piece.FindPieceAtPos(new Vector(searchedPos.X, searchedPos.Y));
                    Piece pieceObst = null;

                    Vector obstPos = new Vector(searchedPos.X - posModifier.X, searchedPos.Y - posModifier.Y);
                    foreach (Vector pos in blkPos) if (obstPos == pos) obstPos = new Vector(0, 0);
                    if (obstPos != new Vector(0, 0)) pieceObst = Piece.FindPieceAtPos(obstPos);

                    if (pieceObst != null)
                        if(pieceObst.side != s && pieceAtDest == null)
                            positions.Add(new Vector(searchedPos.X, searchedPos.Y));
                    
                }

                if (i % 2 == 0) posModifier = new Vector(posModifier.X * -1, posModifier.Y);
                else posModifier = new Vector(posModifier.X, posModifier.Y * -1);
            }

            if (positions.Count > 0) return positions;
            else return null;
        }

        private static List<Vector> DamaMovement(char s, Vector iniPos)
        {
            List<Vector> positions = new List<Vector>();
            Vector dirModifier = new Vector(75, 75);

            for(int i = 0; i < 4; i++)
            {
                int j = 0;
                int numBlockedPos = 0;
                Vector distModifier = new Vector();
                while(j < 7 && numBlockedPos < 2)
                {
                    Vector searchedPos = new Vector(iniPos.X + dirModifier.X + distModifier.X, 
                        iniPos.Y + dirModifier.Y + distModifier.Y);

                    if (searchedPos.X <= 713 && searchedPos.X >= 188 && searchedPos.Y >= 30 && searchedPos.Y <= 555)
                        if (Piece.FindPieceAtPos(searchedPos) == null)
                        {
                            if (numBlockedPos == 1)
                                positions.Add(searchedPos);
                        }
                        else if (Piece.FindPieceAtPos(searchedPos).side != s) numBlockedPos++;
                        else numBlockedPos = 2;
                    else numBlockedPos = 2;

                    distModifier += dirModifier;
                    j++;
                }

                if (i % 2 == 0) dirModifier = new Vector(dirModifier.X * -1, dirModifier.Y);
                else dirModifier = new Vector(dirModifier.X, dirModifier.Y * -1);
            }

            if (positions.Count == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    int j = 0;
                    int numBlockedPos = 0;
                    Vector distModifier = new Vector();
                    while (j < 7 && numBlockedPos < 1)
                    {
                        Vector searchedPos = new Vector(iniPos.X + dirModifier.X + distModifier.X,
                            iniPos.Y + dirModifier.Y + distModifier.Y);

                        if (searchedPos.X <= 713 && searchedPos.X >= 188 && searchedPos.Y >= 30 && searchedPos.Y <= 555)
                            if (Piece.FindPieceAtPos(searchedPos) == null)
                                positions.Add(searchedPos);
                            else numBlockedPos++;
                        else numBlockedPos++;

                        distModifier += dirModifier;
                        j++;
                    }

                    if (i % 2 == 0) dirModifier = new Vector(dirModifier.X * -1, dirModifier.Y);
                    else dirModifier = new Vector(dirModifier.X, dirModifier.Y * -1);
                }
            }

            if (positions.Count > 0) return positions;
            else return null;
        }

        private static List<Vector> DamaChainMovement(char s, Vector iniPos, List<Vector> blkPos)
        {
            List<Vector> positions = new List<Vector>();
            Vector dirModifier = new Vector(75, 75);

            for (int i = 0; i < 4; i++)
            {
                int j = 0;
                bool enemyFound = false;
                Vector distModifier = new Vector();
                while (j < 7)
                {
                    bool blockedPos = false;
                    Vector searchedPos = new Vector(iniPos.X + dirModifier.X + distModifier.X,
                        iniPos.Y + dirModifier.Y + distModifier.Y);

                    if (searchedPos.X <= 713 && searchedPos.X >= 188 && searchedPos.Y >= 30 && searchedPos.Y <= 555)
                    {
                        foreach (Vector pos in blkPos)
                            if (pos == searchedPos) blockedPos = true;
                        if (!blockedPos)
                        {
                            Piece pieceAtPos = Piece.FindPieceAtPos(searchedPos);
                            if (pieceAtPos != null)
                                if (pieceAtPos.side != s) enemyFound = true;
                            if (enemyFound)
                                if (pieceAtPos == null) positions.Add(searchedPos);
                        }
                    }
                    else j = 7;

                    distModifier += dirModifier;
                    j++;
                }

                if (i % 2 == 0) dirModifier = new Vector(dirModifier.X * -1, dirModifier.Y);
                else dirModifier = new Vector(dirModifier.X, dirModifier.Y * -1);
            }

            if (positions.Count > 0) return positions;
            else return null;
        }

        private static List<Vector> PawnMovement(char s, Vector iniPos, bool addCapturePos, bool actvCheckVrf)
        {
            List<Vector> positions = new List<Vector>();
            int posModifier = 0;

            if (!addCapturePos)
            {
                if (s == 'W')
                {
                    posModifier = -75;
                    Vector pos = new Vector(iniPos.X, iniPos.Y + (posModifier * 2));
                    if (iniPos.Y == 480 && Piece.FindPieceAtPos(pos) == null)
                        if (Piece.FindPieceAtPos(new Vector(pos.X, pos.Y - posModifier)) == null)
                            positions.Add(pos);
                }
                else
                {
                    posModifier = 75;
                    Vector pos = new Vector(iniPos.X, iniPos.Y + (posModifier * 2));
                    if (iniPos.Y == 105 && Piece.FindPieceAtPos(pos) == null)
                        if (Piece.FindPieceAtPos(new Vector(pos.X, pos.Y - posModifier)) == null)
                            positions.Add(pos);
                }

                if (Piece.FindPieceAtPos(new Vector(iniPos.X, iniPos.Y + posModifier)) == null)
                    positions.Add(new Vector(iniPos.X, iniPos.Y + posModifier));
            }

            int horztlMod = 75;
            for (int i = 0; i < 2; i++)
            {
                Vector searchedPos = new Vector(iniPos.X + horztlMod, iniPos.Y + posModifier);
                Piece searchedPiece = Piece.FindPieceAtPos(searchedPos);
                bool addPos = false;
                if (addCapturePos) addPos = true;
                else if (searchedPiece != null)
                    if (searchedPiece.side != s) addPos = true;
                if(addPos) positions.Add(searchedPos);
                horztlMod = -horztlMod;
            }

            if (actvCheckVrf) positions = Piece.WouldLeaveKingInCheck(positions, iniPos, s);

            if (positions.Count > 0) return positions;
            else return null;
        }

        private static List<Vector> RookMovement(char s, Vector iniPos, bool actvCheckVrf)
        {
            bool searchingPoss = true;
            Vector posModifier = new Vector(75, 0);
            Vector searchedPos = iniPos + posModifier;
            List<Vector> positions = new List<Vector>();

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    do
                    {
                        if (searchedPos.X >= 188 && searchedPos.X <= 713 && searchedPos.Y >= 30 && searchedPos.Y <= 555)
                        {
                            Piece pieceAtPos = Piece.FindPieceAtPos(searchedPos);
                            if (pieceAtPos == null) positions.Add(searchedPos);
                            else
                            {
                                if (pieceAtPos.side != s) positions.Add(searchedPos);
                                searchingPoss = false;
                            }
                            searchedPos += posModifier;
                        }
                        else searchingPoss = false;
                    } while (searchingPoss);

                    searchingPoss = true;
                    posModifier = -posModifier;
                    searchedPos = iniPos + posModifier;
                }

                posModifier = new Vector(0, 75);
                searchedPos = iniPos + posModifier;
            }

            if (actvCheckVrf) positions = Piece.WouldLeaveKingInCheck(positions, iniPos, s);

            if (positions.Count > 0) return positions;
            else return null;
        }

        private static List<Vector> KnightMovement(char s, Vector iniPos, bool actvCheckVrf)
        {
            List<Vector> positions = new List<Vector>();
            Vector dirModifier = new Vector(150, 0);
            for (int i = 0; i < 4; i++)
            {
                Vector edgeModifier = new Vector();
                if (i < 2) edgeModifier = new Vector(0, 75);
                else edgeModifier = new Vector(75, 0);
                for (int j = 0; j < 2; j++)
                {
                    Vector searchedPos = iniPos + dirModifier + edgeModifier;
                    if (searchedPos.X >= 188 && searchedPos.X <= 713 && searchedPos.Y <= 555 && searchedPos.Y >= 30)
                    {
                        Piece pieceAtPos = Piece.FindPieceAtPos(searchedPos);
                        if (pieceAtPos == null) positions.Add(searchedPos);
                        else if (pieceAtPos.side != s) positions.Add(searchedPos);
                    }
                    edgeModifier = -edgeModifier;
                }
                dirModifier = -dirModifier;
                if (i == 1) dirModifier = new Vector(0, 150);
            }

            if (actvCheckVrf) positions = Piece.WouldLeaveKingInCheck(positions, iniPos, s);

            if (positions.Count > 0) return positions;
            else return null;
        }

        private static List<Vector> BishopMovement(char s, Vector iniPos, bool actvCheckVrf)
        {
            List<Vector> positions = new List<Vector>();
            Vector posModifier = new Vector(75, 75);
            for(int i = 0; i < 4; i++)
            {
                int distModifier = 1;
                bool continueSearch = true;
                while (continueSearch)
                {
                    Vector searchedPos = iniPos + (posModifier * distModifier);
                    if (searchedPos.X >= 188 && searchedPos.X <= 713 && searchedPos.Y >= 30 && searchedPos.Y <= 555)
                    {
                        Piece pieceAtPos = Piece.FindPieceAtPos(searchedPos);
                        if (pieceAtPos == null) positions.Add(searchedPos);
                        else
                        {
                            if (pieceAtPos.side != s) positions.Add(searchedPos);
                            continueSearch = false;
                        }
                    }
                    else continueSearch = false;
                    distModifier++;
                }

                if (i % 2 == 0) posModifier.X = -posModifier.X;
                else posModifier.Y = -posModifier.Y;
            }

            if (actvCheckVrf) positions = Piece.WouldLeaveKingInCheck(positions, iniPos, s);

            if (positions.Count > 0) return positions;
            else return null;
        }

        private static List<Vector> QueenMovement(char s, Vector iniPos, bool actvCheckVrf)
        {
            List<Vector> positions = new List<Vector>();
            List<Vector> linearPositions = RookMovement(s, iniPos, actvCheckVrf);
            List<Vector> diagonalPositions = BishopMovement(s, iniPos, actvCheckVrf);
            if (linearPositions != null) positions.AddRange(linearPositions);
            if (diagonalPositions != null) positions.AddRange(diagonalPositions);

            if (actvCheckVrf) positions = Piece.WouldLeaveKingInCheck(positions, iniPos, s);

            if (positions.Count > 0) return positions;
            else return null;
        }

        private static List<Vector> KingMovement(char s, Vector iniPos, bool actvCheckVrf)
        {
            List<Vector> positions = new List<Vector>();
            Vector posModifier = new Vector(75, 75);
            Vector searchedPos = new Vector();
            for (int i = 0; i < 4; i++)
            {
                searchedPos = iniPos + posModifier;
                if (searchedPos.X >= 188 && searchedPos.X <= 713 && searchedPos.Y >= 30 && searchedPos.Y <= 555)
                {
                    Piece pieceAtPos = Piece.FindPieceAtPos(searchedPos);
                    if (pieceAtPos == null) positions.Add(searchedPos);
                    else if (pieceAtPos.side != s) positions.Add(searchedPos);
                }
                if (i % 2 == 0) posModifier.X = -posModifier.X;
                else posModifier.Y = -posModifier.Y;
            }
            posModifier = new Vector(75, 0);
            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    searchedPos = iniPos + posModifier;
                    if (searchedPos.X >= 188 && searchedPos.X <= 713 && searchedPos.Y >= 30 && searchedPos.Y <= 555)
                    {
                        Piece pieceAtPos = Piece.FindPieceAtPos(searchedPos);
                        if (pieceAtPos == null) positions.Add(searchedPos);
                        else if (pieceAtPos.side != s) positions.Add(searchedPos);
                    }
                    posModifier = -posModifier;
                }
                posModifier = new Vector(0, 75);
            }

            if (actvCheckVrf)
                positions = Piece.ArePositionsInCheck(positions, s);

            if (positions.Count > 0) return positions;
            else return null;
        }
    }
}
