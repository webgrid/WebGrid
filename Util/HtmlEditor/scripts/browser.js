/*
* Defines the heBrowser class that return browser informations.
* version: 1.0
*/
var Browser;

if(Browser == null )
{
	Browser = new Object();
	var userAgent = navigator.userAgent.toLowerCase();
	if(userAgent.indexOf("msie") != -1)
		Browser.IsMSIE = true;
	else
		Browser.IsMSIE = false;
	Browser.IsMozilla = !Browser.IsMSIE;
	if(userAgent.indexOf("netscape") != -1)
		Browser.IsNetscape = true;
	else
		Browser.IsNetscape = false;
	if(userAgent.indexOf("safari") != -1) 
		Browser.IsSafari = true;
	else
		Browser.IsSafari = false;
}

Browser.CheckOpera = function()
{
	var userAgent = navigator.userAgent.toLowerCase();
	if( -1 != userAgent.indexOf("opera"))
		return true;
	return false;
}
// returns browser short name
Browser.GetBrowserName = function()
{
	if(Browser.IsMSIE)
		return "msie";
	if(Browser.IsMozilla)
		return "mozilla";
	if(Browser.IsNetscape)
		return "netscape";
	if(Browser.IsSafari)
		return "safari";
}
