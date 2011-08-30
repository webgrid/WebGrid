/*
* Dialog command class
*/

var DialogCommand = function(name, pageName, width, height, tagValue,isResizeable)
{
	this.Name = name ;
	//this.Title = title ;
	this.PageName = pageName ;
	this.Width = width ;
	this.Height = height ;
	this.TagValue = tagValue;
	this.IsResizeable = isResizeable;
}

DialogCommand.prototype.ExecCommand = function()
{
	//function(name, parent, pageName, width, height, tagValue, isResizeable)
	this.Dlg = new Dialog(this.Name, Engine.Window, this.PageName, this.Width, this.Height, this.TagValue, this.IsResizeable);
	Engine.Dialog = this.Dlg;
}

DialogCommand.prototype.QueryCommandState = function()
{
	return Engine.GetCommandState(this.Name);
}