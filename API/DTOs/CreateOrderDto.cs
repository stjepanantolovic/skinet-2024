using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;

namespace API.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public string CartId { get; set; } = string.Empty;

        [Required]
        public int DeliveryMethodId { get; set; }

        [Required]
        public ShippingAddress ShippingAddress { get; set; }

        [Required]
        public PaymentSummary PaymentSummary { get; set; }
    }
}