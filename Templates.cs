/*
Copyright ©  Olav Christian Botterli.

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/


#region Header

/*
Copyright ©  Olav Christian Botterli. 

Dual licensed under the MIT or GPL Version 2 licenses.

Date: 30.08.2011, Norway.

http://www.webgrid.com
*/

#endregion Header

namespace WebGrid
{
    using System.ComponentModel.Design;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    using WebGrid.Design;
    using WebGrid.Enums;

    internal class GridTemplates : ControlDesigner
    {
        #region Fields

        private GridTemplate m_Templatetype = GridTemplate.None;

        #endregion Fields

        #region Properties

        internal GridTemplate Type
        {
            get { return m_Templatetype; }
            set { m_Templatetype = value; }
        }

        #endregion Properties

        #region Methods

        internal static void AddGridColumn(ref Grid grid, ref IDesignerHost designerhost,
            ref IComponentChangeService componentChangeService,
            ref DesignerTransaction dt, ColumnTemplates template)
        {
            if (template == ColumnTemplates.None)
                return;
            dt = designerhost.CreateTransaction(string.Format("Configure WebGrid as {0}", template));

            switch (template)
            {
                case ColumnTemplates.SelectRowColumn:
                    {
                        SystemColumn systemcolumn = (SystemColumn) designerhost.CreateComponent(typeof (SystemColumn));
                        systemcolumn.ColumnId = "SelectColumn";
                        systemcolumn.Grid = grid;
                        systemcolumn.Title = string.Empty;
                        systemcolumn.Visibility = Visibility.Grid;
                        systemcolumn.SystemColumnType = Enums.SystemColumn.SelectColumn;
                        systemcolumn.DisplayIndex = 5;
                        grid.Columns.Add(systemcolumn);
                    }
                    break;
                case ColumnTemplates.EditRowColumn:
                    {
                        SystemColumn systemcolumn = (SystemColumn) designerhost.CreateComponent(typeof (SystemColumn));
                        systemcolumn.ColumnId = "EditColumn";
                        systemcolumn.Grid = grid;
                        systemcolumn.Html = "Edit row";
                        systemcolumn.Title = string.Empty;
                        systemcolumn.Visibility = Visibility.Grid;
                        systemcolumn.SystemColumnType = Enums.SystemColumn.EditColumn;
                        systemcolumn.DisplayIndex = 7;
                        grid.Columns.Add(systemcolumn);
                    }
                    break;
                case ColumnTemplates.CopyRowColumnn:
                    {
                        SystemColumn systemcolumn = (SystemColumn) designerhost.CreateComponent(typeof (SystemColumn));
                        systemcolumn.ColumnId = "CopyColumn";
                        systemcolumn.Grid = grid;
                        systemcolumn.Html = "Copy row";
                        systemcolumn.Title = string.Empty;
                        systemcolumn.Visibility = Visibility.Grid;
                        systemcolumn.SystemColumnType = Enums.SystemColumn.CopyColumn;
                        systemcolumn.DisplayIndex = 2;
                        grid.Columns.Add(systemcolumn);
                    }
                    break;
                case ColumnTemplates.UpdateGridRecordColumn:
                    {
                        SystemColumn systemcolumn = (SystemColumn) designerhost.CreateComponent(typeof (SystemColumn));
                        systemcolumn.ColumnId = "UpdateRecord";
                        systemcolumn.Grid = grid;
                        systemcolumn.Html = "Update record";
                        systemcolumn.Title = string.Empty;
                        systemcolumn.Visibility = Visibility.Grid;
                        systemcolumn.SystemColumnType = Enums.SystemColumn.UpdateGridRecordColumn;
                        systemcolumn.DisplayIndex = 9;
                        grid.Columns.Add(systemcolumn);
                    }
                    break;
                case ColumnTemplates.UpdateGridRecordsColumn:
                    {
                        SystemColumn systemcolumn = (SystemColumn) designerhost.CreateComponent(typeof (SystemColumn));
                        systemcolumn.ColumnId = "UpdateRecords";
                        systemcolumn.Grid = grid;
                        systemcolumn.Html = "Update all";
                        systemcolumn.Title = string.Empty;
                        systemcolumn.Visibility = Visibility.Grid;
                        systemcolumn.SystemColumnType = Enums.SystemColumn.UpdateGridRecordsColumn;
                        systemcolumn.DisplayIndex = 9;
                        grid.Columns.Add(systemcolumn);
                    }
                    break;
                case ColumnTemplates.DeleteRowColumn:
                    {
                        SystemColumn systemcolumn = (SystemColumn) designerhost.CreateComponent(typeof (SystemColumn));
                        systemcolumn.ColumnId = "DeleteRow";
                        systemcolumn.Html = "Delete row";
                        systemcolumn.Grid = grid;
                        systemcolumn.Title = string.Empty;
                        systemcolumn.Visibility = Visibility.Grid;
                        systemcolumn.SystemColumnType = Enums.SystemColumn.DeleteColumn;
                        grid.Columns.Add(systemcolumn);
                    }
                    break;
                case ColumnTemplates.HtmlEditorColumn:
                    {
                        Text htmlColumn = (Text) designerhost.CreateComponent(typeof (Text));
                        htmlColumn.ColumnId = "HtmlColumn";
                        htmlColumn.Title = "Html column";
                        htmlColumn.Grid = grid;
                        htmlColumn.IsHtml = true;
                        htmlColumn.WidthEditableColumn = Unit.Percentage(90);
                        htmlColumn.HeightEditableColumn = Unit.Pixel(300);
                        htmlColumn.Visibility = Visibility.Detail;
                        htmlColumn.Required = true;
                        grid.Columns.Add(htmlColumn);
                    }
                    break;
                case ColumnTemplates.EmailValidColumn:
                    {
                        Text emailcolumn = (Text) designerhost.CreateComponent(typeof (Text));
                        emailcolumn.ColumnId = "Emailcolumn";
                        emailcolumn.Title = "E-mail address";
                        emailcolumn.Grid = grid;
                        emailcolumn.Visibility = Visibility.Both;
                        emailcolumn.DisplayIndex = grid.Columns.Count*10;
                        emailcolumn.Required = true;
                        grid.Columns.Add(emailcolumn);
                    }
                    break;
            }
             WebGridDesignTime.SaveGridState(dt, grid, componentChangeService);
        }

