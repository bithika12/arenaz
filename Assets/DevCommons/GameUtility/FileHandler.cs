using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace DevCommons
{
    namespace Utility
    {
        public enum EFileHandlerPathType
        {
            DataPath,
            PersistantDataPath,
        }

        public class FileHandler
        {
            // Reads a object from file in specified path
            public static T ReadFromFile<T>(string filePath, out bool fileState)
            {
                string readString = string.Empty;
                string fileLocation = Application.persistentDataPath + filePath;
                bool fileExists = File.Exists(fileLocation);
                fileState = fileExists;

                if (fileExists)
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    FileStream file = File.Open(fileLocation, FileMode.Open);
                    readString = (string)binaryFormatter.Deserialize(file);
                    file.Close();
                }
                return JsonConvert.DeserializeObject<T>(readString);
            }

            // Saves a file with given string content to specified path
            public static void SaveToFile(string data, string filePath, FileMode mode = FileMode.Create)
            {
                string fileLocation = Application.persistentDataPath + filePath;
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream file = File.Open(fileLocation, FileMode.OpenOrCreate);
                binaryFormatter.Serialize(file, data);
                file.Close();
            }

            // Saves a file with given object content to specified path
            public static void SaveToFile<T>(T data, string filePath, FileMode mode = FileMode.Create)
            {
                string fileLocation = Application.persistentDataPath + filePath;
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream file = File.Open(fileLocation, FileMode.OpenOrCreate);
                binaryFormatter.Serialize(file, JsonConvert.SerializeObject(data));
                file.Close();
            }

            // Delete a file from specified path
            public static bool DeleteSaveFile(string filePath)
            {
                string fileLocation = Application.persistentDataPath + filePath;
                try
                {
                    File.Delete(fileLocation);
                    return true;
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                    return false;
                }
            }

            // Saves a key with given string content to player prefs
            public static void SaveToPlayerPrefs(string key, string value)
            {
                PlayerPrefs.SetString(key, value);
            }

            // Reads a string from key in player prefs
            public static string ReadFromPlayerPrefs(string key, string defaultValue = "")
            {
                string value = string.Empty;
                if (PlayerPrefs.HasKey(key))
                    value = PlayerPrefs.GetString(key, defaultValue);
                return value;
            }

            // Delete a key from player prefs
            public static void DeletePlayerPrefsKey(string key)
            {
                if (PlayerPrefs.HasKey(key))
                    PlayerPrefs.DeleteKey(key);
            }

            // Delete a all keys from player prefs
            public static void ClearPlayerPrefs()
            {
                PlayerPrefs.DeleteAll();
            }
        }
    }
}