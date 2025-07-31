
    public class CurrentLevelInfo
    {
        public static CurrentLevelInfo Instance { get; private set; }
        public int LevelId { get; set; }
        
        public CurrentLevelInfo(int levelId)
        {
            Instance = this;
            LevelId = levelId;
        }
    }