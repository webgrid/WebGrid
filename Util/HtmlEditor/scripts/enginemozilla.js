/*
* Mozilla (Geko) specific 
*/


Engine.InitializeComponent = function()
{
	var style = Engine.Document.createElement("STYLE");
	style.type	= 'text/css' ;
	style.innerHTML = ".CTRLhideElm {border: 1px solid #BBBBBB !important;}";
	Engine.Document.getElementsByTagName("HEAD")[0].appendChild(style);

	if(!Browser.CheckOpera())
	{
		Engine.Document.addEventListener("keypress", Engine.OnKeyPress, true);
		Engine.Document.addEventListener("keyup", Engine.DelayOnSelectionChange, false);
		Engine.Document.addEventListener("mouseup", Engine.OnSelectionChange, false);
	}
	else
	{
		Engine.Document.attachEvent("onkeypress", Engine.OnKeyPress);
		Engine.Document.attachEvent("keyup", Engine.DelayOnSelectionChange);
		Engine.Document.attachEvent("onmouseup", Engine.OnSelectionChange);
	}
	
	if(Browser.IsMozilla)
	{
		window.setTimeout("window.onresize()", 1);
	}
}

Engine.DelayOnSelectionChange = function()
{
	if (Engine.OnChangeTimer)
		window.clearTimeout(Engine.OnChangeTimer);
	// prevent to slowing down when tipying to quickly 
	Engine.OnChangeTimer = window.setTimeout(Engine.OnSelectionChange, 100);
}

Engine.OnSelectionChange = function()
{
	Engine.Events.Fire("OnSelectionChange");
}

Engine.OnKeyPress = function(e)
{
	if(13 == e.keyCode && !e.altKey && !e.ctrlKey && !e.shiftKey)
	{
		if(!Config.GetBRonCR())
		{
			if (Engine.Document.queryCommandState("InsertOrderedList") || Engine.Document.queryCommandState("InsertUnorderedList"))
			{
				return true;
			}
			
			Engine.InsertHtml("<p>&nbsp;</p>");
			e.preventDefault() ;
			e.stopPropagation() ;
			return false ;
		}
			
	}
}


Engine.PastePlainText = function()
{
	
	var command = new DialogCommand("PastePlainText","paste.htm",400,330,null,true);
	command.ExecCommand();
	
}

Engine.PasteFromWord = function()
{
	var command = new DialogCommand("PasteFromWord","paste.htm",400,330,null,true);
	command.ExecCommand();
}

Engine.InsertHtml = function(html)
{
	//html = FCKConfig.ProtectedSource.Protect( html ) ;
	//html = FCK.ProtectUrls( html ) ;
	var selection, range, htmlFragment;

	// Delete the actual selection.
	selection  = DOMUtils.Delete();
	
	// Get the first available range.
	var range = selection.getRangeAt(0) ;
	
	// Create a fragment with the input HTML.
	var htmlFragment = range.createContextualFragment(html) ;
	
	// Get the last available node.
	var lastNode = htmlFragment.lastChild ;

	// Insert the fragment in the range.
	range.insertNode(htmlFragment) ;
	
	// Set the cursor after the inserted fragment.
	DOMUtils.SelectNode(lastNode) ;
	DOMUtils.Collapse(false);
	Engine.SetFocus() ;
}
Engine.InsertElement = function(element)
{
	var res = true;
	Engine.SetFocus();
	try
	{
	var sel = DOMUtils.Delete();
	var range = sel.getRangeAt(0);
	range.insertNode(element);
	DOMUtils.SelectNode(element);
	DOMUtils.Collapse(false);
	}catch(ex)
	{
		res = false;
	}
	if(!res && Browser.CheckOpera())
	{
		var elm = Engine.CreateImage();	
		elm.outerHTML = element.outerHTML
	}
}