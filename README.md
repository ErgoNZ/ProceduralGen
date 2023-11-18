# ProceduralGen
This repository is mainly just for my prefab procedural generation experiments. Currently, there are 2 experiments here for you to look at.
## World Generation.cs  
Has impossible spawns for rooms that can never be reached
## RoomGeneration.cs 
### 9/11/2023 
It doesn't give all of the rooms that were requested (Request 50 rooms but only get 35 instead)
### 10/11/2023
Now gives all rooms requested. However, T-shaped rooms have their z-axis facing nodes are randomly disappearing? This leaves massive gaps in the map.
### 13/11/2023
I think the node that was being deleted on the T-shape prefab was bugged and for some reason would be placed at world spawn instead of its proper position. A simple delete and replace of that node seems to have fixed the issue.
