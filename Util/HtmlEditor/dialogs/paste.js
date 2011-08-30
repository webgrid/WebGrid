/*
* Paste functionality
*/

var Engine;
var mode = 0;
window.onload = Load;
var frame, text;
function Load()
{
	if(!window.Editor || !window.Tag)
	{
		window.Editor = window.opener.Editor; //.Engine;
		window.Tag = window.opener.Tag;
	}
	Engine = window.Editor.Engine;
	
	frame = document.getElementById("word");
	text = document.getElementById("text");
	if("pasteplaintext" != window.Tag.CommandName.toLowerCase())
	{
		mode = 1; // paste from word
		frame.style.display = "";
		text.style.display = "none";
		if(frame.contentDocument)
			frame.contentDocument.designMode = "on";
		else
			frame.contentWindow.document.body.contentEditable = true;
	}else
	{
		frame.style.display = "none";
		text.style.display = "";
	}
}

function Ok()
{
	var text = '';
	if(0 == mode)
	{
		// TODO: encode?
		text = document.getElementById("text").value;
		text = text.replace(/\n/g, "<br>");
	}else
	{
		if(frame.contentDocument)
			text = frame.contentDocument.body.innerHTML;
		else
			text = frame.contentWindow.document.body.innerHTML;
		var cleanUp = new CleanUp();
		text = cleanUp.CleanWordText2(text);
	}
	if('' != text)
	{
		Engine.SetFocus();
		Engine.InsertHtml(text,true);
	}
	window.close();
}

function Cancel()
{
	window.close();
}

