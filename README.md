# Procedural Dungeon Generator — Algorithms & Data Structures

A procedural dungeon generation system built in **Unity (C#)**, focused on applying classical algorithms and data structures: recursive spatial partitioning, graph connectivity, BFS, Marching Squares, and Flood Fill.

## Overview

Generation begins with a single `RectInt` of configurable size, which is recursively split — horizontally or vertically at random — until no partition can be divided further without falling below the minimum room size. Each resulting leaf room is added to an adjacency graph (`Graph<RectInt>`) with its centre as a node and shared doors as edges. Once the full graph is built, the 10% smallest rooms (by area) are removed one at a time: before each removal is committed, a **BFS connectivity check** traverses the graph from any node and compares the discovered count against the total node count. If removal would disconnect the dungeon, the room and its door nodes are restored and the operation is skipped — guaranteeing the dungeon always remains fully traversable.

The spatial data is then written into a 2D integer `tileMap` (0 = floor, 1 = wall), with door positions overwriting their boundary cells back to floor. Wall tiles are instantiated using **Marching Squares**: each 2×2 neighbourhood of the tilemap is encoded as a 4-bit integer (values 1–14), selecting the correct wall prefab from a 16-element array. Floor tiles are filled using a **BFS-based Flood Fill** starting from the centre of the first room, spreading to all 4-connected floor neighbours while a `HashSet<Vector3>` prevents duplicate instantiation. The entire generation pipeline is animated step-by-step using Unity coroutines and visual debug drawing, making the algorithm behaviour observable at runtime.

## Tech Stack

| | |
|---|---|
| **Engine** | Unity (C#) |
| **Algorithms** | Recursive BSP · BFS · Flood Fill · Marching Squares |
| **Data Structures** | Graph, Queue, HashSet, Dictionary, 2D array |
| **Libraries** | Unity NavMesh, NaughtyAttributes |

## Key Systems

| Method | Algorithm | Role |
|---|---|---|
| `Splitting()` | Recursive BSP | Divides the dungeon `RectInt` into leaf rooms |
| `RemoveRooms()` | Graph + sort | Removes 10% smallest rooms if connectivity is preserved |
| `CheckConnectivity()` | BFS | Validates full graph connection before/after removal |
| `MarchinSquares()` | Marching Squares | Encodes 2×2 tilemap cells as 4-bit codes to place wall prefabs |
| `FloodFill()` / `TryFlood()` | BFS Flood Fill | Spreads from room centre to instantiate all reachable floor tiles |
| `BuildTileMap()` | 2D array | Writes rooms and doors into the integer grid |

---

*Unity · C# · Procedural Generation · BSP · BFS · Marching Squares · Flood Fill*
