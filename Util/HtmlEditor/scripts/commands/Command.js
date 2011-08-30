/*
*	Base Command class. Represents a simple named command
*	see MSDHTM and Midas reference  
*/

var Command = function(commandName, commandParams)
{
	this.Name = commandName;
	this.Params = commandParams;
}

Command.prototype.ExecCommand = function()
{
	Engine.ExecCommand(this.Name, this.Params);
}

Command.prototype.QueryCommandState = function()
{
	return Engine.GetCommandState(this.Name);
}
