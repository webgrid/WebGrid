/*
* IE specific functions
*/

Engine.InitializeComponent = function()
{
	this.Document.attachEvent("onselectionchange", OnSelectionChange);
	var style = Engine.Document.createElement("STYLE");
	style.type	= 'text/css' ;
	//style.innerText = '';
	style.cssText = ".CTRLhideElm {border: 1px dashed #BBBBBB !important;}";
	Engine.Document.getElementsByTagName("HEAD")[0].appendChild(style);
	
	// keyboards event
	Engine.Document.attachEvent("onkeypress", Engine.OnKeyPress);
	Engine.Document.attachEvent("onselectionchange", Engine.OnSelectionChange ) ;
}

Engine.OnSelectionChange = function()
{
	Engine.Events.Fire("OnSelectionChange") ;
}

Engine.OnKeyPress = function(e)
{
	if(13 == e.keyCode && !e.altKey && !e.ctrlKey && !e.shiftKey)
	{
		if(Config.GetBRonCR())
		{
			if (Engine.Document.queryCommandState("InsertOrderedList") || Engine.Document.queryCommandState("InsertUnorderedList"))
			{
				return true;
			}
			Engine.InsertHtml("<br/>&nbsp;");
			var range = Engine.Document.selection.createRange() ;
			range.moveStart( 'character', -1 ) ;
			range.select() ;
			Engine.Document.selection.clear() ;
			return false ;
		}
	}
}


Engine.PastePlainText = function()
{
	Engine.SetFocus();
	var clipboardText = clipboardData.getData("Text");
	Engine.Document.execCommand("Paste", false, clipboardText); 
}

Engine.PasteFromWord = function()
{
	Engine.SetFocus();
	var html = this.ClipboardHTML();
	var cleanUp = new CleanUp();
	html = cleanUp.CleanWordText2(html);
	delete cleanUp;
	var selection = Engine.Document.selection;
	if ("none" != selection.type.toLowerCase())
		selection.clear() ;
	selection.createRange().pasteHTML(html) ;
}

Engine.ClipboardHTML = function()
{
	var div = Engine.Document.getElementById('_HtmlEditorTmp_clipboardHTML');
	if (!div) 
	{
		var div = Engine.Document.createElement('DIV');
		div.id = '_HtmlEditorTmp_clipboardHTML';
		with (div.style) 
		{
			visibility = 'hidden';
			overflow = 'hidden';
			position = 'absolute';
			width = 1;
			height = 1;
		}
		Engine.Document.body.appendChild(div);
	}
	div.innerHTML = '';
	var rng = Engine.Document.body.createTextRange();
	rng.moveToElementText(div);
	rng.execCommand('Paste');
	var html = div.innerHTML;
	div.innerHTML = '';
	Engine.Document.body.removeChild(div);
	return html;
}

Engine.InsertHtml = function(html)
{
	Engine.SetFocus();
	var sel = Engine.Document.selection;
	if ("none" != sel.type.toLowerCase())
		sel.clear();
	sel.createRange().pasteHTML(html);
}

Engine.InsertElement = function(element)
{
	Engine.InsertHtml(element.outerHTML);
}
