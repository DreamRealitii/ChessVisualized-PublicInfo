using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVisualized.Model {
    public class Enums {
        public enum Team { Black, White }
        public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King }
        public enum GameMode { PhysicalLocal, Bot, OnlineLocal, OnlineMultiplayer }
    }
}
