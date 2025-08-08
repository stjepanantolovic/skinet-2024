using API.DTOs;
using Core.Entities.OrderAggregate;

namespace API.Extensions
{
    public static class OrderMappingExtensions
    {
        public static OrderDto? ToDto(this Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            return new OrderDto
            {
                Id = order.Id,
                BuyerEmail = order.BuyerEmail,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                PaymentSummary = order.PaymentSummary,
                DeliveryMethod = order.DeliveryMethod.Description,
                ShippingPrice = order.DeliveryMethod.Price,
                OrderItems = order.OrderItems.Select(x => x.ToDto()).ToList(),
                SubTotal = order.SubTotal,
                Status = order.Status.ToString(),
                PaymentIntentId = order.PaymentIntentId,
                Total = order.GetTotal()
            };
        }

        public static OrderItemDto ToDto(this OrderItem orderItem)
        {
            if (orderItem == null)
            {
                throw new ArgumentNullException(nameof(orderItem));
            }
            return new OrderItemDto
            {
                ProductId = orderItem.Id,
                ProductName = orderItem.ItemOrdered.ProductName,
                PictureUrl = orderItem.ItemOrdered.PictureUrl,
                Price = orderItem.Price,
                Quantity = orderItem.Quantity
            };
        }

    }
}