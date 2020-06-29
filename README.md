# WeAR
Try clothes in AR.

# Demo
[Youtube Link](https://youtu.be/h1_UmH4Egpw)

## Problem Definition
The problem is to create a mobile application based on AR that can be used by customers to visualize clothes on themselves. This application can be integrated with online shopping websites as an aid to customers who find it difficult to choose the right pattern and size of clothes. All the existing solutions use special hardware for depth sensing and user size and pose estimation. This type of solution cannot be used in an online shopping website as the user may not have the required hardware available with them. The application should be designed in such a way that it needs no external hardware and should be able to run on the user's smartphone with a camera.

## Requirements

* [Unity](http://unity3d.com/) 5.3 or higher.
* [Android SDK](https://developer.android.com/studio/index.html#downloads)
  (when developing for Android).
  
## Building the app

  1. Open the sample project in the Unity editor.
    - Select the `File > Open Project` menu item.
    - If Unity Hub appears, click `Add`. Otherwise click `Open`.
    - Navigate to the sample directory `testapp` in the file dialog and click
      `Open`.
      - You might be prompted to upgrade the project to your version of Unity.
        Click `Confirm` to upgrade the project and continue.
   2. Open the scene `MainScene`.
      - Navigate to `Assets/Scenes/SampleScene.unity` in the `Project` window.
      - Double click on the `SampleScene.unity` file to open it.
   3. Start the server.
      - Start the `WeAR-Server` from [this](https://github.com/ojasskapre/WeAR-Server) repo.
      - Enter the server URL in this app over [here](https://github.com/murtaza98/WeAR/blob/ab6f4173a0700960f17d08d88b76ff62a4333a34/Assets/Scripts/Credentials.cs#L7) and [here](https://github.com/murtaza98/WeAR/blob/ab6f4173a0700960f17d08d88b76ff62a4333a34/Assets/Scripts/Credentials.cs#L8). 
   4. Build for Android
      - Select the File > Build Settings menu option.
      - Select Android in the Platform list.
      - Click Switch Platform to select Android as the target platform.
      - Wait for the spinner (compiling) icon to stop in the bottom right corner of the Unity status bar.
      - Click Build and Run
      
## Testing
  If you want to try out this app, then you can download and install the `apk` on any Android device from [here](https://drive.google.com/file/d/1h9Wr5je8E_fPIkyx_kjB1CcDXtiW8El-/view?usp=sharing). You will need the server running(Step 3 above)
  
## Project Report
A detailed project report for this project can be found [here](./Project-Report.pdf)
