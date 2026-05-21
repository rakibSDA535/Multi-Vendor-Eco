using Shop111.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Shop111.Constants;

namespace Shop111.Models.DTOs
{
    public class UpdateOrderStatusModel
    {
        public int OrderId { get; set; }

        [Required]
        public EOrderStatus OrderStatus { get; set; }

        public IEnumerable<SelectListItem>? OrderStatusList { get; set; }

    }
}

