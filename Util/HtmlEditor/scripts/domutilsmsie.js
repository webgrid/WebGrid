/*
*	IE specific DOM utils implementation
*/

// get selected element
DOMUtils.GetSelectedElement = function()
{
	if ( Engine.Document.selection.type == 'Control' )
	{
		var range = Engine.Document.selection.createRange();
		if (range && range.item ) return range.item(0);
	}
}

DOMUtils.GetParentElement = function()
{
	switch (Engine.Document.selection.type)
	{
		case 'Text':
			return Engine.Document.selection.createRange().parentElement();
		case 'Control' :
			return FCKSelection.GetSelectedElement().parentElement;
		default : // type None
			return 'None';
	}
}
DOMUtils.SelectNode = function(node)
{
	Engine.Focus();
	Engine.Document.selection.empty();
	range = Engine.Document.selection.createRange();
	range.moveToElementText(node);
	range.select();
}

// nodeName - node tag name
DOMUtils.MoveToAncestorNode = function( nodeName )
{
	nodeName = nodeName.toUpperCase();
	var oNode;
	var oRange = Engine.Document.selection.createRange() ;
	if ("Control" == Engine.Document.selection.type)
	{
		for ( i = 0 ; i < oRange.length ; i++ )
		{
			if (oRange(i).parentNode)
			{
				oNode = oRange(i).parentNode ;
				break ;
			}
		}
	}
	else
	{
		oNode = oRange.parentElement() ;
	}

	while ( oNode && oNode.nodeName != nodeName )
	{
		oNode = oNode.parentNode ;
	}
	return oNode ;
}