        internal static void ConfigureGrid(ref Grid grid, ref IDesignerHost designerhost,
            ref IComponentChangeService componentChangeService,
            ref DesignerTransaction dt, GridTemplate template)
        {
            if (template == GridTemplate.None)
                return;

            switch (template)
            {
                case GridTemplate.ProductCatalog:
                    {
                        dt = designerhost.CreateTransaction(string.Format("Configure WebGrid as {0}", template));

                        grid.RecordsPerRow = 3;
                        grid.Width = Unit.Percentage(100);
                        grid.Title = "Product catalog";
                        grid.DefaultVisibility = Visibility.None;
                        grid.DisplayView = DisplayView.Grid;
                        grid.Columns.Clear();

                        Text producttitle = (Text) designerhost.CreateComponent(typeof (Text));
                        producttitle.ColumnId = "productTitle";
                        producttitle.Grid = grid;
                        producttitle.Title = "Title";
                        producttitle.Visibility = Visibility.Both;
                        producttitle.DisplayIndex = 10;

                        grid.Columns.Add(producttitle);

                        File productImageGrid = (File) designerhost.CreateComponent(typeof (File));
                        productImageGrid.ColumnId = "productImageGrid";
                        productImageGrid.Grid = grid;
                        productImageGrid.Title = "Grid image";
                        productImageGrid.HideDetailTitle = true;
                        productImageGrid.NewRowInGrid = true;
                        productImageGrid.Visibility = Visibility.Grid;
                        productImageGrid.DisplayIndex = 20;

                        grid.Columns.Add(productImageGrid);

                        File productImageEdit = (File) designerhost.CreateComponent(typeof (File));
                        productImageEdit.ColumnId = "productImageEdit";
                        productImageEdit.Title = "Edit image";
                        productImageEdit.Grid = grid;
                        productImageEdit.HideDetailTitle = true;
                        productImageEdit.NewRowInGrid = true;
                        productImageEdit.Visibility = Visibility.Detail;
                        productImageEdit.DisplayIndex = 20;

                        grid.Columns.Add(productImageEdit);

                        Text productGridDescription = (Text) designerhost.CreateComponent(typeof (Text));
                        productGridDescription.ColumnId = "productGridDescription";
                        productGridDescription.Title = "Grid description";
                        productGridDescription.Grid = grid;
                        productGridDescription.HideDetailTitle = true;
                        productGridDescription.Visibility = Visibility.Grid;
                        productGridDescription.IsHtml = true;
                        productGridDescription.DisplayIndex = 30;

                        grid.Columns.Add(productGridDescription);

                        Text productEditDescription = (Text) designerhost.CreateComponent(typeof (Text));
                        productEditDescription.ColumnId = "productGridDescription";
                        productEditDescription.Grid = grid;
                        productEditDescription.Title = "Edit description";
                        productEditDescription.HideDetailTitle = true;
                        productEditDescription.IsHtml = true;
                        productEditDescription.Visibility = Visibility.Detail;
                        productEditDescription.DisplayIndex = 30;

                        grid.Columns.Add(productEditDescription);

                        WebGridDesignTime.SaveGridState(dt, grid, componentChangeService);
                        return;
                    }
                case GridTemplate.UserRegistration:
                    {
                        grid.Width = Unit.Pixel(500);
                        grid.Title = "User login";
                        grid.DefaultVisibility = Visibility.None;
                        grid.DisplayView = DisplayView.Detail;
                        grid.AllowCancel = false;
                        grid.AllowUpdate = true;
                        grid.RecordsPerRow = 1;

                        grid.Columns.Clear();

                        Text userloginID = (Text) designerhost.CreateComponent(typeof (Text));
                        userloginID.ColumnId = "LoginID";
                        userloginID.Title = "Login ID";
                        userloginID.Grid = grid;
                        userloginID.HideDetailTitle = true;
                        userloginID.Visibility = Visibility.Detail;
                        userloginID.DisplayIndex = 10;
                        userloginID.Required = true;
                        grid.Columns.Add(userloginID);

                        Text userpassword = (Text) designerhost.CreateComponent(typeof (Text));
                        userpassword.ColumnId = "LoginPassword";
                        userpassword.Title = "Login password";
                        userpassword.Grid = grid;
                        userpassword.IsPassword = true;
                        userpassword.Visibility = Visibility.Detail;
                        userpassword.DisplayIndex = 20;
                        userpassword.ValidExpression = "[RepeatPassword] != [LoginPassword]";
                        userpassword.Required = true;
                        userpassword.MinSize = 5;
                        userpassword.MaxSize = 15;
                        userpassword.SystemMessage = "Your passwords did not match.";

                        grid.Columns.Add(userpassword);

                        Text repeatuserpassword = (Text) designerhost.CreateComponent(typeof (Text));
                        repeatuserpassword.ColumnId = "RepeatPassword";
                        repeatuserpassword.Title = "Repeat password";
                        repeatuserpassword.NewRowInDetail = false;
                        repeatuserpassword.Grid = grid;
                        repeatuserpassword.IsPassword = true;
                        repeatuserpassword.Visibility = Visibility.Detail;
                        repeatuserpassword.DisplayIndex = 30;
                        repeatuserpassword.MinSize = 5;
                        repeatuserpassword.MaxSize = 15;
                        repeatuserpassword.Required = true;

                        grid.Columns.Add(repeatuserpassword);

                        WebGridDesignTime.SaveGridState(dt, grid, componentChangeService);
                        return;
                    }
                case GridTemplate.RowSummary:
                    {
                        grid.Width = Unit.Percentage(100);
                        grid.Title = "Row summary";
                        grid.DefaultVisibility = Visibility.None;
                        grid.RecordsPerRow = 1;
                        grid.DisplayView = DisplayView.Grid;

                        grid.Columns.Clear();

                        Decimal productprice = (Decimal) designerhost.CreateComponent(typeof (Decimal));
                        productprice.ColumnId = "Price";
                        productprice.Title = "Price";
                        productprice.Grid = grid;
                        productprice.Visibility = Visibility.Both;
                        productprice.DisplayIndex = 10;

                        grid.Columns.Add(productprice);

                        Number productQuantity = (Number) designerhost.CreateComponent(typeof (Number));
                        productQuantity.ColumnId = "Quantity";
                        productQuantity.Title = "Quantity";
                        productQuantity.Grid = grid;
                        productQuantity.Visibility = Visibility.Both;
                        productQuantity.DisplayIndex = 20;

                        grid.Columns.Add(productQuantity);

                        Decimal productSubSum = (Decimal) designerhost.CreateComponent(typeof (Decimal));
                        productSubSum.ColumnId = "Total";
                        productSubSum.Title = "Sub total";
                        productSubSum.Grid = grid;
                        productSubSum.Visibility = Visibility.Grid;
                        productSubSum.DisplayIndex = 30;
                        productSubSum.DisplayTotalSummary = true;
                        productSubSum.Sum = "[Quantity]*[Price]";
                        grid.Columns.Add(productSubSum);

                        WebGridDesignTime.SaveGridState(dt, grid, componentChangeService);
                        return;
                    }
                case GridTemplate.DocumentDatabase:
                    {
                        grid.Width = Unit.Percentage(100);
                        grid.Title = "Document database";
                        grid.DefaultVisibility = Visibility.None;
                        grid.RecordsPerRow = 1;
                        grid.DisplayView = DisplayView.Grid;

                        grid.Columns.Clear();

                        Text documenttitle = (Text) designerhost.CreateComponent(typeof (Text));
                        documenttitle.ColumnId = "TitleDocument";
                        documenttitle.Title = "Title";
                        documenttitle.Grid = grid;
                        documenttitle.Visibility = Visibility.Both;
                        documenttitle.DisplayIndex = 10;
                        documenttitle.Required = true;

                        grid.Columns.Add(documenttitle);

                        Text htmldocument = (Text) designerhost.CreateComponent(typeof (Text));
                        htmldocument.ColumnId = "htmlDocument";
                        htmldocument.Title = "Document";
                        htmldocument.Grid = grid;
                        htmldocument.IsHtml = true;
                        htmldocument.WidthEditableColumn = Unit.Percentage(90);
                        htmldocument.HeightEditableColumn = Unit.Pixel(300);
                        htmldocument.Visibility = Visibility.Detail;
                        htmldocument.DisplayIndex = 20;
                        htmldocument.Required = true;

                        grid.Columns.Add(htmldocument);

                        File fileattachment = (File) designerhost.CreateComponent(typeof (File));
                        fileattachment.ColumnId = "FileAttachment";
                        fileattachment.Title = "File Attachment";
                        fileattachment.Grid = grid;
                        fileattachment.Visibility = Visibility.Both;
                        fileattachment.DisplayIndex = 30;
                        fileattachment.Required = false;

                        grid.Columns.Add(fileattachment);

                        WebGridDesignTime.SaveGridState(dt, grid, componentChangeService);
                        return;
                    }
            }
        }

        #endregion Methods
    }
}