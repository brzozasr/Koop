namespace Koop.Models.RepositoryModels
{
    public class CooperatorOrder
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public string OrderStatus { get; set; }
        public double ChosenQuantity { get; set; }
        public int Quantity { get; set; }
    }
}