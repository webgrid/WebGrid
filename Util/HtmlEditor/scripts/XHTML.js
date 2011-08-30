/*
*  XHTML helper
*/

XHTML = function()
{
	this.CurrentLevel = 0;
	this.DTDEmptyElements = /^(?:BASE|META|LINK|HR|BR|PARAM|IMG|AREA|INPUT)$/i ;
	this.DTDElementName = /^[A-Za-z_:][\w.\-:]*$/ ;
	this.DTDBlockElements = /^(?:P|OL|UL|LI|DIV|H1|H2|H3|H4|H5|H6|ADDRESS|PRE|TD|TH)$/i
	this.TableBorder = /\s*CTRLhideElm\s*/ ;
	this.Rules = new Object();
	this.LoadRules();
}
 
// TODO: может быть включать рутовую ноду , а может и не включать, разберусь потом
XHTML.prototype.CreateXHTML = function(sourceNode, formatCode)
{

	this.FormatCode = formatCode;
	this.SourceNode = sourceNode;
	
	// Special blocks are blocks of content that remain untouched during the
	// process. It is used for SCRIPTs and STYLEs.
	this.ImmutableBlocks = new Array();

	// Create the XML DOMDocument object.
	this.XMLParser = this.GetXMLParser("DOMDocument");

	// Add a root element that holds all child nodes.
	this.Root = this.XMLParser.appendChild(this.XMLParser.createElement("xhtml"));

	XHTML.CurrentLevel++ ;

	// TODO: TBD
	//if (includeNode)
	//	this.AppendNode( this.Root, node ) ;
	//else
		this.AppendChildNodes(this.Root, this.SourceNode, false) ;

	
	// Get the resulting XHTML as a string.
	var xhtml = this.ToString(); 

	// Crop the "XHTML" root node.
	xhtml = xhtml.substr(7, xhtml.length - 15);
	
	// Remove the trailing <br> added by Gecko.
	if (Browser.IsMozilla)
		xhtml= xhtml.replace( /<br\/>$/, '' ) ;

	// Add a space in the tags with no closing tags, like <br/> -> <br />
	xhtml = xhtml.replace( /\/>/g, ' />');

	// TODO: implements Formatter class
	if (formatCode)
		//xhtml = CodeFormatter.Format(xhtml) ;
	this.XMLParser = null ;

	return xhtml;
}
XHTML.prototype.AppendChildNodes = function(xmlNode,htmlNode,isBlockElement)
{
	var iCount = 0 ;
	
	var oNode = htmlNode.firstChild ;

	while ( oNode )
	{
		if ( this.AppendNode( xmlNode, oNode ) )
			iCount++ ;

		oNode = oNode.nextSibling ;
	}
	
	if ( iCount == 0 )
	{
	
		// TODO: Fill empty block if need
		// TODO: make configuration properties
		// dont' fill empty block
		/*if ( isBlockElement && Config.FillEmptyBlocks )
		{
			this.AppendEntity( xmlNode, 'nbsp' ) ;
			return ;
		}*/

		if(!this.DTDEmptyElements.test(htmlNode.nodeName))
			xmlNode.appendChild(this.XMLParser.createTextNode(''));
	}
} 

