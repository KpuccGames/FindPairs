using SimpleJson;

public class GameLevelsConfig
{
    private static GameLevelsConfig _instance;
    public static GameLevelsConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameLevelsConfig();
            }

            return _instance;
        }
    }

    public float QuestGenerationTime { get; private set; }

    private int _hardDifficultyMinRemainder;
    private int _extremeDifficultyMinRemainder;

    private float[] _durationsByDifficulty;
    private int[] _questsAmountByDifficulty;
    private int[] _goldAmountByDifficulty;

    public void Init(JsonObject configData)
    {
        _durationsByDifficulty = new float[(int)LevelDifficulty.Count];

        JsonArray durationsData = configData.Get<JsonArray>("level_durations");

        for (int i = 0; i < _durationsByDifficulty.Length; i++)
        {
            _durationsByDifficulty[i] = durationsData.GetFloatAt(i);
        }

        _questsAmountByDifficulty = new int[(int)LevelDifficulty.Count];

        JsonArray questsAmountData = configData.Get<JsonArray>("quests_amount");

        for (int i = 0; i < _questsAmountByDifficulty.Length; i++)
        {
            _questsAmountByDifficulty[i] = questsAmountData.GetIntAt(i);
        }

        _goldAmountByDifficulty = new int[(int)LevelDifficulty.Count];

        JsonArray goldAmountData = configData.Get<JsonArray>("gold_amount");

        for (int i = 0; i < _goldAmountByDifficulty.Length; i++)
        {
            _goldAmountByDifficulty[i] = goldAmountData.GetIntAt(i);
        }

        _hardDifficultyMinRemainder = configData.GetInt("hard_difficulty_min_remainder");
        _extremeDifficultyMinRemainder = configData.GetInt("extreme_difficulty_min_remainder");
    }

    public LevelDifficulty GetLevelDifficulty(int levelNumber)
    {
        // +1, так как уровни начинаются с 1
        int remainder = levelNumber % (Constants.LevelsPerEnvironment + 1);

        if (remainder >= _extremeDifficultyMinRemainder)
        {
            return LevelDifficulty.Extreme;
        }
        else if (remainder >= _hardDifficultyMinRemainder)
        {
            return LevelDifficulty.Hard;
        }

        return LevelDifficulty.Normal;
    }
}
