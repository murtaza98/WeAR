# WeAR
Try clothes in AR.

## Requirements

* [Unity](http://unity3d.com/) 5.3 or higher.
* [Android SDK](https://developer.android.com/studio/index.html#downloads)
  (when developing for Android).
  
## Building the app

  - Open the sample project in the Unity editor.
    - Select the `File > Open Project` menu item.
    - If Unity Hub appears, click `Add`. Otherwise click `Open`.
    - Navigate to the sample directory `testapp` in the file dialog and click
      `Open`.
      - You might be prompted to upgrade the project to your version of Unity.
        Click `Confirm` to upgrade the project and continue.
   - Open the scene `MainScene`.
    - Navigate to `Assets/Scenes/SampleScene.unity` in the `Project` window.
    - Double click on the `SampleScene.unity` file to open it.
   - Start the server
    - Clone this server repo and start the python server
    - Enter the server URL in this app over [here](https://github.com/murtaza98/WeAR/blob/ab6f4173a0700960f17d08d88b76ff62a4333a34/Assets/Scripts/Credentials.cs#L7). 
   - Build for Android
      - Select the File > Build Settings menu option.
      - Select Android in the Platform list.
      - Click Switch Platform to select Android as the target platform.
      - Wait for the spinner (compiling) icon to stop in the bottom right corner of the Unity status bar.
      - Click Build and Run
        
