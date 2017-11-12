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

        public static void MoveAndRest(Unit unit)
        {
            var shelters = unit.Owner.Structures.Where(s => s.Type == "shelter");
            Act.Move(unit, shelters.SelectMany(s => s.Tile.GetNeighbors()));
            // And
            if (unit.CanRest())
            {
                unit.Rest();
            }
        }

        public static void MoveAndRestAndAttack(Unit unit, IEnumerable<Tile> targets)
        {
            if (!unit.HasEnergyToAct())
            {
                MoveAndRest(unit);
            }
            else
            {
                Act.Move(unit, targets.SelectMany(t => t.GetNeighbors()));
                Act.Attack(unit, targets);
            }
        }

        public static void MoveAndRestAndDeconstruct(Unit unit, IEnumerable<Tile> targets)
        {
            if (!unit.HasEnergyToAct())
            {
                MoveAndRest(unit);
            }
            else
            {
                Act.Move(unit, targets.SelectMany(t => t.GetNeighbors()));
                Act.Deconstruct(unit, targets);
            }
        }

        public static void MoveAndRestAndConstruct(Unit unit, IEnumerable<Tile> targets, string type)
        {
            if (!unit.HasEnergyToAct())
            {
                MoveAndRest(unit);
            }
            else
            {
                Act.Move(unit, targets.SelectMany(t => t.GetNeighbors().Concat(new[] { t })));
                Act.Construct(unit, targets, type);
            }
        }
    }
}
