using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.Attributes;

namespace SystemInvoice.Documents
    {
    public enum InvoiceColumnNames
        {
        [DataField(Description = "Не заполнено")]
        Empty,
        [DataField(Description = "Подгруппа товара")]
        SubGroupOfGoods,
        [DataField(Description = "Код подгруппы")]
        GroupCode,
        [DataField(Description = "Товар(Декларация)")]
        NomenclatureDeclaration,
        [DataField(Description = "Наименование (исходное)")]
        OriginalName,
        [DataField(Description = "Торговая марка")]
        ItemTradeMark,
        [DataField(Description = "Производитель")]
        ItemContractor,
        [DataField(Description = "Штрих-код")]
        BarCode,
        [DataField(Description = "Кол-во")]
        Count,
        [DataField(Description = "Цена")]
        Price,
        [DataField(Description = "Наценка")]
        Margin,
        [DataField(Description = "Сумма")]
        Sum,
        [DataField(Description = "Вес ед. товара")]
        UnitWeight,
        [DataField(Description = "Таможенный код внешний")]
        CustomsCodeExtern,
        [DataField(Description = "Таможенный код внутренний")]
        CustomsCodeIntern,
        [DataField(Description = "Артикул")]
        Article,
        [DataField(Description = "Вес нетто")]
        NetWeight,
        [DataField(Description = "Вес брутто")]
        ItemGrossWeight,
        [DataField(Description = "Пол")]
        Gender,
        [DataField(Description = "Места")]
        ItemNumberOfPlaces,
        [DataField(Description = "Ед.(Код)")]
        UnitOfMeasureCode,
        [DataField(Description = "Ед. изм.")]
        UnitOfMeasure,
        [DataField(Description = "Страна")]
        Country,
        [DataField(Description = "Размер")]
        Size,
        [DataField(Description = "Длинна стельки")]
        InsoleLength,
        [DataField(Description = "Состав низ")]
        ContentBottom,
        [DataField(Description = "Состав")]
        Content,
        [DataField(Description = "Код инвойса")]
        InvoiceCode,
        [DataField(Description = "№ инвойса")]
        InvoiceNumber,
        [DataField(Description = "Дата инвойса")]
        InvoiceDate,
        [DataField(Description = "Товар (инвойс)")]
        NomenclatureInvoice,
        [DataField(Description = "Графа 31")]
        Graf31,
        [DataField(Description = "РД код")]
        RDCode1,
        [DataField(Description = "РД №")]
        RDNumber1,
        [DataField(Description = "РД Дата выдачи")]
        RDFromDate1,
        [DataField(Description = "РД Годен до")]
        RDToDate1,
        [DataField(Description = "РД 2 код")]
        RDCode2,
        [DataField(Description = "РД 2 №")]
        RDNumber2,
        [DataField(Description = "РД 2 Дата выдачи")]
        RDFromDate2,
        [DataField(Description = "РД 2 Годен до")]
        RDToDate2,
        [DataField(Description = "РД 3 код")]
        RDCode3,
        [DataField(Description = "РД 3 №")]
        RDNumber3,
        [DataField(Description = "РД 3 Дата выдачи")]
        RDFromDate3,
        [DataField(Description = "РД 3 Годен до")]
        RDToDate3,
        [DataField(Description = "РД 4 код")]
        RDCode4,
        [DataField(Description = "РД 4 №")]
        RDNumber4,
        [DataField(Description = "РД 4 Дата выдачи")]
        RDFromDate4,
        [DataField(Description = "РД 4 Годен до")]
        RDToDate4,
        [DataField(Description = "РД 5 код")]
        RDCode5,
        [DataField(Description = "РД 5 №")]
        RDNumber5,
        [DataField(Description = "РД 5 Дата выдачи")]
        RDFromDate5,
        [DataField(Description = "РД 5 Годен до")]
        RDToDate5,

        [DataField(Description = "Код состава (Зара)_1")]
        ZaraContent1Code,
        [DataField(Description = "Имя состава укр. (Зара)_1")]
        ZaraContent1UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_1")]
        ZaraContent1EnName,
        [DataField(Description = "Код состава (Зара)_2")]
        ZaraContent2Code,
        [DataField(Description = "Имя состава укр. (Зара)_2")]
        ZaraContent2UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_2")]
        ZaraContent2EnName,
        [DataField(Description = "Код состава (Зара)_3")]
        ZaraContent3Code,
        [DataField(Description = "Имя состава укр. (Зара)_3")]
        ZaraContent3UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_3")]
        ZaraContent3EnName,
        [DataField(Description = "Код состава (Зара)_4")]
        ZaraContent4Code,
        [DataField(Description = "Имя состава укр. (Зара)_4")]
        ZaraContent4UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_4")]
        ZaraContent4EnName,
        [DataField(Description = "Код состава (Зара)_5")]
        ZaraContent5Code,
        [DataField(Description = "Имя состава укр. (Зара)_5")]
        ZaraContent5UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_5")]
        ZaraContent5EnName,
        [DataField(Description = "Код состава (Зара)_6")]
        ZaraContent6Code,
        [DataField(Description = "Имя состава укр. (Зара)_6")]
        ZaraContent6UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_6")]
        ZaraContent6EnName,
        [DataField(Description = "Код состава (Зара)_7")]
        ZaraContent7Code,
        [DataField(Description = "Имя состава укр. (Зара)_7")]
        ZaraContent7UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_7")]
        ZaraContent7EnName,
        [DataField(Description = "Код состава (Зара)_8")]
        ZaraContent8Code,
        [DataField(Description = "Имя состава укр. (Зара)_8")]
        ZaraContent8UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_8")]
        ZaraContent8EnName,
        [DataField(Description = "Код состава (Зара)_9")]
        ZaraContent9Code,
        [DataField(Description = "Имя состава укр. (Зара)_9")]
        ZaraContent9UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_9")]
        ZaraContent9EnName,
        [DataField(Description = "Код состава (Зара)_10")]
        ZaraContent10Code,
        [DataField(Description = "Имя состава укр. (Зара)_10")]
        ZaraContent10UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_10")]
        ZaraContent10EnName,
        [DataField(Description = "Код состава (Зара)_11")]
        ZaraContent11Code,
        [DataField(Description = "Имя состава укр. (Зара)_11")]
        ZaraContent11UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_11")]
        ZaraContent11EnName,
        [DataField(Description = "Код состава (Зара)_12")]
        ZaraContent12Code,
        [DataField(Description = "Имя состава укр. (Зара)_12")]
        ZaraContent12UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_12")]
        ZaraContent12EnName,
        [DataField(Description = "Код состава (Зара)_13")]
        ZaraContent13Code,
        [DataField(Description = "Имя состава укр. (Зара)_13")]
        ZaraContent13UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_13")]
        ZaraContent13EnName,
        [DataField(Description = "Код состава (Зара)_14")]
        ZaraContent14Code,
        [DataField(Description = "Имя состава укр. (Зара)_14")]
        ZaraContent14UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_14")]
        ZaraContent14EnName,
        [DataField(Description = "Код состава (Зара)_15")]
        ZaraContent15Code,
        [DataField(Description = "Имя состава укр. (Зара)_15")]
        ZaraContent15UkrName,
        [DataField(Description = "Имя состава англ. (Зара)_15")]
        ZaraContent15EnName,

        [DataField(Description = "Состав KnitWoven .(Marks & Spenser)")]
        MSKnitWovenColumnName,
        [DataField(Description = "Группа товара")]
        GroupOfGoods,
        [DataField(Description = "BNS Номер поставки")]
        BNSInvoicePart,
        [DataField(Description = "Цена с наценкой")]
        PriceWithMargin,
        [DataField(Description = "% Наценки")]
        MarginPrecentage,
        [DataField(Description = "Размер исходный")]
        SizeOriginal,

        [DataField(Description = "Модель")]
        Model,

        [DataField(Description = "Брутто един.")]
        OneItemGross,

        [DataField(Description = "РД 1 основание")]
        RD1BaseNumber,
        [DataField(Description = "РД 2 основание")]
        RD2BaseNumber,
        [DataField(Description = "РД 3 основание")]
        RD3BaseNumber,
        [DataField(Description = "РД 4 основание")]
        RD4BaseNumber,
        [DataField(Description = "РД 5 основание")]
        RD5BaseNumber
        }
    }
