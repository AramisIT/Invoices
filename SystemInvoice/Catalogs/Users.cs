using Aramis.Attributes;
using Aramis.Enums;
using Catalogs;

namespace Catalogs
    {
    [Catalog( Description = "Пользователи", GUID = "AEBA6AB9-0677-4046-81D8-A784115993EA", AllowEnterByPattern = true, UseDescriptionSpellCheck = true, ShowCodeFieldInForm = false )]
    public class Users : CatalogUsers
        {
        #region string Email (Электронный адрес)

        [DataField( Description = "Электронный адрес", ShowInList = true )]
        public string Email
            {
            get
                {
                return z_Email;
                }
            set
                {
                if (z_Email != value)
                    {
                    z_Email = value;
                    NotifyPropertyChanged( "Email" );
                    }
                }
            }

        private string z_Email = "";

        #endregion

        public Users()
            : base()
            {
            }

        }
    }
