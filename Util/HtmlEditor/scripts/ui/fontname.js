/*
* Font name toolbar control. The difference between font name and font size classes is control drawing
*/

var FontName = function(tooltip, style, enabledDesignMode)
{
	this.Command = CommandCollection.GetCommand("FontName");
	this.DesignMode = enabledDesignMode;
}

FontName.prototype = new BaseComboBox();


FontName.prototype.LoadItems = function(container)
{
	ftnames = Config.GetFontNames().split(';');
	for(var i=0; i < ftnames.length; i++)
	{
		this.AddItem(ftnames[i],ftnames[i]);
	}
}

FontName.prototype.ExecuteCommand = function(value)
{
	this.Command.Params = value;
	this.Command.ExecCommand();
}


