/*
* Link dialog functionality
*/

var Engine;
var EditorWindow;
var linkNode;

window.onload = Load;

function Load()
{
	if(!window.Editor)
	{
		window.Editor = window.opener.Editor; //.Engine;
	}
	Engine = window.Editor.Engine;
	linkNode = Engine.Utils.MoveToAncestorNode('a');
	if(linkNode)
	{
		BindFormInfo();
	}
}

function Ok()
{
	//alert("eg: " + );
	if(!linkNode)
	{
		var links = Engine.CreateLink();
		for(var i=0; i<links.length; i++) 
		{
			var link = links[i];
			SetLinkAttributes(link);
		}
	}else
	{
		SetLinkAttributes(linkNode);
	}
	window.opener.focus();
	window.close();
}

function Cancel()
{
	window.close() ;
}

function BindFormInfo()
{
	SetElementValue("href", Engine.Utils.GetAttribute(linkNode,"href"));
	SetElementValue("targetList", Engine.Utils.GetAttribute(linkNode,"target"));
	SetElementValue("title", Engine.Utils.GetAttribute(linkNode,"title"));
	SetElementValue("class", Engine.Utils.GetAttribute(linkNode,"class"));
}

// TODO: move to the base dialog helper class
function SetElementValue(elementName, value) 
{
	//alert(document.forms[0].elements['href']);
	document.forms[0].elements[elementName].value = value;
}
function GetElementValue(elementName) 
{
	//alert(document.forms[0].elements['href']);
	return document.forms[0].elements[elementName].value;
}

function SetLinkAttributes(link) {
	//var formObj = document.forms[0];
	//var href = formObj.href.value;
	//var target = getSelectValue(formObj, 'targetlist');

	// Make anchors absolute
	//if (href.charAt(0) == '#' && tinyMCE.getParam('convert_urls'))
	//	href = tinyMCE.settings['document_base_url'] + href;

	//setAttrib(elm, 'href', convertURL(href, elm));
	Engine.Utils.SetAttribute(link,"href",GetElementValue("href"));
	Engine.Utils.SetAttribute(link,"target",GetElementValue("targetList"));
	Engine.Utils.SetAttribute(link,"title",GetElementValue("title"));
	Engine.Utils.SetAttribute(link,"class",GetElementValue("class"));
}
