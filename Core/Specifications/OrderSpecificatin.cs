using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;

namespace Core.Specifications
{
    public class OrderSpecification : BaseSpecification<Order>
    {

        public OrderSpecification(string email) : base(x => x.BuyerEmail == email)
        {
            AddInclude(x => x.OrderItems);
            AddInclude(x => x.DeliveryMethod);
            AddOrderByDescending(x => x.OrderDate);
        }

        public OrderSpecification(string email, int id) : base(x => x.BuyerEmail == email && x.Id == id)
        {
            //Same as previouse one but this time with strings
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
        }

        public OrderSpecification(string paymentintentId, bool isPaymentIntent) : base(x => x.PaymentIntentId == paymentintentId)
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
        }

        public OrderSpecification(OrderSpecParams specParams) : base(x =>
        string.IsNullOrEmpty(specParams.Status) || x.Status == ParseStatus(specParams.Status))
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
            ApplyPaging(specParams.Skip, specParams.Take);
            AddOrderByDescending(x => x.OrderDate);
        }

        public OrderSpecification(int id) : base(x => x.Id == id)
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");            
        }

        private static OrderStatus? ParseStatus(string status)
        {
            if (Enum.TryParse<OrderStatus>(status, out var result))
            {
                return result;
            }
            return null;
        }
    }
}