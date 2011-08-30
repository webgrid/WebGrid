/*
*
*/
// create XML parser
// type: "XmlHttp" or "DOMDocument"
XHTML.prototype.GetXMLParser  = function(type)
{
	var o;
	switch (type)
	{
		case "DOMDocument":
			o = document.implementation.createDocument( '', '', null );
			break;
		case "XmlHttp":
			o = new XMLHttpRequest();
			break;
		default:
			alert("XML Parser could not be found");
			o = null; 
	}
	return o ;
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

			// Ignore internal attribute starting with "__".
			if (-1 != sAttName.indexOf('__',0))
				continue;
			// There is a bug in Mozilla that returns '_moz_xxx' attributes as specified.
			if ( sAttName.indexOf( '_moz' ) == 0 )
				continue ;
			// There are one cases (on Gecko) when the oAttribute.nodeValue must be used:
			//		- for the "class" attribute
			else if ( sAttName == 'class' )
				sAttValue = oAttribute.nodeValue ;
			// XHTML does't support attribute minimization like "CHECKED". It must be transformed to checked="checked".
			else if ( oAttribute.nodeValue === true )
				sAttValue = sAttName ;
			else
				sAttValue = htmlNode.getAttribute( sAttName, 2 ) ;	// We must use getAttribute to get it exactly as it is defined.

			/*if ( FCKConfig.ForceSimpleAmpersand && sAttValue.replace )
				sAttValue = sAttValue.replace( /&/g, '___FCKAmp___' ) ;*/
			
			this.AppendAttribute( node, sAttName, sAttValue ) ;
		}
	}
} 

XHTML.prototype.LoadRules = function()
{
	this.Rules['a'] = this.Rule;
	this.Rules['img'] = this.ImgRule;
	this.Rules['table'] = this.TableRule;
	this.Rules['td'] = this.TableRule;
}

XHTML.prototype.ToString = function()
{
	var o = new XMLSerializer() ;
	return o.serializeToString(this.Root).replace(/#he%/g,"&");
}

// Gecko based bug, createEntityReference doesn't works
XHTML.prototype.AppendEntity = function(xmlNode, entity)
{
	xmlNode.appendChild( this.XMLParser.createTextNode("#he%" + entity + ";"));
}

