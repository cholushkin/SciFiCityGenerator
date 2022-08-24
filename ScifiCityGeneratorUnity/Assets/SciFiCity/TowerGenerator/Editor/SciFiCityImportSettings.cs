using TowerGenerator;
using UnityEditor;

namespace Game
{
    [InitializeOnLoad]
    public static class SciFiCityImportSettings
    {
        static SciFiCityImportSettings()
        {
            ChunkImportSettingsManager.RegisterChunkImportSettings(
                "Platform", 
                new ChunkImportSettings
                {
                    ChunksOutputPath = "Assets/SciFiCity/Chunks/!Prefabs", 
                    AddColliders = false, 
                    IsPack = false, 
                    EnableImport = false
                });
        }
    }
}