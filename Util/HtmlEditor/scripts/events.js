/*
* advanced events management
*/

Events = function(owner)
{
	this.Owner = owner;
	this.Events = new Object();
}

Events.prototype.AddHandler = function(eventName, handler)
{
	if (null == this.Events[eventName]) 
	{
		this.Events[eventName] = new Array();
	}
	this.Events[eventName][this.Events[eventName].length] = handler;
}

Events.prototype.Fire = function(eventName, params)
{
	var handlers = this.Events[eventName];
	if (null != handlers)
		for (var i = 0; i < handlers.length; i++)
		{
			handlers[i](this.Owner, params);
		}
}
