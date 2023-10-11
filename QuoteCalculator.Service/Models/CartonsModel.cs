using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteCalculator.Service.Models
{

    public class CartonsModel
    {
        public long id { get; set; }
        public string type { get; set; }
        //public CartontypeList type { get; set; }
        public short display { get; set; }
        public int displayorder { get; set; }
        public short standout { get; set; }
        public string description { get; set; }
        public string moveware_description { get; set; }
        public int length { get; set; }
        public int breadth { get; set; }
        public int height { get; set; }
        public string title { get; set; }
        public double volume { get; set; }
        public int weight { get; set; }
        public string filter { get; set; }
        public string image { get; set; }
        public int quantity { get; set; }
        public Nullable<int> UserVolume { get; set; }
        public string FullSizeCartonStr
        {
            get
            {
                string dimensionStr = "";
                dimensionStr = Convert.ToString(quantity + " X " + description + " (" + volume + " cubic feet" + (quantity > 1 ? " each)" : ")"));
                if (length > 0 && breadth > 0 && height > 0)
                {
                    dimensionStr += Convert.ToString(", " + length + " X " + breadth + " X " + height + " Cms");
                }
                if (UserVolume > 0)
                {
                    dimensionStr += Convert.ToString(", " + UserVolume + " Kgs");
                }
                return dimensionStr;
            }
        }
    }
}
