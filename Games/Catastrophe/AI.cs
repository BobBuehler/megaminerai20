// This is where you build your AI for the Catastrophe game.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// <<-- Creer-Merge: usings -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
// you can add additional using(s) here
// <<-- /Creer-Merge: usings -->>

namespace Joueur.cs.Games.Catastrophe
{
    /// <summary>
    /// This is where you build your AI for Catastrophe.
    /// </summary>
    public class AI : BaseAI
    {
        #region Properties
        #pragma warning disable 0169 // the never assigned warnings between here are incorrect. We set it for you via reflection. So these will remove it from the Error List.
        #pragma warning disable 0649
        /// <summary>
        /// This is the Game object itself. It contains all the information about the current game.
        /// </summary>
        public readonly Game Game;
        /// <summary>
        /// This is your AI's player. It contains all the information about your player's state.
        /// </summary>
        public readonly Player Player;
        #pragma warning restore 0169
        #pragma warning restore 0649

        // <<-- Creer-Merge: properties -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add additional properties here for your AI to use

        public static Game GAME;
        public static Player US;
        public static Player THEM;

        public static Dictionary<string, Job> JOBS;
        public static Job CAT_OVERLORD;
        public static Job SOLDIER;
        public static Job GATHERER;
        public static Job BUILDER;
        public static Job MISSIONARY;
        public static Job FRESH_HUMAN;

        public static HashSet<string> STRUCTURES;
        public static HashSet<string> BUILDABLE_STRUCTURES;
        public static Dictionary<string, int> STRUCTURE_COSTS;

        public static string FOOD = "food";
        public static string MATERIAL = "material";

        public static Point[] SPAWN_POINTS;

        // <<-- /Creer-Merge: properties -->>
        #endregion


        #region Methods
        /// <summary>
        /// This returns your AI's name to the game server. Just replace the string.
        /// </summary>
        /// <returns>Your AI's name</returns>
        public override string GetName()
        {
            // <<-- Creer-Merge: get-name -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            return "The Litter Box"; // REPLACE THIS WITH YOUR TEAM NAME!
            // <<-- /Creer-Merge: get-name -->>
        }

        /// <summary>
        /// This is automatically called when the game first starts, once the Game and all GameObjects have been initialized, but before any players do anything.
        /// </summary>
        /// <remarks>
        /// This is a good place to initialize any variables you add to your AI or start tracking game objects.
        /// </remarks>
        public override void Start()
        {
            // <<-- Creer-Merge: start -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.Start();

            AI.GAME = this.Game;
            AI.US = this.Player;
            AI.THEM = this.Game.Players.First(p => p != this.Player);

            AI.JOBS = this.Game.Jobs.ToDictionary(j => j.Title);
            AI.CAT_OVERLORD = AI.JOBS["cat overlord"];
            AI.SOLDIER = AI.JOBS["soldier"];
            AI.GATHERER = AI.JOBS["gatherer"];
            AI.BUILDER = AI.JOBS["builder"];
            AI.MISSIONARY = AI.JOBS["missionary"];
            AI.FRESH_HUMAN = AI.JOBS["fresh human"];

            AI.STRUCTURES = new HashSet<string> { "wall", "shelter", "monument", "neutral", "road" };
            AI.BUILDABLE_STRUCTURES = new HashSet<string> { "wall", "shelter", "monument" };
            AI.STRUCTURE_COSTS = new Dictionary<string, int>
            {
                ["wall"] = AI.GAME.WallMaterials,
                ["shelter"] = AI.GAME.ShelterMaterials,
                ["monument"] = AI.GAME.MonumentMaterials,
                ["neutral"] = AI.GAME.NeutralMaterials,
                ["road"] = 0
            };

            SPAWN_POINTS = new Point[] { new Point(0, (AI.GAME.MapHeight - 1) / 2), new Point(AI.GAME.MapWidth - 1, AI.GAME.MapHeight / 2) };
            // <<-- /Creer-Merge: start -->>
        }

