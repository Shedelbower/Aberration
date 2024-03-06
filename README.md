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

### Day 3
*build 0.03.0*
- Created a more interesting gray box level (doorways and rooms).
- Prototyped cloaking visual effect on meshes.
- Implemented signal networks with interactable switches.
  - Includes power network support.

### Day 4
*build 0.04.1*
- Modeled simple legs and body for quadruped drone.
- Started major refactor of quadruped physics and IK.
  - Attempted to more accurately simulate the quadruped, but this turned out to be a dead end due to the difficulty of making stable physics-based movement that is fun to control while also fully simulated.
- Most of the day was probably a waist :(

### Day 5
*build 0.05.0*
- Reworked quadruped physics and movement to be more stable and fun, at the cost of being less physically accurate.
- Improved procedural animations of quadruped to be more nuanced and realistic.
  - Still need to cleanup code before replacing original Rover classes.

### Day 6
*build 0.06.1*
- More tweaks to quadruped movement and IK.
- Added playable "human" character.
  - Set as the default entity.
  - Controls other entities via a tablet device.
  - Supports walking, sprinting, jumping, gravity, and click-to-interact.
  - Implemented basic camera smoothing, which is particularly useful on stairs.
- Other entities now target the tablet render texture.
  - Each entity can have a custom post-processing volume.
- Overall a productive day :)

### Day 7
*build 0.07.1*
- Tweaks to player and quadruped movement.
- Tablet lift/lower animation.
- Initial UI setup.
- Didn't have much time this evening, so didn't make much progress.



## TODO
1. [x] Swap entities using numbers 1-n
2. [x] Custom rover physics for traveling on ground and up ramps
3. [ ] Security camera zoom in/out

### Rover
1. [ ] Bonk into wall/player (add rotational impulse and/or bounce)
2. [ ] Add auto follow mode to follow player when human.
3. [ ] Add directional consideration to step threshold to lower sideways distance when turning
4. [ ] Use speed of should point for speed calculations to account for turning while standing still
5. [ ] Align lower and upper leg segments so they don't clip.
6. [ ] Press, hold, and release space to wind up a jump?

## Assets

### Fonts
- Google Fonts
  - [Kode Mono](https://fonts.google.com/specimen/Kode+Mono)

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
