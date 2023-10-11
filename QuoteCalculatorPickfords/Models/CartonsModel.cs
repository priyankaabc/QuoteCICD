using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuoteCalculatorPickfords.Models
{
    public class CartonsModel
    {
        public long id { get; set; }
        public string type { get; set; }
        public short display { get; set; }
        public int displayorder { get; set; }
        public short standout { get; set; }
        public string description { get; set; }
        public string moveware_description { get; set; }
        public int length { get; set; }
        public int breadth { get; set; }
        public int height { get; set; }
        public int volume { get; set; }
        public int weight { get; set; }
        public string filter { get; set; }
        public string image { get; set; }
        public int quantity { get; set; }
        public Nullable<int> UserVolume { get; set; }
        public string title { get; set; }
    }
}