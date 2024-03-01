# Aberration

## Day Progress Logs

### Day 1
*build 0.01.0*
- Setup default 3D URP project with git.
- Brainstormed game ideas and chose one that I liked.
- Created simple gray box scene with ProBuilder.
- Implemented basic controllers for playable rovers and security camera.
- Added some manager classes.
- Performed first build test.

### Day 2
*build 0.02.0*
- Replaced rover tank tread placeholders with 2-bone IK legs.
- Implemented physics-based controller for rovers.
  - Supports walking on uneven terrain and inclines.
  - Needs more work to fix bugs and look more natural, but otherwise feels nice.


## TODO
1. [ ] Swap entities using numbers 1-n
2. [x] Custom rover physics for traveling on ground and up ramps
3. [ ] Security camera zoom in/out


## Ideas

### Setup
A disaster has struck a containment facility housing all sort of dangerous anomalies. You are part of a group of survivors who are trying to escape. You have access to a rover drone that you use to scout ahead to clear sections of the facility so the survivors can pass through.

### Gameplay
- Swap between different drones/cameras.
- Carefully navigate the corridors to avoid/corral enemies.
- Find a valid path to the end for your survivors to follow.
- Two game modes:
  - Drone explore & clear.
  - Watch survivors try to follow your path out.
- Number of survivors can act as your "health".
- Drones can be killed by enemies?
- Flagging hazards for survivors?
- Notice irregularities to find where anomalies are.

### Game Views / Playable Entities
- Rover Drone
- Quadcopter
- Security Camera (power required)
- Map View

### Interactable Elements
- Doors
- Alarms (lure)
- Lights
- Lifts (requires two drones?)
