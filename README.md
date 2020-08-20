# EnactmentInterface

## Installation

1. Install Unity Hub 
2. In Unity Hub install Unity version 2018.4.21f1
3. Download EnactmentInterface folder
4. Move the EnactmentInterface folder to the directory you keep your Unity projects
5. In Unity Hub press Add then select the EnactmentInterface folder
6. You should now see EnactmentInterface show in the list of projects
7. Double click the EnactmentInterface project to open it
8. Press the Play button at the top to start the interface

## Project Info

### Scripts
    • Found in the Project section in Scripts folder.
    • Pay most attention to Switch script. It controls all transitions between sections of the interface (e.g. Enactment phase, Planning phase, start screen, etc) plus more.
    • All scripts relating to YOLO tracking are in Scripts > TrackingScripts.
        • Ignore TrackerRotScript and TrackIRObj; they’re remnants from the previous interface and are not currently used.
        • TryDarknet.py starts YOLO tracking and handles the UDP communication between the command line and Unity. It should be run independently via Python 3 before pressing Play in the Unity project.
        • TrackerScript (not to be confused with Tracker) takes the information (puppet positions) sent by TryDarknet and applies them to in-game models.
        • If you are actually using YOLO while playing the interface, you’ll have to edit TryDarknet to use the proper command and path to YOLO on your device. 
        • You’ll also have to check “Use YOLO Tracking” on the start screen for it to work. If you don’t check that, the models you select in pre-planning will spawn randomly instead of according to YOLO tracking.
      
### Scene
    • In the Hierarchy section you’ll see everything in the scene. Press the arrow next to Canvas and you’ll see all the different sections of the interface. Enabling one and disabling the rest will allow you to view a section.
    • For best behavior, only press the Play button when you are viewing only the Start section (I.e. within the Canvas, only Start should be enabled).
    • The GameController in the Hierarchy holds most big scripts and, obviously, plays a big part in controlling the whole interface. Most gameObjects will reference the GameController in their scripts to access Switch.
    • So far, the focus has been “Virtual Puppet Driven Mode”. This mode begins with the Planning phase (whereas the other mode begins with Enactment with no pre-planning).
