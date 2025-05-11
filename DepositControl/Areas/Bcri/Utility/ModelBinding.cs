using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DepositControl.Areas.Bcri.Utility
{

    public class DecimalConverterEx : DecimalConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string))
                return ConvertFrom(value);

            culture = culture ?? CultureInfo.CurrentUICulture;

            string text = ((string)value).Trim();
            NumberFormatInfo formatInfo = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));

            return decimal.Parse(text, NumberStyles.Number, formatInfo);
        }
    }
    public class DoubleConverterEx : DoubleConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string))
                return base.ConvertFrom(value);

            culture = culture ?? CultureInfo.CurrentUICulture;

            string text = ((string)value).Trim();
            NumberFormatInfo formatInfo = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));

            return double.Parse(text, NumberStyles.Number, formatInfo);
        }
    }


    
    public class BooleanConverterEx : BooleanConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string))
                return ConvertFrom(value);

            var val = ((string)value).ToLower().Trim();
            if (val == "true" || val == "on" || val == "yes" || val == "si" || val == "1")
                return true;
            if (val == "false" || val == "off" || val == "no" || val == "0")
                return false;
            if (string.IsNullOrEmpty(val) || val == "null" || val == "undefined" || val == "-1")
                return null;
            return base.ConvertFrom(value);
        }
    }


    public class BoolBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                                    .GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                string val = valueResult?.AttemptedValue?.ToLower().Trim();

                // Si no es nada, continuo con el camino normal
                if (val == null)
                {
                    return val;
                }

                if (!string.IsNullOrEmpty(val) && val.Contains(","))
                    val = val.Split(',')[0].Trim();

                actualValue = val == "true"
                              || val == "on"
                              || val == "yes"
                              || val == "si"
                              || val == "1";
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
    public class BoolNullableBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                                    .GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                string val = valueResult?.AttemptedValue?.ToLower().Trim();

                if (!string.IsNullOrEmpty(val) && val.Contains(","))
                    val = val.Split(',')[0].Trim();

                if (val == "true" || val == "on" || val == "yes" || val == "si" || val == "1")
                    actualValue = true;
                else if (val == "false" || val == "off" || val == "no" || val == "0")
                    actualValue = false;
                else if (string.IsNullOrEmpty(val) || val == "null" || val == "undefined" || val == "-1")
                    actualValue = null;
                else
                    throw new Exception("A bool must be: true|false, on|off, yes:no, 1|0, null undefined -1");
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }

    public class DecimalBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                                    .GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                actualValue = Convert.ToDecimal(valueResult.AttemptedValue.Replace("$", "").Trim(),
                    CultureInfo.CurrentUICulture);
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }

    public class DecimalNullableBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider
                                    .GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(valueResult.AttemptedValue))
                    actualValue = Convert.ToDecimal(valueResult.AttemptedValue.Replace("$", "").Trim()
                                        , CultureInfo.CurrentUICulture);

            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}

