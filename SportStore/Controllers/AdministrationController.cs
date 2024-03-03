using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using SportStore.Models.ViewModels;

namespace SportStore.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministrationController : Controller
    {
        private readonly IStoreRepository storeRepository;
        private readonly IOrderRepository orderRepository;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public AdministrationController(IStoreRepository storeRepository, IOrderRepository orderRepository, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.storeRepository = storeRepository;
            this.orderRepository = orderRepository;
            this.roleManager = roleManager;
            this.userManager = userManager;
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

        public IActionResult DeleteProduct(long id)
        {
            Product? product = storeRepository.GetProductById(id);
            if(product == null)
            {
                //tell the admin that the product does not exist and return them to the avilable lisst of products
            }
            storeRepository.DeleteProduct(product);

            ProductsListViewModel model = new ProductsListViewModel()
            {
                Products = storeRepository.AllProducts
            };
           

            return View("ManageProducts", model);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Product product = new()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Category = model.Cartegory,
                    Price = model.Price
                };

                storeRepository.CreateProduct(product);
                return RedirectToAction("Details", new { id = product.ProductID });
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(long id)
        {
            Product? product = storeRepository.GetProductById(id);
            if (product == null)
            {
                //direct to ProductNotFound page and then to the admin llst of all products
            }
            ProductEditViewModel productEditViewModel = new()
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                Cartegory = product.Category,
                Price = product.Price
            };

            return View(productEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(ProductEditViewModel model)
        {

            if (ModelState.IsValid)
            {
                Product? editedProduct = storeRepository.GetProductById(model.ProductID); 
                if (editedProduct != null)
                {
                    editedProduct.Name = model.Name;
                    editedProduct.Description = model.Description;
                    editedProduct.Category = model.Cartegory;
                    editedProduct.Price = model.Price;
                }
                storeRepository.UpdateProduct(editedProduct);
                return RedirectToAction("Details", new {id = model.ProductID});
            }
            return View();
        }

        public IActionResult Details(long id)
        {
           Product? product = storeRepository.GetProductById(id);
            if (product == null)
            {
                //direct to ProductNotFound page and then to the admin llst of all products
            }

            return View(product);
        }

        public IActionResult ManageProducts()
        {
            var model = new ProductsListViewModel
            {
                Products = storeRepository.AllProducts
            };
            return View(model);
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

            var model = orderRepository.Orders.Where(o => !o.Shipped).ToList() ;
            return View(model);
        }

        public IActionResult ManageShippedOrders()
        {
            var model = orderRepository.Orders.Where(o => o.Shipped).ToList();
            return View(model);
        }
    }
}
