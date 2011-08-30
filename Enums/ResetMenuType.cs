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


namespace WebGrid.Enums
{
    /// <summary>
    /// Defines what reset menu type should be supported by the grid. Default is AlwaysVisible.
    /// </summary>
    public enum BackMenuType
    {
        /// <summary>
        /// If Undefined WebGrid will try to retrieve data from Web.Config else use AlwaysVisible as default.
        /// </summary>
        Undefined = 1,

        /// <summary>
        /// Back/Reset button in the menu is always visibility in the menu.
        /// </summary>
        AlwaysVisible = 1,

        /// <summary>
        /// Back/Reset button in the menu is visibility when there is a active menu item.
        /// </summary>
        OnlyIfActive = 2,

        /// <summary>
        /// Back/Reset button is hidden from the menu.
        /// </summary>
        Disabled = 3
    }
}
