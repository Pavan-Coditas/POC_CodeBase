using RabbitMQBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQEventIntegrationBus.Event
{
    public class OrderEvent :IntegrationEvent
    {
        public string UserName { get; set; }
        public int TotalPrice { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string AddressLine { get; set; }
        public Category Category { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Product> Product { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
    }

}
