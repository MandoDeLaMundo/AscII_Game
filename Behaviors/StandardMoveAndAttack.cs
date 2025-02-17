﻿using AscII_Game.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscII_Game.Behavior
{
	public class StandardMoveAttack : IBehavior
	{
		public bool Act(Monster monster, CommandSystem commandSystem)
		{
			DungeonMap dungeonMap = Game.DungeonMap;
			Player player = Game.Player;
			FieldOfView monsterFov = new FieldOfView(dungeonMap);

			// If the monster has not been alerted, compute a field-of-view
			// Use the monster's Awareness value for the distance in the FoV check
			// If the player is in the monster's FoV then alert it
			// Add a message to the MessageLof regarding this alerted status
			if (!monster.TurnsAlerted.HasValue)
			{
				monsterFoV.ComputeFoV(monster.X, monster.Y, monster.Awareness, true);
				if (monsterFoV.IsInFoV(player.X, player.Y))
				{
					Game.MessageLog.Add($"{monster.Name} is eager to fight {player.Name}");
					monster.turnsAlerted = 1;
				}
			}

			if (monster.TurnsAlerted.HasValue)
			{
				// Before we find a path, make sure to make the monster and player Cells walkable
				dungeonMap.SetIsWalkable(monster.X, monster.Y, true);
				dungeonMap.SetIsWalkable(player.X, player.Y, true);

				Pathfinder pathfinder = new Pathfinder(dungeonMap);
				Path path = null;

				try
				{
					path = pathfinder.ShortestPath(
					dungeonMap.GetCell(monster.X, monster.Y),
					dungeonMap.GetCell(player.X, player.Y));
				}
				catch (PathNotFoundException)
				{
					// The monster can see the player, but cannot find a path to him
					// This could be due to other monsters blocking the way
					// Add a message to the message log that the monster is witing
					Game.MessageLog.Add($"{monster.Name} waits for a turn");
				}

				// Don't forget to set the walkable satatu back to false
				dungeonMap.SetIsWalkable(monster.X, monster.Y, false);
				dungeonMap.SetIsWalkable(player.X, player.Y, false);

				// In the case that there was a path, tell the CommandSystem to move the monster
				if (path != null)
				{
					try
					{
						// TODO: This should be path.StepForward() but there is a bug in RogueSharp V3
						// The bug is that a Path returned from a PathFinder does not include the source Cell
						commandSystem.MoveMonster(monster, path.Steps.First());
					}
					catch (NoMoreStepsException)
					{
						Game.MessageLog.Add($"{monster.Name} growls in frustration");
					}
				}

				monster.TurnsAlerted++;

				// Lose alerted status every 15 turns.
				// As long as the player is still in FoV the monster will stay alert
				// Otherwise the monster will quit chasin the player.
				if (monster.TurnsAlerted > 15)
				{
					monster.TurnsAlerted = null;
				}
			}
			return true;
		}
	}

}