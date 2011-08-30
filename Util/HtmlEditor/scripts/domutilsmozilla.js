/*
* IE specific DOM utils implementation
*/

// get selected element
DOMUtils.GetSelectionType = function()
{
		var selection = Engine.Window.getSelection();
		if(selection && 1 == selection.rangeCount)
		{
			range = selection.getRangeAt(0);
			if (range.startContainer == range.endContainer && 1 == (range.endOffset - range.startOffset))
				return "Control";
		}
	return "Text";
}

DOMUtils.GetParentElement = function()
{
	switch (DOMUtils.GetSelectionType)
	{
	/*	case 'Text':
			return Engine.Document.selection.createRange().parentElement();
		case 'Control' :
			return DOMUtils.GetSelectedElement().parentElement;
		default : // type None
			return; */
	}
}

DOMUtils.GetSelectedElement = function()
{
	if ( DOMUtils.GetSelectionType() == 'Control' )
	{
		selection = Engine.Window.getSelection();
		return selection.anchorNode.childNodes[selection.anchorOffset];
	}
}


DOMUtils.SelectNode = function(node)
{
	var range, selection;
	Engine.SetFocus() ;
	range = Engine.Document.createRange();
	range.selectNode(node);
	selection = Engine.Window.getSelection();
	selection.removeAllRanges();
	selection.addRange(range);
}

DOMUtils.MoveToAncestorNode = function(nodeName)
{
	nodeName = nodeName.toUpperCase();
	var oContainer = this.GetSelectedElement() ;
	if ( ! oContainer )
		oContainer = Engine.Window.getSelection().getRangeAt(0).startContainer ;

	while ( oContainer )
	{
		if ( oContainer.tagName == nodeName ) 
			return oContainer ;
		oContainer = oContainer.parentNode ;
	}
	return null;
}

DOMUtils.Delete = function()
{
	var selection = Engine.Window.getSelection();
	for(var i=0 ;i < selection.rangeCount; i++)
		selection.getRangeAt(i).deleteContents();
	return selection;
}
DOMUtils.Collapse = function(moveToStart)
{
	var selection = Engine.Window.getSelection();
	if (null == moveToStart || moveToStart === true )
		selection.collapseToStart();
	else
		selection.collapseToEnd();
}
