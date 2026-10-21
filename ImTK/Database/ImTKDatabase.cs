using ImTK.Core;

namespace ImTK.Database
{
    /// <summary>
    /// 本地可讀寫資料庫 (Local Read-Write Database)。
    /// 負責存取使用者設定檔、存檔等會變動的資源。
    /// 資源載入後 IsReadOnly = false，可透過 MarkDirty() 與 SaveAssets() 寫回磁碟。
    /// </summary>
    public static class ImTKDatabase
    {
        internal static AssetManager Manager { get; private set; } = null!;

        public static void Initialize()
        {
            if (Manager == null)
            {
                Manager = new AssetManager(ImTKEnvironment.LocalDataPath, isReadOnly: false);
            }
        }

        private static void EnsureInitialized()
        {
            if (Manager == null)
            {
                Initialize();
            }
        }

        /// <summary>
        /// 從任意絕對路徑載入並解析資源（跳過 LocalDataPath 前綴安全檢查，不納入預設本地快取）。
        /// </summary>
        /// <typeparam name="T">期望的資源型別。</typeparam>
        /// <param name="absolutePath">檔案絕對路徑。</param>
        /// <returns>載入的資源物件。</returns>
        public static T ImportExternal<T>(string absolutePath) where T : IAsset
        {
            EnsureInitialized();
            return Manager.ImportExternal<T>(absolutePath);
        }

        /// <summary>
        /// 從本地資料庫中載入資源。
        /// </summary>
        /// <typeparam name="T">期望的資源型別。</typeparam>
        /// <param name="path">相對於 LocalDataPath 的相對路徑。</param>
        /// <returns>載入的資源物件。</returns>
        public static T Load<T>(string path) where T : IAsset
        {
            EnsureInitialized();
            return Manager.Load<T>(path);
        }

        /// <summary>
        /// 註冊資源解析器 (Importer)。
        /// </summary>
        public static void RegisterImporter(System.Type assetType, object importerTypeOrInstance)
        {
            EnsureInitialized();
            Manager.RegisterImporter(assetType, importerTypeOrInstance);
        }

        /// <summary>
        /// 註冊資源匯出器 (Exporter)。
        /// </summary>
        public static void RegisterExporter(System.Type assetType, object exporterTypeOrInstance)
        {
            EnsureInitialized();
            Manager.RegisterExporter(assetType, exporterTypeOrInstance);
        }

        /// <summary>
        /// 將所有標記為 Dirty 的資源寫回磁碟。
        /// </summary>
        public static void SaveAssets()
        {
            EnsureInitialized();
            Manager.SaveAssets();
        }

        /// <summary>
        /// 僅將指定的特定資源寫回磁碟（若被標記為 Dirty）。
        /// 杜絕全庫遍歷連帶刷盤造成的寫入放大與競爭。
        /// </summary>
        /// <param name="specific">目標資源物件。</param>
        public static void SaveAsset(IAsset specific)
        {
            EnsureInitialized();
            Manager.SaveAsset(specific);
        }

        internal static void UnloadAll()
        {
            EnsureInitialized();
            Manager.UnloadAll();
        }

        /// <summary>
        /// 從本地資料庫中卸載指定資源。
        /// </summary>
        /// <param name="path">相對於 LocalDataPath 的相對路徑。</param>
        public static void Unload(string path)
        {
            EnsureInitialized();
            Manager.Unload(path);
        }
    }
}
