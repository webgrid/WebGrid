/*
*	Represents a dialog 
*/

Dialog = function(name, parent, pageName, width, height, tagValue, isResizeable)
{
	this.Tag;
	this.parentWindow = parent;
	
	var top  = (parent.screen.height - height)/2;
	var left = (parent.screen.width  - width)/2;

	var option  = "location=no,menubar=no,status=yes,toolbar=no,dependent=yes,dialog=yes,minimizable=no,modal=yes,alwaysRaised=yes" +
		",resizable="  + (isResizeable ? "yes": "no") +
		",width="  + width + ",height=" + height + ",top="  + top + ",left=" + left ;
	if(!this.parentWindow)
		this.parentWindow = window;
	if(null == this.Tag)
		this.Tag = new Object();
	
	this.Tag.CommandName = name;
	this.Tag.TagValue = tagValue;
	
	this.parentWindow.Editor = window;

	var dialog = this.parentWindow.open("htmleditor.aspx?res=dialog&id=" + pageName, '', option, true);
	dialog.Editor = window;
	dialog.Tag = this.Tag;
	
	if(!dialog)
		alert("Close PopUp blockers.");
	
}



/*Dialog.OpenDialog = function(name, pageName, width, height, tagValue, isResizeable)
{
	// Setup the dialog info.
	thisvar oDialogInfo = new Object() ;
	oDialogInfo.Title = dialogTitle ;
	oDialogInfo.Page = dialogPage ;
	oDialogInfo.Editor = window ;
	oDialogInfo.CustomValue = customValue ;		// Optional
	
	var sUrl = FCKConfig.BasePath + 'fckdialog.html' ;
	this.Show( oDialogInfo, dialogName, sUrl, width, height, parentWindow, resizable ) ;
}
*/

