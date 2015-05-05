using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {

	public void ChangeToScene (string sceneToChangeTo) {
		Application.LoadLevel (sceneToChangeTo);
	}

	public void EnterSettingScene () {
				if (PlayerPrefs.GetInt ("Upgrade2") == 0) {
						ChangeToScene ("menuScene");
				} else {
						ChangeToScene ("settingScene");
				}
		}
}
