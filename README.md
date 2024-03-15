# Formative 04: Procedural Generation for a RogueLite

I haven't been able to work as much as I would have liked (because of illnesses and all that stuff) but here's what is planned:

## Three layers of Procedural Generation:

### 1. Overworld Map/Island type selection
 * Sources:
   * [Map Generation in Slay the Spire](https://steamcommunity.com/sharedfiles/filedetails/?id=2830078257) by steam user jomami
   * [Slay The Spire Style Map in Unity Package: How To](https://www.youtube.com/watch?v=P9ogBkLWmPQ) by Gamedev Journey
 * I have not done this one yet, I would like to try and make my own version rather than just taking an already made one

### 2. Islands (IslandGenerator Scene - can generate from the Inspector)
 * Sources: 
	 * [[Unity] Procedural Cave Generation (E01. Cellular Automata)](https://www.youtube.com/watch?v=v7yyZZjF1z4) by Sebastian Lague
   * [RANDOM MAP GENERATOR - EASY UNITY TUTORIAL](https://www.youtube.com/watch?v=D4EOgZyNk-k&t=1s) by Blackthornprod
   * [Procedural generation of 2D maps in Unity](https://pavcreations.com/procedural-generation-of-2d-maps-in-unity/) by PavCreations
 * The layout is done, islands are procedurally generated using Cellular Automata, then split into two tilemaps: one for the land, that will be walkable and one for the sea, that will not be walkable
 * TODO: Tweak the random tiles placement, probably using some sort of WFC; using the random option from ruletiles does not look great
 * TODO: "Decorate" islands, place landmarks, interactive tiles and dungeon entrances
 * (MAYBE: make procedurally generated treasure maps & associated islands (have a treasure placed at a certain place relative to a landmark and the map say so accordingly), but that will probably come last, if I have time)

### 3. Dungeon Generation (DungeonGenerator Scene - need to press play for it to generate the map)
 * Sources:
	 * [Procedural Generation in Unity (Tutorial)](https://www.youtube.com/watch?v=nADIYwgKHv4) by Nebula Coding
   * [An introduction to procedural lock and key dungeon generation](https://www.youtube.com/watch?v=BM_4Z27d4rI) by The Shaggy Dev
   * [Lock and Key Dungeons](https://www.boristhebrave.com/2021/02/27/lock-and-key-dungeons/) by BorisTheBrave
   * [Procedural Dungeon Generator in Unity [TUTORIAL]](https://www.youtube.com/watch?v=gHU5RQWbmWE) by SilverlyBee
 * Prototype is done
 * TODO: Replace the sprites with actual rooms
 * TODO: Give each room a value according to how far from the starting point it is (like in a Tree Graph)
 * TODO: Dungeons will follow a simple lock & key model (e.g. The Legend of Zelda(1986)). The furthest room will be assigned the Boss or Exit room type, second furthest that is at the end of a path could be a boss key, etc.
 * TODO: Room value will also correspond to a difficulty score that enemy generation will be based on, to have an increase in difficulty over the course of the dungeon
