using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Order_DTO;
using DATN_ACV_DEV.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DATN_ACV_DEV.Controllers.Order;

[ApiController]
public class OrderDetailAdminController : ControllerBase
{
    private readonly DBContext _context;

    public OrderDetailAdminController(DBContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("/api/admin/orders/{id}")]
    public async Task<IActionResult> Proccess([FromRoute] string id)
    {
        var order = await _context.TbOrders.AsNoTracking()
            .Include(e => e.Customer)
            .Include(e => e.TbOrderDetails)
            .ThenInclude(e => e.Product)
            .ThenInclude(e => e.Image)
            .Include(e => e.AddressDelivery)
            .Select(e => new OrderDetail()
            {
                Id = e.Id,
                Customer = new CustomerInfo
                {
                    Id = e.Customer.Id,
                    Name = e.Customer.Name,
                    Address = e.Customer.Adress,
                    PhoneNumber = e.Customer.Phone
                },
                ShippingInfo = e.AddressDelivery == null ? null : new ShippingInfo
                {
                    Name = e.AddressDelivery!.ReceiverName,
                    PhoneNumber = e.AddressDelivery!.ReceiverPhone,
                    Address = $"{e.AddressDelivery.WardName}, {e.AddressDelivery.DistrictName}, {e.AddressDelivery.ProvinceName}"
                },
                PaymentInfo = new PaymentInfo
                {
                    TotalDiscount = 0,
                    ShippingFee = e.AmountShip ?? 0,
                    TotalTax = e.TotalAmount == 0 ? 0 : e.TotalAmount * 10 / 100,
                    TotalAmount = e.TotalAmount,
                    Status = e.Status ?? 0
                },
                IsCustomerTakeYourSelf = e.IsCustomerTakeYourself,
                IsSameAsCustomerAddress = e.IsShippingAddressSameAsCustomerAddress,
                PaymentMethodName = "Chuyển khoản ngân hàng",
                VoucherDiscountAmount = 0,
                StatusText = Common.ConvertStatusOrder(e.Status ?? 0),
                Status = e.Status ?? 0,
                Items = e.TbOrderDetails.Select(d => new OrderItem()
                {
                    Id = d.Id,
                    Price = d.Product.Price,
                    Quantity = d.Quantity,
                    ProductImage = d.Product.Image.Url,
                    ProductName = d.Product.Name
                })
            }).FirstOrDefaultAsync(e => e.Id == Guid.Parse(id));

        if(order != null && order.ShippingInfo == null)
            order.ShippingInfo = ShippingInfo.Default();

        return Ok(new BaseResponse<OrderDetail>()
        {
            Data = order ?? new OrderDetail()
        });
    }
}