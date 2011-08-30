
/*
	Implements editor mode command functionality
*/
var ModeCommand = function()
{
}

ModeCommand.prototype.ExecCommand = function()
{
	Engine.SwitchMode();
}

ModeCommand.prototype.QueryCommandState = function()
{
	return (Engine.Mode == DESIGN_MODE) ? CMD_ENABLED : CMD_APPLIED;
}
