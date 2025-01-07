using Newtonsoft.Json;

namespace Logic.Model
{
    /// <summary>
    /// Provides methods for saving, loading, and deleting SRModel instances from the file system.
    /// </summary>
    public static class ModelFileSystem
    {
        /// <summary>
        /// Saves the current model to the file system.
        /// </summary>
        public static void SaveModel()
        {
            string folderName = AppSettings.ModelCurrent.Name;
            string fileName = AppSettings.ModelCurrent.Name + ".json";
            string path = Path.Combine(AppSettings.ModelsFolderPath, folderName, fileName);

            string json = JsonConvert.SerializeObject(AppSettings.ModelCurrent, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            Directory.CreateDirectory(Path.Combine(AppSettings.ModelsFolderPath, folderName));
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Saves the current model to the file system, optionally renaming the folder and file.
        /// </summary>
        /// <param name="oldName">The old name of the model.</param>
        /// <param name="newName">The new name of the model.</param>
        public static void SaveModel(string oldName, string newName)
        {
            if (oldName == newName)
            {
                SaveModel();
                return;
            }

            string oldFolderPath = Path.Combine(AppSettings.ModelsFolderPath, oldName);
            string newFolderPath = Path.Combine(AppSettings.ModelsFolderPath, newName);

            string oldFilePath = Path.Combine(newFolderPath, oldName + ".json");
            string newFilePath = Path.Combine(newFolderPath, newName + ".json");

            string json = JsonConvert.SerializeObject(AppSettings.ModelCurrent, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            Directory.Move(oldFolderPath, newFolderPath);
            File.WriteAllText(newFilePath, json);
            File.Delete(oldFilePath);
        }

        /// <summary>
        /// Deletes the specified model from the file system.
        /// </summary>
        /// <param name="modelName">The name of the model to delete.</param>
        public static void DeleteModel(string modelName)
        {
            string folderName = modelName;
            string path = Path.Combine(AppSettings.ModelsFolderPath, folderName);

            Directory.Delete(path, true);
        }

        /// <summary>
        /// Retrieves a list of all models stored in the file system.
        /// </summary>
        /// <returns>A list of SRModel instances.</returns>
        public static List<SRModel> GetModelList()
        {
            List<SRModel> models = new List<SRModel>();

            string directoryPath = AppSettings.ModelsFolderPath;
            string[] folders = Directory.GetDirectories(directoryPath);
            List<string> jsonFiles = new List<string>();

            foreach (string folder in folders)
            {
                string[] files = Directory.GetFiles(folder, "*.json");
                if (files.Length != 0)
                    jsonFiles.Add(files[0]);
            }

            foreach (string filePath in jsonFiles)
            {
                string jsonContent = File.ReadAllText(filePath);

                SRModel model = JsonConvert.DeserializeObject<SRModel>(jsonContent, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

                if (model != null)
                    models.Add(model);
            }

            return models;
        }

        /// <summary>
        /// Saves the acoustic model of the current SRModel instance to the file system.
        /// </summary>
        public static void SaveAcousticModel()
        {
            string folderName = AppSettings.ModelCurrent.Name;
            string path = Path.Combine(AppSettings.ModelsFolderPath, folderName);
            AppSettings.ModelCurrent.AcousticModel.SaveModel(path);
        }

        /// <summary>
        /// Loads the acoustic model of the current SRModel instance from the file system.
        /// </summary>
        public static void LoadAcousticModel()
        {
            string folderName = AppSettings.ModelCurrent.Name;
            string path = Path.Combine(AppSettings.ModelsFolderPath, folderName);
            AppSettings.ModelCurrent.AcousticModel.LoadModel(path);
        }
    }
}
