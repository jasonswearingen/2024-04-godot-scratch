﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace NotNot;
//public partial class EditorHotReloadHelper : Node
//{
//   private static bool isReloadRequested = false;
//   public EditorHotReloadHelper()
//   {
//      this._PrintInfo(".ctor");
//      if (isReloadRequested is false && Engine.IsEditorHint() && IsInsideTree())
//      {
//         this._PrintWarn("EditorHotReloadHelper()  RELOAD REQUESTED");
//         isReloadRequested = true;

//         //We are running in the editor, and even though this object was just .CTOR'd,
//         //we are already inside the SceneTree.
//         //CAUSE: The Godot Editor has Cold-Reloaded the CSharp project,
//         //which is an effect of building the .csproj externally (By Visual Studio)

//         //this has caused all managed (Csharp) objects to be recreated,
//         //but godot has NOT reinitialized them, and indeed the native godot objects are still initialized properly.
//         //this leaves managed objects broken, and they need to be reinitialized.
//         //the only way to do this is to force the Godot Editor to reload the entire scene
//         //this will allow csharp tools to properly initialize,
//         //as the native godot objects reinitialize (which drives the managed objects)

//         //need to use a callback because we are currently in a critical path,
//         //and attempting to reload the editor will crash if we do it now.
//         var immediateReload = false;
//         _Engine.ReloadScene(immediateReload);

//         //however this "AutoloadHost" is loaded prior to the scene,
//         //so it will not be fixed when the scene reloads.
//         //it is still broken, as the entire managed assembly has been cold-reloaded.
//         //so before our "CallDeferred" scene reload occurs (above), we need to manually reinitialize this AutoloadHost
//         //so that it's DI capabilities are available prior to the scene reload.

//         //i'm not sure why this can't be inside the "CallDeferred", but it doesn't work there.
//         //I think it's because the init has to happen in the same stack call as the .ctor() call.

//      }
//   }
//}
