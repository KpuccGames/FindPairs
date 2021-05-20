using SimpleJson;

public class QuestsConfig
{
    private static QuestsConfig _instance;
    public static QuestsConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new QuestsConfig();
            }

            return _instance;
        }
    }

    private int[] _minimumRequiredCards;
    private int[] _maximumRequiredCards;
    private int[] _maximumRequiredCardTypes;

    public void Init(JsonObject json)
    {
        _minimumRequiredCards = new int[(int)LevelDifficulty.Count];
        JsonArray minimumReqCardsData = json.Get<JsonArray>("minimum_required_cards");

        for (int i = 0; i < minimumReqCardsData.Count; i++)
        {
            _minimumRequiredCards[i] = minimumReqCardsData.GetIntAt(i);
        }

        _maximumRequiredCards = new int[(int)LevelDifficulty.Count];
        JsonArray maximumReqCardsData = json.Get<JsonArray>("maximum_required_cards");

        for (int i = 0; i < maximumReqCardsData.Count; i++)
        {
            _maximumRequiredCards[i] = maximumReqCardsData.GetIntAt(i);
        }

        _maximumRequiredCardTypes = new int[(int)LevelDifficulty.Count];
        JsonArray maximumReqCardTypesData = json.Get<JsonArray>("minimum_required_card_types");

        for (int i = 0; i < maximumReqCardTypesData.Count; i++)
        {
            _maximumRequiredCardTypes[i] = maximumReqCardTypesData.GetIntAt(i);
        }
    }

    public int GetMinimumCardsRequiredForQuest(LevelDifficulty levelDifficulty)
    {
        return _minimumRequiredCards[(int)levelDifficulty];
    }

    public int GetMaximumCardsRequiredForQuest(LevelDifficulty levelDifficulty)
    {
        return _maximumRequiredCards[(int)levelDifficulty];
    }

    public int GetMaximumCardTypesRequiredForQuest(LevelDifficulty levelDifficulty)
    {
        return _maximumRequiredCardTypes[(int)levelDifficulty];
    }
}
