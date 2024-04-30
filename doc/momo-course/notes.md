
# DevEnv

## Setting up Visual Studio with Godot 4.x

This enables debugging, console logs, and hot-reload.

1. create a new godot 4 project normally, through the editor.  Be sure to setup VS as your external editor in the godot options.
2. create your C# script attached to a node through the editor (as normal).  This will cause the editor to auto generate a csproj+solution for you.
3. Test build/run via the godot editor, to make sure it all works.
4. In Visual Studio, create a new **Launch Profile** (under project Debug Properties) for an **Executable**
5. set the **executable path** to a relative path to the godot binary, from your csproj's location.  example: `..\..\bin\Godot_v4.0-beta8_mono_win64\Godot_v4.0-beta8_mono_win64.exe`
6. set the **command line arguments** to simply startup the project in the current directory.  example: `--path . --verbose`
7. set the **working directory** to the current.  example: `.`
8. Set **enable native code debugging** if you want to see better errors in the output window.   Leaving this disabled allows hot-reload to work (!!!) but various godot cpp errors won't be shown.
9. a workaround to no Console output:
   - add arguments `>out.log 2>&1` to the Launch Profile,  which will redirect stdout and stderr to file.
   - then open the `out.log` in some tool that can auto-scroll to the last line.   I use the VSCode extension "**Log Viewer**" which works great.
     - Configure `Log Viewer`: edit your `settings.json` and add a `logViewer.watch` node with a wildcard like `"./**/*.gdlog"`
 
TLDR:
- executable path: `bin\Godot_v4.2.2-stable_mono_win64\Godot_v4.2.2-stable_mono_win64.exe`
- cmdline args:  `--path . --verbose >out.gdlog 2>&1`
- working dir: `.`
- 
## Attaching a library project
1. Add library project to your godot game's `.SLN` as normal library
2. reference godot assemblies via nuget
3. for **HotReload** to work, in the project.csproj, add `<Version>0.0.0.0</Version>`.   Do this if you see an error like this:
	``` error
	12:56 25.35 [Error] V:\godot-cslearn\projects\squash-creeps-csharp\godot\SquashCreeps.csproj (line 1): error CS7038: Failed to emit module 'SquashCreeps': Changing the version of an assembly reference is not allowed during debugging: 'ClassLibrary1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null' changed version to '0.0.0.0'.
	12:56 25.35 Invalid changes were found. Please refer to the Error List window to fix those issues.
	```


