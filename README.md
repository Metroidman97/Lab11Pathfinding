# Lab11Pathfinding

##How does each pathfinding algorithms calculate and prioritize paths?
GBF just tries to get as close as possible to the goal and changes direction when it can't, Dijkstra is GBF but it uses a weighted scale, and A* is Dijkstra with a heuristic function to find the shortest route.

##What challenges arise when dynamically updating obstacles in real-time?
Making it so the algorithm can adjust its path in an efficient and optimized manner, as generating paths can be resource intensive.

##Which algorithm should you choose and how should you adapt it for larger grids or open-world settings?
GBF and optimized with a Hierarchical system to essentially break the map into different levels/scales and do each one at a time.

##What would your approach be if you were to add weighted cells (e.g., "difficult terrain" areas)?
A dynamic pathfinding system that would update if the target actually entered the difficult terrain, to determine if the agent should traverse the terrain or simply go around it.
