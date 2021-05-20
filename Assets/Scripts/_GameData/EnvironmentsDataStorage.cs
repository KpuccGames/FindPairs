using SimpleJson;
using UnityEngine;
using System.IO;

public class EnvironmentData : IDataStorageObject
{
    public string Id { get; private set; }
    public int StartLevelNumber { get; private set; }
    public string[] EnvironmentCards { get; private set; }

    private string _backgroundPath;

    public bool Init(JsonObject json)
    {
        Id = (string)json["id"];
        StartLevelNumber = json.GetInt("start_level_number");
        _backgroundPath = (string)json["background"];

        JsonArray cardIds = json.Get<JsonArray>("cards");

        EnvironmentCards = new string[cardIds.Count];

        for (int i = 0; i < EnvironmentCards.Length; i++)
        {
            EnvironmentCards[i] = cardIds.GetAt<string>(i);
        }

        return true;
    }

    public Sprite GetBackground()
    {
        return Resources.Load<Sprite>(Path.Combine("_EnvironmentBackgrounds", _backgroundPath));
    }
}

public class EnvironmentsDataStorage : BaseDataStorage<EnvironmentData, EnvironmentsDataStorage>
{
    public EnvironmentsDataStorage() : base("environments") { }
}
