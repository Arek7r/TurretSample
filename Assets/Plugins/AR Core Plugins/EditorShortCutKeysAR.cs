using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Assembly = System.Reflection.Assembly;
#if  UNITY_EDITOR
public class EditorShortCutKeysAR : ScriptableObject {

	// [MenuItem("Edit/RunA _F1")] // shortcut key F5 to Play (and exit playmode also)
	// static void PlayGame()
	// {
	// 	if (!Application.isPlaying)
	// 	{
	// 		//EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false);
	// 		EditorApplication.ExecuteMenuItem("Edit/Play");
	// 	}
	// 	else
	// 	{
	// 		EditorApplication.ExecuteMenuItem("Edit/Play");
	//
	// 		//EditorUtility.RequestScriptReload();
	// 	}
	//
	// }
	//
	// [MenuItem("Edit/PauseA _F3")] // shortcut key F5 to Play (and exit playmode also)
	// static void PauseGame() {
	// 	if (!Application.isPlaying) {
	// 		EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "", false);
	// 	}
	// 	EditorApplication.ExecuteMenuItem("Edit/Pause");
	//}
	
	
	[MenuItem("Edit/Force Garbage Collection _F5")]
	public static void GarbageCollect()
	{
		EditorUtility.UnloadUnusedAssetsImmediate();
		GC.Collect();
		ClearLogConsole();
		Debug.ClearDeveloperConsole();
		Debug.Log("Clear");

	}
	
	// [MenuItem("Edit/FillMaterial Dissolve _F4")]
	// public static void DoOnScene()
	// {
	// 	
	// }
	//
	// [MenuItem("Edit/Force Garbage Collection _F6")]
	// public static void RefreshUILineRenderer()
	// {
	// }

	[MenuItem("Edit/FindPlayerUI  &S")] 
	static void FindPlayerUI() 
	{ 
		var selection =  GameObject.FindWithTag("PlayerUI")?.transform;
		if (selection)
			Selection.activeTransform = selection;
	} 
	
	[MenuItem("Edit/FindPlayer  &f")] 
	static void FindPlayer() 
	{ 
		var selection =  GameObject.FindWithTag("Player")?.transform;
		if (selection == null)
		{
			 selection =  GameObject.FindWithTag("PlayerSpawn")?.transform;
		}

		if (selection)
			Selection.activeTransform = selection;
	} 
	
	
	[MenuItem("Edit/FindCamera  &c")] 
	static void FindCamera() 
	{ 
		var selection =  GameObject.FindWithTag("MainCamera")?.transform;
		if (selection)
			Selection.activeTransform = selection;
	} 
	
	[MenuItem("Edit/DoTask  &B")] 
	static void DoTask()
	{
		
		// var x = GameObject.FindObjectsOfType<PathCreator>();
		// foreach (var item in x)
		// {
		// 	Debug.Log("AR: edited"); 
		// 	//item.EditorData.vertexPathMaxAngleError = 2;
		// 	foreach (var instanceBotVehicle in AreaManager.Instance.botVehicles)
		// 	{
		// 		instanceBotVehicle.DestroyVehicleDebug();
		// 	}
		// }
	} 
	
