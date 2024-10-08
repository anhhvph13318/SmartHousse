﻿using GUI.Models.DTOs.Order_DTO;
using Newtonsoft.Json;
using System.Security.Claims;

namespace GUI
{
    public class UserSession
    {
        private readonly IHttpContextAccessor _http;

        public UserSession(IHttpContextAccessor http)
        {
            _http = http;
        }

        public Guid UserId
        {
            get
            {
                string id = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception();
                return Guid.Parse(id);
            }
        }
    }

    public static class Extension
    {
        private const string CURRENT_ORDER = "CURRENT_ORDER";

        public static OrderDetail GetCurrentOrder(this ISession session, bool clearFirst = false)
        {
            var json = session.GetString(CURRENT_ORDER);
            if (string.IsNullOrEmpty(json) || clearFirst)
            {
                var order = new OrderDetail()
                {
                    Id = Guid.Empty,
                    Code = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    ShippingInfo = new ShippingInfo(),
                    PaymentInfo = new PaymentInfo(),
                    Customer = new CustomerInfo(),
                    Items = new List<OrderItem>()
                };

                session.SaveCurrentOrder(order);
                return order;
            }

            return JsonConvert.DeserializeObject<OrderDetail>(json) ?? throw new NullReferenceException();
        }

        public static void SaveCurrentOrder(this ISession session, OrderDetail order)
        {
            session.SetString(CURRENT_ORDER, JsonConvert.SerializeObject(order));
        }
    }
}
