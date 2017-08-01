using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamplePushNotification.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {


        #region INotifyPropertyChanged Member

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region INotifyDataErrorInfo Member
        public bool HasErrors => (_validationErrors.Count > 0);

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_validationErrors.ContainsKey(propertyName))
            {
                return null;
            }

            return _validationErrors[propertyName];
        }

        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private readonly Dictionary<string, ICollection<string>> _validationErrors = new Dictionary<string, ICollection<string>>();

        #endregion

        /* Alternative solution using LINQ */
        protected bool ValidateModelProperty(object value, string propertyName)
        {
            bool isValid = true;

            if (_validationErrors.ContainsKey(propertyName))
                _validationErrors.Remove(propertyName);

            PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName);
            //IList<string> validationErrors = (from validationAttribute in propertyInfo.GetCustomAttributes(true).OfType<ValidationAttribute>()
            //                                  where !validationAttribute.IsValid(value)
            //                                  select validationAttribute.FormatErrorMessage(string.Empty)).ToList();

            var validationErrors = propertyInfo.GetCustomAttributes(true).OfType<ValidationAttribute>()
                .Where(v => !v.IsValid(value))
                .Select(v => v.FormatErrorMessage(string.Empty))
                .ToList();

            if (validationErrors != null && validationErrors.Count > 0)
            {
                _validationErrors.Add(propertyName, validationErrors);

                isValid = false;
            }

            OnErrorsChanged(propertyName);

            return isValid;
        }

        protected void ValidateModel()
        {
            _validationErrors.Clear();
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(this, null, null);
            if (!Validator.TryValidateObject(this, validationContext, validationResults, true))
            {
                foreach (ValidationResult validationResult in validationResults)
                {
                    string property = validationResult.MemberNames.ElementAt(0);
                    if (_validationErrors.ContainsKey(property))
                    {
                        _validationErrors[property].Add(validationResult.ErrorMessage);
                    }
                    else
                    {
                        _validationErrors.Add(property, new List<string> { validationResult.ErrorMessage });
                    }
                }
            }

            /* Raise the ErrorsChanged for all properties explicitly */
            //RaiseErrorsChanged("Username");
            //RaiseErrorsChanged("Name");

            foreach (var property in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.SetField | BindingFlags.GetField | BindingFlags.Instance))
            {
                ValidateModelProperty(property.GetValue(this), property.Name);
            }
        }

    }
}
