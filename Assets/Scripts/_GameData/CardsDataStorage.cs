using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using UnityEngine;

public class CardData : IDataStorageObject
{
    public string Id { get; private set; }

    private string _icon;

    public bool Init(JsonObject json)
    {
        Id = (string)json["id"];
        _icon = (string)json["icon"];

        return true;
    }

    public Sprite GetIcon()
    {
        return Resources.Load<Sprite>($"_CardIcons/{_icon}");
    }
}

public class CardsDataStorage : BaseDataStorage<CardData, CardsDataStorage>
{
    public CardsDataStorage() : base("cards") { }
}
