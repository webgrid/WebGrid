/*
* Collection of the existing  commands 
*/

var CommandCollection = new Object();
CommandCollection.List = new Object();

CommandCollection.Add = function(commandName, commandObject)
{
	this.List[commandName] = commandObject;
}

CommandCollection.GetCommand = function(commandName)
{
	switch(commandName)
	{
		case "Cut":
			o = new Cut();
			break;
		case "Copy":
			o = new Cut();
			break;
		case "Paste":
			o = new Paste();
			break;
		case "PastePlainText":
			o = new PastePlainText();
			break;
		case "PasteFromWord":
			o = new PasteFromWord();
			break;
		case "Link":
			//(name, parent, pageName, width, height, tagValue, isResizeable)
			o = new DialogCommand("CreateLink","link.htm",330,210,null,true);
			break;
		case "ForeColor":
			o = new DialogCommand("ForeColor","colorpicker.htm",300,200,null,true);
			break;
		case "BackColor":
			if(Browser.IsMozilla)
				o = new DialogCommand("HiliteColor","colorpicker.htm",220,200,null,true);
			else
				o = new DialogCommand("BackColor","colorpicker.htm",220,200,null,true);
			break;
		case "Image":
			o = new DialogCommand("InsertImage","image.htm",280,250,Config.GetImagePath(),true);
			break;
		case "Table":
			o = new DialogCommand("Table","table.htm",260,200,null,true);
			break;
		case "Mode":
			o = new ModeCommand();
			break;
		default:
			// TODO: validate new command and return null if command isn't valid
			o = new Command(commandName);
	}
	CommandCollection.Add(o);
	return o;
}