        /// <summary>
        /// This is automatically called every time the game (or anything in it) updates.
        /// </summary>
        /// <remarks>
        /// If a function you call triggers an update, this will be called before that function returns.
        /// </remarks>
        public override void GameUpdated()
        {
            // <<-- Creer-Merge: game-updated -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.GameUpdated();
            // <<-- /Creer-Merge: game-updated -->>
        }

        /// <summary>
        /// This is automatically called when the game ends.
        /// </summary>
        /// <remarks>
        /// You can do any cleanup of you AI here, or do custom logging. After this function returns, the application will close.
        /// </remarks>
        /// <param name="won">True if your player won, false otherwise</param>
        /// <param name="reason">A string explaining why you won or lost</param>
        public override void Ended(bool won, string reason)
        {
            // <<-- Creer-Merge: ended -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            base.Ended(won, reason);
            // <<-- /Creer-Merge: ended -->>
        }


        /// <summary>
        /// This is called every time it is this AI.player's turn.
        /// </summary>
        /// <returns>Represents if you want to end your turn. True means end your turn, False means to keep your turn going and re-call this function.</returns>
        public bool RunTurn()
        {
            // <<-- Creer-Merge: runTurn -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
            // Put your game logic here for runTurn
            
            Console.WriteLine($"TURN #{this.Game.CurrentTurn}");
            this.BobStrat();

            Console.WriteLine(String.Join("", GetUnits(AI.US).Select(u => u.Job.Title[0]).OrderBy(c => c)));
            Console.WriteLine(String.Join("", GetUnits(AI.THEM).Select(u => u.Job.Title[0]).OrderBy(c => c)));

            return true;
            // <<-- /Creer-Merge: runTurn -->>
        }

        public void Uproot()
        {
            // Spawn one build and rest soldiers
            var builder = AI.US.Units.FirstOrDefault(u => u.Job == AI.BUILDER);
            if (builder == null)
            {
                builder = AI.US.Units.FirstOrDefault(u => u.CanChangeJob(AI.BUILDER));
                if (builder != null)
                {
                    builder.ChangeJob(AI.BUILDER.Title);
                }
            }
            AI.US.Units.Where(u => u != builder && u.CanChangeJob(AI.SOLDIER)).ForEach(f => f.ChangeJob(AI.SOLDIER.Title));


            // Choose new home tile in a corner
            var newHome = Act.GetCorners().Where(c => c.ToTile().IsPathable() || c.Equals(AI.US.Cat.ToPoint())).MinByValue(c => c.ManhattanDistance(AI.US.Cat.ToPoint()));

            // Move Overlord to new home
            Act.Move(AI.US.Cat, new[] { newHome.ToTile() });

            // Move and deconstruct if builder has < 50 material
            if (builder != null && builder.Materials < AI.STRUCTURE_COSTS["shelter"])
            {
                Solver.MoveAndRestAndDeconstruct(builder, AI.GAME.Tiles.Where(t => Act.CanDeconstruct(builder, t, false)));
            }

            // Move and construct if builder has >= material
            if (builder != null && builder.Materials >= AI.STRUCTURE_COSTS["shelter"])
            {
                Solver.MoveAndRestAndConstruct(builder, new[] { newHome.ToTile() }, "shelter");
            }

            if (newHome.ToTile().Structure != null)
            {
                this.SoldierZerg();
            }

            // Move soldiers to adj adj around new home if no shelter
            // Move units to sq adj around new home if shelter done
            // Change builder to soldier if shelter done
        }

        public void ZergBlock()
        {
            AI.US.Units.Where(u => u.CanChangeJob(AI.GATHERER)).ForEach(f => f.ChangeJob(AI.GATHERER.Title));
            AI.US.Units.Where(u => u.Job == AI.GATHERER).ForEach(s => Act.Move(s, AI.US.Cat.Tile.GetNeighbors()));
            AI.US.Units.Where(u => u.Job == AI.GATHERER).ForEach(s => s.Rest());
        }

