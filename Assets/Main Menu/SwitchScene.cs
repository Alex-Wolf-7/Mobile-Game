using UnityEngine.SceneManagement;
using UnityEngine;

public class SwitchScene : MonoBehaviour {
	public string sceneName;

	public void switchScene() {
		SceneManager.LoadScene(sceneName);
	}
}