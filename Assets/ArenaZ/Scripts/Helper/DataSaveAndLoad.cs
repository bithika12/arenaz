using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ArenaZ.Data 
{
	public class DataSaveAndLoad
	{
        string path = string.Empty;
        private string fileName;
        private string folderName;
        private byte[] data;
        private byte[] loadedData = Array.Empty<byte>();
        public byte[] LoadedData { get { return loadedData; } }

        public DataSaveAndLoad(string fileName, string folderName)
        {
            this.fileName = fileName;
            this.folderName = folderName;
        }

        public DataSaveAndLoad(string fileName, string folderName, byte[] data)
        {
            this.fileName = fileName;
            this.folderName = folderName;
            this.data = data;
        }

        private void createPath()
        {
            string appName = Application.productName;
            Debug.Log("Creating Directory...");
            string androidPath = Path.Combine(appName, folderName, fileName);
            path = Path.Combine("mnt/sdcard/", androidPath);
            string directoryName = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        public void SaveToDisk(byte[] data)
        {
            createPath();
            Debug.Log("Trying to get Directory info");
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
            FileInfo[] fileInfo = directoryInfo.GetFiles();
            if (fileInfo.Length == 0)
            {
                createNewFile();
            }
            else
            {
                if (fileInfo[0].Name.Contains(fileName))
                {
                    Debug.Log("Updating Existing Data");
                    resetDataOnExistingFile(fileInfo[0]);
                }
            }

        }

        private void createNewFile()
        {
            Debug.Log("Saving New Data...");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = File.Create(path);
            formatter.Serialize(fileStream, data);
            fileStream.Close();
        }

        private void resetDataOnExistingFile(FileInfo fileInfo)
        {
            Debug.Log("Updating...");
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = fileInfo.Open(FileMode.Open);
            fileStream.Flush();
            binaryFormatter.Serialize(fileStream, data);
            fileStream.Close();
        }

        public void LoadDataFromStorage()
        {
            createPath();
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
            FileInfo[] fileInfo = directoryInfo.GetFiles();
            Debug.Log("FileInfo Lenght:   " + fileInfo.Length);
            if (fileInfo.Length == 0 || !fileInfo[0].Name.Contains(fileName))
            {
                return;
            }
            loadedData = getDataFromFile(fileInfo[0]);
        }

        private byte[] getDataFromFile(FileInfo file)
        {
            Debug.Log("Reading Data On File: " + file.Name);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = file.Open(FileMode.Open);
            byte[] bytes = (byte[])binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return bytes;
        }
    }
}
