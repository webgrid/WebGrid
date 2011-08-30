
var ToolBarItemCollection = new Object();
ToolBarItemCollection.Items = new Object();

ToolBarItemCollection.GetItem = function(name)
{
	// check existing
	var item = ToolBarItemCollection.Items[name];
	if (item) 
	{
		//alert("create cn:" + name);
		return item;
	}

	switch (name)
	{
		case 'Bold': 
			item = new Button('Bold', 'Bold', null, null, true, true); 
			//commandName, text, tooltip, style , contextSensitive, enabledDesignMode
			break;
		case 'Italic': 
			item = new Button('Italic', null, null, null, true , true ); 
			break;
		case 'Underline': 
			item = new Button('Underline', null, null, false, true, true ); 
			break;
		case 'OrderedList':
			item = new Button('InsertOrderedList',null,null, false, true, true); 
			break;
		case 'UnorderedList':
			item = new Button('InsertUnorderedList',null,null, false, true, true); 
			break;
		case 'Outdent':
			item = new Button('Outdent',null,null, false, true, true); 
			break;
		case 'Indent':
			item = new Button('Indent',null,null, false, true, true); 
			break;
		case 'JustifyLeft': 
			item = new Button('JustifyLeft',null, null, false, true, true); 
			break;
		case 'JustifyCenter': 
			item = new Button('JustifyCenter', null, null, false, true , true); 
			break;
		case 'JustifyRight': 
			item = new Button('JustifyRight', null, null, false, true , true); 
			break;
		case 'JustifyFull': 
			item = new Button('JustifyFull', null, null, false, true, true ); 
			break;
		case 'StrikeThrough': 
			item = new Button('StrikeThrough',null, null, false, true , true); 
			break;
		case 'Cut': 
			item = new Button(name,null, null, false, true , true); 
			break;
		case 'Copy': 
			item = new Button(name,null, null, false, true , true); 
			break;
		case 'Paste': 
			item = new Button(name,null, null, false, true , true); 
			break;
		case 'PastePlainText':
			item = new Button(name,"Paste as Plain Text", null, false, true , true); 
			break;
		case 'PasteFromWord':
			item = new Button(name,"Paste from Word", null, false, true , true); 
			break;
		case 'FontSize': 
			item = new FontSize(name,null, true); 
			break;
		case 'FontName': 
			item = new FontName(name,null, true); 
			break;
		case 'Link':
			item = new Button(name,"Create / Edit Link", null, false, true , true); 
			break;
		case 'Unlink':
			item = new Button(name,"Delete Link", null, false, true , true); 
			break;
		case 'ForeColor':
			item = new Button(name,"Font Color", null, false, true , true); 
			break;
		case 'BackColor':
			item = new Button(name,"Background Color", null, false, true , true); 
			break;
		case 'Image':
			item = new Button(name,"Insert / Edit Image", null, false, true , true); 
			break;
		case 'Table':
			item = new Button(name,"Insert Table", null, false, true , true); 
			break;
		case 'Mode':
			item = new SwitchButton(name,"Design / Source Mode", null, false, true , true); 
			break;
		case "-":
			item = new Separator();
			break;
		default:
			
			alert("Unknown toolbar item - " + name);
			return null;
	}
	ToolBarItemCollection.Items[name] = item;
	return item;
}