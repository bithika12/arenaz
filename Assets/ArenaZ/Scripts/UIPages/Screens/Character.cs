using UnityEngine;
using UnityEngine.UI;

namespace ArenaZ.Screens 
{
	public class Character : MonoBehaviour
	{
       [SerializeField]private Image characterImage;

        public void SetCharacterImage(string colorName)
        {
            characterImage.sprite = null;
            string path = GameResources.characterImageFolderPath + "/" + transform.name + colorName;
            Debug.Log("PathName: " + path);
            characterImage.sprite = Resources.Load<Sprite>(path);
        }
	}
}
