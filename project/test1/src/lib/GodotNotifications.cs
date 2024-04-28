using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test1.src.lib;
/// <summary>
/// Enumeration of notification values used by Godot nodes.
/// You can use this when overriding Node._Notification(int) to better understand the `what` value.
/// </summary>
public enum GodotNotifications
{
   /// 
   /// Notification received when the object is initialized, before its script is attached.
   /// Used internally.
   /// 
   GodotObject_Postinitialize = 0,

   /// 
   /// Notification received when the object is about to be deleted. Can act as the
   /// deconstructor of some programming languages.
   /// 
   GodotObject_Predelete = 1,

   /// 
   /// Notification received when the object finishes hot reloading. This notification
   /// is only sent for extensions classes and derived.
   /// 
   GodotObject_ExtensionReloaded = 2,



   /// <summary>
   /// Notification received when the node enters a Godot.SceneTree. 
   /// See Godot.Node._EnterTree.
   /// 
   /// This notification is received before the related Godot.Node.TreeEntered signal.
   /// </summary>
   Node_EnterTree = 10,

   /// <summary>
   /// Notification received when the node is about to exit a Godot.SceneTree. See Godot.Node._ExitTree.
   /// 
   /// This notification is received after the related Godot.Node.TreeExiting signal.
   /// </summary>
   Node_ExitTree = 11,

   /// <summary>
   /// Deprecated. This notification is no longer emitted. Use Godot.Node.NotificationChildOrderChanged instead.
   /// </summary>
   [Obsolete("This constant is deprecated.")]
   Node_MovedInParent = 12,

   /// <summary>
   /// Notification received when the node is ready. See Godot.Node._Ready.
   /// </summary>
   Node_Ready = 13,

   /// <summary>
   /// Notification received when the node is paused. See Godot.Node.ProcessMode.
   /// </summary>
   Node_Paused = 14,

   /// <summary>
   /// Notification received when the node is unpaused. See Godot.Node.ProcessMode.
   /// </summary>
   Node_Unpaused = 15,

   /// <summary>
   /// Notification received from the tree every physics frame when Godot.Node.IsPhysicsProcessing returns true.
   /// See Godot.Node._PhysicsProcess(System.Double).
   /// </summary>
   Node_PhysicsProcess = 16,

   /// <summary>
   /// Notification received from the tree every rendered frame when Godot.Node.IsProcessing returns true.
   /// See Godot.Node._Process(System.Double).
   /// </summary>
   Node_Process = 17,

   /// <summary>
   /// Notification received when the node is set as a child of another node (see Godot.Node.AddChild(Godot.Node,System.Boolean,Godot.Node.InternalMode) and Godot.Node.AddSibling(Godot.Node,System.Boolean)).
   /// 
   /// Note: This does not mean that the node entered the Godot.SceneTree.
   /// </summary>
   Node_Parented = 18,

   /// <summary>
   /// Notification received when the parent node calls Godot.Node.RemoveChild(Godot.Node) on this node.
   /// 
   /// Note: This does not mean that the node exited the Godot.SceneTree.
   /// </summary>
   Node_Unparented = 19,

   /// <summary>
   /// Notification received only by the newly instantiated scene root node, when Godot.PackedScene.Instantiate(Godot.PackedScene.GenEditState) is completed.
   /// </summary>
   Node_SceneInstantiated = 20,

   /// <summary>
   /// Notification received when a drag operation begins. All nodes receive this notification, not only the dragged one.
   /// 
   /// Can be triggered either by dragging a Godot.Control that provides drag data (see Godot.Control._GetDragData(Godot.Vector2)) or using Godot.Control.ForceDrag(Godot.Variant,Godot.Control).
   /// 
   /// Use Godot.Viewport.GuiGetDragData to get the dragged data.
   /// </summary>
   Node_DragBegin = 21,

   /// <summary>
   /// Notification received when a drag operation ends.
   /// 
   /// Use Godot.Viewport.GuiIsDragSuccessful to check if the drag succeeded.
   /// </summary>
   Node_DragEnd = 22,

