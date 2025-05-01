using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SilkSareeEcommerce.Models;
using SilkSareeEcommerce.Services;

namespace SilkSareeEcommerce.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyAddresses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // ✅ Correct method that returns List<SavedAddress>
            var addresses = await _userService.GetListSavedAddressesAsync(userId);

            return View(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> SetDefaultAddress(int addressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var addresses = await _userService.GetListSavedAddressesAsync(userId);

            foreach (var address in addresses)
            {
                address.IsDefault = address.Id == addressId;
            }

            await _userService.UpdateSavedAddressesAsync(addresses);

            TempData["Success"] = "Default address updated successfully!";
            return RedirectToAction("MyAddresses");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var address = await _userService.GetListSavedAddressesAsync(userId);
            var addressToDelete = address.FirstOrDefault(a => a.Id == addressId);

            if (addressToDelete != null)
            {
                await _userService.DeleteAddressAsync(addressId);
                TempData["Success"] = "Address deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Address not found!";
            }

            return RedirectToAction("MyAddresses");
        }

        [HttpGet]
        public async Task<IActionResult> EditAddress(int addressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var address = await _userService.GetListSavedAddressesAsync(userId);
            var addressToEdit = address.FirstOrDefault(a => a.Id == addressId);

            if (addressToEdit == null)
            {
                TempData["Error"] = "Address not found!";
                return RedirectToAction("MyAddresses");
            }

            return View(addressToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> EditAddress(SavedAddress model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"❌ Field: {key}, Error: {error.ErrorMessage}");
                    }
                }

                return View(model);
            }

            await _userService.UpdateAddressAsync(model);
            TempData["Success"] = "Address updated successfully!";
            return RedirectToAction("MyAddresses");
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found!";
                return RedirectToAction("Index");
            }
            return View(user);
        }


        [HttpPost]
        public async Task<IActionResult> EditProfile(ApplicationUser model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID

            if (!ModelState.IsValid)
                return View(model);

            await _userService.UpdateUserAsync(model, userId); // Pass the userId
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }



        //[HttpPost]
        //public async Task<IActionResult> EditProfile(ApplicationUser model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    await _userService.UpdateUserAsync(model);
        //    TempData["Success"] = "Profile updated successfully!";
        //    return RedirectToAction("Index");
        //}



    }
}
