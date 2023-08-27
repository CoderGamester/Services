﻿using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameLovers.Services.Editor
{
	/// <summary>
	/// Set the internal version in any VersionService instances in the project before building
	/// and whenever the project loads.
	/// </summary>
	public static class VersionEditorUtils
	{
		private const int ShortenedCommitLength = 8;

		/// <summary>
		/// Set the internal version before building the app.
		/// </summary>
		public static void SetAndSaveInternalVersion(bool isStoreBuild)
		{
			var newVersionData = GenerateInternalVersionSuffix(isStoreBuild);
			var newVersionDataSerialized = JsonUtility.ToJson(newVersionData);
			var oldVersionDataSerialized = LoadVersionDataSerializedSync();
			if (newVersionDataSerialized.Equals(oldVersionDataSerialized, StringComparison.Ordinal))
			{
				return;
			}

			Debug.Log($"Saving new version data: {newVersionDataSerialized}");
			SaveVersionData(newVersionDataSerialized);
		}

		/// <summary>
		/// Loads the game version saved in disk into string format
		/// </summary>
		public static string LoadVersionDataSerializedSync()
		{
			var textAsset = Resources.Load<TextAsset>(VersionServices.VersionDataFilename);
			if (!textAsset)
			{
				Debug.LogError("Could not load internal version from Resources.");
				return string.Empty;
			}

			var serialized = textAsset.text;
			Resources.UnloadAsset(textAsset);
			return serialized;
		}

		/// <summary>
		/// Set the internal version for when the app plays in editor.
		/// </summary>
		[InitializeOnLoadMethod]
		private static void OnEditorLoad()
		{
			SetAndSaveInternalVersion(false);
		}

		private static VersionServices.VersionData GenerateInternalVersionSuffix(bool isStoreBuild)
		{
			var data = new VersionServices.VersionData();

			using (var repo = new GitEditorProcess(Application.dataPath))
			{
				try
				{
					if (!repo.IsValidRepo())
					{
						Debug.LogWarning("Project is not a git repo. Internal version not set.");
					}
					else
					{
						var branch = repo.GetBranch();
						if (string.IsNullOrEmpty(branch))
						{
							Debug.LogWarning("Could not get git branch for internal version");
						}
						else
						{
							data.BranchName = branch;
						}

						var commitHash = repo.GetCommitHash();
						if (string.IsNullOrEmpty(commitHash))
						{
							Debug.LogWarning("Could not get git commit for internal version");
						}
						else
						{
							data.Commit = commitHash.Substring(0, ShortenedCommitLength);
						}
					}
				}
				catch (Exception e)
				{
					Debug.LogException(e);
					Debug.LogWarning("Could not execute git commands. Internal version not set.");
				}
			}

			data.BuildNumber = PlayerSettings.iOS.buildNumber;
			data.BuildType = isStoreBuild ? "prod" : "dev";

			return data;
		}

		/// <summary>
		/// Set the internal version of this application and save it in resources. This should be
		/// called at edit/build time.
		/// </summary>
		private static void SaveVersionData(string serializedData)
		{
			const string assets = "Assets";
			const string resources = "Build/Resources";

			var absDirPath = Path.Combine(Application.dataPath, resources);
			if (!Directory.Exists(absDirPath))
			{
				Directory.CreateDirectory(absDirPath);
			}

			// delete old file with incorrect extension
			const string assetExtension = ".asset";
			var absFilePath = Path.Combine(absDirPath, VersionServices.VersionDataFilename);
			if (File.Exists(Path.ChangeExtension(absFilePath, assetExtension)))
			{
				AssetDatabase.DeleteAsset(
					Path.Combine(assets, resources,
						Path.ChangeExtension(VersionServices.VersionDataFilename, assetExtension)));
			}

			// create new text file
			const string textExtension = ".txt";
			File.WriteAllText(Path.ChangeExtension(absFilePath, textExtension), serializedData);

			AssetDatabase.ImportAsset(
				Path.Combine(assets, resources,
					Path.ChangeExtension(VersionServices.VersionDataFilename, textExtension)));
		}
	}
}
