using AutoMapper;
using FerozeHub.Services.ShoppingAPI.Data;
using FerozeHub.Services.ShoppingAPI.Models;
using FerozeHub.Services.ShoppingAPI.Models.Dto;
using FerozeHub.Services.ShoppingAPI.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FerozeHub.Services.ShoppingAPI.Controllers;

[Route("api/cartAPI")]
[ApiController]

public class CartAPIController(ApplicationDbContext dbContext, IMapper mapper,IProductService productService,ICouponService couponService,IEmailService emailService) : BaseController
{
    [HttpGet("GetCart/{userId}")]
    public async Task<IActionResult> GetCartAsync(string userId)
    {
        try
        {
            CartDto cart = new()
            {
                CartHeader = mapper.Map<CartHeaderDto>(dbContext.CartHeaders.First(u => u.UserId == userId))
            };
            cart.CartDetails =
                mapper.Map<IEnumerable<CartDetailsDto>>(dbContext.CartDetails.Where(u =>
                    u.CartHeaderId == cart.CartHeader.CartHeaderId));
            IEnumerable<ProductDto> productsDtos =await productService.GetProductsAsync();
            foreach (var item in cart.CartDetails)
            {
                item.Product=productsDtos.FirstOrDefault(u=>u.ProductId == item.ProductId);
                cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
            }

            if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
            {
                CouponDto couponDto = await couponService.GetCoupon(cart.CartHeader.CouponCode);
                if (couponDto != null && cart.CartHeader.CartTotal > couponDto.MinAmount)
                {
                    cart.CartHeader.CartTotal -= couponDto.DiscountAmount;
                    cart.CartHeader.Discount=couponDto.DiscountAmount;
                }
            }
            
            return CreateResponse(cart,true,"You Cart Retrieved Successfully");

        }
        catch (Exception exception)
        {
            return CreateResponse(null,false,"Error on retrieving cart"+exception.Message);
        }
    }

    [HttpPost("ApplyCoupon")]
    public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
    {
        try
        {
            var cartfromDb = await dbContext.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
            cartfromDb.CouponCode=cartDto.CartHeader.CouponCode;
            dbContext.CartHeaders.Update(cartfromDb);
            await dbContext.SaveChangesAsync();
            return CreateResponse(null, true, "Success on applying coupon");
        }
        catch (Exception exception)
        {
            return CreateResponse(null,false,"Error on applying coupon"+exception.Message);
        }
    }
    
    [HttpPost("RemoveCoupon")]
    public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
    {
        try
        {
            var cartfromDb = await dbContext.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
            cartfromDb.CouponCode="";
            dbContext.CartHeaders.Update(cartfromDb);
            await dbContext.SaveChangesAsync();
            return CreateResponse(null, true, "Success on removing coupon");
        }
        catch (Exception exception)
        {
            return CreateResponse(null,false,"Error on removing coupon"+exception.Message);
        }
    }

    [HttpPost("CartUpsert")]
    public async Task<IActionResult> CartUpSert(CartDto cartDto)
    {
        try
        {
            // Check if the CartHeader exists for the user
            var existingCartHeader = await dbContext.CartHeaders.AsNoTracking()
                .FirstOrDefaultAsync(ch => ch.UserId == cartDto.CartHeader.UserId);

            if (existingCartHeader == null)
            {
                // Scenario 1: User Adds First Item to Cart
                CartHeader newCartHeader = mapper.Map<CartHeader>(cartDto.CartHeader);

                dbContext.CartHeaders.Add(newCartHeader);
                await dbContext.SaveChangesAsync();
                cartDto.CartDetails.First().CartHeaderId = newCartHeader.CartHeaderId;
                dbContext.CartDetails.Add(mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                await dbContext.SaveChangesAsync();
            }
            else
            {
                    var  existingCartDetails = await dbContext.CartDetails.AsNoTracking()
                        .FirstOrDefaultAsync(cd => cd.CartHeaderId == existingCartHeader.CartHeaderId
                                              && cd.ProductId == cartDto.CartDetails.First().ProductId);

                    if (existingCartDetails == null)
                    {
                        // User Adds New Item to Cart
                        cartDto.CartDetails.First().CartHeaderId = existingCartHeader.CartHeaderId;
                        dbContext.CartDetails.Add(mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        // User Updates Quantity of an Existing Item
                        cartDto.CartDetails.First().Count += existingCartDetails.Count;
                        cartDto.CartDetails.First().CartHeaderId = existingCartDetails.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = existingCartDetails.CartDetailsId;
                        dbContext.CartDetails.Update(mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await dbContext.SaveChangesAsync();

                    }
                return CreateResponse(cartDto,true,"CartUpsert Success");
            }
            return CreateResponse(cartDto, true, "Cart");
        }
        catch (Exception ex)
        {
            return CreateResponse(null,false,"Error updating the cart. " + ex.Message);
        }
    }
    [HttpPost("RemoveCart")]
    public async Task<IActionResult> RemoveCart([FromBody]int  cartDetailsId)
    {
        try
        {
            var cartDetails = dbContext.CartDetails.First(u => u.CartDetailsId == cartDetailsId);
            
            int totalCountofCartItem=dbContext.CartDetails.Where(u=>u.CartHeaderId==cartDetails.CartHeaderId).Count();
            dbContext.CartDetails.Remove(cartDetails);
            if (totalCountofCartItem == 1)
            {
                var cartHeaderToRemove =
                   await dbContext.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                dbContext.CartHeaders.Remove(cartHeaderToRemove);
            }

            dbContext.SaveChangesAsync();
            return CreateResponse(null, true, "CartRemoved Successfully");
        }
        catch (Exception ex)
        {
            return CreateResponse(null,false,"Error Removing Cart. " + ex.Message);
        }
    }

    [HttpPost("EmailCartRequest")]
    public async Task<IActionResult> CreateEmailCart([FromBody] CartDto cartDto)
    {
        emailService.EmailCartSend(cartDto);
        return CreateResponse(null,true,"Email");
    }
}
