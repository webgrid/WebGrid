/*
*	Utility functions, DOM, html elements, etc  
*/

var	DOMUtils = new Object();

Engine.DOMUtils = DOMUtils;

// add event handler 
DOMUtils.AddControlFormSubmitHandler = function(handler)
{
	// Gets the linked field form
	var controlForm = Engine.TextField.form;

	// Attaches the functionPointer call to the onsubmit event
	if (controlForm.attachEvent)
	{
		controlForm.attachEvent("onsubmit", handler);
	}
	else
	{
		controlForm.addEventListener("submit", handler, true);
	}
	 if ( window.parent.document.getElementById('WebGrid_EnabledAnthem') == null ) {
	
		if ( null == controlForm.updateHtmlEditor) 
		{
			controlForm.updateHtmlEditor = new Array();
			if (!controlForm.originalSubmit && ( typeof( controlForm.submit ) == 'function' 
				|| ( !controlForm.submit.tagName && !controlForm.submit.length ) ) )
			{
				//alert("add handler: " + Engine.ID);
				controlForm.oldSubmit = controlForm.submit;
				controlForm.submit = DOMUtils_Submit;
			}
		}
		controlForm.updateHtmlEditor[controlForm.updateHtmlEditor.length] = handler;
	}
}

function DOMUtils_Submit()
{
	if (this.updateHtmlEditor)
	{
		// Call event handlers for all instances of the editor
		for (var i = 0; i < this.updateHtmlEditor.length; i++)
		{
			this.updateHtmlEditor[i]();
		}
	}
	this.oldSubmit();
}

/**
 * Returns a element by a specific attribute and it's value.
 *
 * @param {HTMLElement} n Element to search in.
 * @param {string} e Element name to search for.
 * @param {string} a Attribute name to search for.
 * @param {string} v Attribute value to search for.
 * @return HTML element that matched the criterias or null on failure.
 * @type HTMLElement
 */
DOMUtils.GetElementByAttributeValue = function(node, elmentName, attributeName, attributeValue) 
{
	return (nodes = this.GetElementsByAttributeValue(node, elmentName, attributeName, attributeValue)).length == 0 ? null : nodes[0];
}

/**
 * Returns a element array by a specific attribute and it's value.
 *
 * @param {HTMLElement} n Element to search in.
 * @param {string} e Element name to search for.
 * @param {string} a Attribute name to search for.
 * @param {string} v Attribute value to search for.
 * @return HTML element array that matched the criterias or null on failure.
 * @type Array
 */
DOMUtils.GetElementsByAttributeValue = function(node, elmentName, attributeName, attributeValue) 
{

	var nodes = node.getElementsByTagName(elmentName); 
	var o = new Array();
	for (i=0; i<nodes.length; i++) {
		if (-1 != this.GetAttribute(nodes[i], attributeName).indexOf(attributeValue))
			o[o.length] = nodes[i];
	}
	return o;
}

/**
 * Returns the attribute value of a element or the default value if it wasn't found.
 *
 * @param {HTMLElement} elm HTML element to get attribute from.
 * @param {string} name Attribute name to retrive.
 * @param {string} default_value Optional default value to return, this value defaults to a empty string.
 * @return Attribute value or default value if it wasn't found in element.
 * @type string
 */
DOMUtils.GetAttribute = function(node, attributeName, defaultValue) {
	if (typeof(defaultValue) == "undefined")
		defaultValue = "";

	// Not a element
	if (!node || node.nodeType != 1)
		return defaultValue;

	var value = node.getAttribute(attributeName,2);

	// Try className for class attrib
	if (attributeName == "class" && !value)
		value = node.className;

	/*// Workaround for a issue with Firefox 1.5
	if (Browser.IsMozilla && attributeName == "src" && node.src != null && node.src != "")
		value = elm.src;

	if (attributeName == "http-equiv" && Browser.IsMSIE)
		value = node.httpEquiv;

	if (attributeName == "style" && !Browser.isOpera)
		value = node.style.cssText;*/

	return (value && value != "") ? value : defaultValue;
}

DOMUtils.SetAttribute = function(node, attributeName, value) 
{
	if (typeof(value) == "undefined" || value == null) 
	{
		value = "";
	}

	if (value != "") {
		node.setAttribute(attributeName.toLowerCase(), value);

		if (attributeName == "style")
			attributeName = "style.cssText";

		if (attributeName.substring(0, 2) == 'on')
			value = 'return true;' + value;

		if (attributeName == "class")
			attributeName = "className";

		eval('node.' + attributeName + "=value;");
	} else
		node.removeAttribute(attributeName);
}

// TODO: validate existing customer and hideelm classes and 
DOMUtils.ShowHiddenElements = function(node, className)
{
	var arElements = node.getElementsByTagName("TABLE");
	for(var i = 0; i < arElements.length; i++)
	{
		var addClass  = (DOMUtils.GetAttribute(arElements[i],"border") == "0") ? true : false;
		if(addClass)
		{
			var tmpClass = DOMUtils.GetHiddenClass(DOMUtils.GetAttribute(arElements[i],"class"),true,className);
			DOMUtils.SetAttribute(arElements[i], "class", tmpClass);
			for(var y = 0; y <arElements[i].rows.length; y++) 
			{
				for ( var x=0; x< arElements[i].rows[y].cells.length; x++) 
				{
					var intClass = DOMUtils.GetHiddenClass(DOMUtils.GetAttribute(arElements[i].rows[y].cells[x], "class"), true, className);
					DOMUtils.SetAttribute(arElements[i].rows[y].cells[x], "class", intClass);
					var intText = arElements[i].rows[y].cells[x].innerHTML;
					if(intText == "" && Browser.IsMozilla)
						arElements[i].rows[y].cells[x].innerHTML = '<br _moz_editor_bogus_node="TRUE">';
				}
			}
		}
	}
}

// addClass - true if class should be added, false if removed
DOMUtils.GetHiddenClass = function(elementClass, addClass, hiddedClassName)
{
	elementClasses = elementClass.split(' ');
	var exists = false;
	var className = '';
	for (var j=0; j<elementClasses.length; j++) 
	{
		if (j > 0)
			className += " ";
		if (elementClasses[j] != hiddedClassName)
		{
			className += elementClasses[j];
		}
	}
	if(addClass)
	{
		if(j > 0)
			className += " ";
		className += hiddedClassName;
	}
	return className;
}
