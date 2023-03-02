
namespace BLIT.Investments;

public class Investment
{
    public BuyData BuyData { get; protected set; }

    public Investment(BuyData buyData)
    {
        BuyData = buyData;
    }
}