        public void SoldierZerg()
        {
            AI.US.Units.Where(u => u.CanChangeJob(AI.SOLDIER)).ForEach(f => f.ChangeJob(AI.SOLDIER.Title));
            AI.US.Units.Where(u => u.Job == AI.SOLDIER).ForEach(s => Solver.MoveAndRestAndAttack(s, AI.THEM.Units.Select(u => u.Tile)));
        }

        public void BobStrat()
        {
            this.BobJobs();

            this.BobMissionaries();
            this.BobSoldiers();
            this.BobBuilders();
            this.BobGatherers();

            GetUnits(AI.US).ForEach(u => Solver.MoveAndRest(u));
        }

        public void BobJobs()
        {
            var desiredCats = new List<Job>
            {
                AI.MISSIONARY,
                AI.GATHERER,
                AI.SOLDIER,
                AI.BUILDER,
                AI.SOLDIER,
                AI.MISSIONARY,
                AI.SOLDIER,
                AI.SOLDIER,
                AI.MISSIONARY,
                AI.GATHERER,
                AI.SOLDIER, AI.SOLDIER, AI.SOLDIER, AI.SOLDIER, AI.SOLDIER,
                AI.SOLDIER, AI.SOLDIER, AI.SOLDIER, AI.SOLDIER, AI.SOLDIER,
                AI.SOLDIER, AI.SOLDIER, AI.SOLDIER, AI.SOLDIER, AI.SOLDIER,
                AI.SOLDIER, AI.SOLDIER, AI.SOLDIER, AI.SOLDIER, AI.SOLDIER,
                AI.SOLDIER, AI.SOLDIER, AI.SOLDIER, AI.SOLDIER, AI.SOLDIER
            };

            if (GetUnits(AI.THEM, AI.SOLDIER).Count(s => s.ToPoint().IsInStepRange(AI.US.Cat.ToPoint(), 12)) > 1)
            {
                Console.WriteLine("MILITARIZE");
                desiredCats.Insert(0, AI.SOLDIER);
                desiredCats.Insert(0, AI.SOLDIER);
            }

            var haveCats = GetUnits(AI.US).Where(c => c != this.Player.Cat).ToList();
            var slottedCats = new List<Unit>();
            foreach (var d in desiredCats)
            {
                var index = haveCats.FindIndex(u => u.Job == d);
                if (index != -1)
                {
                    slottedCats.Add(haveCats[index]);
                    haveCats.RemoveAt(index);
                }
                else
                {
                    slottedCats.Add(null);
                }
            }
            slottedCats.AddRange(haveCats);
            for (int i = 0; i < slottedCats.Count; ++i)
            {
                if (slottedCats[i] == null)
                {
                    var takeIndex = slottedCats.FindLastIndex(u => u != null);
                    if (takeIndex > i)
                    {
                        slottedCats[i] = slottedCats[takeIndex];
                        slottedCats.RemoveAt(takeIndex);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
            for (int i = 0; i < slottedCats.Count; ++i)
            {
                var c = slottedCats[i];
                if (c != null && c.Job != desiredCats[i])
                {
                    Solver.MoveAndRestAndChangeJob(c, desiredCats[i]);
                }
            }
        }

        public void BobMissionaries()
        {
            // var first = Solver.GetNearestPair(GetUnits(AI.US, AI.MISSIONARY), g => AI.SPAWN_POINTS.Any(s => g.IsInStepRange(s, AI.MISSIONARY.Moves + 1)));
            GetUnits(AI.US, AI.MISSIONARY).ForEach(u => Solver.MoveAndRestAndConvert(u, AI.GAME.Units.Where(n => u.CanConvert(n, false))));
        }

        public void BobSoldiers()
        {
            if (GetUnits(AI.THEM, AI.SOLDIER).Count(s => s.ToPoint().IsInStepRange(AI.US.Cat.ToPoint(), 8)) > 1)
            {
                Console.WriteLine("DEFEND");
                var first = Solver.GetNearestPair(GetUnits(AI.US, AI.SOLDIER), g => g.IsInStepRange(AI.US.Cat.ToPoint(), 1));
                if (first != null)
                {
                    Act.Move(first.Item1, AI.US.Cat.Tile.GetNeighbors());
                    var second = Solver.GetNearestPair(GetUnits(AI.US, AI.SOLDIER).Where(s => s != first.Item1), g => g.IsInStepRange(AI.US.Cat.ToPoint(), 1));
                    if (second != null)
                    {
                        Act.Move(second.Item1, AI.US.Cat.Tile.GetNeighbors());
                    }
                }
            }

            GetUnits(AI.US, AI.SOLDIER).ForEach(u =>
            {
                if (Act.GetRegenAmount(u) > 0 && u.Energy < 90)
                {
                    u.Rest();
                }
                else if (u.Energy < 50)
                {
                    Solver.MoveAndRest(u);
                }
                else
                {
                    Solver.MoveAndAttack(u, GetUnits(AI.THEM).Select(e => e.Tile));
                    if (u.Owner == AI.US) // if not dead
                    {
                        Solver.MoveAndAttack(u, GetStructures(AI.THEM).Select(e => e.Tile));
                    }
                }
            });
        }

        public void BobBuilders()
        {
            // TODO: Needs better targets. Maybe put it closer to opposite sides.
            var shelterSites = AI.GAME.Tiles.Where(t => !t.GetNeighbors().Any(n => n.Structure != null && n.Structure.Type == "shelter") && t.GetNeighbors().Any(n => n.Structure != null && n.Structure.Type == "road"));

            // any tile along road, that is 2 distance away from other shelter site
            var shelterSitesV2 = AI.GAME.Tiles.Where(t =>
                // All structureless tiles along road
                t.GetNeighbors().Any(n => n.Structure != null && n.Structure.Type == "road")
                &&
                // Within 2 distance of a shelter
                !AI.GetStructures(AI.US, "shelter").Any(s => s.Tile.ToPoint().IsInSquareRange(t.ToPoint(), 2))
            );

            GetUnits(AI.US, AI.BUILDER).ForEach(b => Solver.MoveAndRestAndConstruct(b, shelterSitesV2.Where(t => Act.CanConstruct(b, t, "shelter", false)), "shelter"));
            GetUnits(AI.US, AI.BUILDER).Where(b => b.Materials < AI.STRUCTURE_COSTS["shelter"]).ForEach(u => Solver.MoveAndRestAndDeconstruct(u, AI.GAME.Tiles.Where(t => u.CanDeconstruct(t, false))));
        }

        public void BobGatherers()
        {
            GetUnits(AI.US, AI.GATHERER).ForEach(g => Solver.MoveAndRestAndHarvest(g, AI.GAME.Tiles.Where(t => Act.CanHarvest(g, t, 2, false))));
            GetUnits(AI.US, AI.GATHERER).Where(g => g.Food > 0).ForEach(u => Solver.MoveAndRestAndDrop(u, GetStructures(AI.US, "shelter").Select(s => s.Tile), AI.FOOD));
        }

        public static IEnumerable<Structure> GetStructures(Player player, string type = null)
        {
            return AI.GAME.Structures.Where(s => s.Owner == player && s.Tile != null && (type == null || s.Type == type));
        }

        public static IEnumerable<Unit> GetUnits(Player player, Job job = null)
        {
            return AI.GAME.Units.Where(u => u.Owner == player && (job == null || u.Job == job));
        }

        // <<-- Creer-Merge: methods -->> - Code you add between this comment and the end comment will be preserved between Creer re-runs.
        // you can add additional methods here for your AI to call

        /// <summary>
        /// Checks if there's a friendly shelter in myNeighbors
        /// </summary>
        /// <param name="myNeighbors">The tiles to check</param>
        /// <returns>True if a friendly shelter was found, false otherwise</returns>
        public bool CheckForShelter(IEnumerable<Tile> myNeighbors)
        {
            // Using .Any(), lambda expression, and null-propagation (?. operator):
            return myNeighbors.Any(neighbor => neighbor.Structure?.Type == "shelter" && neighbor.Structure.Owner == this.Player);
        }
        // <<-- /Creer-Merge: methods -->>
        #endregion
    }
}