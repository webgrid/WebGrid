/*
Copyright ©  Olav Christian Botterli. All Rights Reserved. 

Ask for permission if you want to use or change any part of the source code.

E-mail: olav@webgrid.com, olav@botterli.com (Olav Botterli)

http://www.webgrid.com
*/


/*
Copyright ©  Olav Christian Botterli. All Rights Reserved. 

Ask for permission if you want to use or change any part of the source code.

E-mail: olav@webgrid.com, olav@botterli.com (Olav Botterli)

http://www.webgrid.com
*/


using System.ComponentModel;

namespace WebGrid
{
    /// <summary>
    /// This class is used to serialize the elements for the collection property <see cref="WebGrid.Columns">WebGrid.Columns</see>.
    /// This class inherits from the <see cref="WebGrid.Columns.DateTime">WebGrid.Columns.DateTime</see> class.
    /// </summary>
    /// <exclude/>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class DateTime : Columns.DateTime
    {
        public DateTime()
        {
            m_Columncreatedbywebgrid = true;
        }
    }

    /// <summary>
    /// This class is used to serialize the elements for the collection property <see cref="WebGrid.Columns">WebGrid.Columns</see>.
    /// This class inherits from the <see cref="WebGrid.Columns.Decimal">WebGrid.Columns.Decimal</see> class.
    /// </summary>
    /// <exclude/>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class Decimal : Columns.Decimal
    {
        public Decimal()
        {
            m_Columncreatedbywebgrid = true;
        }
    }


    /// <summary>
    /// This class is used to serialize the elements for the collection property <see cref="WebGrid.Columns">WebGrid.Columns</see>.
    /// This class inherits from the <see cref="WebGrid.Columns.Number">WebGrid.Columns.Number</see> class.
    /// </summary>
    /// <exclude/>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class Number : Columns.Number
    {
        public Number()
        {
            m_Columncreatedbywebgrid = true;
        }
    }

    /// <summary>
    /// This class is used to serialize the elements for the collection property <see cref="WebGrid.Columns">WebGrid.Columns</see>.
    /// This class inherits from the <see cref="WebGrid.Columns.Foreignkey">WebGrid.Columns.Foreignkey</see> class.
    /// </summary>
    /// <exclude/>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class Foreignkey : Columns.Foreignkey
    {
    }

    /// <summary>
    /// This class is used to serialize the elements for the collection property <see cref="WebGrid.Columns">WebGrid.Columns</see>.
    /// This class inherits from the <see cref="WebGrid.Columns.ManyToMany">WebGrid.Columns.ManyToMany</see> class.
    /// </summary>
    /// <exclude/>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ManyToMany : Columns.ManyToMany
    {
    }

    /// <summary>
    /// This class is used to serialize the elements for the collection property <see cref="WebGrid.Columns">WebGrid.Columns</see>.
    /// This class inherits from the <see cref="WebGrid.Columns.SystemColumn">WebGrid.Columns.SystemColumn</see> class.
    /// </summary>
    /// <exclude/>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class SystemColumn : Columns.SystemColumn
    {
    }

    /// <summary>
    /// This class is used to serialize the elements for the collection property <see cref="WebGrid.Columns">WebGrid.Columns</see>.
    /// This class inherits from the <see cref="WebGrid.Columns.Text">WebGrid.Columns.Text</see> class.
    /// </summary>
    /// <exclude/>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class Text : Columns.Text
    {
        public Text()
        {
            m_Columncreatedbywebgrid = true;
        }
    }

    /// <summary>
    /// This class is used to serialize the elements for the collection property <see cref="WebGrid.Columns">WebGrid.Columns</see>.
    /// This class inherits from the <see cref="WebGrid.Columns.File">WebGrid.Columns.File</see> class.
    /// </summary>
    /// <exclude/>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class File : Columns.File
    {
    }

    /// <summary>
    /// This class is used to serialize the elements for the collection property <see cref="WebGrid.Columns">WebGrid.Columns</see>.
    /// This class inherits from the <see cref="WebGrid.Columns.File">WebGrid.Columns.File</see> class.
    /// </summary>
    /// <exclude/>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class Image : Columns.Image
    {
    }

    /// <summary>
    /// This class is used to serialize the elements for the collection property <see cref="WebGrid.Columns">WebGrid.Columns</see>.
    /// This class inherits from the <see cref="WebGrid.Columns.Checkbox">WebGrid.Columns.Checkbox</see> class.
    /// </summary>
    /// <exclude/>
    [DesignTimeVisible(true),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class Checkbox : Columns.Checkbox
    {
    }

    /// <summary>
    /// This class is used to serialize the elements for the collection property <see cref="WebGrid.Columns">WebGrid.Columns</see>.
    /// This class inherits from the <see cref="WebGrid.Columns.Chart">WebGrid.Columns.Chart</see> class.
    /// </summary>
    /// <exclude/>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public class Chart : Columns.Chart
    {
    }

    /// <summary>
    /// This class is used to serialize the elements for the collection property <see cref="WebGrid.Columns">WebGrid.Columns</see>.
    /// This class inherits from the <see cref="WebGrid.Columns.GridColumn">WebGrid.Columns.GridColumn</see> class.
    /// </summary>
    /// <exclude/>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public class GridColumn : Columns.GridColumn
    {
    }



    /// <summary>
    /// 
    /// </summary>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public class ColumnTemplate : Columns.ColumnTemplate
    {
    }

    /// <summary>
    /// 
    /// </summary>
    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    public class ToolTipColumn : Columns.ToolTipColumn
    {
    }

    /*
       [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    internal class GridModeSettings : Design.GridModeSettings
    {

    }

    [DesignTimeVisible(false),
     Browsable(true),
     EditorBrowsable(EditorBrowsableState.Never)]
    internal class EditModeSettings : Design.EditModeSettings
    {

    }*/
}