XHTML.prototype.AppendNode = function( xmlNode, htmlNode )
{
	if ( !htmlNode )
		return ;

	switch ( htmlNode.nodeType )
	{
		case 1 : // NODE_ELEMENT (1)
		
			// Known mozilla feature
			if ( Browser.IsMozilla && htmlNode.hasAttribute('_moz_editor_bogus_node') )
				return false;
			
			// Get the element name.
			var nodeName = htmlNode.nodeName ;
			
			//Add namespace:
			if ( Browser.IsMSIE && htmlNode.scopeName && htmlNode.scopeName != 'HTML' )
				nodeName = htmlNode.scopeName + ':' + nodeName ;

			// Check if the node name is valid, otherwise ignore this tag.
			// If the nodeName starts with a slash, it's closing tag.
			if ( !this.DTDElementName.test(nodeName))
				return false ;

			nodeName = nodeName.toLowerCase() ;

			if ( Browser.IsMozilla && nodeName == 'br' && htmlNode.hasAttribute('type') && htmlNode.getAttribute('type', 2) == '_moz' )
				return false ;

			// The already processed nodes must be marked to avoid then to be duplicated (bad formatted HTML).
			// So here, the "mark" is checked... if the element is Ok, then mark it.
			if ( htmlNode.__level && htmlNode.__level == XHTML.CurrentLevel)
				return false ;

			var node = this.CreateNode(nodeName);
			
			// Add all attributes.
			this.AppendAttributes( xmlNode, htmlNode, node, nodeName) ;
			
			htmlNode.__level = this.CurrentLevel;

			// Tag specific processing.
			var rule = this.Rules[nodeName] ;

			if (rule)
			{
				node = rule(node,htmlNode, this) ;
				if (!node) 
					break;
			}
			else
				this.AppendChildNodes( node, htmlNode, this.DTDBlockElements.test(nodeName));

			xmlNode.appendChild( node ) ;

			break ;

		case 3 : //NODE_TEXT (3)
			var nodeValue = htmlNode.nodeValue.replace( /\n/g, ' ');
			this.AppendTextNode(xmlNode, nodeValue);
			break ;

		case 8 : // NODE_COMMENT (8)
			try 
			{ 
				xmlNode.appendChild( this.XMLParser.createComment(htmlNode.nodeValue));
			}
			catch (e) {}
			break ;

		// Unknown or Unsupported Node type.
		default :
			xmlNode.appendChild( this.XMLParser.createComment( "Element not supported - Type: " + htmlNode.nodeType + " Name: " + htmlNode.nodeName ) ) ;
			break ;
	}
	return true ;
}

XHTML.prototype.AppendTextNode = function(targetNode, textValue )
{
	var asPieces = textValue.match(HTMLEntities.Regexp) ;
	if ( asPieces )
	{
		for ( var i = 0 ; i < asPieces.length ; i++ )
		{
			if ( asPieces[i].length == 1 )
			{
				var sEntity = HTMLEntities.Items[asPieces[i]];
				if ( sEntity != null )
				{
					this.AppendEntity( targetNode, sEntity ) ;
					continue ;
				}
			}
			targetNode.appendChild( this.XMLParser.createTextNode( asPieces[i] ) ) ;
		}
	}
}

XHTML.prototype.CreateNode = function(nodeName)
{
	/* add properties ForceStrongEm
	switch (nodeName)
	{
		case 'b' :
			nodeName = 'strong' ;
			break ;
		case 'i' :
			nodeName = 'em' ;
			break ;
	}*/
	return this.XMLParser.createElement(nodeName);
} 



XHTML.prototype.AppendAttribute = function( xmlNode, attributeName, attributeValue )
{
	try
	{
		// Create the attribute.
		var oXmlAtt = this.XMLParser.createAttribute( attributeName ) ;

		oXmlAtt.value = attributeValue ? attributeValue : '' ;

		// Set the attribute in the node.
		xmlNode.attributes.setNamedItem( oXmlAtt ) ;
	}
	catch (e)
	{}
}





// Custom TAGs' rules

XHTML.prototype.ARule = function( node, htmlNode, object)
{
	var sSavedUrl = htmlNode.getAttribute( '_fcksavedurl' ) ;
	if ( sSavedUrl && sSavedUrl.length > 0 )
		object.AppendAttribute( node, 'href', sSavedUrl ) ;

	object.AppendChildNodes( node, htmlNode, false ) ;

	return node ;
}
XHTML.prototype.ImgRule = function( node, htmlNode, object)
{
	// The "ALT" attribute is required in XHTML.
	 if (!node.attributes.getNamedItem('alt'))
		object.AppendAttribute( node, 'alt', '' );

	var sSavedUrl = htmlNode.getAttribute( '_fcksavedurl' ) ;
	if ( sSavedUrl && sSavedUrl.length > 0 )
		object.AppendAttribute( node, 'src', sSavedUrl ) ;

	return node ;
}
// clear tmp classes
XHTML.prototype.TableRule = function(node, htmlNode, object)
{
	var styleAtt = node.attributes.getNamedItem('class') ;
	if(styleAtt)
	{
		if(object.TableBorder.test(styleAtt.nodeValue))
		{
			var sClass = styleAtt.nodeValue.replace(object.TableBorder, '');
			if ( sClass.length == 0 )
				node.attributes.removeNamedItem( 'class' ) ;
			else
				object.AppendAttribute( node, 'class', sClass ) ;
		}
		
	}
	object.AppendChildNodes( node, htmlNode, false ) ;
	return node ;
}

/*	TODO: Implements the following rule
	also see browser specific scripts
XHTML.ScriptRule 
XHTML.StyleRule
XHTML.BaseRule
*/