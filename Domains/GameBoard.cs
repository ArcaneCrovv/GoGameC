using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace WindowsFormsApp2.Domains
{
    public class GameBoard
    {
        public GameBoard(int size)
        {
            Size = size;
            Board = new BoardColor[size, size];
            for (var i = 0; i < size; i++)
            for (var j = 0; j < size; j++)
                Board[i, j] = BoardColor.Liberty;
        }
        public int Size { get; set; }
        public BoardColor[,] Board { get; set; }
        
        public event Action<int, int, BoardColor> BoardStateChanged;

        public Dictionary<BoardColor, int> CountTerritories()
        {
            var territories = new Dictionary<BoardColor, int>
            {
                {BoardColor.Black, 0},
                {BoardColor.White, 0}
            };
            var added = new HashSet<Point>();
            
            for (var i = 0; i < Size; i++)
            for (var j = 0; j < Size; j++)
            {
                var currentPoint = new Point(i, j);
                if (StoneColorOn(currentPoint) != BoardColor.Liberty || added.Contains(currentPoint)) 
                    continue;
                
                var toAdd = TerritoryOwnerAndScore(currentPoint);
                if (toAdd.Item1 != BoardColor.Liberty)
                    territories[toAdd.Item1] += toAdd.Item2.Count;
                added.UnionWith(toAdd.Item2);
            }

            return territories;
        }

        public ValueTuple<BoardColor, HashSet<Point>> TerritoryOwnerAndScore(Point libertyPoint)
        {
            var owners = new HashSet<BoardColor>();
            var territoryPoints = new HashSet<Point>();
            foreach (var point in StoneGroupWithPoint(libertyPoint))
            {
                foreach (var nearPoint in NearPoints(point))
                {
                    if (StoneColorOn(nearPoint) != BoardColor.Liberty)
                        owners.Add(StoneColorOn(nearPoint));
                }

                territoryPoints.Add(point);
            }

            return (FindOwner(owners), territoryPoints);
        }

        private BoardColor FindOwner(HashSet<BoardColor> owners)
        {
            return owners.Count == 1 ? owners.First() : BoardColor.Liberty;
        }
        
        public int? SetStoneNullIfCant(Point point, BoardColor color)
        {
            if (StoneColorOn(point) != BoardColor.Liberty) return null;
            
            ChangeColorOn(point, color);
            var score = TryKillAround(point);
            if (score > 0) return score;

            if (IsGroupAlive(point))
                return 0;
            
            ChangeColorOn(point, BoardColor.Liberty);
            return null;
        }

        public int TryKillAround(Point point)
        {
            if (StoneColorOn(point) == BoardColor.Liberty)
                throw new ArgumentException();

            var score = EnemyStonesNear(point)
                .Where(enemy => !IsGroupAlive(enemy))
                .Sum(KillGroupReturnScore);

            return score;
        }

        public IEnumerable<Point> EnemyStonesNear(Point point)
        {
            if (StoneColorOn(point) == BoardColor.Liberty)
                throw new ArgumentException();

            var enemyColor = StoneColorOn(point) == BoardColor.Black ? BoardColor.White : BoardColor.Black;
            foreach (var enemyPoint in NearPoints(point)
                .Where(nearPoint => StoneColorOn(nearPoint) == enemyColor))
            {
                yield return enemyPoint;
            }
        }

        public int KillGroupReturnScore(Point point)
        {
            var killedStones = 0;
            foreach (var stone in StoneGroupWithPoint(point))
            {
                ChangeColorOn(stone, BoardColor.Liberty);
                killedStones++;
            }

            return killedStones;
        }
        
        public bool IsGroupAlive(Point pointInGroup)
        {
            if (Board[pointInGroup.X, pointInGroup.Y] == BoardColor.Liberty)
                throw new ArgumentException();

            return StoneGroupWithPoint(pointInGroup)
                .SelectMany(NearPoints)
                .Any(nearPoint => StoneColorOn(nearPoint) == BoardColor.Liberty);
        }

        public IEnumerable<Point> StoneGroupWithPoint(Point point)
        {
            return StoneGroupWithPoint(point, Board[point.X, point.Y]);
        }
        
        public IEnumerable<Point> StoneGroupWithPoint(Point point, BoardColor groupColor)
        {
            var checkedHash = new HashSet<Point>();
            var toCheck = new Queue<Point>();
            toCheck.Enqueue(point);
            checkedHash.Add(point);
            while (toCheck.Count != 0)
            {
                var currentPoint = toCheck.Dequeue();
                yield return currentPoint;
                foreach (var nearPoint in NearPoints(currentPoint))
                {
                    if (Board[nearPoint.X, nearPoint.Y] == groupColor
                        && !checkedHash.Contains(nearPoint))
                        toCheck.Enqueue(nearPoint);
                    
                    checkedHash.Add(nearPoint);
                }
            }
        }
        
        public IEnumerable<Point> NearPoints(Point point)
        {
            var shifts = new List<Point>
            {
                new Point(0, -1),
                new Point(-1, 0),
                new Point(1, 0),
                new Point(0, 1)
            };

            foreach (var shift in shifts)
            {
                var newPoint = new Point(point.X + shift.X, point.Y + shift.Y);
                if (IsPointInBordBounds(newPoint))
                    yield return newPoint;
            }
        }

        public BoardColor StoneColorOn(int x, int y)
        {
            return Board[x, y];
        }
        
        public BoardColor StoneColorOn(Point point)
        {
            return Board[point.X, point.Y];
        }

        public void ChangeColorOn(int x, int y, BoardColor color)
        {
            ChangeColorOn(new Point(x, y), color);
        }
        
        public void ChangeColorOn(Point point, BoardColor color)
        {
            Board[point.X, point.Y] = color;
            if (BoardStateChanged != null) 
                BoardStateChanged(point.X, point.Y, color);
        }
        
        public bool IsPointInBordBounds(Point point)
        {
            return Size > point.X && point.X >= 0 && Size > point.Y && point.Y >= 0;
        }
    }
}