	[MenuItem("Tools/Toggle Inspector Lock #2")] // shift + 2
	public static void ToggleInspectorLock()
	{
		EditorWindow inspectorToBeLocked = EditorWindow.mouseOverWindow; // "EditorWindow.focusedWindow" can be used instead
 
		Type projectBrowserType = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.ProjectBrowser");
 
		Type inspectorWindowType = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
 
		PropertyInfo propertyInfo;
		if (inspectorToBeLocked.GetType() == projectBrowserType)
		{
			propertyInfo = projectBrowserType.GetProperty("isLocked", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		}
		else if (inspectorToBeLocked.GetType() == inspectorWindowType)
		{
			propertyInfo = inspectorWindowType.GetProperty("isLocked");
		}
		else
		{
			return;
		}
 
		bool value = (bool)propertyInfo.GetValue(inspectorToBeLocked, null);
		propertyInfo.SetValue(inspectorToBeLocked, !value, null);
		inspectorToBeLocked.Repaint();
	}

	static MethodInfo _clearConsoleMethod;
	static MethodInfo clearConsoleMethod {
		get {
			if (_clearConsoleMethod == null) {
				Assembly assembly = Assembly.GetAssembly (typeof(SceneView));
				Type logEntries = assembly.GetType ("UnityEditor.LogEntries");
				_clearConsoleMethod = logEntries.GetMethod ("Clear");
			}
			return _clearConsoleMethod;
		}
	}
 
	public static void ClearLogConsole() {
		clearConsoleMethod.Invoke (new object (), null);
	}
	#region GUI

	
	[MenuItem("uGUI/Anchors to Corners #1")]
	static void AnchorsToCorners(){
		foreach(Transform transform in Selection.transforms){
			RectTransform t = transform as RectTransform;
			RectTransform pt = Selection.activeTransform.parent as RectTransform;

			if(t == null || pt == null) return;
			
			Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
				t.anchorMin.y + t.offsetMin.y / pt.rect.height);
			Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
				t.anchorMax.y + t.offsetMax.y / pt.rect.height);

			t.anchorMin = newAnchorsMin;
			t.anchorMax = newAnchorsMax;
			t.offsetMin = t.offsetMax = new Vector2(0, 0);
		}
	}
	
	[MenuItem("uGUI/Corners to Anchors %]")]
	static void CornersToAnchors(){
		foreach(Transform transform in Selection.transforms){
			RectTransform t = transform as RectTransform;

			if(t == null) return;

			t.offsetMin = t.offsetMax = new Vector2(0, 0);
		}
	}

	[MenuItem("uGUI/Mirror Horizontally Around Anchors %;")]
	static void MirrorHorizontallyAnchors(){
		MirrorHorizontally(false);
	}

	[MenuItem("uGUI/Mirror Horizontally Around Parent Center %:")]
	static void MirrorHorizontallyParent(){
		MirrorHorizontally(true);
	}

	static void MirrorHorizontally(bool mirrorAnchors){
		foreach(Transform transform in Selection.transforms){
			RectTransform t = transform as RectTransform;
			RectTransform pt = Selection.activeTransform.parent as RectTransform;
			
			if(t == null || pt == null) return;

			if(mirrorAnchors){
				Vector2 oldAnchorMin = t.anchorMin;
				t.anchorMin = new Vector2(1 - t.anchorMax.x, t.anchorMin.y);
				t.anchorMax = new Vector2(1 - oldAnchorMin.x, t.anchorMax.y);
			}

			Vector2 oldOffsetMin = t.offsetMin;
			t.offsetMin = new Vector2(-t.offsetMax.x, t.offsetMin.y);
			t.offsetMax = new Vector2(-oldOffsetMin.x, t.offsetMax.y);

			t.localScale = new Vector3(-t.localScale.x, t.localScale.y, t.localScale.z);
		}
	}

	[MenuItem("uGUI/Mirror Vertically Around Anchors %'")]
	static void MirrorVerticallyAnchors(){
		MirrorVertically(false);
	}
	
	[MenuItem("uGUI/Mirror Vertically Around Parent Center %\"")]
	static void MirrorVerticallyParent(){
		MirrorVertically(true);
	}
	
	static void MirrorVertically(bool mirrorAnchors){
		foreach(Transform transform in Selection.transforms){
			RectTransform t = transform as RectTransform;
			RectTransform pt = Selection.activeTransform.parent as RectTransform;
			
			if(t == null || pt == null) return;
			
			if(mirrorAnchors){
				Vector2 oldAnchorMin = t.anchorMin;
				t.anchorMin = new Vector2(t.anchorMin.x, 1 - t.anchorMax.y);
				t.anchorMax = new Vector2(t.anchorMax.x, 1 - oldAnchorMin.y);
			}
			
			Vector2 oldOffsetMin = t.offsetMin;
			t.offsetMin = new Vector2(t.offsetMin.x, -t.offsetMax.y);
			t.offsetMax = new Vector2(t.offsetMax.x, -oldOffsetMin.y);
			
			t.localScale = new Vector3(t.localScale.x, -t.localScale.y, t.localScale.z);
		}
	}
	#endregion

}
// [InitializeOnLoad]
// public static class EditorSceneMemoryManager
// {
// 	static EditorSceneMemoryManager()
// 	{
// 		EditorSceneManager.sceneOpened += OnSceneOpened;
// 	}
// 	static void OnSceneOpened(Scene scene, OpenSceneMode mode)
// 	{
// 		EditorShortCutKeysAR.GarbageCollect();
// 	}
// }



#endif
