/*
*  Cut, copy and paste commands
*/

//**********************************************************
// Cut command

Cut = function()
{
	this.Name = "Cut";
}
Cut.prototype.ExecCommand = function()
{
	try
	{
		Engine.ExecCommand(this.Name);
	}
	catch(e)
	{
		alert("Your browser security settings don't permit the editor to automatically execute cutting operations. Please use the keyboard for that (Ctrl+X).");
	}
}

Cut.prototype.QueryCommandState = function()
{
	return Engine.GetCommandState("Cut");
}

//**********************************************************
// Copy command

Copy = function()
{
	this.Name = "Copy";
}
Copy.prototype.ExecCommand = function()
{
	try
	{
		Engine.ExecCommand(this.Name);
	}
	catch(e)
	{
		alert("Your browser security settings don't permit the editor to automatically execute copying operations. Please use the keyboard for that (Ctrl+C).");
	}
}

Copy.prototype.QueryCommandState = function()
{
	return Engine.GetCommandState("Copy");
}

//**********************************************************
// Paste command

Paste = function()
{
	this.Name = "Paste";
}
Paste.prototype.ExecCommand = function()
{
	try
	{
		Engine.ExecCommand(this.Name);
	}
	catch(e)
	{
		alert("Your browser security settings don't permit the editor to automatically execute pasting operations. Please use the keyboard for that (Ctrl+V).");
	}
}

Paste.prototype.QueryCommandState = function()
{
	return Engine.GetCommandState("Paste");
}

//**********************************************************
// Paste from Word command

PasteFromWord = function()
{
	this.Name = "PasteFromWord";
}

PasteFromWord.prototype.ExecCommand = function()
{
	
	Engine.PasteFromWord();
}

PasteFromWord.prototype.QueryCommandState = function()
{
	return Engine.GetCommandState("Paste");
}

//**********************************************************
// Paste as Plain Text command

PastePlainText = function()
{
	this.Name = "PastePlainText";
}

PastePlainText.prototype.ExecCommand = function()
{
	Engine.PastePlainText();
}

PastePlainText.prototype.QueryCommandState = function()
{
	return Engine.GetCommandState("Paste");
}
