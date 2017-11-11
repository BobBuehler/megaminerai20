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

        public Random Rand;
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
            return "Catastrophe C# Player"; // REPLACE THIS WITH YOUR TEAM NAME!
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

            this.Rand = new Random();
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

            // Output the current turn of the game
            Console.WriteLine($"TURN #{this.Game.CurrentTurn}");

            // Grab all of your units
            IList<Unit> units = this.Player.Units;

            // Grab one of your shelters
            Structure myShelter = null;
            foreach (Structure structure in this.Player.Structures)
            {
                if (structure.Type == "shelter")
                {
                    myShelter = structure;
                    break;
                }
            }

            // Loop through all of your units & define what to do for each of the different unit types
            foreach (Unit myUnit in units)
            {
                // Display info about current unit
                Console.WriteLine($"Current Unit: {myUnit.Id}, Job Title: {myUnit.Job.Title}");

                if (myUnit.Job.Title == "fresh human")
                {
                    // Check if the Unit can move to its shelter
                    if (myShelter != null && myUnit.Moves > 0)
                    {
                        // Try to find a path to shelter
                        List<Tile> path = this.FindPath(myUnit.Tile, myShelter.Tile);

                        // if there is a path, move along it
                        //   - length 0 means no path could be found to the tile or the unit is
                        //     top of the shelther
                        //   - length 1 means the unit is adjacent to the shelter
                        if (path.Count > 1)
                        {
                            if (myUnit.Move(path[0]))
                            {
                                Console.WriteLine($"Moving {myUnit} to {path[0]}");
                            }
                        }
                    }

                    // Create a job list that doesn't include the cat overlord or fresh human
                    // Note: This uses C#'s LINQ. The equivalent foreach loop is below for reference, but it's worth looking into LINQ.
                    List<string> myJobs = (from job in this.Game.Jobs
                                           where job.Title != "cat overlord" && job.Title != "fresh human"
                                           select job.Title).ToList();

                    /* Using a foreach loop:
                    List<string> myJobs = new List<string>();
                    foreach (var job in this.Game.Jobs)
                    {
                        if (job.Title != "cat overlord" && job.Title != "fresh human")
                        {
                            myJobs.Add(job.Title);
                        }
                    }
                    */

                    // Grab the neighbors of the current Unit
                    List<Tile> myNeighbors = myUnit.Tile.GetNeighbors();
                    // Add the tile that the unit is standing on in case it is on top of a shelter
                    myNeighbors.Add(myUnit.Tile);

                    // Check for a shelter in my_neighbors
                    if (this.CheckForShelter(myNeighbors))
                    {
                        // Try to change the job of the fresh human to a random job from myJobs
                        if (myUnit.ChangeJob(myJobs[this.Rand.Next(myJobs.Count)]))
                        {
                            Console.WriteLine($"Successfully changed {myUnit} to Job '{myUnit.Job.Title}'");
                        }
                    }
                }
                else if (myUnit.Job.Title == "soldier")
                {
                    // Grab the enemy Units
                    IList<Unit> enemyUnits = this.Player.Opponent.Units;

                    // Create a path variable for use below
                    List<Tile> path = null;

                    // Check if the Unit has enough energy to attack
                    if (myUnit.Energy > myUnit.Job.ActionCost)
                    {
                        if (enemyUnits.Any()) // Equivalent to enemyUnits.Count > 0
                        {
                            // Define a path to the first enemy unit.
                            path = this.FindPath(myUnit.Tile, enemyUnits[0].Tile);
                        }
                        else
                        {
                            // There is no unit to path to, so use an empty path instead
                            path = new List<Tile>();
                        }

                        // Try to move the soldier towards the enemy
                        if (path.Count > 1)
                        {
                            if (myUnit.Move(path[0]))
                            {
                                Console.WriteLine($"Moving {myUnit} to {path[0]}");
                            }
                        }

                        // If the soldier is right next to the enemy
                        else if (path.Count == 1)
                        {
                            // Make sure the unit hasn't already acted
                            if (!myUnit.Acted)
                            {
                                if (myUnit.Attack(path[0]))
                                {
                                    Console.WriteLine($"{myUnit} is attacking {enemyUnits[0]}");
                                }
                            }
                        }
                    }

                    // Move the unit back to a shelter to recharge
                    else
                    {
                        // Make sure a shelter exists
                        if (myShelter != null)
                        {
                            path = this.FindPath(myUnit.Tile, myShelter.Tile);

                            // Try to move the soldier towards the shelter
                            if (path.Count > 1)
                            {
                                if (myUnit.Move(path[0]))
                                {
                                    Console.WriteLine($"Moving {myUnit} to {path[0]}");
                                }
                            }

                            // If the unit isn't trying to move to the shelter, have it try to recharge (rest)
                            else
                            {
                                // Grab the neighbors of the current Unit
                                List<Tile> myNeighbors = myUnit.Tile.GetNeighbors();
                                // Add the tile that the unit is standing on in case it is on top of a shelter
                                myNeighbors.Add(myUnit.Tile);

                                // Check for a shelter in the units neighbors
                                if (this.CheckForShelter(myNeighbors))
                                {
                                    // Try to recharge (rest)
                                    if (myUnit.Rest())
                                    {
                                        Console.WriteLine($"{myUnit} is resting");
                                    }
                                }
                            }
                        }
                    }
                }
                else if (myUnit.Job.Title == "gatherer")
                {
                    // Grab the neighbors of the current Unit
                    List<Tile> myNeighbors = myUnit.Tile.GetNeighbors();
                    // Add the tile that the unit is standing on in case it is on top of a food resource
                    myNeighbors.Add(myUnit.Tile);

                    // Loop through the neighboring tiles
                    foreach (Tile neighbor in myNeighbors)
                    {
                        // Check if food can be harvested from the neighboring tile
                        if (neighbor.HarvestRate > 0 && neighbor.TurnsToHarvest == 0)
                        {
                            // Attempt to harvest food
                            if (myUnit.Harvest(neighbor))
                            {
                                Console.WriteLine($"{myUnit} harvested food from {neighbor}");
                                break;
                            }
                        }
                    }

                    // Check if the unit has any food
                    if (myUnit.Food > 0)
                    {
                        // Try to drop all your food because, you know, who needs it? Why did you even harvest it?
                        if (myUnit.Drop(myUnit.Tile, "food", 0))
                        {
                            Console.WriteLine($"{myUnit} dropped their food on {myUnit.Tile}");
                        }
                    }
                }
                else if (myUnit.Job.Title == "builder")
                {
                    // Grab the neighbors of the current Unit
                    List<Tile> myNeighbors = myUnit.Tile.GetNeighbors();

                    // Loop through the neighboring tiles
                    foreach (Tile neighbor in myNeighbors)
                    {
                        // Check for materials on a neighboring tile
                        if (neighbor.Materials > 0)
                        {
                            // Try to pickup as many materials as possible
                            if (myUnit.Pickup(neighbor, "materials"))
                            {
                                Console.WriteLine($"{myUnit} is picking up materials on {neighbor}");
                            }
                        }
                    }

                    // Loop through the neighboring tiles
                    foreach (Tile neighbor in myNeighbors)
                    {
                        // Check for a neutral structure nearby
                        if (neighbor.Structure != null && neighbor.Structure.Type == "neutral")
                        {
                            // Attempt to deconstruct the neutral structure
                            if (myUnit.Deconstruct(neighbor))
                            {
                                Console.WriteLine($"{myUnit} deconstructed a Structure on {neighbor}");
                            }
                        }
                    }

                    // See if the builder can build a wall
                    if (myUnit.Materials >= 75)
                    {
                        // Loop through the neighboring tiles
                        foreach (Tile neighbor in myNeighbors)
                        {
                            // Check if any of the surrounding tiles are open to build something on
                            if (neighbor.Structure == null && neighbor.Unit == null)
                            {
                                // Attempt to contruct wall
                                if (myUnit.Construct(neighbor, "wall"))
                                {
                                    Console.WriteLine($"{myUnit} constructed a wall on {neighbor}");
                                }
                            }
                        }
                    }
                }
                else if (myUnit.Job.Title == "missionary")
                {
                    // Grab the neighbors of the current Unit
                    List<Tile> myNeighbors = myUnit.Tile.GetNeighbors();

                    // Loop through neighboring tiles
                    foreach (Tile neighbor in myNeighbors)
                    {
                        // Find a neutral fresh human
                        if (neighbor.Unit != null && neighbor.Unit.Job.Title == "fresh human" && neighbor.Unit.Owner == null)
                        {
                            // Attempts to convert neutral fresh human
                            if (myUnit.Convert(neighbor))
                            {
                                Console.WriteLine($"{myUnit} converted {neighbor.Unit} to a friendly fresh human");
                            }
                        }
                    }
                }
                else //(myUnit.Job.Title == "cat overlord")
                {
                    // stand there and look pretty?
                }

                // Used for spacing between unit output
                Console.WriteLine();
            }

            // Used for spacing between output for each turn
            Console.Write("\n\n\n");

            return true;
            // <<-- /Creer-Merge: runTurn -->>
        }

        /// <summary>
        /// A very basic path finding algorithm (Breadth First Search) that when given a starting Tile, will return a valid path to the goal Tile.
        /// </summary>
        /// <remarks>
        /// This is NOT an optimal pathfinding algorithm. It is intended as a stepping stone if you want to improve it.
        /// </remarks>
        /// <param name="start">the starting Tile</param>
        /// <param name="goal">the goal Tile</param>
        /// <returns>A list of Tiles representing the path where the first element is a valid adjacent Tile to the start, and the last element is the goal. Or an empty list if no path found.</returns>
        List<Tile> FindPath(Tile start, Tile goal)
        {
            // no need to make a path to here...
            if (start == goal)
            {
                return new List<Tile>();
            }

            // the tiles that will have their neighbors searched for 'goal'
            Queue<Tile> fringe = new Queue<Tile>();

            // How we got to each tile that went into the fringe.
            Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();

            // Enqueue start as the first tile to have its neighbors searched.
            fringe.Enqueue(start);

            // keep exploring neighbors of neighbors... until there are no more.
            while (fringe.Any())
            {
                // the tile we are currently exploring.
                Tile inspect = fringe.Dequeue();

                // cycle through the tile's neighbors.
                foreach (Tile neighbor in inspect.GetNeighbors())
                {
                    if (neighbor == goal)
                    {
                        // Follow the path backward starting at the goal and return it.
                        List<Tile> path = new List<Tile>();
                        path.Add(goal);

                        // Starting at the tile we are currently at, insert them retracing our steps till we get to the starting tile
                        for (Tile step = inspect; step != start; step = cameFrom[step])
                        {
                            path.Insert(0, step);
                        }

                        return path;
                    }

                    // if the tile exists, has not been explored or added to the fringe yet, and it is pathable
                    if (neighbor != null && !cameFrom.ContainsKey(neighbor) && neighbor.IsPathable())
                    {
                        // add it to the tiles to be explored and add where it came from.
                        fringe.Enqueue(neighbor);
                        cameFrom.Add(neighbor, inspect);
                    }

                } // foreach(neighbor)

            } // while(fringe not empty)

            // if you're here, that means that there was not a path to get to where you want to go.
            //   in that case, we'll just return an empty path.
            return new List<Tile>();
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

            /* Using foreach loop (equivalent code):
            // Loop through neighbors of the unit
            foreach (Tile neighbor in myNeighbors)
            {
                // Check and see if the unit is next to one of its shelters
                if (neighbor.Structure != null && neighbor.Structure.Type == "shelter" && neighbor.Structure.Owner == this.Player)
                {
                    // Shelter found
                    return true;
                }
            }

            // No shelter found
            return false;
            */
        }
        // <<-- /Creer-Merge: methods -->>
        #endregion
    }
}
