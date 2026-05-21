using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop111.Repositories;
using System.Security.Claims;

namespace Shop111.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepo;

        public UserOrderController(IUserOrderRepository userOrderRepo)
        {
            _userOrderRepo = userOrderRepo;
        }
        public async Task<IActionResult> UserOrders()
        {
            var orders = await _userOrderRepo.UserOrders();
            return View(orders);
        }
    }
}


//namespace CardCore.Controllers
//{
//    [Authorize]
//    public class UserOrderController : Controller
//    {
//        private readonly ApplicationDbContext _db;

//        public UserOrderController(ApplicationDbContext db)
//        {
//            _db = db;
//        }

//        public IActionResult UserOrders()
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            var orders = _db.Orders
//                .Where(o => o.UserId == userId && !o.IsDeleted)
//                .Include(o => o.OrderDetail)
//                .ThenInclude(d => d.Product)
//                .ThenInclude(p => p.Genre)
//                .ToList();

//            return View(orders);
//        }
//    }
//}
