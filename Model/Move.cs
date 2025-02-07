using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVisualized.Model {
    public class Move {
        public Piece movingPiece;
        public Position destination;
        public bool isCapture, isBlocked, isProtecting, isNotAllowedInCheck;
        public bool isCheck, isCheckmate, isSelfCheck, isSelfCheckmate, isStalemate, isSelfStalemate;
        public bool isPromotion, isCastle, isEnPassant;

        private static bool detectStatemate = false;

        public Move(Piece movingPiece, int x, int y) {
            this.movingPiece = movingPiece;
            this.destination = new Position(x, y);
        }

        public Move(Piece movingPiece, Position destination) {
            this.movingPiece = movingPiece;
            this.destination = destination;
        }

        public static void SetDetectStalemate(bool ds) {
            detectStatemate = ds;
        }

        // Tells you if a specific move is allowed in the game.
        public bool IsLegal() {
            return !isBlocked && !isProtecting && !isSelfCheck && !isSelfCheckmate && !isSelfStalemate && !isNotAllowedInCheck;
        }

        public bool IsLegalOutOfCheck() {
            return !isBlocked && !isProtecting && !isSelfCheck && !isSelfCheckmate && !isSelfStalemate;
        }

        public bool IsProtecting() {
            return isProtecting && (!isBlocked && !isSelfCheck && !isSelfCheckmate && !isSelfStalemate && !isNotAllowedInCheck);
        }

        public bool IsSpecial() {
            return isPromotion || isCastle || isEnPassant;
        }

        // Calculate all the possible moves of a chess board and their boolean properties.
        public static List<Move> GetAllMoves(Board board, bool inCheck, bool intoCheck) {
            List<Move> result = board.GetPotentialMoves();
            MarkMoves(result, board);
            MarkCheckMoves(result, board, inCheck, intoCheck);
            CheckCastleMovesBlocked(result, board);
            return result;
        }

        private static void MarkMoves(List<Move> moves, Board board) {
            foreach (Move move in moves) {
                Piece piece = move.movingPiece;

                // Mark moves blocked by other pieces
                if (board.IsPieceBetween(piece.GetPosition(), move.destination, piece.GetPieceType())) {
                    move.isBlocked = true;
                    continue;
                }

                // Mark castling moves
                if (piece.GetPieceType() == Enums.PieceType.King &&
                    Math.Abs(piece.GetX() - move.destination.X) > 1) {
                    move.isCastle = true;
                    continue;
                }

                // Mark moves that protect other pieces on its team (exclude pawns moving straight forward)
                if (board.TeammateAt(move.destination, piece.GetTeam()) != null &&
                    !(piece.GetPieceType() == Enums.PieceType.Pawn && piece.GetX() == move.destination.X)) {
                    move.isProtecting = true;
                    continue;
                }

                // Mark en passant moves
                Piece enemyPiece = board.EnemyAt(move.destination, piece.GetTeam());
                if (piece.GetPieceType() == Enums.PieceType.Pawn &&
                    piece.GetX() != move.destination.X && enemyPiece == null) {
                    move.isEnPassant = true;
                    continue;
                }

                // Mark promotional moves
                if (piece.GetPieceType() == Enums.PieceType.Pawn &&
                    (move.destination.Y == 0 || move.destination.Y == 7)) {
                    move.isPromotion = true;
                    // Mark moves that capture other pieces (exclude pawns moving straight forward)
                    if (enemyPiece != null && !(piece.GetPieceType() == Enums.PieceType.Pawn
                        && piece.GetX() == enemyPiece.GetX()))
                        move.isCapture = true;
                    continue;
                }

                // Mark moves that capture other pieces (exclude pawns moving straight forward)
                if (enemyPiece != null && !(piece.GetPieceType() == Enums.PieceType.Pawn
                    && piece.GetX() == enemyPiece.GetX()))
                    move.isCapture = true;

                // Mark moves that put the king next to the enemy king
                Piece surroundingPiece;
                if (piece.GetPieceType() == Enums.PieceType.King) {
                    if (move.destination.X > 0 && move.destination.Y > 0) {
                        surroundingPiece = board.EnemyAt(new(move.destination.X - 1, move.destination.Y - 1), piece.GetTeam());
                        if (surroundingPiece != null && surroundingPiece.GetPieceType() == Enums.PieceType.King) {
                            move.isSelfCheck = true;
                            continue;
                        }
                    }
                    if (move.destination.X > 0) {
                        surroundingPiece = board.EnemyAt(new(move.destination.X - 1, move.destination.Y), piece.GetTeam());
                        if (surroundingPiece != null && surroundingPiece.GetPieceType() == Enums.PieceType.King) {
                            move.isSelfCheck = true;
                            continue;
                        }
                    }
                    if (move.destination.X > 0 && move.destination.Y < 7) {
                        surroundingPiece = board.EnemyAt(new(move.destination.X - 1, move.destination.Y + 1), piece.GetTeam());
                        if (surroundingPiece != null && surroundingPiece.GetPieceType() == Enums.PieceType.King) {
                            move.isSelfCheck = true;
                            continue;
                        }
                    }
                    if (move.destination.Y < 7) {
                        surroundingPiece = board.EnemyAt(new(move.destination.X, move.destination.Y + 1), piece.GetTeam());
                        if (surroundingPiece != null && surroundingPiece.GetPieceType() == Enums.PieceType.King) {
                            move.isSelfCheck = true;
                            continue;
                        }
                    }
                    if (move.destination.X < 7 && move.destination.Y < 7) {
                        surroundingPiece = board.EnemyAt(new(move.destination.X + 1, move.destination.Y + 1), piece.GetTeam());
                        if (surroundingPiece != null && surroundingPiece.GetPieceType() == Enums.PieceType.King) {
                            move.isSelfCheck = true;
                            continue;
                        }
                    }
                    if (move.destination.X < 7) {
                        surroundingPiece = board.EnemyAt(new(move.destination.X + 1, move.destination.Y), piece.GetTeam());
                        if (surroundingPiece != null && surroundingPiece.GetPieceType() == Enums.PieceType.King) {
                            move.isSelfCheck = true;
                            continue;
                        }
                    }
                    if (move.destination.X < 7 && move.destination.Y > 0) {
                        surroundingPiece = board.EnemyAt(new(move.destination.X + 1, move.destination.Y - 1), piece.GetTeam());
                        if (surroundingPiece != null && surroundingPiece.GetPieceType() == Enums.PieceType.King) {
                            move.isSelfCheck = true;
                            continue;
                        }
                    }
                    if (move.destination.Y > 0) {
                        surroundingPiece = board.EnemyAt(new(move.destination.X, move.destination.Y - 1), piece.GetTeam());
                        if (surroundingPiece != null && surroundingPiece.GetPieceType() == Enums.PieceType.King) {
                            move.isSelfCheck = true;
                            continue;
                        }
                    }
                }
            }
        }

        private static void MarkCheckMoves(List<Move> moves, Board board, bool inCheck, bool intoCheck) {
            bool whiteCurrentlyInCheck = IsTeamInCheck(moves, board, Enums.Team.White),
                blackCurrentlyInCheck = IsTeamInCheck(moves, board, Enums.Team.Black);
            bool whitePotentiallyInCheck, blackPotentiallyInCheck,
                whitePotentiallyInMate, blackPotentiallyInMate;
            Parallel.ForEach(moves, move => {
                //foreach (Move move in moves) {
                if (move.isBlocked || move.isSelfCheck)
                    return;
                //continue;
                Enums.Team team = move.movingPiece.GetTeam();
                if (move.isCastle && ((whiteCurrentlyInCheck && team == Enums.Team.White) || (blackCurrentlyInCheck && team == Enums.Team.Black))) {
                    move.isNotAllowedInCheck = true; // Castling not allowed while in check
                    return;
                    //continue;
                }
                if (inCheck && (detectStatemate || whiteCurrentlyInCheck || blackCurrentlyInCheck)) {
                    Board potential = Board.BoardAfterMove(board, move, moves, false, false);
                    bool isInCheck = IsTeamInCheck(potential.GetAllMoves(), potential, team);
                    move.isNotAllowedInCheck = isInCheck;
                }
                if (intoCheck && !move.isNotAllowedInCheck && !whiteCurrentlyInCheck && !blackCurrentlyInCheck) {
                    Board potential = Board.BoardAfterMove(board, move, moves, true, false);
                    whitePotentiallyInCheck = IsTeamInCheck(potential.GetAllMoves(), potential, Enums.Team.White);
                    blackPotentiallyInCheck = IsTeamInCheck(potential.GetAllMoves(), potential, Enums.Team.Black);
                    whitePotentiallyInMate = IsTeamInMate(potential.GetAllMoves(), Enums.Team.White);
                    blackPotentiallyInMate = IsTeamInMate(potential.GetAllMoves(), Enums.Team.Black);
                    move.isCheck = (whitePotentiallyInCheck && team == Enums.Team.Black) || (blackPotentiallyInCheck && team == Enums.Team.White);
                    move.isSelfCheck = (whitePotentiallyInCheck && team == Enums.Team.White) || (blackPotentiallyInCheck && team == Enums.Team.Black);
                    move.isStalemate = (whitePotentiallyInMate && team == Enums.Team.Black) || (blackPotentiallyInMate && team == Enums.Team.White);
                    move.isSelfStalemate = (whitePotentiallyInMate && team == Enums.Team.White) || (blackPotentiallyInMate && team == Enums.Team.Black);
                    move.isCheckmate = move.isCheck && move.isStalemate;
                    move.isSelfCheckmate = move.isSelfCheck && move.isSelfStalemate;
                }
            });
            //}
        }

        // If enemy pieces occupy or threaten empty gap between king and rook, block castle moves.
        private static void CheckCastleMovesBlocked(List<Move> moves, Board board) {
            foreach (Move move in moves) {
                if (move.isCastle && move.IsLegal()) {
                    if (move.destination.X < move.movingPiece.GetX()) {
                        for (int i = 1; i < 4; i++) {
                            if (board.PieceAt(new(i, move.movingPiece.GetY())) != null) {
                                move.isBlocked = true;
                                continue;
                            }
                            foreach (Move move2 in moves) {
                                if (move2.IsLegal() && move2.movingPiece.GetTeam() != move.movingPiece.GetTeam()
                                    && move2.destination.Equals(new Position(i, move.movingPiece.GetY()))) {
                                    move.isBlocked = true;
                                    continue;
                                }
                            }
                        }
                    } else {
                        for (int i = 6; i > 4; i--) {
                            if (board.PieceAt(new(i, move.movingPiece.GetY())) != null) {
                                move.isBlocked = true;
                                continue;
                            }
                            foreach (Move move2 in moves) {
                                if (move2.IsLegal() && move2.movingPiece.GetTeam() != move.movingPiece.GetTeam()
                                    && move2.destination.Equals(new Position(i, move.movingPiece.GetY()))) {
                                    move.isBlocked = true;
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Returns true if enemy team has a capturing move on the specified team's king.
        private static bool IsTeamInCheck(List<Move> moves, Board board, Enums.Team team) {
            foreach (Move move in moves)
                if (move.movingPiece.GetTeam() != team && move.IsLegal() && move.isCapture) {
                    Piece piece = board.TeammateAt(move.destination, team);
                    if (piece != null && piece.GetPieceType() == Enums.PieceType.King)
                        return true;
                }
            return false;
        }

        // Returns true if specified team has no legal moves.
        private static bool IsTeamInMate(List<Move> moves, Enums.Team team) {
            foreach (Move move in moves)
                if (move.movingPiece.GetTeam() == team && move.IsLegal())
                    return false;
            return true;
        }

        override
        public bool Equals(object? o) {
            if (o is Move m)
                return movingPiece.Equals(m.movingPiece) && destination.Equals(m.destination);
            return false;
        }

        override
        public int GetHashCode() {
            return movingPiece.GetHashCode() * destination.GetHashCode();
        }

        // convert to/from bytes for sending over the internet
        override
        public string ToString() {
            string piece = movingPiece.ToString();
            string dest = destination.ToString();
            return piece + "," + dest;
        }

        public static Move FromString(string str) {
            string[] split = str.Split(',');
            Piece piece = Piece.FromString(split[0]);
            Position destination = Position.FromString(split[1]);
            return new(piece, destination);
        }

        // Convert to a message useful to players
        public string ToUIString(Position originalPosition) {
            StringBuilder result = new();
            result.Append(movingPiece.GetPieceType().ToString());
            result.Append(' ');
            result.Append(originalPosition.ToUIString());
            result.Append(" -> ");
            result.Append(destination.ToUIString());
            return result.ToString();
        }
    }
}
