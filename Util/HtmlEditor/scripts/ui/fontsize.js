/*
* Font size toolbar control
*/

var FontSize = function( tooltip, style, enabledDesignMode )
{
	this.Command = CommandCollection.GetCommand("FontSize");
	this.DesignMode = enabledDesignMode;
}

FontSize.prototype = new BaseComboBox();

FontSize.prototype.LoadItems = function(container)
{
	sizes = Config.GetFontSizes().split(',');
	for(var i = 0; i < sizes.length; i++)
	{
		pair = sizes[i].split('*');
		this.AddItem(pair[0],pair[1]);
	}
}

FontSize.prototype.ExecuteCommand = function(value)
{
	this.Command.Params = value;
	this.Command.ExecCommand();
}