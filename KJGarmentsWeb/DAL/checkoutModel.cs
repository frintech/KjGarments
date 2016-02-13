using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace KJGarmentsWeb.DAL
{
    public class OneOfTwoRequired : ValidationAttribute, IClientValidatable
    {
        private const string defaultErrorMessage = "{0} or {1} is required.";

        private string otherProperty;

        public OneOfTwoRequired(string otherProperty)
            : base(defaultErrorMessage)
        {
            if (string.IsNullOrEmpty(otherProperty))
            {
                throw new ArgumentNullException("otherProperty");
            }

            this.otherProperty = otherProperty;
        }

        public override string FormatErrorMessage(string name)
        {
           return string.Format(ErrorMessageString, name, otherProperty);
           
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo otherPropertyInfo = validationContext.ObjectInstance.GetType().GetProperty(otherProperty);

            if (otherPropertyInfo == null)
            {
                return new ValidationResult(string.Format("Property '{0}' is undefined.", otherProperty));
            }

            var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (otherPropertyValue == null && value == null)
            {
               // return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName)); 
                return new ValidationResult("Please enter either your Mobile No or Email ID");
            }

            return ValidationResult.Success;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = "Please enter either your Mobile No or Email ID",//FormatErrorMessage(metadata.DisplayName),
                //This is the name of the method aaded to the jQuery validator method (must be lower case)
                ValidationType = "oneoftworequired"
            };

        }
    }
}