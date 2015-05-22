using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Attributes;
using Aramis.Core;

namespace SystemInvoice
    {
    public class SolutionRoles : AramisRoles
        {
        public override void AddPermissionsToNotAuthorizedRoles(AramisRoleDefinition role)
            {
            }

        [DataField(Description = "Загрузка базы товаров Электролюкс")]
        public static bool ElectroluxLoader
            {
            get { return checkRole(() => ElectroluxLoader); }
            }
        }
    }
