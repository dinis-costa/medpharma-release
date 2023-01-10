
using FFImageLoading.Work;
using System;
using static System.Net.WebRequestMethods;

namespace Medpharma.Mobile.Models
{
    public class OrderResponse
    {
        public int id { get; set; }
        public DateTime orderDate { get; set; }
        public object deliveryDate { get; set; }
        public object customer { get; set; }
        public Item[] items { get; set; }
        public int lines { get; set; }
        public int quantity { get; set; }
        public float cValue { get; set; }
        public DateTime orderDateLocal { get; set; }
        public int orderSent { get; set; }

        public class Item
        {
            public int id { get; set; }
            public Product product { get; set; }
            public float price { get; set; }
            public int quantity { get; set; }
            public float value { get; set; }
        }

        public class Product
        {
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public bool needsPrescription { get; set; }
            public int stock { get; set; }
            public float price { get; set; }
            public string imageId { get; set; }
            public string imageFullPath { get; set; }
        }
    }
}
