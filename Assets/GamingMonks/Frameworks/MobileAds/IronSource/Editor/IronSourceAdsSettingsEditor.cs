﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GamingMonks.Utils;
using UnityEditor;
using UnityEngine;

namespace GamingMonks.Ads
{	
	[CustomEditor(typeof(IronSourceAdsSettings))]
	public class IronSourceAdsSettingsEditor : CustomInspectorHelper 
	{
		private bool cache = false;
		IronSourceAdsSettings adSettings;
		GUIStyle labelStyle;

		SDKInfo thisSDKInfo = new SDKInfo("IronSource", "IronSourceJSON", "HB_IRONSOURCE");
		static bool thisSdkExists = false;
		static bool thisDefineSymbolExists = false;

		private SerializedProperty appId_android;
		private SerializedProperty appId_iOS;
		private SerializedProperty bannerAdPosition;
		
		string[] bannerAdPositions;
		int selectedBannerPosition = 0;

		public override void OnInspectorGUI()
    	{
			serializedObject.Update();

			if (!cache) {
				adSettings = (IronSourceAdsSettings)target;				
				AutoDetectSdk();
				labelStyle = new GUIStyle(GUI.skin.label);
				labelStyle.fontStyle = FontStyle.Bold;

				appId_android = serializedObject.FindProperty("appId_android");
				appId_iOS = serializedObject.FindProperty("appId_iOS");
				bannerAdPosition = serializedObject.FindProperty("bannerAdPosition");
				bannerAdPositions = EnumUtils.GetValuesAsStringArray<BannerAdPosition>();
				selectedBannerPosition = bannerAdPosition.intValue;

				cache = true;
			}

			EditorGUILayout.Space();
			if(appId_android != null) {
				DrawAdsSettings();
			}
			EditorGUILayout.Space();
			DrawScriptingDefineSymbol();

			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(adSettings);
		}

		void DrawAdsSettings() 
		{
			labelStyle.fontStyle = FontStyle.Bold;
			
			BeginBox();
			GUILayout.Space(2);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("IronSource Ads Settings", labelStyle);
			EditorGUILayout.EndHorizontal();
			DrawLine();

			EditorGUILayout.LabelField("Android Settings : ",labelStyle);
			GUILayout.Space(3);
			labelStyle.fontStyle = FontStyle.Normal;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("App Id : ",labelStyle, GUILayout.MaxWidth(140));
			appId_android.stringValue = EditorGUILayout.TextField(appId_android.stringValue);
			EditorGUILayout.EndHorizontal();


			labelStyle.fontStyle = FontStyle.Bold;
			GUILayout.Space(10);
			EditorGUILayout.LabelField("iOS Settings : ",labelStyle);
			GUILayout.Space(3);
			labelStyle.fontStyle = FontStyle.Normal;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("App Id : ",labelStyle, GUILayout.MaxWidth(140));
			appId_iOS.stringValue = EditorGUILayout.TextField(appId_iOS.stringValue);
			EditorGUILayout.EndHorizontal();


			GUILayout.Space(2);
			DrawLine();
			GUILayout.Space(2);

			labelStyle.fontStyle = FontStyle.Bold;
			EditorGUILayout.LabelField("Common Settings : ",labelStyle);
			GUILayout.Space(3);
			labelStyle.fontStyle = FontStyle.Normal;


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Banner Ad Placement : ",labelStyle, GUILayout.MaxWidth(140));
			selectedBannerPosition =  EditorGUILayout.Popup(selectedBannerPosition, bannerAdPositions);
			bannerAdPosition.intValue = selectedBannerPosition;
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(5);
			
			EndBox();
		}

		void DrawScriptingDefineSymbol() {
			
			GUI.enabled = !EditorApplication.isCompiling;
			BeginBox();
			GUI.backgroundColor = Color.grey;
			GUIStyle style2 = new GUIStyle(GUI.skin.button);
			style2.richText = true;
			style2.fontStyle = FontStyle.Bold;
			style2.fixedHeight = 30;
	
			if(thisSdkExists) {
				if(!thisDefineSymbolExists) {
					if (GUILayout.Button(new GUIContent("Add Scripting Define Symbol"), style2)) {
						AddScriptingDefineSymbol(thisSDKInfo.sdkName, thisSDKInfo.sdkScriptingDefineSymbol, true);
					}
					EditorGUILayout.HelpBox("Scripting symbol is required in order to serve ads with selected sdk. ", MessageType.None, true);
				} else {
					if (GUILayout.Button(new GUIContent("Remove Scripting Define Symbol"), style2)){
						RemoveScriptingDefineSymbol(thisSDKInfo.sdkName, thisSDKInfo.sdkScriptingDefineSymbol, true);
					}
				}
			} else {
				DrawSDKDetection();
			}
			GUI.backgroundColor = Color.white;
			EndBox();
		}

		void DrawSDKDetection()
		{
			BeginBox();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.HelpBox("IronSource SDK not detected, Please import to serve ads with IronSource SDK. ", MessageType.Warning, true);
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("You can download IronSource SDK from",GUILayout.MaxWidth(220));

			Color fontColor = labelStyle.normal.textColor;
			labelStyle.fontStyle = FontStyle.Bold;
			labelStyle.normal.textColor = Color.red;

			if(GUILayout.Button("Here.", labelStyle)) {
				Application.OpenURL("https://developers.ironsrc.com/ironsource-mobile/unity/unity-plugin/");
			}

			labelStyle.normal.textColor = fontColor;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			EndBox();
		}

		void AutoDetectSdk() 
		{
			// thisSdkExists = NamespaceUtils.CheckNamespacesExists(thisSDKInfo.sdkNameSpace);
			thisSdkExists = (System.IO.Directory.Exists(Application.dataPath + "/IronSource"));
			thisDefineSymbolExists = ScriptingDefineSymbolEditor.HasDefineSymbol(thisSDKInfo.sdkScriptingDefineSymbol);
			VerifySDKImportInfo(thisSDKInfo, thisSdkExists, thisDefineSymbolExists);
		}

		static void VerifySDKImportInfo(SDKInfo currentSdkInfo, bool sdkExists, bool defineSymbolExists) {
			if(sdkExists) {
				if(!defineSymbolExists) {
					AddScriptingDefineSymbol(currentSdkInfo.sdkName, currentSdkInfo.sdkScriptingDefineSymbol);
				}
			} else {
				RemoveScriptingDefineSymbol(currentSdkInfo.sdkName, currentSdkInfo.sdkScriptingDefineSymbol);
			}
		}

		static void AddScriptingDefineSymbol( string sdkName, string symbol, bool addForced = false) {
			if((!EditorPrefs.HasKey("userRemoved_"+sdkName)) || addForced) {
				ScriptingDefineSymbolEditor.AddScriptingDefineSymbol(symbol);
				thisDefineSymbolExists = true;
			}
			
		}

		static void RemoveScriptingDefineSymbol(string sdkName, string symbol, bool forced = false) {
			if(ScriptingDefineSymbolEditor.HasDefineSymbol(symbol)) {
				ScriptingDefineSymbolEditor.RemoveScriptingDefineSymbol(symbol);
			}

			if(forced) {
				EditorPrefs.SetInt("userRemoved_"+sdkName, 1);
			}
			thisDefineSymbolExists = false;
		}
	}
}