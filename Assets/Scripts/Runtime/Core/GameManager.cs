using Game.Gameplay;
using Game.Progress;
using Game.Settings;
using Game.UI;
using GamePlugins.Attributes;
using GamePlugins.Singleton;
using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
	[DontDestroyOnLoadSingleton, AutoCreateSingleton(false, false)]
	public class GameManager : Singleton<GameManager>
	{
		#region Static
		public static bool isLoadiongScenes = false;

		private bool intializedLoading = false;
		private const string alwaysLoadedScene = "AlwaysLoaded";
		private const string uiScene = "UIHub";
		private const string gameplayScene = "Gameplay";
		#endregion

		#region Fields
		[SerializeField]
		private GameplaySettings gameplaySettings;
		[SerializeField]
		private DebugSettings debugSettings;
		[SerializeField]
		private LoadingScreenUI loadingScreen;
		[SerializeField]
		private TextMeshProUGUI buildVersionText;

#if UNITY_EDITOR
#endif
		#endregion

		#region Properties
		public GameplaySettings GameplaySettings => gameplaySettings;

		public bool IsLoading
		{
			get
			{
				return isLoadiongScenes || loadingScreen.IsVisible();
			}
		}
		#endregion

		#region Events
		#endregion

		#region MonoBehaviour
		#endregion

		#region Methods
		protected override void OnAwakeCalled()
		{
			base.OnAwakeCalled();
			Initialize();
		}

		public void Initialize()
		{
			if (intializedLoading)
				return;

			buildVersionText.SetTextSafe(GetBuildVersion());

			ToggleLoadingScreen(true, true);

			intializedLoading = true;
			StartCoroutine(InitializeCO());
		}

		public string GetBuildVersion()
		{
			return $"v {GetCurrentVersion()}";
		}

		public static string GetCurrentVersion()
		{
			return UnityEngine.Application.version;
		}

		public void LoadGameplay()
		{
			StartCoroutine(LoadGameplayCO());
		}

		public void LoadMenu()
		{
			StartCoroutine(LoadUICO());
		}

		public void ReLoadGameplay()
		{
			ToggleLoadingScreen(true);
			StartCoroutine(ReLoadGameplayCO());
		}

		public void ToggleLoadingScreen(bool value, bool instant = false)
		{
			if (value)
				loadingScreen.Show(instant);
			else
				loadingScreen.Hide();
		}

		private IEnumerator InitializeCO()
		{
			Scene activeScene = SceneManager.GetActiveScene();
			yield return null;

			ResourceManager.Instance.RegisterResource<GameplaySettings>(gameplaySettings);

			if(debugSettings == null)
			{
				Debug.LogError("Missing DebugSettings ref! Will create a temporary one");
				debugSettings = new DebugSettings();
			}

			ResourceManager.Instance.RegisterResource<DebugSettings>(debugSettings);

			if (activeScene.name == alwaysLoadedScene)
			{
				yield return StartCoroutine(LoadUICO(false));
			}
			else
			{
				yield return StartCoroutine(LoadGameplayCO(false));
			}
		}

		static IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadMode, bool setActiveScene = false)
		{
			bool isLoaded = IsSceneLoaded(sceneName);

			if (!isLoaded)
			{
				bool loaded = false;
				if (!loaded)
				{
					UnityEngine.AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadMode);
					while (asyncOperation != null && !asyncOperation.isDone)
					{
						yield return null;
					}
				}
			}

			if (setActiveScene)
			{
				Scene scene = SceneManager.GetSceneByName(sceneName);
				SceneManager.SetActiveScene(scene);
			}
		}

		static void LoadScene(string sceneName, LoadSceneMode loadMode, bool setActiveScene = false)
		{
			SceneManager.LoadScene(sceneName, loadMode);
		}

		static bool IsSceneLoaded(string sceneName)
		{
			Scene scene = SceneManager.GetSceneByName(sceneName);
			if (scene.IsValid() && scene.isLoaded)
				return true;
			return false;
		}

		private IEnumerator UnloadScenes()
		{
			for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
			{
				Scene s = SceneManager.GetSceneAt(i);
				if (s.name != alwaysLoadedScene && s.name != uiScene)
				{
					UnityEngine.AsyncOperation aso = SceneManager.UnloadSceneAsync(s);
					while (!aso.isDone) yield return null;
				}
			}
		}

		private IEnumerator LoadGameplayCO(bool toggleLoadingScreen = true)
		{
			if (isLoadiongScenes) yield break;

			isLoadiongScenes = true;
			if (toggleLoadingScreen)
			{
				ToggleLoadingScreen(true);
				yield return new WaitForSeconds(.5f);
			}

			ProgressManager.Instance.EnsureProgress();

			yield return StartCoroutine(UnloadScenes());
			yield return StartCoroutine(LoadSceneAsync(uiScene, LoadSceneMode.Additive, true));
			UIManager.Instance.ClearUI();
			yield return StartCoroutine(LoadSceneAsync(gameplayScene, LoadSceneMode.Additive, true));

			GameplayController.Instance.InitializeGameplay();

			ToggleLoadingScreen(false);

			yield return null;
			isLoadiongScenes = false;
		}

		private IEnumerator LoadUICO(bool toggleLoadingScreen = true)
		{
			if (isLoadiongScenes) yield break;

			if (toggleLoadingScreen)
			{
				ToggleLoadingScreen(true);
				yield return new WaitForSeconds(.5f);
			}
			UIManager.Instance.ClearUI();
			yield return StartCoroutine(UnloadScenes());
			yield return StartCoroutine(LoadSceneAsync(uiScene, LoadSceneMode.Additive, true));
			yield return null;

			UINavigator.Instance.Initialize();
			UINavigator.Instance.ShowMainMenuUI();
			yield return null;

			ToggleLoadingScreen(false);
			yield return new WaitForSeconds(.5f);

			yield return null;
			isLoadiongScenes = false;
		}

		private IEnumerator ReLoadGameplayCO()
		{
			yield return null;
			yield return StartCoroutine(UnloadScenes());
			yield return null;
			yield return StartCoroutine(LoadGameplayCO());
		}

		IEnumerator ResetProgressAndOpenMainMenuCO()
		{
			ToggleLoadingScreen(true);
			yield return new WaitForSeconds(.5f);

			UIManager.Instance.CloseAll();
			yield return new WaitForSeconds(.2f);

			yield return new WaitForSeconds(.5f);
			ToggleLoadingScreen(false);
		}

		public void QuitGame()
		{
			Application.Quit();
		}

#if UNITY_EDITOR
		public static void LoadGameplayCO_Editor()
		{
			if (!isLoadiongScenes)
			{
				LoadScene(alwaysLoadedScene, LoadSceneMode.Additive);
			}
		}
#endif
		#endregion
	}
}