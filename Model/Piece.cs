using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVisualized.Model {
    /*
     * Defines all chess pieces, where they are, and where they can move.
     */
    public partial class Piece {

        private readonly Enums.Team team;
        private Enums.PieceType pieceType;
        private int x, y;
        public bool hasMoved;
        public bool enPassantable;

        public Piece(Enums.Team team, Enums.PieceType pieceType, int x, int y, bool hasMoved, bool enPassantable) {
            this.team = team;
            this.pieceType = pieceType;
            this.x = x;
            this.y = y;
            this.hasMoved = hasMoved;
            this.enPassantable = enPassantable;
        }

        public Piece(Piece piece) {
            this.team = piece.team;
            this.pieceType = piece.pieceType;
            this.x = piece.x;
            this.y = piece.y;
            this.hasMoved = piece.hasMoved;
            this.enPassantable = piece.enPassantable;
        }

        public Position GetPosition() {
            return new Position(x, y);
        }

        public int GetX() {
            return x;
        }

        public int GetY() {
            return y;
        }

        public Enums.Team GetTeam() {
            return team;
        }

        public Enums.PieceType GetPieceType() {
            return pieceType;
        }

        public bool HasMoved() {
            return hasMoved;
        }

        public bool IsEnPassantable() {
            return enPassantable;
        }

        public void Promote(Enums.PieceType newType) {
            pieceType = newType;
        }

        public void Move(int newX, int newY) {
            x = newX;
            y = newY;
        }

        // Gets all the positions a piece can move to.
        public List<Position> PieceMoves(Board board) {
            List<Position> result = new();
            Piece otherPiece;
            int newX, newY;
            switch (pieceType) {

                case Enums.PieceType.Pawn:
                    if (team == Enums.Team.White) {
                        // First move
                        if (!hasMoved)
                            result.Add(new Position(x, y + 2));
                        // Regular move
                        if (y < 7)
                            result.Add(new Position(x, y + 1));
                        else break;
                        // Regular attack
                        if (x > 0) {
                            otherPiece = board.PieceAt(new Position(x - 1, y + 1));
                            if (otherPiece != null)
                                result.Add(new Position(x - 1, y + 1));
                        }
                        if (x < 7) {
                            otherPiece = board.PieceAt(new Position(x + 1, y + 1));
                            if (otherPiece != null)
                                result.Add(new Position(x + 1, y + 1));
                        }
                        // En Passant
                        if (x > 0) {
                            otherPiece = board.EnemyAt(new Position(x - 1, y), team);
                            if (otherPiece != null && otherPiece.enPassantable)
                                result.Add(new Position(x - 1, y + 1));
                        }
                        if (x < 7) {
                            otherPiece = board.EnemyAt(new Position(x + 1, y), team);
                            if (otherPiece != null && otherPiece.enPassantable)
                                result.Add(new Position(x + 1, y + 1));
                        }
                    } else {
                        // First move
                        if (!hasMoved)
                            result.Add(new Position(x, y - 2));
                        // Regular move
                        if (y < 7)
                            result.Add(new Position(x, y - 1));
                        else break;
                        // Regular attack
                        if (x > 0) {
                            otherPiece = board.PieceAt(new Position(x - 1, y - 1));
                            if (otherPiece != null)
                                result.Add(new Position(x - 1, y - 1));
                        }
                        if (x < 7) {
                            otherPiece = board.PieceAt(new Position(x + 1, y - 1));
                            if (otherPiece != null)
                                result.Add(new Position(x + 1, y - 1));
                        }
                        // En Passant
                        if (x > 0) {
                            otherPiece = board.EnemyAt(new Position(x - 1, y), team);
                            if (otherPiece != null && otherPiece.enPassantable)
                                result.Add(new Position(x - 1, y - 1));
                        }
                        if (x < 7) {
                            otherPiece = board.EnemyAt(new Position(x + 1, y), team);
                            if (otherPiece != null && otherPiece.enPassantable)
                                result.Add(new Position(x + 1, y - 1));
                        }
                    }
                    break;

                case Enums.PieceType.Knight:
                    if (x >= 2) {
                        if (y >= 1)
                            result.Add(new Position(x - 2, y - 1));
                        if (y <= 6)
                            result.Add(new Position(x - 2, y + 1));
                    }
                    if (x >= 1) {
                        if (y >= 2)
                            result.Add(new Position(x - 1, y - 2));
                        if (y <= 5)
                            result.Add(new Position(x - 1, y + 2));
                    }
                    if (x <= 5) {
                        if (y >= 1)
                            result.Add(new Position(x + 2, y - 1));
                        if (y <= 6)
                            result.Add(new Position(x + 2, y + 1));
                    }
                    if (x <= 6) {
                        if (y >= 2)
                            result.Add(new Position(x + 1, y - 2));
                        if (y <= 5)
                            result.Add(new Position(x + 1, y + 2));
                    }
                    break;

                case Enums.PieceType.Bishop:
                    newX = x + 1; newY = y + 1;
                    while (newX <= 7 && newY <= 7) {
                        result.Add(new Position(newX, newY));
                        newX++; newY++;
                    }
                    newX = x + 1; newY = y - 1;
                    while (newX <= 7 && newY >= 0) {
                        result.Add(new Position(newX, newY));
                        newX++; newY--;
                    }
                    newX = x - 1; newY = y + 1;
                    while (newX >= 0 && newY <= 7) {
                        result.Add(new Position(newX, newY));
                        newX--; newY++;
                    }
                    newX = x - 1; newY = y - 1;
                    while (newX >= 0 && newY >= 0) {
                        result.Add(new Position(newX, newY));
                        newX--; newY--;
                    }
                    break;

                case Enums.PieceType.Rook:
                    newX = x + 1;
                    while (newX <= 7) {
                        result.Add(new Position(newX, y));
                        newX++;
                    }
                    newX = x - 1;
                    while (newX >= 0) {
                        result.Add(new Position(newX, y));
                        newX--;
                    }
                    newY = y + 1;
                    while (newY <= 7) {
                        result.Add(new Position(x, newY));
                        newY++;
                    }
                    newY = y - 1;
                    while (newY >= 0) {
                        result.Add(new Position(x, newY));
                        newY--;
                    }
                    break;

                case Enums.PieceType.Queen:
                    newX = x + 1; newY = y + 1;
                    while (newX <= 7 && newY <= 7) {
                        result.Add(new Position(newX, newY));
                        newX++; newY++;
                    }
                    newX = x + 1; newY = y - 1;
                    while (newX <= 7 && newY >= 0) {
                        result.Add(new Position(newX, newY));
                        newX++; newY--;
                    }
                    newX = x - 1; newY = y + 1;
                    while (newX >= 0 && newY <= 7) {
                        result.Add(new Position(newX, newY));
                        newX--; newY++;
                    }
                    newX = x - 1; newY = y - 1;
                    while (newX >= 0 && newY >= 0) {
                        result.Add(new Position(newX, newY));
                        newX--; newY--;
                    }
                    newX = x + 1;
                    while (newX <= 7) {
                        result.Add(new Position(newX, y));
                        newX++;
                    }
                    newX = x - 1;
                    while (newX >= 0) {
                        result.Add(new Position(newX, y));
                        newX--;
                    }
                    newY = y + 1;
                    while (newY <= 7) {
                        result.Add(new Position(x, newY));
                        newY++;
                    }
                    newY = y - 1;
                    while (newY >= 0) {
                        result.Add(new Position(x, newY));
                        newY--;
                    }
                    break;

                case Enums.PieceType.King:
                    if (x < 7) {
                        result.Add(new Position(x + 1, y));
                        if (y > 0)
                            result.Add(new Position(x + 1, y - 1));
                    }
                    if (x > 0) {
                        result.Add(new Position(x - 1, y));
                        if (y < 7)
                            result.Add(new Position(x - 1, y + 1));
                    }
                    if (y < 7) {
                        result.Add(new Position(x, y + 1));
                        if (x < 7)
                            result.Add(new Position(x + 1, y + 1));
                    }
                    if (y > 0) {
                        result.Add(new Position(x, y - 1));
                        if (x > 0)
                            result.Add(new Position(x - 1, y - 1));
                    }
                    // Castling
                    otherPiece = board.TeammateAt(new Position(0, y), team);
                    if (!hasMoved && otherPiece != null && !otherPiece.hasMoved)
                        result.Add(new Position(x - 2, y));
                    otherPiece = board.TeammateAt(new Position(7, y), team);
                    if (!hasMoved && otherPiece != null && !otherPiece.hasMoved)
                        result.Add(new Position(x + 2, y));
                    break;
                default: throw new Exception("Bad piece type.");
            }

            return result;
        }

        override
        public bool Equals(object? o) {
            if (o is Piece p) {
                return team == p.team && pieceType == p.pieceType && x == p.x && y == p.y &&
                    hasMoved == p.hasMoved && enPassantable == p.enPassantable;
            }
            return false;
        }

        override
        public int GetHashCode() {
            int result = 17;
            result *= 23 + x.GetHashCode();
            result *= 23 + y.GetHashCode();
            result *= 23 + team.GetHashCode();
            result *= 23 + pieceType.GetHashCode();
            //result *= 23 + hasMoved.GetHashCode();
            //result *= 23 + enPassantable.GetHashCode();
            return result;
        }

        // convert to/from string for sending over the internet
        override
        public string ToString() {
            string result = "";
            result += team.ToString() + " ";
            result += pieceType.ToString() + " ";
            result += x.ToString() + " ";
            result += y.ToString() + " ";
            result += hasMoved.ToString() + " ";
            result += enPassantable.ToString();
            return result;
        }

        public static Piece? FromString(string str) {
            if (str.Equals("null"))
                return null;
            string[] pieces = str.Split(' ');
            Enums.Team team = Enum.Parse<Enums.Team>(pieces[0]);
            Enums.PieceType pieceType = Enum.Parse<Enums.PieceType>(pieces[1]);
            int x = int.Parse(pieces[2]), y = int.Parse(pieces[3]);
            bool hasMoved = bool.Parse(pieces[4]), enPassantable = bool.Parse(pieces[5]);
            return new(team, pieceType, x, y, hasMoved, enPassantable);
        }
    }
}
