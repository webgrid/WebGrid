/*

*/

/*
* Simple toolbar button
*/

var SwitchButton = function(commandName, text, tooltip, style , contextSensitive)
{
	this.Command			= CommandCollection.GetCommand(commandName);
	this.Text				= text ? text : commandName;
	this.Tooltip			= tooltip ? tooltip : ( text ? text : commandName);
	//this.Style				= style ? style : FCK_TOOLBARITEM_ONLYICON;
	//this.IconPath			= Config.GetBasePath() + '' + commandName.toLowerCase() + '.gif';
	this.IconPath			= commandName.toLowerCase() + 'on.gif';
	this.IconPathOff        = commandName.toLowerCase() + 'off.gif';
	this.State				= CMD_ENABLED;
	this.ContextSensitive = contextSensitive;
}

//Button.prototype = new ToolBarItem();

SwitchButton.prototype.Create = function(parent)
{
	this.DOMDiv = document.createElement( 'div' );
	this.DOMDiv.className = 'bt_off';

	this.DOMDiv.SwitchButton = this;
	
	var sHtml =
		'<table title="' + this.Tooltip + '" cellspacing="0" cellpadding="0" border="0" align=center valign=middle>' +
			'<tr>';
	
	//if ( this.Style != FCK_TOOLBARITEM_ONLYTEXT ) 
	if(Config.GetEditorImagePath())
	{
		sHtml += '<td width=24 align="center" class="TB_Icon">';
		sHtml += '<img id="designImage" src="' + Config.GetEditorImagePath() + this.IconPath + '" width="20" height="20"';
		if(Config.GetMode()==DESIGN_MODE)
		{
			sHtml += '>'
			sHtml += '<img id="sourceImage" src="' + Config.GetEditorImagePath() + this.IconPath + '" width="20" height="20" style=" display: none" >';
		}
		else
		{
			sHtml += ' style=" display: none" >';
			sHtml += '<img id="sourceImage" src="' + Config.GetEditorImagePath() + this.IconPath + '" width="20" height="20"  >';
		}
		
		sHtml += '</td>';
	}
	else
	{
		sHtml += '<td width=24 align="center" class="TB_Icon">';
		sHtml += '<img id="designImage" src="htmleditor.aspx?res=image&id=' + this.IconPath + '" width="16" height="16"';
		if(Config.GetMode()==DESIGN_MODE)
		{
			sHtml += ' >';
			sHtml += '<img id="sourceImage" src="htmleditor.aspx?res=image&id=' + this.IconPathOff + '" width="16" height="16"  style=" display: none">';
		}
		else
		{
			sHtml += '  style=" display: none" >';
			sHtml += '<img id="sourceImage" src="htmleditor.aspx?res=image&id=' + this.IconPathOff + '" width="16" height="16" >';
		}
		
		sHtml += '</td>';
	}
	
	//if ( this.Style != FCK_TOOLBARITEM_ONLYICON ) 
	//	sHtml += '<td class="TB_Text" nowrap>' + this.Label + '</td>';
	
	sHtml +=	
			'</tr>' +
		'</table>';
	
	this.DOMDiv.innerHTML = sHtml;
	var oCell = parent.DOMRow.insertCell(-1);
	oCell.appendChild( this.DOMDiv );
	
	// DEBUG VERSION ONLY!
	this.DOMDiv.onmouseover	= SwitchButton_OnMouseOnOver;
	this.DOMDiv.onmouseout	= SwitchButton_OnMouseOnOut;
	this.DOMDiv.onclick		= SwitchButton_OnClick;
}

SwitchButton.prototype.Update = function()
{
	var newState = this.Command.QueryCommandState();
	if(newState == this.State) 
		return;
	this.State = newState;
	
	switch (this.State )
	{
		case CMD_APPLIED:
			this.DOMDiv.className = 'bt_on' ;

			this.DOMDiv.onmouseover	= SwitchButton_OnMouseOnOver ;
			this.DOMDiv.onmouseout	= SwitchButton_OnMouseOnOut ;
			this.DOMDiv.onclick		= SwitchButton_OnClick ;
			
			break ;
		case CMD_ENABLED :
			this.DOMDiv.className = 'bt_off' ;

			///this.DOMDiv.onmouseover	= FCKToolbarButton_OnMouseOffOver ;
			//this.DOMDiv.onmouseout	= FCKToolbarButton_OnMouseOffOut ;
			//this.DOMDiv.onclick		= FCKToolbarButton_OnClick ;
			break ;
		default :
			//this.Disable() ;
			break ;
	}
}

function SwitchButton_OnMouseOnOver()
{
	this.className = 'bt_on bt_on_over';
}

function SwitchButton_OnMouseOnOut()
{
	this.className = 'bt_off';
}
	
function SwitchButton_OnMouseOffOver()
{
	this.className = 'bt_on bt_off_over';
}

function SwitchButton_OnMouseOffOut()
{
	this.className = 'bt_off';
}
	
function SwitchButton_OnClick(e)
{
	this.SwitchButton.Click(e);
	return false;
}

SwitchButton.prototype.Click = function(e)
{
	if(document.getElementById('designImage').style.display == "none")
	{
		document.getElementById('designImage').style.display = "inline";
		document.getElementById('sourceImage').style.display = "none";
	}
	else
	{
		document.getElementById('sourceImage').style.display = "inline";
		document.getElementById('designImage').style.display = "none";
	}
	
	this.Command.ExecCommand();
}

SwitchButton.prototype.Enable = function()
{
	this.Update() ;
}

SwitchButton.prototype.Disable = function()
{
	this.State = CMD_DISABLED ;
	this.DOMDiv.className = 'bt_off';
	/*this.DOMDiv.onmouseover	= null ;
	this.DOMDiv.onmouseout	= null ;
	this.DOMDiv.onclick		= null ;*/
}