   /// <summary>
   /// Notification received when the node's Godot.Node.Name or one of its ancestors' Godot.Node.Name is changed. This notification is not received when the node is removed from the Godot.SceneTree.
   /// </summary>
   Node_PathRenamed = 23,

   /// <summary>
   /// Notification received when the list of children is changed. This happens when child nodes are added, moved or removed.
   /// </summary>
   Node_ChildOrderChanged = 24,

   /// <summary>
   /// Notification received from the tree every rendered frame when Godot.Node.IsProcessingInternal returns true.
   /// </summary>
   Node_InternalProcess = 25,

   /// <summary>
   /// Notification received from the tree every physics frame when Godot.Node.IsPhysicsProcessingInternal returns true.
   /// </summary>
   Node_InternalPhysicsProcess = 26,

   /// <summary>
   /// Notification received when the node enters the tree, just before Godot.Node.NotificationReady may be received. Unlike the latter, it is sent every time the node enters tree, not just once.
   /// </summary>
   Node_PostEnterTree = 27,

   /// <summary>
   /// Notification received when the node is disabled. See Godot.Node.ProcessModeEnum.Disabled.
   /// </summary>
   Node_Disabled = 28,

   /// <summary>
   /// Notification received when the node is enabled again after being disabled. See Godot.Node.ProcessModeEnum.Disabled.
   /// </summary>
   Node_Enabled = 29,

   /// <summary>
   /// Notification received right before the scene with the node is saved in the editor. This notification is only sent in the Godot editor and will not occur in exported projects.
   /// </summary>
   Node_EditorPreSave = 9001,

   /// <summary>
   /// Notification received right after the scene with the node is saved in the editor. This notification is only sent in the Godot editor and will not occur in exported projects.
   /// </summary>
   Node_EditorPostSave = 9002,

   /// <summary>
   /// Notification received when the mouse enters the window.
   /// 
   /// Implemented for embedded windows and on desktop and web platforms.
   /// </summary>
   Node_WMMouseEnter = 1002,

   /// <summary>
   /// Notification received when the mouse leaves the window.
   /// 
   /// Implemented for embedded windows and on desktop and web platforms.
   /// </summary>
   Node_WMMouseExit = 1003,

   /// <summary>
   /// Notification received from the OS when the node's Godot.Window ancestor is focused. This may be a change of focus between two windows of the same engine instance, or from the OS desktop or a third-party application to a window of the game (in which case Godot.Node.NotificationApplicationFocusIn is also received).
   /// 
   /// A Godot.Window node receives this notification when it is focused.
   /// </summary>
   Node_WMWindowFocusIn = 1004,

   /// <summary>
   /// Notification received from the OS when the node's Godot.Window ancestor is defocused. This may be a change of focus between two windows of the same engine instance, or from a window of the game to the OS desktop or a third-party application (in which case Godot.Node.NotificationApplicationFocusOut is also received).
   /// 
   /// A Godot.Window node receives this notification when it is defocused.
   /// </summary>
   Node_WMWindowFocusOut = 1005,

   /// <summary>
   /// Notification received from the OS when a close request is sent (e.g. closing the window with a "Close" button or Alt + F4).
   /// 
   /// Implemented on desktop platforms.
   /// </summary>
   Node_WMCloseRequest = 1006,

   /// <summary>
   /// Notification received from the OS when a go back request is sent (e.g. pressing the "Back" button on Android).
   /// 
   /// Implemented only on iOS.
   /// </summary>
   Node_WMGoBackRequest = 1007,

   /// <summary>
   /// Notification received when the window is resized.
   /// 
   /// Note: Only the resized Godot.Window node receives this notification, and it's not propagated to the child nodes.
   /// </summary>
   Node_WMSizeChanged = 1008,

   /// <summary>
   /// Notification received from the OS when the screen's dots per inch (DPI) scale is changed. Only implemented on macOS.
   /// </summary>
   Node_WMDpiChange = 1009,

   /// <summary>
   /// Notification received when the mouse cursor enters the Godot.Viewport's visible area, that is not occluded behind other Godot.Controls or Godot.Windows, provided its Godot.Viewport.GuiDisableInput is false and regardless if it's currently focused or not.
   /// </summary>
   Node_VpMouseEnter = 1010,

