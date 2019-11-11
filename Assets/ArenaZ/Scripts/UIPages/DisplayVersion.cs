using UnityEngine;
using UnityEngine.UI;

public class DisplayVersion : MonoBehaviour {

    [Tooltip("The prefix to the version"), SerializeField]
    string prefix = "v";

    [Tooltip("The Texts to display the version on"), SerializeField]
    Text[] texts;

    void Awake() {
        foreach(Text text in texts) {
            text.text = string.Format("{0}{1}", prefix, Application.version);
        }
    }
}
