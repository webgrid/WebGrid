/*
* Base engine class for HtmlEditor 
* clientId - the name of the control instance 
*/

// Editor Status.
var STATUS_NOTLOADED = 0;
var STATUS_ACTIVE = 1;
var STATUS_COMPLETE	= 2;

var Engine;

if(null == Engine)
{
	Engine = new Object();
	Engine.Status = STATUS_NOTLOADED;
	Engine.ID = Config.ClientID;
	Engine.Dialog = null;
	Engine.Config = Config;
	Engine.BrowserObj = Browser;
	Engine.Mode = Config.GetMode();
	
}

Engine.Init = function() 
{
	this.Events = new Events(Engine);
	// editor's window Get the editor's window and document (DOM)
	this.Window	= window.frames['editor'];
	// editor's document 
	this.Document = Engine.Window.document;
	
	
	this.Utils = DOMUtils;
	
	// control's hidded Text field 
	// TODO: check IE 5.5/NN
	this.TextField = window.parent.document.getElementById(Engine.ID); 
	// add onsubmit event handler
	DOMUtils.AddControlFormSubmitHandler(Engine.SetDocumentText);
	
	Engine.SetDocument(this.GetDocumentText());
	this.ToolBars = new Toolbars();
	this.ToolBars.Load(); 
	
	
	this.Events.AddHandler("OnSelectionChange", Engine.ToolBars.Refresh);
	Engine.ToolBars.Refresh();
	//Engine.ToolBars.Refresh();
}


// run the current instance of the editor
// this function should be call after all files are loaded
Engine.Run = function()
{
	this.Init();
	this.InitializeComponent();
	window.setTimeout( "window.onresize()", 10 );
	Engine.SetFocus();
	DOMUtils.ShowHiddenElements(Engine.Document,"CTRLhideElm");
	Engine.Events.Fire("OnSelectionChange");
}

Engine.GetDocumentText = function()
{
	return Engine.TextField.value;
} 

Engine.SetDocumentText = function()
{
    try
	{
	var designMode = (Engine.Mode == DESIGN_MODE) ? true : false;
	if(!designMode)
		Engine.SwitchMode();
	if('html' == Config.GetTextFormat().toLowerCase())
	{
		Engine.TextField.value = Engine.Document.body.innerHTML;	
	}
	else
	{
		var Xhtml = new XHTML();
		Engine.TextField.value= Xhtml.CreateXHTML(Engine.Document.body, true);
	}
	}
	catch(e){}
} 

// TODO: check cross-platform
Engine.SetFocus = function()
{
	try
	{
		Engine.Window.focus();
	}
	catch(e){}
}

// Move to browser specific files
Engine.SetDocument = function(html)
{
	editHtml =
		Config.Properties["doctype"] +
		'<html>' +
		'<head><title></title>';// +
	//editHtml += "<style>.CTRLhideElm {border: 1px dashed #BBBBBB !important;}</style>"; 
	//editHtml += "<style>TABLE {	border: #d3d3d3 1px dotted ;}</style>";

		//'<link href="' + FCKConfig.EditorAreaCSS + '" rel="stylesheet" type="text/css" />' +
		//'<link href="' + FCKConfig.FullBasePath + 'css/fck_internal.css' + '" rel="stylesheet" type="text/css" _fcktemp="true" />';

	//	sHtml += FCK.TempBaseTag;
	editHtml += '</head><body>' + html  + '</body></html>';
	
	try
	{
		this.Document.designMode = 'on';
	}
	catch(e){};
	
	Engine.SetFocus() ;
	if(Browser.IsMozilla)
	{
		// TODO: check that Control.Text isn't full page 
		this.Document.body.innerHTML = html;
	}
	else
	{
		this.Document.open();
		this.Document.writeln("<html><head><style>.CTRLhideElm {border: 1px solid #BBBBBB !important;}</style></head><body></body></html>");
		this.Document.close();
		Engine.Document.body.createTextRange().pasteHTML(html);
	}
	
	DOMUtils.ShowHiddenElements(Engine.Document,"CTRLhideElm");
}

Engine.SwitchMode = function()
{
  var designMode = (Engine.Mode == DESIGN_MODE) ? true : false;
	document.getElementById('eEditor').style.display	= designMode ? 'none' : '' ;
	document.getElementById('eSource').style.display	= designMode ? '' : 'none' ;
	Engine.ToolBars.RefreshMode(designMode);
	if(designMode)
	{
		var source;
		if('html' == Config.GetTextFormat().toLowerCase())
		{
		    	source = Engine.Document.body.innerHTML;	
		}
		else
		{
			var Xhtml = new XHTML();
			source = Xhtml.CreateXHTML(Engine.Document.body, true);
		}
		document.getElementById('source').value = source;
	}
	else
	{
	    
	    Engine.SetDocument(document.getElementById('source').value);
		window.setTimeout(Engine.ToolBars.Refresh, 1);
		Engine.InitializeComponent();
		Engine.ToolBars.Refresh();
		Engine.SetFocus();
    }
	
	Engine.Mode = designMode ? SOURCE_MODE : DESIGN_MODE;
}

Engine.ExecCommand = function(commandName, commandParameter)
{
	Engine.SetFocus();
	Engine.Document.execCommand(commandName, false, commandParameter); 
	Engine.Events.Fire("OnSelectionChange");
}
// returns const
Engine.GetCommandState = function(commandName)
{
	try
	{
		if(Engine.Document.queryCommandEnabled(commandName))
		{
			return Engine.Document.queryCommandState(commandName)? CMD_APPLIED : CMD_ENABLED;
		}
		else
		{
			return CMD_DISABLED;
		}
	}
	catch(e)
	{
		return CMD_ENABLED;
	}
}
Engine.GetCommandValue = function(commandName)
{
	var value;
	var state = Engine.GetCommandState(commandName);
	
	if (CMD_DISABLED == state) 
		return null;
	try
	{
		value = Engine.Document.queryCommandValue(commandName);
	}
	catch(e) {}
	
	return value ? value : "";
}


// event handlers
function OnSelectionChange()
{
	Engine.Events.Fire("OnSelectionChange");
}

// creates and returns element's array
Engine.CreateLink = function()
{
	var uniqueHref = "#he" + new Date().toString() + "#" ;
	var res = Engine.ExecCommand("CreateLink", uniqueHref);
	var elements = DOMUtils.GetElementsByAttributeValue(Engine.Document.body, "a", "href", uniqueHref);
	return elements;
}

Engine.CreateImage = function()
{
	var uniqueSrc = "#src" + new Date().toString() + "#" ;
	var res = Engine.ExecCommand("InsertImage", uniqueSrc);
	var elements = DOMUtils.GetElementByAttributeValue(Engine.Document.body, "img", "src", uniqueSrc);
	return elements;
}

Engine.CreateElement = function(tagName)
{
	var element  = Engine.Document.createElement(tagName)
	element.setAttribute( '_heelement', 1 ) ;
	Engine.InsertElement(element) ;
	var insertedElement = DOMUtils.GetElementByAttributeValue(Engine.Document.body,"table","_heelement",1);
	if(insertedElement)
	{
		insertedElement.removeAttribute('_heelement');
		return insertedElement;
	}
	return null;
}