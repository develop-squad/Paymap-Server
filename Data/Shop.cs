using System.Linq;

namespace PAYMAP_BACKEND.Data
{
    public class Shop
    {
        public string Name;
        public int Sido;
        public int Sigungu;
        public string AddressSido;
        public string AddressSigungu;
        public string Address;
        public string Type;

        public Shop()
        {
            
        }

        public Shop(CrawlData crawlData)
        {
            Name = crawlData.Name;
            Sido = crawlData.CodeSido;
            Sigungu = crawlData.CodeSigungu;
            AddressSido = crawlData.AddressSiDo;
            AddressSigungu = crawlData.AddressSiGunGu;
            Address = crawlData.Address;
            Type = crawlData.Type;
        }
    }
}