
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KFP.DATA
{
    public class ModelBase : ObservableObject, IDataErrorInfo
    {
        public string this[string columnName]
        {
            get
            {
                return OnValidate(columnName);
            }
        }

        [NotMapped]
        public string Error
        {
            get
            {
                return null;
            }
        }


        public virtual string? OnValidate(string propertyName)
        {
            var context = new ValidationContext(this)
            {
                MemberName = propertyName
            };

            var results = new Collection<ValidationResult>();
            var isValid = Validator.TryValidateProperty(this.GetType().GetProperty(propertyName).GetValue(this), context, results);
            return !isValid ? results[0].ErrorMessage : null;
        }

        public bool IsValid()
        {
            var context = new ValidationContext(this);
            var results = new Collection<ValidationResult>();

            var isValid = Validator.TryValidateObject(this, context, results, true);

            return isValid;
        }
    }
}
