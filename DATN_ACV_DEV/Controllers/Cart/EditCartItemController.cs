﻿using AutoMapper;
using DATN_ACV_DEV.Controllers.Condition;
using DATN_ACV_DEV.Entity;
using DATN_ACV_DEV.FileBase;
using DATN_ACV_DEV.Model_DTO.Cart_DTO;
using DATN_ACV_DEV.Model_DTO.Category_DTO;
using DATN_ACV_DEV.Model_DTO.HomePage;
using DATN_ACV_DEV.Model_DTO.Image_DTO;
using DATN_ACV_DEV.Model_DTO.Product_DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DATN_ACV_DEV.Controllers
{
    [Route("api/EditCartItem")]
    [ApiController]
    public class EditCartItemController : ControllerBase, IBaseController<EditCartRequest, EditCartResponse>
    {
        private readonly DBContext _context;
        private EditCartRequest _request;
        private readonly IMapper _mapper;
        private BaseResponse<EditCartResponse> _res;
        private EditCartResponse _response;
        private TbCart _Cart;
        private TbCartDetail _CartDetail;
        private bool checkCart = false;
        private string _apiCode = "EditCartItem";
        private string _conC01 = "C01";
        private string _conC02 = "C02";
        private string _conC01Field = "ProductID";
        private string _conC02Field = "Quantity";
        public EditCartItemController(DBContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _res = new BaseResponse<EditCartResponse>()
            {
                Status = StatusCodes.Status200OK.ToString(),
                Data = null
            };
            _response = new EditCartResponse();
        }

        public void AccessDatabase()
        {
            _context.Update(_CartDetail);
            _context.SaveChanges();
			      _response.quantity = _request.Quantity ?? 0;
			      _response.price = _CartDetail.Product.Price;
            _res.Data = _response;
        }

        public void CheckAuthorization()
        {

            if (_request.LoginType)
            {
                _request.Authorization(_context, _apiCode);
            }
            else
            {
                _request.AuthorizationCustomer(_context, _apiCode);
            }
        }

        public void GenerateObjects()
        {
			_CartDetail = _context.TbCartDetails.Where(c => c.Id == _request.CartDetaiID).FirstOrDefault();
			if (_CartDetail != null)
			{
				if (_request.Quantity != null)
				{
					if (_request.IsIncrement)
					{
						_CartDetail.Quantity += _request.Quantity;
					}
					else
					{
						_CartDetail.Quantity = _request.Quantity;
					}
                    _request.Quantity = _CartDetail.Quantity;
					if (_CartDetail.Quantity <= 0)
					{
						throw new Exception("Invalid");
					}
				}
			}
		}

        public void PreValidation()
        {
            #region sản phẩm trong giỏ hàng không tồn tại 
            ConditionCart.EditCartItem_C01(_context, _request, _apiCode, _conC02, _conC02Field);
            #endregion

            #region Số lượng sản phẩm không đủ 
            ConditionCart.EditCartItem_C02(_context, _request, _apiCode, _conC02, _conC02Field);
            #endregion
        }
        [HttpPost]
        [Route("Process")]
        public BaseResponse<EditCartResponse> Process(EditCartRequest request)
        {
            try
            {
                _request = request;
                //CheckAuthorization();
                GenerateObjects();
                PreValidation();
                //PostValidation();
                AccessDatabase();
            }
            catch (ACV_Exception ex0)
            {
                _res.Status = StatusCodes.Status400BadRequest.ToString();
                _res.Messages = ex0.Messages;
            }
            catch (Exception ex)
            {
                _res.Status = StatusCodes.Status500InternalServerError.ToString();
                _res.Messages.Add(Message.CreateErrorMessage(_apiCode, _res.Status, ex.Message, string.Empty));
            }
            return _res;

        }
    }
}
