﻿using GUI.FileBase;

namespace GUI.Models.DTOs.Product_DTO
{
    public class GetListProductRequest : BaseRequest
    {
        public string? Name { get; set; }
        public Guid? CategoryID { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public int? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? OffSet { get; set; } = 0;
        public int? Limit { get; set; }
    }
}
