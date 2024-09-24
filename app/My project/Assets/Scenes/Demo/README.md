# Demo Scenes

The scenes found within this folder are for Demo application.
- Does not support live media upload or download
- Does not support live vide-to-AR conversion
- Comes with pre-defined demo point-of-interest, varying depending on the demo location
- Two locations: Novotel and University

## Scenes

Total of 7 scenes used exclusively by the Demo application

### Landing

Landing scene for the Demo. Can choose between the 'Novotel' and 'University' demo set-up

### Demo Create

Scene for creating a new PoI (Point-of-interest) on the device's current GPS coordinate. The coordinate is taken when the 'upload' button is clicked.

The user can choose within the supplied demo AR options to create a new PoI.

### Demo Explore

Scene with Niantic Lightship Maps to display GPS-accurate map based on the device's current location. If the Location service is turned off, then the default GPS location based on the demo location selected from the Landing scene will be used.

Existing PoI is displayed using the origami crane model. Interaction is locked by the proximity of the device to the PoI's GPS location. Default threshold for interaction is 11 metres. In example, the device must be within 11 m radius from the PoI location to interact with it.

Interactable PoIs are coloured blue, whilst locked PoIs are coloured grey.

Distance is calculated using Pythagoras's Theorem 

d = sqrt((x1 - x2)^2 + (y1 - y2)^2)

### Demo Gallery

A scene to display all PoIs created by the user.
For the demo context, two items are preloaded to test and demo the gallery feature.
All subsequent new PoI creations will be added to the scene.

The PoIs displayed within the Gallery scene are not bound by the actual distance between the user device and the PoI location. 

### Demo Main

Main 'home' scene for the demo application.

### Demo Position

Scene to allow users to adjust the position of the AR object relative to the user's current position.

The position refers to the Vector3 (x, y, z) coordinate of the AR GameObject within the AR scene, not the GPS coordinate.

The Vector3 is relative to the XR Origin GameObject as the AR object is placed as a child of the XR Origin.

### Demo AR

AR scene to display the AR video of the birds.
Uses Chroma Key shader to remove Green (RGB (0, 0, 255)) pixels from the video. 

## Assets used

If there are any issues for the assets used please contact me at yisang.summer@gmail.com

If you are the owner of the YouTube video used within this project and wish to have the video removed, please let me know! I'll remove it :D

### UI assets

Kenney's Assets (https://kenney.nl/assets)

### Videos

Pukeko
https://www.youtube.com/watch?v=gXWXVH_Xl74


Kokako https://www.youtube.com/watch?v=jstijJD2390


Kereru https://www.youtube.com/watch?v=otiMgysM6NA


### Photos