using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UIElements;

public static class SaveLoadManager
{
    public static void SaveLevelData(LevelDataSO level)
    {
        // Creamos objeto que contiene los datos a guardar y path donde guardar.
        LevelSaveData levelData = new LevelSaveData(level);
        string dataPath = Application.persistentDataPath + "/level" + level.number + ".sav";

        // Creamos filestream para crear y guardar archivo.
        FileStream fileStream = new FileStream(dataPath, FileMode.Create);

        // Creamos formateador a y de binario.
        BinaryFormatter binFormat = new BinaryFormatter();

        // Guardamos archivo y cerramos stream.
        binFormat.Serialize(fileStream, levelData);
        fileStream.Close();

        Debug.Log("Level " + level.number + " data saved!");
    }

    public static LevelSaveData LoadLevelData(int number)
    {
        string dataPath = Application.persistentDataPath + "/level" + number + ".sav";

        if (File.Exists(dataPath))
        {
            // Creamos filestream para cargar archivo.
            FileStream fileStream = new FileStream(dataPath, FileMode.Open);

            // Creamos formateador a y de binario.
            BinaryFormatter binFormat = new BinaryFormatter();

            // Creamos objeto que contiene los datos de guardado.
            LevelSaveData levelSaveData = (LevelSaveData)binFormat.Deserialize(fileStream);

            // Cerramos stream y devolvemos datos de guardado.
            fileStream.Close();
            Debug.Log("Level " + number + " data loaded!");
            return levelSaveData;
        }
        else
        {
            Debug.LogWarning("No save data found for level " + number);
            return null;
        }
    }
}