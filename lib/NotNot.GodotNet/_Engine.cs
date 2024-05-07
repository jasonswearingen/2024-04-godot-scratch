using Godot;

public static class _Engine
{
   public static void ReloadScene(bool isExecutedImmediately = false)
   {


      void _DoSceneReload()
      {
         _GD.Log("RELOADING SCENE START", Colors.Yellow);

         
         var editor = EditorInterface.Singleton;
         //editor.RestartEditor(false);
         editor.ReloadSceneFromPath(EditorInterface.Singleton.GetEditedSceneRoot().SceneFilePath);


         _GD.Log("RELOADING SCENE DONE", Colors.Yellow);
      }

      if (Engine.IsEditorHint())
      {
         _GD.Log("RELOADING SCENE TRIGGER!", Colors.Yellow);
         //need to use a callback because we are currently in a critical path,
         //and attempting to reload the editor will crash if we do it now.
         if (isExecutedImmediately)
         {
            _DoSceneReload();
         }
         else
         {
            Callable.From(_DoSceneReload).CallDeferred();
         }
      }

      
   }

   public static SceneTree SceneTree => (Engine.GetMainLoop() as SceneTree)!;
}