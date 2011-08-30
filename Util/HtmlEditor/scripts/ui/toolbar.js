/*
*  This class represents a toolbar
*/

// constructor
ToolBar = function()
{
	this.Items = new Array();
	
	var e = this.DOMTable = document.createElement("table");
	e.className = "toolbar";
	e.style.styleFloat = e.style.cssFloat = "left";
	e.border = 0;
	e.cellPadding = 0;
	e.cellSpacing = 0;
	this.DOMRow = e.insertRow(-1);
	// TODO: add grip options 
	cell = this.DOMRow.insertCell(-1);
		if(Config.GetEditorImagePath())
		cell.innerHTML  = '<img src="' + Config.GetEditorImagePath() + "grip.gif" + '" width="5" height="24"></td>';
	else
	cell.innerHTML = '<img src="htmleditor.aspx?res=image&id=grip.gif" width="5" height="24" >';
	
	Toolbars.ToolbarPlaceHolder.appendChild(e);
}
ToolBar.prototype.Add = function(item)
{
	this.Items[ this.Items.length ] = item;
	item.Create(this);
}
ToolBar.prototype.AddEndOfToolbar = function()
{
	cell = this.DOMRow.insertCell(-1);
	//cell.style.height = '24px';
	cell.innerHTML = "";
}

ToolBar.prototype.Refresh = function()
{
	
	for(var i =0; i < this.Items.length; i++)
	{	
		if(true == this.Items[i].ContextSensitive)
		{
			this.Items[i].Update();
		}
	}
}