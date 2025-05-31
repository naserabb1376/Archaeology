using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class CustomerRegisterModel
    {
        public String TypeSearchName { get; set; }

        public CustomerRegisterModel()
        {
            customerlist = new List<customerlist>();
            pagenumbers = new List<int>();
        }

        public List<int> pagenumbers { get; set; }
        public int page { get; set; }
        public IList<customerlist> customerlist { get; set; }
    }

    public class customerlist
    {
      
        public IList<SelectListItem> RoleList { get; set; }
        public string time { get; set; }

        public customerlist()
        {
         
            RoleList = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "نام را وارد نمایید")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "نام خانوادگی را وارد نمایید")]
        public string LastName { get; set; }



        [Display(Name = "کلمه کاربری")]
        [Remote("CheckUserName", "Account", "Admin", ErrorMessage = "این کلمه کاربری قبلا استفاده شده است", HttpMethod = "Post")]
        [Required(ErrorMessage = "کلمه کاربری خانوادگی را وارد نمایید")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "رمز عبور را وارد نمایید")]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        //[Display(Name = "تایید رمز عبور")]
        //[Required(ErrorMessage = "رمز عبور مجدد را وارد نمایید")]
        //[Compare("Password", ErrorMessage = "رمز عبور ها یکسان نیست")]
        //public string ConfirmPassword { get; set; }

        //[EmailAddress(ErrorMessage = "فرمت پست الکترونیکی  را  صحیح وارد نمایید")]

        [Display(Name = "تلفن ثابت")]
        public string Telephone { get; set; }

        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Display(Name = "آدرس کاربر")]
        public string Street { get; set; }

        [Display(Name = "نام نقش")]
        public string RoleName { get; set; }

        [Display(Name = "ایدی نقش")]
        public int RoleId { get; set; }

        [Display(Name = "نام شهر")]
        public string CityName { get; set; }

        public string type { get; set; }

        public string TimeOfLogin { get; set; }
    }

    public class CustomerLoginModel
    {
        public string returnUrl { get; set; }

        [Display(Name = "کلمه کاربری")]
        [Required(ErrorMessage = "کلمه کاربری خانوادگی را وارد نمایید")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "رمز عبور را وارد نمایید")]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بیاور")]
        public bool RememberMe { get; set; }
    }
}