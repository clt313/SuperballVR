# SuperballVR
Superball VR is a new and unique take on the classic games of tennis and
volleyball. It combines the two in a virtual reality environment accessible by
anyone with a PCVR system.

## Capstone Info
Project documents are tracked in [this Google Drive folder](https://drive.google.com/drive/folders/18jeZMgjLvT-QLkRAkY97e1_U2WnQU-8i?usp=sharing).

For the developers: don't forget to use Issues, and track how much time you spend!

## Getting Started
This project uses Unity Hub to develop in the Unity3D environment. To set up your workspace for the first time, follow these steps:

### Register License
1. Log in to Unity or create an account at https://id.unity.com/. Feel free to use
any email you want on this step.

2. Apply for a Unity license. Students get the Pro license for free at
https://unity.com/products/unity-student. Input the requested details, making sure
to use your UF email here.

3. You should get two emails shortly after. One is a welcome email and the other has
your license key. Keep that key handy.

### Download and Install
4. Download Unity Hub at https://unity.com/download. Unity Hub allows you to manage
your projects and Unity installations in one place.

5. Install the Unity Hub. Once finished, run it.

6. Sign in to your Unity account and add your license with the license key. This
should take you to the Projects page.

7. Go to the `Installs` tab, and select `Install Editor`. Select the version currently
being used by the project (defined [here](https://github.com/clt313/SuperballVR/blob/first-map/ProjectSettings/ProjectVersion.txt)).
At minimum, select the `Windows Build Support` module since that's our target platform.

### Import and Open Project
8. Now that Unity is installed, we can import our project. In any folder, open
a shell and type the following command to pull the project from GitHub:
```
git clone https://github.com/clt313/SuperballVR.git
```

9. Go back to Unity Hub. Under the `Projects` tab, click the `Open` dropdown and
select `Add project from disk`. Find the SuperballVR folder you created and select
it. The SuperballVR project should be visible from Unity Hub.

10. Open the `SuperballVR` project. This can take some time, especially if opening
for the first time due to installing packages. This should take around 5 minutes.

11. You're good to go! Use your favorite IDE to code and use Unity to connect
everything together.

### IDE Setup
Here are some guides on how to setup [VSCode](https://code.visualstudio.com/docs/other/unity)
or [Visual Studio](https://learn.microsoft.com/en-us/visualstudio/gamedev/unity/get-started/getting-started-with-visual-studio-tools-for-unity)
for Unity development, including intellisense and other useful features.

### Solving a Merge Conflict
If you ever find that the branch you're working on is x commits behind main, you've
got a merge conflict coming. Insert the following into the repo's `.git\config` file:
```
[merge]
tool = unityyamlmerge
[mergetool "unityyamlmerge"]
trustExitCode = false
cmd = 'C:\\Program Files\\Unity\\Editor\\Data\\Tools\\UnityYAMLMerge.exe' merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"
```

Now whenever you run into a merge conflict, use git mergetool to fix the issue.
There may be more steps to it, but this process hasn't been tested yet. 😅

For a deeper dive, take a look at https://github.com/anacat/unity-mergetool.

### Final Note
If these directions are inaccurate at any point, feel free to modify them.
