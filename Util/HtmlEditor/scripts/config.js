
var Config;

if(null == Config)
	Config = new Object();
	Config.ClientID = clientId;
// parse url variables

Config.ParseUrlVariables = function()
{
	var aParams = document.location.search.substr(1).split('&');
	this.UrlParams = Object();
	// one parameter should be in the URL at least.
	for ( var i = 0; i < aParams.length; i++ )
	{
		var params = aParams[i].split('=');
		if('clientid' == params[0].toLowerCase())
			this.ClientID = params[1];
		else
			this.UrlParams[params[0]]= params[1];
	}
}
Config.ParseUrlVariables();

Config.ParseConfigString = function()
{
	this.Properties = new Object();
	var configString = window.parent.document.getElementById(this.ClientID + "__CONFIG");
	if (!configString) 
		return;
	var pairs = configString.value.split('&');

	for ( var i = 0; i < pairs.length; i++ )
	{
		if ( pairs[i].length == 0 ) // empty (may be default) value
			continue;

		var pair = pairs[i].split('=');
		var sKey = unescape(pair[0]);
		var sVal = unescape(pair[1]);
		
		if ( sVal.toLowerCase() == "true" )	// If it is a boolean TRUE.
			this.Properties[ sKey ] = true;
			
		else if ( sVal.toLowerCase() == "false" )	// If it is a boolean FALSE.
			this.Properties[ sKey ] = false;
			
		else if ( ! isNaN( sVal ) )					// If it is a number.
			this.Properties[ sKey ] = parseInt( sVal );
			
		else										// In any other case it is a string.
			this.Properties[ sKey ] = sVal;
	}
}
Config.ParseConfigString();

Config.GetBasePath = function()
{
	return this.Properties["basepath"]
}
Config.GetDocType = function()
{
	return this.Properties["doctype"];
}
Config.GetFontSizes = function()
{
	return this.Properties["fontsize"];
}
Config.GetFontNames = function()
{
	return this.Properties["fonts"];
}
Config.GetTextFormat = function()
{
	return this.Properties["textformat"];
}

Config.GetImagePath = function()
{
	return this.Properties["imagepath"];
}
Config.GetMode = function()
{
	return this.Properties["mode"];
}
Config.GetBRonCR = function()
{
	return this.Properties["usebroncr"];
}
Config.GetEditorImagePath = function()
{
	if( this.Properties["editorimagepath"])
		return this.Properties["editorimagepath"];
	else
		return null;
}
Config.GetEditorCSS = function()
{
	if( this.Properties["editorcss"])
		return this.Properties["editorcss"];
	else
		return null;
}

// editorimagepath
// this is a DEBUG version. Release version should load it from config field
Config.Toolbars = [['FontName','FontSize'],['Bold','Italic','Underline','StrikeThrough'],['OrderedList','UnorderedList','-','Outdent','Indent'],
				['JustifyLeft','JustifyCenter','JustifyRight','JustifyFull'],['Link','Unlink','Image'],['ForeColor','BackColor'],['Cut','Copy','Paste','PastePlainText','PasteFromWord'],
				['Table'],['Mode']];