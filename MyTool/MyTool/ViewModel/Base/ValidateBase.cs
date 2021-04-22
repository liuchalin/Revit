using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace MyTool.ViewModel
{
    public abstract class ValidateBase : INotifyPropertyChanged, IDataErrorInfo
    {
        public string this[string columnName]
        {
            get
            {
                var validationResults = new List<ValidationResult>();
                var property = GetType().GetProperty(columnName);
                Contract.Assert(property != null);
                var validationContext = new ValidationContext(this) { MemberName = columnName };
                var isValid = Validator.TryValidateProperty(property.GetValue(this), validationContext, validationResults);
                if (isValid)
                {
                    return null;
                }
                return string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage).ToArray());
            }
        }

        public string Error
        {
            get
            {
                var propertyInfos = GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                foreach (var propertyInfo in propertyInfos)
                {
                    var errorMsg = this[propertyInfo.Name];
                    if (errorMsg != null)
                    {
                        return errorMsg;
                    }
                }
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(property));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public virtual bool IsEnable { get; set; }
    }
}
