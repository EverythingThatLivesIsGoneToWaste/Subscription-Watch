using subscription_watch.Enums;
using System.ComponentModel.DataAnnotations;

namespace subscription_watch.DTOs
{
    public class PlanCreateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal DefaultPrice { get; set; }
        [Required]
        public Currency DefaultCurrency { get; set; }
        [Required]
        public BillingPeriod BillingPeriod { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
