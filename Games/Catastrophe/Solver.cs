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

        public static Unit GetNearest(IEnumerable<Unit> units, Func<Point, bool> isGoal)
        {
            return units.MinByValue(u => GetTurnCountToMove(u, isGoal));
        }

        public static void MoveAndRest(Unit unit, double neededEnergy = 100)
        {
            if (unit.Energy >= neededEnergy)
            {
                return;
            }
            var shelters = unit.Owner.Structures.Where(s => s.Type == "shelter");
            Act.Move(unit, shelters.SelectMany(s => s.Tile.GetNeighbors()));
            // And
            if (unit.CanRest())
            {
                unit.Rest();
            }
        }

        public static void MoveAndRestAndChangeJob(Unit unit, Job job)
        {
            MoveAndRest(unit, 100);
            Act.Move(unit, unit.Owner.Cat.Tile.GetNeighbors());
            if (unit.CanChangeJob(job))
            {
                unit.ChangeJob(job.Title);
            }
        }

        public static void MoveAndRestAndAttack(Unit unit, IEnumerable<Tile> targets)
        {
            MoveAndRest(unit, unit.GetActionCost());
            Act.Move(unit, targets.SelectMany(t => t.GetNeighbors()));
            Act.Attack(unit, targets);
        }

        public static void MoveAndRestAndDeconstruct(Unit unit, IEnumerable<Tile> targets)
        {
            MoveAndRest(unit, unit.GetActionCost());
            Act.Move(unit, targets.SelectMany(t => t.GetNeighbors()));
            Act.Deconstruct(unit, targets);
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
    }
}
