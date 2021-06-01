using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CWBFightClub.Models
{
    public enum PaymentPeriod
    {
        Monthly = 0,
        [Display(Name = "Quarterly")]
        [Description("Quarterly")]
        ThreeMonth = 1,
        Yearly = 2
    }

    public static class EnumHelper
    {
        /// <summary>
        /// Given an enum value, if a <see cref="DescriptionAttribute"/> attribute has been defined on it, then return that.
        /// Otherwise return the enum name.
        /// </summary>
        /// <typeparam name="T">Enum type to look in</typeparam>
        /// <param name="value">Enum value</param>
        /// <returns>Description or name</returns>
        public static string ToDescription<T>(this T value) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("ToDescription only works on enums.");
            }
            var fieldName = Enum.GetName(typeof(T), value);
            if (fieldName == null)
            {
                return string.Empty;
            }
            var fieldInfo = typeof(T).GetField(fieldName, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
            if (fieldInfo == null)
            {
                return string.Empty;
            }
            var descriptionAttribute = (DescriptionAttribute)fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
            if (descriptionAttribute == null)
            {
                return fieldInfo.Name;
            }
            return descriptionAttribute.Description;
        }
    }    
}
