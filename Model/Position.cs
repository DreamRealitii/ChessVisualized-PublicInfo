using System.Diagnostics.CodeAnalysis;

namespace ChessVisualized.Model {
    // 2D position on 8x8 chess board.
    // White starts on bottom end.
    // (0, 0) is bottom left.
    // X = Column, Y = Row.
    public class Position {
        private int x, y;
        public Position(int x, int y) {
            SetPosition(x, y);
        }

        public int X { get { return x; } }
        public int Y { get { return y; } }

        public void SetPosition(int newX, int newY) {
            if (newX < 0)
                throw new Exception("X Position outside left bound");
            if (newX > 7)
                throw new Exception("X Position outside right bound");
            if (newY < 0)
                throw new Exception("Y Position outside lower bound");
            if (newY > 7)
                throw new Exception("Y Position outside upper bound");
            x = newX;
            y = newY;
        }

        override
        public bool Equals(object? otherObject) {
            if (otherObject is Position p)
                return x == p.x && y == p.y;
            return false;
        }

        override
        public int GetHashCode() {
            return x + (8 * y);
        }

        // convert to/from string for sending over the internet
        override
        public string ToString() {
            return x.ToString() + " " + y.ToString();
        }

        public string ToUIString() {
            char x = 'u';
            switch (X) {
                case 0: x = 'a'; break;
                case 1: x = 'b'; break;
                case 2: x = 'c'; break;
                case 3: x = 'd'; break;
                case 4: x = 'e'; break;
                case 5: x = 'f'; break;
                case 6: x = 'g'; break;
                case 7: x = 'h'; break;
            }
            string y = (Y + 1).ToString();
            return x.ToString() + y;
        }

        public static Position FromString(string str) {
            string[] coordinates = str.Split(" ");
            int x = int.Parse(coordinates[0]), y = int.Parse(coordinates[1]);
            return new Position(x, y);
        }

        public class PositionComparer : Comparer<Position> {
            public override int Compare(Position? a, Position? b) {
                if (a == null || b == null)
                    return 0;
                if (a.x == b.x)
                    return a.y - b.y;
                return a.x - b.x;
            }
        }
    }
}
