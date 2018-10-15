using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace API.Models.Validations
{
    public class UniqueUserAttribute : ValidationAttribute
    {
        public UniqueUserAttribute()
        {
            ErrorMessage = "user name nust be unique";
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = ValidationResult.Success;
            try
            {
                string userName = value.ToString();
                bool isUnique = DB.Users.FirstOrDefault(user => user.UserName.Equals(userName)) == null;
                if (isUnique == false)
                    validationResult = new ValidationResult(ErrorMessageString);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return validationResult;
        }
    }
}