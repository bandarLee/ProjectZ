public enum BoxType
{
    Diner,
    PoliceLarge,
    PoliceSmall,
    PostOfficeLarge,
    PostOfficeSmall,
    WarehouseLarge,
    WarehouseSmall
}

[System.Serializable]
public struct BoxTypeConfig
{
    public BoxType boxType;
    public int itemCount;
    public float foodProbability;
    public float weaponProbability;
    public float healProbability;
    public float mentalProbability;
    public float etcProbability;
}