   /// <summary>
   /// Notification received when the mouse cursor leaves the Godot.Viewport's visible area, that is not occluded behind other Godot.Controls or Godot.Windows, provided its Godot.Viewport.GuiDisableInput is false and regardless if it's currently focused or not.
   /// </summary>
   Node_VpMouseExit = 1011,

   /// <summary>
   /// Notification received from the OS when the application is exceeding its allocated memory.
   /// 
   /// Implemented only on iOS.
   /// </summary>
   Node_OsMemoryWarning = 2009,

   /// <summary>
   /// Notification received when translations may have changed. Can be triggered by the user changing the locale. Can be used to respond to language changes, for example to change the UI strings on the fly. Useful when working with the built-in translation support, like Godot.GodotObject.Tr(Godot.StringName,Godot.StringName).
   /// </summary>
   Node_TranslationChanged = 2010,

   /// <summary>
   /// Notification received from the OS when a request for "About" information is sent.
   /// 
   /// Implemented only on macOS.
   /// </summary>
   Node_WMAbout = 2011,

   /// <summary>
   /// Notification received from Godot's crash handler when the engine is about to crash.
   /// 
   /// Implemented on desktop platforms, if the crash handler is enabled.
   /// </summary>
   Node_Crash = 2012,

   /// <summary>
   /// Notification received from the OS when an update of the Input Method Engine occurs (e.g. change of IME cursor position or composition string).
   /// 
   /// Implemented only on macOS.
   /// </summary>
   Node_OsImeUpdate = 2013,

   /// <summary>
   /// Notification received from the OS when the application is resumed.
   /// 
   /// Implemented only on Android.
   /// </summary>
   Node_ApplicationResumed = 2014,

   /// <summary>
   /// Notification received from the OS when the application is paused.
   /// 
   /// Implemented only on Android.
   /// </summary>
   Node_ApplicationPaused = 2015,

   /// <summary>
   /// Notification received from the OS when the application is focused, i.e. when changing the focus from the OS desktop or a third-party application to any open window of the Godot instance.
   /// 
   /// Implemented on desktop platforms.
   /// </summary>
   Node_ApplicationFocusIn = 2016,

   /// <summary>
   /// Notification received from the OS when the application is defocused, i.e. when changing the focus from any open window of the Godot instance to the OS desktop or a third-party application.
   /// 
   /// Implemented on desktop platforms.
   /// </summary>
   Node_ApplicationFocusOut = 2017,

   /// <summary>
   /// Notification received when the Godot.TextServer is changed.
   /// </summary>
   Node_TextServerChanged = 2018,

   /// 
   /// Godot.Node3D nodes receive this notification when their global transform changes.
   /// This means that either the current or a parent node changed its transform.
   /// 
   /// In order for Godot.Node3D.NotificationTransformChanged to work, users first need
   /// to ask for it, with Godot.Node3D.SetNotifyTransform(System.Boolean). The notification
   /// is also sent if the node is in the editor context and it has at least one valid
   /// gizmo.
   /// 
   Node3D_TransformChanged = 2000,

   /// 
   /// Godot.Node3D nodes receive this notification when they are registered to new
   /// Godot.World3D resource.
   /// 
   Node3D_EnterWorld = 41,

   /// 
   /// Godot.Node3D nodes receive this notification when they are unregistered from
   /// current Godot.World3D resource.
   /// 
   Node3D_ExitWorld = 42,

   /// 
   /// Godot.Node3D nodes receive this notification when their visibility changes.
   /// 
   Node3D_VisibilityChanged = 43,

   /// 
   /// Godot.Node3D nodes receive this notification when their local transform changes.
   /// This is not received when the transform of a parent node is changed.
   /// 
   /// In order for Godot.Node3D.NotificationLocalTransformChanged to work, users first
   /// need to ask for it, with Godot.Node3D.SetNotifyLocalTransform(System.Boolean).
   /// 
   Node3D_LocalTransformChanged = 44,
}