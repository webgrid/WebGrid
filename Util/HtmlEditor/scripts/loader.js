/*
*	This script is used to download required files. 
*	version: 1.0
*/

function Window_OnLoad()
{
	// Well-known NN bug with a frame rendering.
	if(Browser.IsNetscape)
			document.getElementById('richEditor').style.paddingRight = '2px';
	//LoadStyles();
	Engine.Run();
}
window.onload = Window_OnLoad;

function Window_OnResize()
{
	// Well-known Geko based bugs.
	if(Browser.IsMozilla )
	{
		var frame = document.getElementById('editor');
		frame.height = 0 ;
		var cl = (Engine.Mode == DESIGN_MODE) ? 'richEditor' : 'eSource';
		var cell = document.getElementById(cl) ;
		frame.height = cell.offsetHeight - 2;
		//alert("Window_OnResize");
	}
}
window.onresize = Window_OnResize;

