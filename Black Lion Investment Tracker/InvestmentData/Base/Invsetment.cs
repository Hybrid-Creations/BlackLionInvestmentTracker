using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BLIT.Investments;

[DataContract]
public class Investment
{
    [DataMember]
    public BuyData BuyData { get; protected set; }

    [DataMember]
    public List<SellData> SellDatas { get; protected set; } = new();

    public Investment(BuyData buyData)
    {
        BuyData = buyData;
    }

    public Investment() { }

    public virtual int AverageIndividualProfit => throw new System.NotImplementedException();
    public virtual int TotalNetProfit => throw new System.NotImplementedException();
    public virtual bool IndividualSellPriceDifferent => throw new System.NotImplementedException();
    public virtual int IndividualSellPrice => throw new System.NotImplementedException();
    public virtual int AverageIndividualSellPrice => throw new System.NotImplementedException();
    public virtual int TotalSellPrice => throw new System.NotImplementedException();
}
