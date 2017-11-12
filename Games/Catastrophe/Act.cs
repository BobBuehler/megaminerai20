using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Joueur.cs.Games.Catastrophe
{
    public static class Act
    {

        public static double GetActionCost(this Unit unit)
        {
            if (unit.Owner.Structures.Any(s => s.Type == "monument" && s.Tile.ToPoint().IsInSquareRange(unit.ToPoint(), s.EffectRadius)))
            {
                return AI.GAME.MonumentCostMult * unit.Job.ActionCost;
            }
            return unit.Job.ActionCost;
        }

        public static bool HasEnergyToAct(this Unit unit)
        {
            return unit.Energy >= unit.GetActionCost();
        }

        public static bool CanAct(this Unit unit)
        {
            return !unit.Acted && unit.HasEnergyToAct();
        }

        public static bool CanAttack(this Unit unit, Tile target, bool checkRange = true)
        {
            if (unit.Job != AI.SOLDIER || !unit.CanAct())
            {
                return false;
            }

            if (target.Structure != null && target.Structure.Type != "road")
            {
            }
            else if (target.Unit != null)
            {
                if (target.Unit.Owner == unit.Owner)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (checkRange && !unit.ToPoint().IsInStepRange(target.ToPoint(), 1))
            {
                return false;
            }

            return true;
        }

        public static bool CanChangeJob(this Unit unit, Job job)
        {
            return !unit.Acted && unit.Job != AI.CAT_OVERLORD && job != AI.CAT_OVERLORD && unit.Job != job && unit.Energy >= 100 && unit.ToPoint().IsInSquareRange(unit.Owner.Cat.ToPoint(), 1);
        }

        public static bool CanConstruct(this Unit unit, Tile target, string type, bool checkRange = true)
        {
            if (unit.Job != AI.BUILDER || !unit.CanAct())
            {
                return false;
            }

            if (target.Structure != null)
            {
                return false;
            }

            if (!AI.BUILDABLE_STRUCTURES.Contains(type))
            {
                return false;
            }

            if (target.Unit != null && type != "shelter")
            {
                return false;
            }

            if (unit.Materials + target.Materials < AI.STRUCTURE_COSTS[type])
            {
                return false;
            }

            if (checkRange && !unit.ToPoint().IsInStepRange(target.ToPoint(), 1))
            {
                return false;
            }

            return true;
        }

        public static bool CanConvert(this Unit unit, Unit target, bool checkRange = true)
        {
            if (unit.Job != AI.MISSIONARY || !unit.CanAct())
            {
                return false;
            }

            if (target.Owner == unit.Owner)
            {
                return false;
            }

            if (checkRange && !unit.ToPoint().IsInStepRange(target.ToPoint(), 1))
            {
                return false;
            }

            return true;
        }

        public static bool CanDeconstruct(this Unit unit, Tile target, bool checkRange = true)
        {
            if (unit.Job != AI.BUILDER || !unit.CanAct())
            {
                return false;
            }

            if (target.Structure == null || target.Structure.Type == "road")
            {
                return false;
            }

            if (target.Structure.Owner == unit.Owner)
            {
                return false;
            }

            if (unit.Materials + unit.Food >= unit.Job.CarryLimit)
            {
                return false;
            }

            if (checkRange && !unit.ToPoint().IsInStepRange(target.ToPoint(), 1))
            {
                return false;
            }

            return true;
        }

        public static bool CanDrop(this Unit unit, Tile target, string resource, bool checkRange = true)
        {
            if (target.Structure != null)
            {
                if (target.Structure.Type == "shelter")
                {
                    if (target.Structure.Owner != unit.Owner)
                    {
                        return false;
                    }
                    else if (resource != AI.FOOD)
                    {
                        return false;
                    }
                }
                else if (target.Structure.Type != "road")
                {
                    return false;
                }
            }

            if (checkRange && !unit.ToPoint().IsInStepRange(target.ToPoint(), 1))
            {
                return false;
            }

            return true;
        }

        public static bool CanMove(this Unit unit, Tile target = null, bool checkRange = false)
        {
            if (unit.Moves < 1)
            {
                return false;
            }

            if (target != null && !target.IsPathable())
            {
                return false;
            }

            if (checkRange && !unit.ToPoint().IsInStepRange(target.ToPoint(), 1))
            {
                return false;
            }

            return true;
        }

        public static bool CanRest(this Unit unit)
        {
            if (unit.Acted)
            {
                return false;
            }

            if (unit.Energy >= 100)
            {
                return false;
            }

            if (!unit.Tile.GetNeighbors().Any(n => n.Structure != null && n.Structure.Owner == unit.Owner && n.Structure.Type == "shelter"))
            {
                return false;
            }

            return true;
        }

        public static void Move(Unit unit, IEnumerable<Tile> targets)
        {
            if (unit.CanMove())
            {
                var goals = new HashSet<Point>(targets.Select(t => t.ToPoint()));
                var path = Solver.GetPath(new[] { unit.ToPoint() }, p => goals.Contains(p)).ToArray();
                for (int i = 1; i < path.Length && unit.CanMove(); i++)
                {
                    unit.Move(path[i].ToTile());
                }
            }
        }

        public static void Attack(Unit unit, IEnumerable<Tile> targets)
        {
            var target = targets.FirstOrDefault(t => unit.CanAttack(t));
            if (target != null)
            {
                unit.Attack(target);
            }
        }

        public static void Construct(Unit unit, IEnumerable<Tile> targets, string type)
        {
            var target = targets.FirstOrDefault(t => unit.CanConstruct(t, type));
            if (target != null)
            {
                unit.Drop(target, AI.MATERIAL, AI.STRUCTURE_COSTS[type] - target.Materials);
                unit.Construct(target, type);
            }
        }

        public static void Deconstruct(Unit unit, IEnumerable<Tile> targets)
        {
            var target = targets.FirstOrDefault(t => unit.CanDeconstruct(t));
            if (target != null)
            {
                unit.Deconstruct(target);
            }
        }

        public static IEnumerable<Point> GetCorners()
        {
            yield return new Point(0, 0);
            yield return new Point(0, AI.GAME.MapHeight - 1);
            yield return new Point(AI.GAME.MapWidth - 1, AI.GAME.MapHeight - 1);
            yield return new Point(AI.GAME.MapWidth - 1, 0);
        }
    }
}