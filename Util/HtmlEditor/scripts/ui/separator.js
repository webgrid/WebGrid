/*
*	Toolbar separator
*/

var Separator = function()
{
}

Separator.prototype.Create = function(parent)
{
	//alert("start separator");
	var src;
	if(Config.GetEditorImagePath())
		src = '<img src="' + Config.GetEditorImagePath() + "separator.gif" + '" width="2" height="20"></td>';
	else
		src = '<img src="htmleditor.aspx?res=image&id=separator.gif" width="2" height="20" >';
	var oCell = parent.DOMRow.insertCell(-1);
	oCell.className = "bt_off";
	oCell.innerHTML = src;
}

Separator.prototype.Enable = function()
{
	return;
}

Separator.prototype.Disable = function()
{
	return;
}