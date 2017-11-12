using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Joueur.cs.Games.Catastrophe
{
    public static class Solver
    {
        public static IEnumerable<Point> GetPath(IEnumerable<Point> starts, Func<Point, bool> isGoal)
        {
            var search = new AStar<Point>(
                starts,
                isGoal,
                (a, b) => 1,
                p => 0,
                p => p.ToTile().GetNeighbors().Where(n => n.IsPathable()).Select(t => t.ToPoint())
            );
            return search.Path;
        }

        public static int GetTurnCountToMove(Unit unit, Func<Point, bool> isGoal)
        {
            var search = new AStar<Point>(
                new[] { unit.ToPoint() },
                isGoal,
                (a, b) => 1,
                p => 0,
                p => p.ToTile().GetNeighbors().Where(n => n.IsPathable()).Select(t => t.ToPoint())
            );
            return (int)Math.Ceiling((search.Path.Count() - 2) / (double)unit.Job.Moves);
        }

        public static Tuple<Unit, Point> GetNearestPair(IEnumerable<Unit> units, Func<Point, bool> isGoal)
        {
            var path = GetPath(units.Select(u => u.ToPoint()), isGoal);
            if (path.Count() == 0)
            {
                return null;
            }
            return Tuple.Create(path.First().ToUnit(), path.Last());
        }

        public static void MoveAndRest(Unit unit, double neededEnergy = 100)
        {
            if (unit.Energy >= neededEnergy)
            {
                return;
            }
            var shelters = AI.GetStructures(unit.Owner, "shelter");
            Act.Move(unit, shelters.SelectMany(s => s.Tile.GetSquareNeighbors()));
            // And
            if (unit.CanRest())
            {
                unit.Rest();
            }
        }

        public static void MoveToAndRest(Unit unit, IEnumerable<Point> targets, double neededEnergy = 100)
        {
            if (unit.Energy >= neededEnergy)
            {
                return;
            }
            var shelters = AI.GetStructures(unit.Owner, "shelter");
            Act.Move(unit, shelters.SelectMany(s => s.Tile.GetSquareNeighbors()));
            // And
            if (unit.CanRest())
            {
                unit.Rest();
            }
        }

        public static void MoveAndRestAndChangeJob(Unit unit, Job job)
        {
            MoveAndRest(unit, 100);
            Act.Move(unit, unit.Owner.Cat.Tile.GetSquareNeighbors());
            if (unit.CanChangeJob(job))
            {
                unit.ChangeJob(job.Title);
            }
        }

        public static void MoveAndRestAndAttack(Unit unit, IEnumerable<Tile> targets)
        {
            MoveAndRest(unit, unit.GetActionCost());
            MoveAndAttack(unit, targets);
        }

        public static void MoveAndAttack(Unit unit, IEnumerable<Tile> targets)
        {
            Act.Move(unit, targets.SelectMany(t => t.GetNeighbors()));
            Act.Attack(unit, targets);
        }

        public static void MoveAndRestAndConstruct(Unit unit, IEnumerable<Tile> targets, string type)
        {
            MoveAndRest(unit, unit.GetActionCost());
            Act.Move(unit, targets.SelectMany(t => t.GetNeighbors().Concat(t)));
            Act.Construct(unit, targets, type);
        }

        public static void MoveAndRestAndConvert(Unit unit, IEnumerable<Unit> targets)
        {
            MoveAndRest(unit, unit.GetActionCost());
            Act.Move(unit, targets.SelectMany(t => t.Tile.GetNeighbors()));
            Act.Convert(unit, targets);
        }

        public static void MoveAndRestAndDeconstruct(Unit unit, IEnumerable<Tile> targets)
        {
            MoveAndRest(unit, unit.GetActionCost());
            Act.Move(unit, targets.SelectMany(t => t.GetNeighbors()));
            Act.Deconstruct(unit, targets);
        }

        public static void MoveAndRestAndDrop(Unit unit, IEnumerable<Tile> targets, string resource)
        {
            Act.Move(unit, targets.SelectMany(t => t.GetNeighbors().Concat(t)));
            Act.Drop(unit, targets, resource);
        }

        public static void MoveAndRestAndHarvest(Unit unit, IEnumerable<Tile> targets)
        {
            MoveAndRest(unit, unit.GetActionCost());
            Act.Move(unit, targets.SelectMany(t => t.GetNeighbors().Concat(t)));
            Act.Harvest(unit, targets);
        }
    }
}
