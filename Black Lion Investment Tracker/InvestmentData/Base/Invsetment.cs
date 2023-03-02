
using System.Runtime.Serialization;

namespace BLIT.Investments;

[DataContract]
public class Investment
{
    [DataMember] public BuyData BuyData { get; protected set; }

    public Investment(BuyData buyData)
    {
        BuyData = buyData;
    }

    public Investment() { }
}
