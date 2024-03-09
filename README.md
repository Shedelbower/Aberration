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

### Day 8
*build 0.08.0*
- Support dynamic syncing of playable entities to tablet device.
- Added emergency lights that turn on when the power is off.

### Day 9
*build 0.09.0*
- Added powered doors with interactable switches.
  - Have a nice cancellable animation.
  - Will detect any obstructions and refuse to close.
- Other minor changes.
- Didn't have much time this evening, but the doors feel nice.




## TODO

### Level Design
1. [x] Replace point lights with much more efficient spot lights
2. [ ] Find textures to replace gray box

### Player
1. [x] Add small spot light around player (tablet light?)
   1. [ ] Sample pixels of screen for immersion?
2. [ ] Crouch toggle (same button as quadruped)
3. [ ] Lower collider to hit quadrupeds when standing
4. [ ] Add health UI (screen effect)
5. [ ] Add death state (animation + game over screen)

### Enemy
1. [ ] Enemy controller
2. [ ] Navigate to player
3. [ ] Roam ability
4. [ ] Static screen effect (shader on render texture)
5. [ ] Cloak warping effect for player visual?
 
### Rover
1. [x] IK Arm
   1. [x] Interact with objects
   2. [ ] Use time-based lerping for a nicer animation
2. [ ] Fix IK leg issues
3. [ ] Add auto flip correction when turned over

### Navigation
1. [ ] Develop navigation system for quadruped and enemies using navmesh
2. [ ] Only follow if visible
3. [ ] Use bread crumbs to last visible position?
4. [ ] Should use ray casts for physics validity
5. [ ] Automatic State System for Quadruped
   1. [ ] In Route Mode
   2. [ ] Can't find target
   3. [ ] Reached target

### UI
1. [ ] Level name fade text
2. [ ] Interactable hover text + basic info (powered/open/synced)
3. [ ] Icons
   1. [ ] Entity icons
   2. [ ] Power On/Off icon
   3. [ ] Switch open/closed unpowered icon
4. [ ] Entity selection (inventory bar style)
5. [ ] Customized HUD for each entity type
6. [ ] Broken glass overlay for integrity (3 stages?)
7. [ ] UI Input Icons from pack

#### Rover HUD
1. [ ] Name
2. [ ] Night vision
3. [ ] Techno Info
   1. [ ] Speed
   2. [ ] Tilt / Roll / Yaw
   3. [ ] Integrity
4. [ ] Auto mode
   1. Navigating...
   2. Reached target
   3. Searching for target
5. [ ] Need human assistance
6. [ ] Kill screen
7. Collision warning (player or wall)
8. Is interacting

#### Security Camera HUD
1. [ ] Name
2. [ ] Night vision
3. [ ] Angle
4. [ ] Zoom level
5. [ ] Kill screen

### Audio
1. [ ] Audio Manager (singleton with pooled audio sources)
   1. [ ] Play at position
   2. [ ] Attach sounds to transforms
2. [ ] Quadruped pitch movement sound by speed?
3. Find assets on freesound
   1. [ ] Lights on/off
   2. [ ] Rover move
   3. [ ] Rover step
   4. [ ] Player step
   5. [ ] Door open / close
   6. [ ] Fuse box interact
   7. [ ] Switch open/close
   8. [ ] Ambient
   9. [ ] Rumble/boom for incident start
   9. [ ] Tablet startup
   10. [ ] Tablet UI buttons
   11. [ ] Menu buttons

### Menus
1. [ ] Start screen
   1. [ ] Level selection
   2. [ ] Credits / About
   3. [ ] Quit
2. [ ] Pause menu
   1. [ ] Restart level
   2. [ ] Return to main menu
   3. [ ] Quit Game
   4. [ ] Resume
   5. [ ] View Controls
3. [ ] Game Over Menu
   1. [ ] Restart level
   2. [ ] Return to main menu
4. [ ] Next Level Menu
   1. [ ] Next level
   2. [ ] Return to main menu

### Tablet
1. [ ] Add tablet syncing animation / delay
2. [ ] Add startup loading screen
3. [ ] Fix animation parent movement stutter
4. [ ] Bonk to interact

### Doors
1. [ ] Add vertical sliding door that only quadruped can crawl under
2. [x] Add multiple check colliders, one for each side of door.
3. [ ] Damage/push entities when shutting on them?

### Juice
1. [ ] Add player camera shake
2. [ ] Light fades on and off 
3. [ ] Dust particles

### Lift Interactable
1. [ ] Raise/lower with switch
   2. [ ] Work with physics objects on top. Make super slow?

### Alarm Interactable
1. [ ] Turn on/off with switch
2. [ ] Attract enemies
3. [ ] Loud sound
4. [ ] Warning lights


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
