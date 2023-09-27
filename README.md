# The Room VR: A Dark Matter - Mixed Reality Mod

## About
This mod enables Mixed Reality support by fixing the SteamVR External Camera implementation which already exists in game. This allows you to manually composite Background, yourself / your avatar, and foreground from a third person perspective to put you 'in' the game.
## Installing
To install the mod do the following:
* Install MelonLoader against the game
* Copy `Mixed Reality Mod.dll` to the  `The Room VR\Mods` folder

## Running
#### Third Controller = Camera

Before you run the game, and indeed before you even start SteamVR, you will need to make sure that your controllers are turned on in the correct order to make sure that the camera attaches to the correct device. 
Whichever the third controller or tracker device is that's connected to SteamVR will be picked up as the camera.

You can use the LIV virtual tracker as this device, but it likes to attach early, so try to turn on your controllers as soon as (if not before) SteamVR starts.


#### ExternalCamera.cfg

You will need to put an ExternalCamera.cfg file in the game folder: `\Steam\steamapps\common\The Room VR\ExternalCamera.cfg`

If you need a template one, 
[you can get it from here](https://github.com/Hacked-Mixed-Reality/Mixed-Reality-Example-Files/blob/main/ExternalCamera.cfg).

If you want more information on the ExternalCamera.cfg to edit the settings then I've got a [video about it here](https://youtu.be/-JGRTxSIRq4): 
If you just want to skip to the part where I tell you how to generate it from LIV with your calibrated camera settings, 
[click here](https://youtu.be/-JGRTxSIRq4?t=549).

#### Running the game
Run the game as normal and the Melonloader window will pop up as well as the game window. Once the splash screen concludes it should start the quadrant view.

#### Compositing 
LIV has removed support for quad compositing, so you will need to do it another way. I've created an OBS plugin that will do all the hard compositing work for you, 
[which is available here](https://github.com/Hacked-Mixed-Reality/MixedRealityQuadCompositor).

You can see how it works in 
[my tutorial video here](https://youtu.be/kVuOIPcCZDw).

## Building
Although I'd like to, it's not possible to build from Github actions, as game DLL dependencies would need to be uploaded to Github. As these original assets are copywrit I don't think that's a great idea.

So to build it yourself:
* Clone the repository
* Install MelonLoader against the game
* Change the folder locations in Mixed Reality Mod.csproj within the tags `<Assemblies>` and `<MelonLoaderDir>` from `D:\Steam\steamapps\common\The Room VR` to wherever you have your copy of the game installed