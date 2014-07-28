using System;
using System.Windows.Forms;
using Aramis.Core;
using Aramis.DatabaseConnector;
using Aramis.Enums;
using Aramis.Platform;
using Aramis.SystemConfigurations;
using Aramis.UI;
using Aramis.UI.WinFormsDevXpress;
using Catalogs;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

namespace Aramis.CommonForms
    {

    [Aramis.Attributes.View( DBObjectGuid = "AEBA6AB9-0677-4046-81D8-A784115993EA", ViewType = ViewFormType.CatalogItem )]
    public partial class UsersItemForm : DevExpress.XtraBars.Ribbon.RibbonForm, IItemForm
        {

        private const string WARNING_1 = "Мобильные номера должны быть уникалыми для каждого пользователя.\r\nВведенный номер уже указан для пользователя \"{0}\"!";

        #region Поля и свойства

        private Users item;

        public IDatabaseObject Item
            {
            get
                {
                return item;
                }
            set
                {
                item = ( Users ) value;
                }
            }

        public Users User
            {
            get
                {
                return ( Users ) item;
                }
            }

        #endregion

        #region Event handling

        private void Itemform_Load(object sender, EventArgs e)
            {
            if ( !User.IsNew && User.MobilePhone > 0 )
                {
                string mobileNum = User.MobilePhone.ToString();
                stringMobilePhone.Text = string.Format("+{0} ({1}) {2}-{3}-{4}", mobileNum.Substring(0, 2), mobileNum.Substring(2, 3), mobileNum.Substring(5, 3), mobileNum.Substring(8, 2), mobileNum.Substring(10, 2));
                }
            }

        public UsersItemForm()
            {
            InitializeComponent();
            }

        private void TryCancel()
            {
            Close();
            }

        private void Itemform_KeyDown(object sender, KeyEventArgs e)
            {
            if ( e.KeyCode == Keys.Escape )
                {
                TryCancel();
                }
            else if ( e.KeyCode == Keys.Enter && e.Control )
                {
                OK_ItemClick(null, null);
                }
            else if ( e.KeyCode == Keys.S && e.Control )
                {
                Write_ItemClick(null, null);
                }

            }

        #endregion

        private bool Write()
            {
            if ( !SetMobileNumber() )
                {
                return false;
                }

            return Item.Write() == WritingResult.Success;
            }

        private bool SetMobileNumber()
            {
            string mobileNum = stringMobilePhone.Text.Replace(" ", "").Replace("_", "").Replace("+", "").Replace("(", "").Replace(")", "").Replace("-", "");

            if ( mobileNum.Length != 12 )
                {
                User.MobilePhone = 0;
                }
            else
                {
                long longMobile = Convert.ToInt64(mobileNum);
                if ( phoneIsNotUnique(longMobile) )
                    {
                    return false;
                    }
                User.MobilePhone = longMobile;
                }
            return true;
            }

        private bool phoneIsNotUnique(long longMobile)
            {
            Query query = DB.NewQuery("select top 1 cat.Description from Users as cat where cat.MobilePhone = @Phone and cat.Id <> @CurrentUserId");
            query.AddInputParameter("CurrentUserId", User.Id);
            query.AddInputParameter("Phone", longMobile);

            object result = query.SelectScalar();
            if ( result == null )
                {
                return false;
                }
            else
                {
                string.Format(WARNING_1, result.ToString().Trim()).WarningBox();
                return true;
                }

            }

        private void OK_ItemClick(object sender, ItemClickEventArgs e)
            {
            if ( Write() )
                {
                Close();
                }
            }

        private void Write_ItemClick(object sender, ItemClickEventArgs e)
            {
            Write();
            }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
            {
            TryCancel();
            }

        private void UsersItemForm_FormClosed(object sender, FormClosedEventArgs e)
            {
            if ( !User.IsNew && SystemAramis.CurrentUser.Ref == User.Ref )
                {
                UIConsts.Skin = User.Skin;
                }
            }

        private void Password_Enter(object sender, EventArgs e)
            {
            ClearControl(EnteredPassword);
            ClearControl(RepeatedPassword);
            }

        private void ClearControl(TextEdit textEdit)
            {
            if ( textEdit.Text == CatalogUsers.EMPTY_PASSWORD && !textEdit.Properties.ReadOnly )
                {
                textEdit.Text = "";
                }
            }

        private void Skin_Modified(object sender, EventArgs e)
            {
            if ( !User.IsNew && SystemAramis.CurrentUser.Ref == User.Ref )
                {
                UIConsts.Skin = ( Skins ) ( Skin.SelectedIndex );

                UIConsts.WindowsManager.GetFormsList(AramisObjectType.Catalog, true).ForEach(ItemFormTuner.ComplateFormSkinUpdating);
                UIConsts.WindowsManager.GetFormsList(AramisObjectType.Document, true).ForEach(ItemFormTuner.ComplateFormSkinUpdating);
                }
            }

        private void detach_ItemClick(object sender, ItemClickEventArgs e)
            {
            MdiParent = null;
            }

        private void GenerateSignatureKeysButton_ItemClick( object sender, ItemClickEventArgs e )
            {
            //((Users)Item).GenerateSignatureKeysPair();
            }

        }
    }