/*
* Represents a set of the toolbars
*/
function Toolbars()
{
	Toolbars.List = new Array();
}

Toolbars.prototype.Load = function()
{
	// get toolbar placeholder
	Toolbars.ToolbarPlaceHolder = document.getElementById("tbplace");
	var toolbarCollection = Config.Toolbars;
	for(var i = 0; i < toolbarCollection.length; i++)
	{
		// load toolbar
		var items = toolbarCollection[i];
		var toolbar = new ToolBar();
		for(var j = 0; j < items.length; j++)
		{
			// get toolbar item
			var item = ToolBarItemCollection.GetItem(items[j]);
			// add into toolbar
			toolbar.Add(item);
		}
		toolbar.AddEndOfToolbar();
		Toolbars.List[Toolbars.List.length] = toolbar;
	}
}

Toolbars.prototype.Refresh = function()
{
	for(var i = 0; i < Toolbars.List.length; i++)
		Toolbars.List[i].Refresh();
}
Toolbars.prototype.RefreshMode = function(designMode)
{
	for(var i = 0; i < Toolbars.List.length; i++)
		for(var k =0; k < Toolbars.List[i].Items.length; k++)
		{	
			if(designMode == Toolbars.List[i].Items[k].DesignMode)
			{
				Toolbars.List[i].Items[k].Disable();
			}
			else
			{
				Toolbars.List[i].Items[k].Enable();
			}
		}
}
