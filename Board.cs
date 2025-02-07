using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVisualized.Model {
    public class Board {
        private readonly Piece[][] pieces;
        private Enums.Team whoseTurnIsIt;
        private List<Move> allMoves;
        private readonly List<Piece> whiteCapturedPieces, blackCapturedPieces;
        private readonly Dictionary<Piece, List<Position>> potentialMoves;
        private bool whiteInCheckmate = false, blackInCheckmate = false;
        private bool whiteInStalemate = false, blackInStalemate = false;

        public Board(List<Piece> pieces, Enums.Team whoseTurnIsIt) {
            this.pieces = new Piece[8][];
            for (int i = 0; i < 8; i++)
                this.pieces[i] = new Piece[8];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    this.pieces[i][j] = null;
            foreach (Piece piece in pieces)
                this.pieces[piece.GetY()][piece.GetX()] = piece;

            this.whoseTurnIsIt = whoseTurnIsIt;

            whiteCapturedPieces = new(16);
            blackCapturedPieces = new(16);
            potentialMoves = new(32);
            foreach (Piece piece in pieces)
                potentialMoves.Add(piece, piece.PieceMoves(this));

            allMoves = Move.GetAllMoves(this, true, true);
        }

        // For previewing moves on boards
        private Board(Piece[][] pieces, Enums.Team whoseTurnIsIt, List<Move> allMoves, Dictionary<Piece, List<Position>> potentialMoves, List<Piece> whiteCapturedPieces, List<Piece> blackCapturedPieces, bool whiteInCheckmate, bool whiteInStalemate, bool blackInCheckmate, bool blackInStalemate) {
            this.pieces = new Piece[8][];
            for (int i = 0; i < 8; i++)
                this.pieces[i] = new Piece[8];

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (pieces[i][j] != null)
                        this.pieces[i][j] = new(pieces[i][j]);
                    else this.pieces[i][j] = null;
                }
            }

            this.whoseTurnIsIt = whoseTurnIsIt;
            this.allMoves = new(allMoves);
            this.potentialMoves = new(potentialMoves);
            this.whiteCapturedPieces = new(whiteCapturedPieces);
            this.blackCapturedPieces = new(blackCapturedPieces);
            this.whiteInCheckmate = whiteInCheckmate;
            this.whiteInStalemate = whiteInStalemate;
            this.blackInCheckmate = blackInCheckmate;
            this.blackInStalemate = blackInStalemate;
        }

        // For boards sent through string, regenerates allMoves.
        private Board(Piece[][] pieces, Enums.Team whoseTurnIsIt, Dictionary<Piece, List<Position>> potentialMoves, List<Piece> whiteCapturedPieces, List<Piece> blackCapturedPieces, bool whiteInCheckmate, bool whiteInStalemate, bool blackInCheckmate, bool blackInStalemate) {
            this.pieces = new Piece[8][];
            for (int i = 0; i < 8; i++)
                this.pieces[i] = new Piece[8];

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (pieces[i][j] != null)
                        this.pieces[i][j] = new(pieces[i][j]);
                    else this.pieces[i][j] = null;
                }
            }

            this.whoseTurnIsIt = whoseTurnIsIt;
            this.potentialMoves = new(potentialMoves);
            this.whiteCapturedPieces = new(whiteCapturedPieces);
            this.blackCapturedPieces = new(blackCapturedPieces);
            this.whiteInCheckmate = whiteInCheckmate;
            this.whiteInStalemate = whiteInStalemate;
            this.blackInCheckmate = blackInCheckmate;
            this.blackInStalemate = blackInStalemate;

            allMoves = Move.GetAllMoves(this, true, true);
        }

        // For saving games
        public Board(Board board) {
            this.pieces = new Piece[8][];
            for (int i = 0; i < 8; i++)
                this.pieces[i] = new Piece[8];

            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (pieces[i][j] != null)
                        this.pieces[i][j] = new(board.pieces[i][j]);
                    else this.pieces[i][j] = null;
                }
            }

            this.whoseTurnIsIt = board.whoseTurnIsIt;
            this.allMoves = new(board.allMoves);
            this.potentialMoves = new(board.potentialMoves);
            this.whiteCapturedPieces = new(board.whiteCapturedPieces);
            this.blackCapturedPieces = new(board.blackCapturedPieces);

            this.whiteInCheckmate = board.whiteInCheckmate;
            this.whiteInStalemate = board.whiteInStalemate;
            this.blackInCheckmate = board.blackInCheckmate;
            this.blackInStalemate = board.blackInStalemate;
        }

        public Enums.Team WhoseTurnIsIt() {
            return whoseTurnIsIt;
        }

        public bool IsWhiteInCheckmate() {
            return whiteInCheckmate;
        }

        public bool IsBlackInCheckmate() {
            return blackInCheckmate;
        }

        public bool IsWhiteInStalemate() {
            return whiteInStalemate;
        }

        public bool IsBlackInStalemate() {
            return blackInStalemate;
        }

        public List<Move> GetAllMoves() {
            return allMoves;
        }

        public List<Move> GetPotentialMoves() {
            List<Move> result = new(250);
            foreach (Piece piece in potentialMoves.Keys)
                foreach (Position position in potentialMoves[piece])
                    result.Add(new(piece, position));
            return result;
        }

        public List<Piece> GetAllPieces() {
            List<Piece> result = new();
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (pieces[i][j] != null)
                        result.Add(pieces[i][j]);
            return result;
        }

        public List<Piece> GetMyPieces(Enums.Team myTeam) {
            List<Piece> result = new();
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (pieces[i][j] != null && pieces[i][j].GetTeam() == myTeam)
                        result.Add(pieces[i][j]);
            return result;
        }

        public List<Piece> GetEnemyPieces(Enums.Team myTeam) {
            List<Piece> result = new();
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (pieces[i][j] != null && pieces[i][j].GetTeam() != myTeam)
                        result.Add(pieces[i][j]);
            return result;
        }

        public List<Piece> GetWhiteCapturedPieces() {
            return whiteCapturedPieces;
        }

        public List<Piece> GetBlackCapturedPieces() {
            return blackCapturedPieces;
        }

        public Piece PieceAt(Position position) {
            return pieces[position.Y][position.X];
        }

        public Piece TeammateAt(Position position, Enums.Team myTeam) {
            Piece pieceAt = pieces[position.Y][position.X];
            if (pieceAt != null && pieceAt.GetTeam() == myTeam)
                return pieceAt;
            return null;
        }

        public Piece EnemyAt(Position position, Enums.Team myTeam) {
            Piece pieceAt = pieces[position.Y][position.X];
            if (pieceAt != null && pieceAt.GetTeam() != myTeam)
                return pieceAt;
            return null;
        }

        // Returns true if there is a piece in any position between "here" or "there".
        // Positions need to be straight or diagonal from each other, unless you are a knight.
        public bool IsPieceBetween(Position here, Position there, Enums.PieceType type) {
            Piece piece;
            if (type == Enums.PieceType.Knight)
                return false;

            int deltaX = there.X - here.X, deltaY = there.Y - here.Y;
            int delta = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));
            if (deltaX > 0) deltaX = 1; else if (deltaX < 0) deltaX = -1;
            if (deltaY > 0) deltaY = 1; else if (deltaY < 0) deltaY = -1;
            for (int i = 1; i < delta; i++) {
                int y = here.Y + (i * deltaY), x = here.X + (i * deltaX);
                piece = pieces[y][x];
                if (piece != null)
                    return true;
            }

            piece = pieces[there.Y][there.X];
            if (piece != null && type == Enums.PieceType.Pawn && deltaX == 0)
                return true;

            return false;
        }

        // Returns true if the move succeeds
        public bool MakeAMove(Piece piece, Position destination, bool inCheck = true, bool intoCheck = true) {
            // Check if move is valid.
            Move foundMove = null;
            bool validMoveFound = false;
            foreach (Move move in allMoves) {
                if (move.movingPiece.Equals(piece) && move.destination.Equals(destination) && move.IsLegal()) {
                    foundMove = move;
                    validMoveFound = true;
                    break;
                }
            }
            if (!validMoveFound)
                return false;

            // Make all pieces not en passant-able, update moves of threatening pawns
            Piece leftPiece, rightPiece;
            foreach (Piece aPiece in GetAllPieces()) {
                if (aPiece.enPassantable) {
                    potentialMoves.Remove(aPiece);
                    aPiece.enPassantable = false;
                    potentialMoves.Add(aPiece, aPiece.PieceMoves(this));
                    if (aPiece.GetX() > 0) {
                        leftPiece = PieceAt(new(aPiece.GetX() - 1, aPiece.GetY()));
                        if (leftPiece != null && !leftPiece.Equals(piece) && leftPiece.GetPieceType() == Enums.PieceType.Pawn)
                            potentialMoves[leftPiece] = leftPiece.PieceMoves(this);
                    }
                    if (aPiece.GetX() < 7) {
                        rightPiece = PieceAt(new(aPiece.GetX() + 1, aPiece.GetY()));
                        if (rightPiece != null && !rightPiece.Equals(piece) && rightPiece.GetPieceType() == Enums.PieceType.Pawn)
                            potentialMoves[rightPiece] = rightPiece.PieceMoves(this);
                    }
                }
            }

            // Regular move
            if (foundMove.isCapture) {
                AddCapturedPiece(pieces[destination.Y][destination.X]);
                potentialMoves.Remove(pieces[destination.Y][destination.X]);
            }
            pieces[destination.Y][destination.X] = piece;
            pieces[piece.GetY()][piece.GetX()] = null;
            potentialMoves.Remove(piece);
            piece.hasMoved = true;

            // Castling to the left
            if (piece.GetPieceType() == Enums.PieceType.King && piece.GetX() - 2 == destination.X) {
                Piece rook = pieces[destination.Y][0];
                potentialMoves.Remove(rook);
                pieces[destination.Y][3] = rook;
                rook.Move(3, destination.Y);
                rook.hasMoved = true;
                pieces[destination.Y][0] = null;
                potentialMoves.Add(rook, rook.PieceMoves(this));
            }

            // Castling to the right
            if (piece.GetPieceType() == Enums.PieceType.King && piece.GetX() + 2 == destination.X) {
                Piece rook = pieces[destination.Y][7];
                potentialMoves.Remove(rook);
                pieces[destination.Y][5] = rook;
                rook.Move(5, destination.Y);
                rook.hasMoved = true;
                pieces[destination.Y][7] = null;
                potentialMoves.Add(rook, rook.PieceMoves(this));
            }

            // En passant to the left
            Piece leftPawn, rightPawn;
            if (foundMove.isEnPassant && piece.GetX() > 0) {
                leftPiece = PieceAt(new(piece.GetX() - 1, piece.GetY()));
                if (piece.GetX() - 1 == destination.X && leftPiece != null) {
                    AddCapturedPiece(leftPiece);
                    pieces[leftPiece.GetY()][leftPiece.GetX()] = null;
                    potentialMoves.Remove(leftPiece);
                    // Update any pawns that also threatened the en passanted piece
                    if (leftPiece.GetX() > 0) {
                        leftPawn = PieceAt(new(leftPiece.GetX() - 1, leftPiece.GetY() - 1));
                        if (leftPawn != null && leftPawn.GetPieceType() == Enums.PieceType.Pawn)
                            potentialMoves[leftPawn] = leftPawn.PieceMoves(this);
                        leftPawn = PieceAt(new(leftPiece.GetX() - 1, leftPiece.GetY() + 1));
                        if (leftPawn != null && leftPawn.GetPieceType() == Enums.PieceType.Pawn)
                            potentialMoves[leftPawn] = leftPawn.PieceMoves(this);
                    }
                    if (leftPiece.GetX() < 7) {
                        rightPawn = PieceAt(new(leftPiece.GetX() + 1, leftPiece.GetY() - 1));
                        if (rightPawn != null && rightPawn.GetPieceType() == Enums.PieceType.Pawn)
                            potentialMoves[rightPawn] = rightPawn.PieceMoves(this);
                        rightPawn = PieceAt(new(leftPiece.GetX() + 1, leftPiece.GetY() + 1));
                        if (rightPawn != null && rightPawn.GetPieceType() == Enums.PieceType.Pawn)
                            potentialMoves[rightPawn] = rightPawn.PieceMoves(this);
                    }
                }
            }

            // En passant to the right
            if (foundMove.isEnPassant && piece.GetX() < 7) {
                rightPiece = PieceAt(new(piece.GetX() + 1, piece.GetY()));
                if (piece.GetX() + 1 == destination.X && rightPiece != null) {
                    AddCapturedPiece(rightPiece);
                    pieces[rightPiece.GetY()][rightPiece.GetX()] = null;
                    potentialMoves.Remove(rightPiece);
                    // Update any pawns that also threatened en passanted piece
                    if (rightPiece.GetX() > 0) {
                        leftPawn = PieceAt(new(rightPiece.GetX() - 1, rightPiece.GetY() - 1));
                        if (leftPawn != null && leftPawn.GetPieceType() == Enums.PieceType.Pawn)
                            potentialMoves[leftPawn] = leftPawn.PieceMoves(this);
                        leftPawn = PieceAt(new(rightPiece.GetX() - 1, rightPiece.GetY() + 1));
                        if (leftPawn != null && leftPawn.GetPieceType() == Enums.PieceType.Pawn)
                            potentialMoves[leftPawn] = leftPawn.PieceMoves(this);
                    }
                    if (rightPiece.GetX() < 7) {
                        rightPawn = PieceAt(new(rightPiece.GetX() + 1, rightPiece.GetY() - 1));
                        if (rightPawn != null && rightPawn.GetPieceType() == Enums.PieceType.Pawn)
                            potentialMoves[rightPawn] = rightPawn.PieceMoves(this);
                        rightPawn = PieceAt(new(rightPiece.GetX() + 1, rightPiece.GetY() + 1));
                        if (rightPawn != null && rightPawn.GetPieceType() == Enums.PieceType.Pawn)
                            potentialMoves[rightPawn] = rightPawn.PieceMoves(this);
                    }
                }
            }

            // Maybe make the moving piece en passant-able
            if (piece.GetPieceType() == Enums.PieceType.Pawn && Math.Abs(piece.GetY() - destination.Y) == 2) {
                piece.enPassantable = true;
                if (piece.GetX() > 0) {
                    leftPiece = PieceAt(new(destination.X - 1, destination.Y));
                    if (leftPiece != null && leftPiece.GetPieceType() == Enums.PieceType.Pawn)
                        potentialMoves[leftPiece] = leftPiece.PieceMoves(this);
                }
                if (piece.GetX() < 7) {
                    rightPiece = PieceAt(new(destination.X + 1, destination.Y));
                    if (rightPiece != null && rightPiece.GetPieceType() == Enums.PieceType.Pawn)
                        potentialMoves[rightPiece] = rightPiece.PieceMoves(this);
                }
            }

            // Update king moves when castling becomes unavailable
            Piece king;
            if (piece.GetPosition().Equals(new Position(0, 0)) || piece.GetPosition().Equals(new Position(7, 0)) ||
                destination.Equals(new Position(0, 0)) || destination.Equals(new Position(7, 0))) {
                king = pieces[0][4];
                if (king != null && king.GetPieceType() == Enums.PieceType.King)
                    potentialMoves[king] = king.PieceMoves(this);
            }
            if (piece.GetPosition().Equals(new Position(0, 7)) || piece.GetPosition().Equals(new Position(7, 7)) ||
                destination.Equals(new Position(0, 7)) || destination.Equals(new Position(7, 7))) {
                king = pieces[7][4];
                if (king != null && king.GetPieceType() == Enums.PieceType.King)
                    potentialMoves[king] = king.PieceMoves(this);
            }

            // Update moves of diagonal pawns you move away from
            if (piece.GetY() < 7) {
                if (piece.GetX() > 0) {
                    leftPawn = PieceAt(new(piece.GetX() - 1, piece.GetY() + 1));
                    if (leftPawn != null && leftPawn.GetPieceType() == Enums.PieceType.Pawn && !leftPawn.Equals(piece))
                        potentialMoves[leftPawn] = leftPawn.PieceMoves(this);
                }
                if (piece.GetX() < 7) {
                    rightPawn = PieceAt(new(piece.GetX() + 1, piece.GetY() + 1));
                    if (rightPawn != null && rightPawn.GetPieceType() == Enums.PieceType.Pawn && !rightPawn.Equals(piece))
                        potentialMoves[rightPawn] = rightPawn.PieceMoves(this);
                }
            }
            if (piece.GetY() > 0) {
                if (piece.GetX() > 0) {
                    leftPawn = PieceAt(new(piece.GetX() - 1, piece.GetY() - 1));
                    if (leftPawn != null && leftPawn.GetPieceType() == Enums.PieceType.Pawn && !leftPawn.Equals(piece))
                        potentialMoves[leftPawn] = leftPawn.PieceMoves(this);
                }
                if (piece.GetX() < 7) {
                    rightPawn = PieceAt(new(piece.GetX() + 1, piece.GetY() - 1));
                    if (rightPawn != null && rightPawn.GetPieceType() == Enums.PieceType.Pawn && !rightPawn.Equals(piece))
                        potentialMoves[rightPawn] = rightPawn.PieceMoves(this);
                }
            }

            // Update moves of diagonal pawns you move towards
            if (destination.Y < 7) {
                if (destination.X > 0) {
                    leftPawn = PieceAt(new(destination.X - 1, destination.Y + 1));
                    if (leftPawn != null && leftPawn.GetPieceType() == Enums.PieceType.Pawn && !leftPawn.Equals(piece))
                        potentialMoves[leftPawn] = leftPawn.PieceMoves(this);
                }
                if (destination.X < 7) {
                    rightPawn = PieceAt(new(destination.X + 1, destination.Y + 1));
                    if (rightPawn != null && rightPawn.GetPieceType() == Enums.PieceType.Pawn && !rightPawn.Equals(piece))
                        potentialMoves[rightPawn] = rightPawn.PieceMoves(this);
                }
            }
            if (destination.Y > 0) {
                if (destination.X > 0) {
                    leftPawn = PieceAt(new(destination.X - 1, destination.Y - 1));
                    if (leftPawn != null && leftPawn.GetPieceType() == Enums.PieceType.Pawn && !leftPawn.Equals(piece))
                        potentialMoves[leftPawn] = leftPawn.PieceMoves(this);
                }
                if (destination.X < 7) {
                    rightPawn = PieceAt(new(destination.X + 1, destination.Y - 1));
                    if (rightPawn != null && rightPawn.GetPieceType() == Enums.PieceType.Pawn && !rightPawn.Equals(piece))
                        potentialMoves[rightPawn] = rightPawn.PieceMoves(this);
                }
            }

            // Update piece position and promote
            if (piece.GetPieceType() == Enums.PieceType.Pawn && (destination.Y == 0 || destination.Y == 7))
                piece.Promote(Enums.PieceType.Queen);
            piece.Move(destination.X, destination.Y);
            potentialMoves.Add(piece, piece.PieceMoves(this));

            //CheckPotentialMovesKeys();
            //CheckPotentialMoves();

            // Switch turns
            if (whoseTurnIsIt == Enums.Team.White)
                whoseTurnIsIt = Enums.Team.Black;
            else whoseTurnIsIt = Enums.Team.White;

            // Check and set checkmate/stalemate flags
            if (foundMove.isCheckmate)
                if (piece.GetTeam() == Enums.Team.Black)
                    whiteInCheckmate = true;
                else blackInCheckmate = true;

            bool whiteMoveExists = false, blackMoveExists = false;
            foreach (Move move in allMoves) {
                if (move.IsLegal() && move.movingPiece.GetTeam() == Enums.Team.White) {
                    whiteMoveExists = true;
                    break;
                }
            }
            foreach (Move move in allMoves) {
                if (move.IsLegal() && move.movingPiece.GetTeam() == Enums.Team.Black) {
                    blackMoveExists = true;
                    break;
                }
            }

            if (!whiteMoveExists)
                whiteInStalemate = true;
            if (!blackMoveExists)
                blackInStalemate = true;

            allMoves = Move.GetAllMoves(this, inCheck, intoCheck);
            return true;
        }

        private void AddCapturedPiece(Piece piece) {
            if (piece.GetTeam() == Enums.Team.White)
                whiteCapturedPieces.Add(piece);
            else blackCapturedPieces.Add(piece);
        }

        /*private void CheckPotentialMoves() {
            foreach (Piece piece in potentialMoves.Keys) {
                HashSet<Position> a = new(potentialMoves[piece]);
                HashSet<Position> b = new(piece.PieceMoves(this));
                if (!a.SetEquals(b))
                    throw new Exception("Piece moves not updated properly");
            }
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (pieces[i][j] == null)
                        continue;
                    HashSet<Position> a = new(potentialMoves[pieces[i][j]]);
                    HashSet<Position> b = new(pieces[i][j].PieceMoves(this));
                    if (!a.SetEquals(b))
                        throw new Exception("Piece moves not updated properly");
                }
            }
        }*/

        // Will break castle moves that haven't yet been checked for blocking
        /*private void CheckPotentialMovesKeys(int offset = 0) {
            int count = offset;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (pieces[i][j] != null)
                        count++;
            if (potentialMoves.Keys.Count != count)
                throw new Exception("Wrong number of potential pieces");
            foreach (Piece piece in potentialMoves.Keys)
                if (!potentialMoves.ContainsKey(piece))
                    throw new Exception("Piece keys not updated properly");
        }*/

        // Creates the starting layout of a standard chess game.
        public static Board DefaultGame() {
            List<Piece> pieces = new() {
                // Rooks
                new Piece(Enums.Team.White, Enums.PieceType.Rook, 0, 0, false, false),
                new Piece(Enums.Team.White, Enums.PieceType.Rook, 7, 0, false, false),
                new Piece(Enums.Team.Black, Enums.PieceType.Rook, 0, 7, false, false),
                new Piece(Enums.Team.Black, Enums.PieceType.Rook, 7, 7, false, false),

                // Knights
                new Piece(Enums.Team.White, Enums.PieceType.Knight, 1, 0, false, false),
                new Piece(Enums.Team.White, Enums.PieceType.Knight, 6, 0, false, false),
                new Piece(Enums.Team.Black, Enums.PieceType.Knight, 1, 7, false, false),
                new Piece(Enums.Team.Black, Enums.PieceType.Knight, 6, 7, false, false),

                // Bishops
                new Piece(Enums.Team.White, Enums.PieceType.Bishop, 2, 0, false, false),
                new Piece(Enums.Team.White, Enums.PieceType.Bishop, 5, 0, false, false),
                new Piece(Enums.Team.Black, Enums.PieceType.Bishop, 2, 7, false, false),
                new Piece(Enums.Team.Black, Enums.PieceType.Bishop, 5, 7, false, false),

                // Queens
                new Piece(Enums.Team.White, Enums.PieceType.Queen, 3, 0, false, false),
                new Piece(Enums.Team.Black, Enums.PieceType.Queen, 3, 7, false, false),

                // Kings
                new Piece(Enums.Team.White, Enums.PieceType.King, 4, 0, false, false),
                new Piece(Enums.Team.Black, Enums.PieceType.King, 4, 7, false, false),
            };

            // Pawns
            for (int i = 0; i < 8; i++) {
                pieces.Add(new Piece(Enums.Team.White, Enums.PieceType.Pawn, i, 1, false, false));
                pieces.Add(new Piece(Enums.Team.Black, Enums.PieceType.Pawn, i, 6, false, false));
            }

            return new Board(pieces, Enums.Team.White);
        }

        public static Board BoardAfterMove(Board board, Move move, List<Move> allMoves, bool inCheck, bool intoCheck) {
            Board result = new(board.pieces, board.whoseTurnIsIt, allMoves, board.potentialMoves, board.whiteCapturedPieces, board.blackCapturedPieces, board.whiteInCheckmate, board.whiteInStalemate, board.blackInCheckmate, board.blackInStalemate);
            Piece piece = result.pieces[move.movingPiece.GetY()][move.movingPiece.GetX()];
            result.MakeAMove(piece, move.destination, inCheck, intoCheck);
            return result;
        }

        override
        public string ToString() {
            StringBuilder result = new("       0      1      2      3      4      5      6      7      \n");
            for (int y = 7; y >= 0; y--) {
                result.Append(y + "      ");
                for (int x = 0; x < 8; x++) {
                    Piece piece = pieces[y][x];
                    if (piece != null)
                        result.Append(string.Format("{0, -7}", piece.GetPieceType().ToString()));
                    else result.Append("X      ");
                }
                result.Append('\n');
            }
            return result.ToString();
        }

        override
        public bool Equals(object? o) { // C# sucks so I have to check this all manually.
            if (o is Board other) {
                for (int i = 0; i < 8; i++) {
                    for (int j = 0; j < 8; j++) {
                        if (pieces[i][j] == null && other.pieces[i][j] == null)
                            continue;
                        if (pieces[i][j] == null ^ other.pieces[i][j] == null)
                            return false;
                        if (!pieces[i][j].Equals(other.pieces[i][j]))
                            return false;
                    }
                }
                Piece[] keysA = potentialMoves.Keys.ToArray(), keysB = other.potentialMoves.Keys.ToArray();
                if (!keysA.SequenceEqual(keysB))
                    return false;
                foreach (Piece key in keysA)
                    if (!potentialMoves[key].SequenceEqual(other.potentialMoves[key]))
                        return false;

                if (whiteCapturedPieces.Count != other.whiteCapturedPieces.Count)
                    return false;
                for (int i = 0; i < whiteCapturedPieces.Count; i++)
                    if (!whiteCapturedPieces[i].Equals(other.whiteCapturedPieces[i]))
                        return false;

                if (blackCapturedPieces.Count != other.blackCapturedPieces.Count)
                    return false;
                for (int i = 0; i < blackCapturedPieces.Count; i++)
                    if (!blackCapturedPieces[i].Equals(other.blackCapturedPieces[i]))
                        return false;

                return whoseTurnIsIt == other.whoseTurnIsIt && whiteInCheckmate == other.whiteInCheckmate && whiteInStalemate == other.whiteInStalemate &&
                    blackInCheckmate == other.blackInCheckmate && blackInStalemate == other.blackInStalemate;
            }
            return false;
        }

        // convert to/from bytes for sending over the internet
        public string ToNetworkString() {
            StringBuilder result = new();

            // Pieces
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (pieces[i][j] != null) result.Append(pieces[i][j].ToString());
                    else result.Append("null");
                    result.Append(',');
                }
            }
            result.Remove(result.Length - 1, 1);
            result.Append('|');

            // Turn
            result.Append(whoseTurnIsIt.ToString());
            result.Append('|');

            // Captured pieces
            int length = result.Length;
            foreach (Piece piece in whiteCapturedPieces)
                result.Append(piece.ToString() + ',');
            if (result.Length > length)
                result.Remove(result.Length - 1, 1);
            result.Append('|');

            length = result.Length;
            foreach (Piece piece in blackCapturedPieces)
                result.Append(piece.ToString() + ',');
            if (result.Length > length)
                result.Remove(result.Length - 1, 1);
            result.Append('|');

            // Potential moves
            foreach (KeyValuePair<Piece, List<Position>> pair in potentialMoves) {
                result.Append(pair.Key.ToString() + ',');
                foreach (Position pos in pair.Value)
                    result.Append(pos.ToString() + ',');
                result.Remove(result.Length - 1, 1);
                result.Append(';');
            }
            result.Remove(result.Length - 1, 1);
            result.Append('|');

            // Mate flags
            result.Append(whiteInCheckmate.ToString() + ',');
            result.Append(blackInCheckmate.ToString() + ',');
            result.Append(whiteInStalemate.ToString() + ',');
            result.Append(blackInStalemate.ToString());

            return result.ToString();
        }

        public static Board FromNetworkString(string str) {
            string[] components = str.Split('|');

            // Pieces
            Piece[][] pieces = new Piece[8][];
            for (int i = 0; i < 8; i++)
                pieces[i] = new Piece[8];
            string[] pieceStr = components[0].Split(',');

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    pieces[i][j] = Piece.FromString(pieceStr[(i * 8) + j]);

            // Turn
            Enums.Team turn = Enum.Parse<Enums.Team>(components[1]);

            // Captured Pieces
            string[] whiteCapStr = components[2].Split(','), blackCapStr = components[3].Split(',');
            List<Piece> whiteCapturedPieces = new(16), blackCapturedPieces = new(16);
            if (!whiteCapStr[0].Equals(""))
                foreach (string wcs in whiteCapStr)
                    whiteCapturedPieces.Add(Piece.FromString(wcs));
            if (!blackCapStr[0].Equals(""))
                foreach (string bcs in blackCapStr)
                    whiteCapturedPieces.Add(Piece.FromString(bcs));

            // Potential Moves
            Dictionary<Piece, List<Position>> potentialMoves = new(32);
            string[] pairs = components[4].Split(';');
            foreach (string pair in pairs) {
                string[] keyValue = pair.Split(',');
                Piece key = Piece.FromString(keyValue[0]);
                List<Position> value = new();
                for (int i = 1; i < keyValue.Length; i++)
                    value.Add(Position.FromString(keyValue[i]));
                potentialMoves.Add(key, value);
            }

            // Mate Flags
            string[] flags = components[5].Split(',');
            bool whiteInCheckmate = bool.Parse(flags[0]);
            bool whiteInStalemate = bool.Parse(flags[1]);
            bool blackInCheckmate = bool.Parse(flags[2]);
            bool blackInStalemate = bool.Parse(flags[3]);

            return new(pieces, turn, potentialMoves, whiteCapturedPieces, blackCapturedPieces, whiteInCheckmate, whiteInStalemate, blackInCheckmate, blackInStalemate);
        }
    }
}