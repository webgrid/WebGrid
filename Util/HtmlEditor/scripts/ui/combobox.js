/*
*	Abstract Base Class for a Combo Box
*/

BaseComboBox = function()
{
}
function BaseComboBox_OnSelectChanged(e)
{
	value = this.options[this.selectedIndex].value;
	this.BaseComboBox.ExecuteCommand(value);
}
BaseComboBox.prototype.Create = function(parent)
{

	this.DOMSelect = document.createElement("select");
	this.DOMSelect.BaseComboBox = this;
	this.DOMSelect.className = "selectList";
	this.DOMSelect.style.width = "70px";
	this.LoadItems(this);
	
	var oCell = parent.DOMRow.insertCell(-1);
	oCell.appendChild(this.DOMSelect );
	//alert(oCell.innerHTML);
	this.DOMSelect.onchange = BaseComboBox_OnSelectChanged;
}

BaseComboBox.prototype.AddItem = function(text, value)
{
	// current implementation ia a standard select element
	var option = document.createElement("option");
	this.DOMSelect.options.add(option);
	option.value = text;
	option.innerHTML = value;
}

BaseComboBox.prototype.Enable = function()
{
	this.DOMSelect.disabled = false;
}
BaseComboBox.prototype.Disable = function()
{
	this.DOMSelect.disabled = true;
}