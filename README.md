# Ruins-of-Ghelinnal-City
A side-scroller hybrid RPG in homage to classic FF games, and using lore from a home-brew D&D campaign.

Current builds are supplied in the Builds folder, and scripts and their support resources (csv files) are in the Assets folder. Other assets, such as images, audio, prefabs, scenes, are not shared.

Feel free to browse! Thanks for stopping by.

## The Story
A hero has wandered through the Briarwood forest and stumbled into the ruins of Ghelinnal City. Once inhabited by the elves, it was leveled in a magical catastrophe known as The Cataclysm. A remnant of elves, now blessed with werebear lycanthrope by their patron, Sehanine Moonbow, hold out in the center of the city against goblin and gnoll attacks. Though their might held fast for centuries, it has begun to wane, and the werebears are left weakened and dwindling in number. Ridara, the werebear high priest, suspects trouble down in the catacombs beneath the temple, and asks the hero to investigate. Calling upon the aid of the werebears and other adventurers hired for the task, the hero delves into the labyrinth below, discovering entire worlds and riches beneath.

## Gameplay
The player assembles a party of up to four heroes, and outfits them with consumable potions and tomes as well as weapons and armor. A side-scrolling dungeon level is entered, where the player can maneuver an avatar around obstacles and enemies. If an enemy is encountered, it triggers a battle between the hero party and a group of enemies. Victory in battle yields gold, though the party will sustain injury in the process. The objective of each dungeon level is to defeat the powerful boss at its end, but doing so requires multiple trips into the dungeons to earn gold for upgrade purchases in the shop and to increase the size of the party. Completing a dungeon unlocks additional stock in the shop, the next dungeon, and additional lore for the story. Complete all seven dungeons to win the game, unlocking the final pieces of the story (as well as unlocking unlimited items from the shop).

## Refactoring To-Do
* Create a generic Panel and PanelMonitor that can be used and reused for highlighting and selecting throughout all menus.
  * Build off work done with PartyMenuPanel and PanelSelection
  * An InvPanelMonitor and InvPanel would inherit and would have children for: BattleInventory, EquipMenu, InventoryMenu, Shop.
  * A Slot would inherit from this, and be used for FileMenu and the Dungeon choosing scene.
  * A Tab version would exist for Help and Talk menus.
  * Modify prefabs to accomodate. This would give the game a consistent UX.
* Create a generic Name/HP/MP/Sliders object that works in battle UIs as well as menus.
* Create a Battler (or Combatant?) class that stores BattleID, HP, MP, IsDead, and similar variables.
  * Make BattleHero and BattleEnemy inherit from this.
  * Use Battler for most situations that presently rely on two sets of similar code.
* Replace SpriteSwap EventTimer-based pseudoanimations with animations
* Replace some EventTimers and other events with Coroutines.
* Scan for timers that are created but are not destroyed.
* Move Game-level objects from the static BattleLoader to a more central location.
  * Reduce BattleLoader to its core function: facilitating the transition between DungeonLevel scenes and Battle scenes.
* In DamageDisplay, roll the function of the travelTimer into the multiHitTimer.
* Add the EnemyDeath component at runtime when a death begins, rather than from the beginning. Use a coroutine instead of Update().
* Reduce redundancies in DamageDisplay and HeroDamageDisplay.
* Use a struct instead of parellel lists in TurnCounter.
* Consider a LinkedList instead of a List in TurnCounter.
* Convert generic objects to Battler objects to reduce redundancies
* Incorporate BattleInventory into current TurnCounter and DamageDisplay systems, to remove the TurnInvoker and unique events.
* Break ConfigData into parent and child classes, so that every ConfigData object does not contain all three Dictionarys.
* Make a Enemy_Dungeon_FX_Data container class, instead of using a List<object>, for exporting and importing.
* Improve HeroMovement, especially walking down slopes.
* Reduce the number of unique Events.
* Rework the EventManager to use generic and/or annonymous Interfaces instead of a block of code for each event.
* Make a master Menu object for all prefab menus to inherit from.
  * Base off the very simple About menu.
  * Would contain the very common Click_Close() method.
  * Would contain fields for who spawned it, and who it spawned. (or see below)
* Give MenuManager a LinkedList or Stack to track which menu prefabs are overlapping its predecessors.
  * Rather than individual menu objects tracking who preceded it and using methods to activate/deactive from menu to menu, use the MenuManager to track this, so individual menus don't have to track this.
* Give the Help menu data, rather than rely on the editor.
* Finish typing out monster data for use as defaults.
* Add a cheat in the shop to increase StockLevel and DungeonLevelAccess incrementally, to test balance throughout the game.

<!-- end of the list -->

## Features To-Do
Leave the previous list, doggonit
- More character-specific abilites and spells.
  - Make heroes targetable by abilities (Ex. Cure) and potions.
- More unique monster abilities and AI.
  - Start with monster healing.
- System for unlocking hero abilities.
- A TurnCounter system that incorporates Agility in the turn order.
  - Turns shown by a modified icon queue (similar to FF X).
  - Turn position advanced or delayed by ability used (similar to FFT, FF X).
- Gamepad support.
- More special FX.
- More stuff to do in dungeon levels besides move and jump.
  - More hazards, powerups.
  - Arrows, boulders, bombs, other interactables.
- Animated backgrounds for menus and dungeons, werebear and town artwork.
- Implement tooltips.
- Possibly display data about an item before purchase. It is requested, but I kind of like the gamble.
- HUD display in dungeon of hero's condition.

<!-- end of the list -->

## Dream Big To-Do
* Hire an artist to create actual tile and sprite assets, rather than borrow from FF VI.
* Hire a composer to create actual music, rather than borrow from FF VI.
* Use currently-owned and invest in more Unity Asset packs for audio clips, rather than borrow from FF VI.
* Expand the game 10x.
* Publish the game on a public platform (Steam).
* Branch out to consoles.
* Make a little pocket change?
