/*
* IE specific XHTML functions
*/

// create XML pareser
// type: "XmlHttp" or "DOMDocument"
XHTML.prototype.GetXMLParser = function(type)
{
	var o;
	switch (type)
	{
		case "XmlHttp":
			o = new ActiveXObject("MSXML2.XmlHttp");
			break ;
		case "DOMDocument" :
			o = new ActiveXObject("MSXML2.DOMDocument");
			break ;
		default:
			alert("XML Parser could not be found");
			o = null; 
	}
	return o;
}



XHTML.prototype.AppendAttributes = function(xmlNode,htmlNode,node,nodeName)
{
	var aAttributes = htmlNode.attributes ;
	for ( var n = 0 ; n < aAttributes.length ; n++ )
	{
		var oAttribute = aAttributes[n] ;

		if ( oAttribute.specified )
		{
			var sAttName = oAttribute.nodeName.toLowerCase() ;
			var sAttValue ;

			//Ignore any internal attribute starting with "__".
			if (-1 != sAttName.indexOf('__',0))
				continue;
			// The following must be done because of a bug on IE regarding the style
			// attribute. It returns "null" for the nodeValue.
			if ( sAttName == 'style' )
				sAttValue = htmlNode.style.cssText ;
			// There are two cases when the oAttribute.nodeValue must be used:
			//		- for the "class" attribute
			//		- for events attributes (on IE only).
			else if ( sAttName == 'class' || sAttName.indexOf('on') == 0 )
				sAttValue = oAttribute.nodeValue ;
			else if ( nodeName == 'body' && sAttName == 'contenteditable' )
				continue ;
			// XHTML doens't support attribute minimization like "CHECKED". 
			else if ( oAttribute.nodeValue === true )
				sAttValue = sAttName ;
			// We must use getAttribute to get it exactly as it is defined.
			else if ( ! (sAttValue = htmlNode.getAttribute( sAttName, 2 ) ) )
				sAttValue = oAttribute.nodeValue ;
			this.AppendAttribute( node, sAttName, sAttValue ) ;
		}
	}
} 

XHTML.prototype.LoadRules = function()
{
	this.Rules['a'] = this.Rule;
	this.Rules['img'] = this.ImgRule;
	this.Rules['font'] = this.FontRule;
	this.Rules['div'] = this.DivRule;
	this.Rules['option'] = this.OptionRule;
	this.Rules['table'] = this.TableRule;
	this.Rules['td'] = this.TableRule;
}

XHTML.prototype.ToString = function()
{
	return this.Root.xml;
}

XHTML.prototype.AppendEntity = function(xmlNode, entity)
{
	xmlNode.appendChild( this.XMLParser.createEntityReference( entity ) ) ;
}

// See KB article "<FONT> -> <FONT size=+0>"
XHTML.prototype.FontRule = function( node, htmlNode, object)
{
	if ( node.attributes.length == 0 )
		node = this.XMLParser.createDocumentFragment() ;
	object.AppendChildNodes( node, htmlNode ) ;
	return node ;
}
// See KB article "Loss "align" attribute for DIV"
XHTML.prototype.DivRule = function( node, htmlNode, object)
{
	if ( htmlNode.align.length > 0 )
		object.AppendAttribute( node, 'align', htmlNode.align ) ;
	object.AppendChildNodes( node, htmlNode ) ;
	return node ;
}
// See KB article "Ignores "SELECTED" attribute"
XHTML.prototype.OptionRule = function( node, htmlNode, object)
{
	if ( htmlNode.selected && !node.attributes.getNamedItem( 'selected' ) )
		object.AppendAttribute( node, 'selected', 'selected' ) ;
	object.AppendChildNodes( node, htmlNode ) ;
	return node ;
}


