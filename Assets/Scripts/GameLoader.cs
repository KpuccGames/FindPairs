using UnityEngine;
using SimpleJson;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    [Header("Game Data")]
    [SerializeField] private TextAsset _cardsData;
    [SerializeField] private TextAsset _questsData;
    [SerializeField] private TextAsset _gameLevelsConfig;
    [SerializeField] private TextAsset _environmentsData;

    private void Start()
    {
        JsonObject dataObject = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(_cardsData.text);
        CardsDataStorage.Instance.Init(dataObject.Get<JsonArray>("cards"));

        dataObject = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(_questsData.text);
        QuestsConfig.Instance.Init(dataObject.Get<JsonObject>("quests_config"));

        dataObject = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(_environmentsData.text);
        EnvironmentsDataStorage.Instance.Init(dataObject.Get<JsonArray>("environments"));

        dataObject = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(_gameLevelsConfig.text);
        GameLevelsConfig.Instance.Init(dataObject.Get<JsonObject>("game_levels_config"));

        // TODO
        // Создавать профиль игрока, чтобы в следующей сцене инициализировать данные по текущему состоянию профиля
    }

    public void OnClickStartGame()
    {
        SceneManager.LoadScene(SceneNames.GameScene);
    }
}
