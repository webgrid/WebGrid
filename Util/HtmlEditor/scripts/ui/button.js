/*
* Simple toolbar button
*/

var Button = function(commandName, text, tooltip, style , contextSensitive, enabledDesignMode)
{
	this.Command			= CommandCollection.GetCommand(commandName);
	this.Text				= text ? text : commandName;
	this.Tooltip			= tooltip ? tooltip : ( text ? text : commandName);
	//this.Style				= style ? style : FCK_TOOLBARITEM_ONLYICON;
	//this.IconPath			= Config.GetBasePath() + '' + commandName.toLowerCase() + '.gif';
	this.IconPath			= commandName.toLowerCase() + '.gif';
	this.State				= CMD_DISABLED;
	this.ContextSensitive = contextSensitive;
	this.DesignMode = enabledDesignMode; // button supports design mode only 
}

//Button.prototype = new ToolBarItem();

Button.prototype.Create = function(parent)
{
	this.DOMDiv = document.createElement( 'div' );
	this.DOMDiv.className = 'bt_off';

	this.DOMDiv.Button = this;
	
	var sHtml =
		'<table title="' + this.Tooltip + '" cellspacing="0" cellpadding="0" border="0" align=center valign=middle>' +
			'<tr>';
	
	//if ( this.Style != FCK_TOOLBARITEM_ONLYTEXT ) 
	if(Config.GetEditorImagePath())
		sHtml += '<td width=24 align="center" class="TB_Icon"><img src="' + Config.GetEditorImagePath() + this.IconPath + '" width="20" height="20"></td>';
	else
		sHtml += '<td width=24 align="center" class="TB_Icon"><img src="htmleditor.aspx?res=image&id=' + this.IconPath + '" width="20" height="20"></td>';
	
	//if ( this.Style != FCK_TOOLBARITEM_ONLYICON ) 
	//	sHtml += '<td class="TB_Text" nowrap>' + this.Label + '</td>';
	
	sHtml +=	
			'</tr>' +
		'</table>';
	
	this.DOMDiv.innerHTML = sHtml;
	var oCell = parent.DOMRow.insertCell(-1);
	oCell.appendChild( this.DOMDiv );
	
	// DEBUG VERSION ONLY!
	/*this.DOMDiv.onmouseover	= Button_OnMouseOnOver;
	this.DOMDiv.onmouseout	= Button_OnMouseOnOut;
	this.DOMDiv.onclick		= Button_OnClick;*/
}

Button.prototype.Update = function()
{
	var newState = this.Command.QueryCommandState();
	if(newState == this.State) 
		return;
	this.State = newState;
	
	switch (this.State )
	{
		case CMD_APPLIED:
			this.DOMDiv.className = 'bt_on' ;

			this.DOMDiv.onmouseover	= Button_OnMouseOnOver ;
			this.DOMDiv.onmouseout	= Button_OnMouseOnOut ;
			this.DOMDiv.onclick		= Button_OnClick ;
			
			break ;
		case CMD_ENABLED :
			this.DOMDiv.className = 'bt_off' ;
			this.DOMDiv.onmouseover	= Button_OnMouseOffOver ;
			this.DOMDiv.onmouseout	= Button_OnMouseOffOut ;
			this.DOMDiv.onclick		= Button_OnClick ;
			break ;
		default :
			this.Disable() ;
			break ;
	}
}

function Button_OnMouseOnOver()
{
	this.className = 'bt_on bt_on_over';
}

function Button_OnMouseOnOut()
{
	this.className = 'bt_off';
}
	
function Button_OnMouseOffOver()
{
	this.className = 'bt_on bt_off_over';
}

function Button_OnMouseOffOut()
{
	this.className = 'bt_off';
}
	
function Button_OnClick(e)
{
	this.Button.Click(e);
	return false;
}

Button.prototype.Click = function(e)
{
	this.Command.ExecCommand();
}

Button.prototype.Enable = function()
{
	this.Update() ;
}

Button.prototype.Disable = function()
{
	this.State = CMD_DISABLED ;
	this.DOMDiv.className = 'bt_disabled';
	this.DOMDiv.onmouseover	= null ;
	this.DOMDiv.onmouseout	= null ;
	this.DOMDiv.onclick		= null ;
}