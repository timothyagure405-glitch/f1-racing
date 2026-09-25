<<<<<<< HEAD
# Unity F1 Racing Game Prototype

This project is a Unity-ready, lightweight F1-inspired racing prototype built around:

- realistic acceleration and braking with engine RPM and gear selection
- AI opponent that follows checkpoints and competes for pace
- DRS (Drag Reduction System) activation and drag reduction
- advanced controls including steering, braking, traction, and handbrake behavior

## Folder layout

- `Assets/Scripts/Vehicle/CarController.cs` - player racing physics and controls
- `Assets/Scripts/Vehicle/AICarController.cs` - AI logic for opponent cars
- `Assets/Scripts/Vehicle/DRSSystem.cs` - DRS activation and drag reduction
- `Assets/Scripts/Track/WaypointPath.cs` - waypoint system for AI pathing

## How to use in Unity

1. Create a new Unity 3D project or open this folder as the project root.
2. In a new scene, create:
   - a `Plane` or `Road` object for the track
   - a `Cube` or `Capsule` car body parented to a `Rigidbody`
   - four `WheelCollider` objects (front left, front right, rear left, rear right)
3. Attach `CarController` to the player car GameObject.
4. Attach `AICarController` and `DRSSystem` to the AI or player car as needed.
5. Create a `WaypointPath` object with child transforms as track markers for AI.
6. Press Play and use:
   - `W` / `S` for throttle and brake
   - `A` / `D` for steering
   - `Space` for handbrake
   - `Left Ctrl` to toggle DRS when the speed threshold is reached

## Notes

This is a gameplay prototype rather than a full production-grade racing sim. It gives you a strong foundation for a Unity F1-style game and is designed to be expanded with:

- full wheel meshes and suspension tuning
- lap timing and leaderboard logic
- track collisions and wall penalties
- HUD, minimap, and camera follow
- more advanced tire model and slip angle simulation
=======
# f1-racing
Realistic F1 racing game with online weekly leagues
>>>>>>> a4fc2ac9c2fc4dc061ff295a556ae28cef30979e
