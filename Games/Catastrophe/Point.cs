using System;
using System.Collections.Generic;
using System.Linq;

namespace Joueur.cs.Games.Catastrophe
{
    public struct Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            Point o = (Point)obj;
            return o.x == x && o.y == y;
        }

        public override int GetHashCode()
        {
            int result = x;
            result = 31 * result + y;
            return result;
        }

        public override string ToString()
        {
            return String.Format("({0},{1})", x, y);
        }
    }

    public static class PointExtensions
    {
        public static Point ToPoint(this Tile tile)
        {
            return new Point(tile.X, tile.Y);
        }
        public static Point ToPoint(this Unit unit)
        {
            return unit.Tile.ToPoint();
        }

        public static Tile ToTile(this Point point)
        {
            return AI.GAME.Tiles[point.x + point.y * AI.GAME.MapWidth];
        }

        public static Unit ToUnit(this Point point)
        {
            return AI.GAME.Tiles[point.x + point.y * AI.GAME.MapWidth].Unit;
        }

        public static bool IsOnBoard(this Point point)
        {
            return point.x >= 0 && point.y >= 0 && point.x < AI.GAME.MapWidth && point.y < AI.GAME.MapHeight;
        }

        public static IEnumerable<Point> GetSquareNeighbors(this Point point)
        {
            var points = new[]
            {
                new Point(point.x - 1, point.y - 1),
                new Point(point.x - 1, point.y),
                new Point(point.x - 1, point.y + 1),
                new Point(point.x, point.y - 1),
                new Point(point.x, point.y + 1),
                new Point(point.x + 1, point.y - 1),
                new Point(point.x + 1, point.y),
                new Point(point.x + 1, point.y + 1),
            };
            return points.Where(p => p.IsOnBoard());
        }
    }
}
