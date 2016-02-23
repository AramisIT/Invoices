using Aramis.Attributes;

namespace Catalogs
    {
    [Catalog(Description = "Пользователи", GUID = "AEBA6AB9-0677-4046-81D8-A784115993EA", AllowEnterByPattern = true, UseDescriptionSpellCheck = true, ShowCodeFieldInForm = false)]
    public interface IUsers : ICatalogUsers
        {
        [DataField(Description = "Электронный адрес", ShowInList = true)]
        string Email { get; set; }
        }
    }
