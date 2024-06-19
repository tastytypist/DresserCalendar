﻿/*
 * DresserCalendar - Access your calendar behind a furniture in-game.
 * Copyright (C) 2024-present tastytypist
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace DresserCalendar;

/// <summary>The mod entry point.</summary>
internal sealed class ModEntry : Mod
{
    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        // Hook game events.
        helper.Events.Input.ButtonPressed += OnButtonPressed;
    }

    /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        // Check whether
        // 1. a game has been loaded and the player is able to interact,
        // 2. the player is currently in their farmhouse, and
        // 3. the `action` button is pressed.
        if (Context.IsPlayerFree && Game1.player.currentLocation is FarmHouse && e.Button.IsActionButton())
        {
            // Find all calendars in the farmhouse.
            foreach (Furniture furniture in Game1.player.currentLocation.furniture)
            {
                if (furniture.QualifiedItemId == "(F)1402") // Calendar item id
                {
                    Vector2 playerLocation = new Vector2(Game1.player.StandingPixel.X, Game1.player.StandingPixel.Y);
                    Vector2 cursorLocation = e.Cursor.AbsolutePixels;
                    Monitor.LogOnce($"Calendar found, boundingBox: {furniture.boundingBox.Value}", LogLevel.Debug);
                    // Check whether
                    // 1. the cursor is hovering over the calendar, and
                    // 2. there is a tile between the player and the calendar.
                    if (
                        furniture.boundingBox.Value.Contains((int)cursorLocation.X, (int)cursorLocation.Y)
                        && furniture.boundingBox.Value.Contains((int)playerLocation.X, (int)(playerLocation.Y - 4 * 64f))
                    )
                    {
                        // Supress the action button to anticipate an interactable item is in front of the player.
                        Helper.Input.Suppress(e.Button);
                        // Open the calendar.
                        Game1.activeClickableMenu = new Billboard();
                        Monitor.Log($"Player location: {playerLocation}", LogLevel.Debug);
                        Monitor.Log($"Cursor location: {cursorLocation}", LogLevel.Debug);
                        Monitor.Log("Calendar showed", LogLevel.Info);
                    }
                }
            }
        }
    }
}
