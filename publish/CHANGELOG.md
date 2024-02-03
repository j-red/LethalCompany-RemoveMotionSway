### v1.2.3

Fixed issue with config FOV options being applied incorrectly and console log spam error when in the terminal menu.

### v1.2.2

Added configuration settings to change the intensity of motion sway/head bobbing animations, including to values greater than 1.0. Configuration settings to change motion intensity or disable it entirely should appear after running the game with the mod installed.

### v1.2.0

New:
* Added configuration settings to toggle FOV lock and specify locked FOV. Edit these in the BepInEx/Config/jred.RemoveMotionSway.cfg file or through your mod manager of choice. (Run the game first to generate the file if not present)

Fixes:
* Resolved issue with helmet overlay appearing in first-person after leaving and re-joining a server.
* Fixed an issue where overhead player names would not face towards the player in first-person.

### v1.1.1

Removed rotational "wobble" on the gameplay camera when walking, running, or crouching. Also removed the FOV change when starting to sprint.
