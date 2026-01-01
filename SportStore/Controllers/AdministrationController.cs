using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Services.IServices;
using SportStore.ViewModels.ProductVM;
using SportStore.ViewModels.Role;
using System.Runtime.CompilerServices;

namespace SportStore.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministrationController : Controller
    {
        private readonly IStoreRepository storeRepository;
        private readonly IOrderRepository orderRepository;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AdministrationController(IStoreRepository storeRepository, IOrderRepository orderRepository, RoleManager<IdentityRole> roleManager, 
                                        UserManager<ApplicationUser> userManager,IWebHostEnvironment webHostEnvironment)
        {
            this.storeRepository = storeRepository;
            this.orderRepository = orderRepository;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.RoleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if (role != null)
            {
                var model = new List<UserRoleViewModel>();
                

                foreach (var user in userManager.Users)
                {
                    var userRoleViewModel = new UserRoleViewModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName
                    };

                    userRoleViewModel.Selected = (await userManager.IsInRoleAsync(user, role.Name)) ? true : false;
                    
                    model.Add(userRoleViewModel);

                }
                return View(model);

            }
            else
            {
                ViewBag.ErrorMessage = $"Role with id {roleId} cannot be found";
                return View("NotFound");
            }

        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role != null)
            {
                for (int i = 0; i < model.Count; i++)
                {
                    var user = await userManager.FindByIdAsync(model[i].UserId);

                    IdentityResult? result = null;
                    //check if  selected and not in role
                    if (model[i].Selected && !(await userManager.IsInRoleAsync(user, role.Name)))
                    {
                        // add that user to the role
                        result = await userManager.AddToRoleAsync(user, role.Name);

                    }
                    //check if not selected and  in role
                    else if (!model[i].Selected && await userManager.IsInRoleAsync(user, role.Name)) 

                    {
                        result = await userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                    else { continue; }//when selected and in role And when not selected and not in role, do nothing

                    if (result.Succeeded)
                    {
                        if (i < model.Count - 1)
                            continue;
                        else
                            return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }

                return RedirectToAction("EditRole", new { Id = roleId });

            }
            else
            {
                ViewBag.ErrorMessage = $"Role with id {roleId} cannot be found";
                return View("NotFound");
            }
            return View();
        }

            [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var editRole = await roleManager.FindByIdAsync(id);

            if (editRole != null)
            {
                var model = new EditRoleViewModel
                {
                    Id = editRole.Id,
                    RoleName = editRole.Name
                };

                foreach (var user in userManager.Users)
                {
                    if (await userManager.IsInRoleAsync(user, editRole.Name))
                    {
                        model.Users.Add(user.UserName);
                    }
                }
                return View(model);

            }
            else
            {
                ViewBag.ErrorMessage = $"Role with id {editRole?.Id} cannot be found";
                return View("NotFound");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var Role = await roleManager.FindByIdAsync(model.Id);

            if (Role != null)
            {
                Role.Name = model.RoleName;

                var result = await roleManager.UpdateAsync(Role);

                if (result.Succeeded)
                {
                    return RedirectToAction("listrole");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            else
            {
                ViewBag.ErrorMessage = $"Role with id {model.Id} cannot be found";
                return View("NotFound");
            }

            
        }

        [HttpGet]
        public IActionResult ListRole()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoleAsync(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                var result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "home");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        

        public IActionResult ManageOrders(int orderId)
        {
            if(orderId != 0)
            {
                Order order = orderRepository.GetOrder(orderId);
                if (order == null)
                {
                    //take the user to a NotFound page
                }
                order.Shipped = true;
                orderRepository.SaveOrder(order);
            }

            var model = orderRepository.Orders.Where(o => !o.Shipped) ;
            return View(model);
        }

        public IActionResult ManageShippedOrders()
        {
            var model = orderRepository.Orders.Where(o => o.Shipped);
            return View(model);
        }
    }
}
