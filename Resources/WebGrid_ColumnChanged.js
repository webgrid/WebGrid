var savecolumncolor;

//-------------------------------------------------------------------
// isChanged(input_object)
//   Returns true if input object's value has changed since it was
//   created.
//-------------------------------------------------------------------
function hasChanged(obj) 
{
    if(  getInputValue(obj) != getInputDefaultValue(obj) )
        return true;
    return false;
}

//-------------------------------------------------------------------
// isChanged(input_object)
//   Returns true if input object's value has changed since it was
//   created.
//-------------------------------------------------------------------


function isChanged(obj, colour)
{

if(  getInputValue(obj) != getInputDefaultValue(obj) ) {
		if( savecolumncolor == null && typeof(obj.style.backgroundColor) !="undefined" )
			savecolumncolor= obj.style.backgroundColor;
		obj.style.backgroundColor=colour
}
else if( typeof(savecolumncolor) != "undefined") {
	obj.style.backgroundColor = savecolumncolor;
}
}

//-------------------------------------------------------------------
// isArray(obj)
// Returns true if the object is an array, else false
//-------------------------------------------------------------------

function isArray(obj){return(typeof(obj.length)=="undefined")?false:true;}

//-------------------------------------------------------------------
// getInputValue(input_object[,delimiter])
//   Get the value of any form input field
//   Multiple-select fields are returned as comma-separated values, or
//   delmited by the optional second argument
//   (Doesn't support input types: button,file,reset,submit)
//-------------------------------------------------------------------

function getInputValue(obj,delimiter) 
{
	var use_default=(arguments.length>2)?arguments[2]:false;
	if (isArray(obj) && (typeof(obj.type)=="undefined")) 
	{
		var values=new Array();
		for(var i=0;i<obj.length;i++)
		{
			var v=getSingleInputValue(obj[i],use_default,delimiter);
			if(v!=null){values[values.length]=v;
		}
	}
		return commifyArray(values,delimiter);
	}
	return getSingleInputValue(obj,use_default,delimiter);
}

//-------------------------------------------------------------------
// commifyArray(array[,delimiter])
//   Take an array of values and turn it into a comma-separated string
//   Pass an optional second argument to specify a delimiter other than
//   comma.
//-------------------------------------------------------------------

function commifyArray(obj,delimiter){
	if (typeof(delimiter)=="undefined" || delimiter==null) {
		delimiter = ",";
		}
	var s="";
	if(obj==null||obj.length<=0){return s;}
	for(var i=0;i<obj.length;i++){
		s=s+((s=="")?"":delimiter)+obj[i].toString();
		}
	return s;
	}


//-------------------------------------------------------------------
// getSingleInputValue(input_object,use_default,delimiter)
//   Utility function used by others
//-------------------------------------------------------------------

function getSingleInputValue(obj,use_default,delimiter) {
	switch(obj.type){
		case 'radio': case 'checkbox': return(((use_default)?obj.defaultChecked:obj.checked)?obj.value:null);
		case 'text': case 'hidden': case 'textarea': case 'file' : return(use_default)?obj.defaultValue:obj.value;
		case 'password': return((use_default)?null:obj.value);
		case 'select-one':
			if (obj.options==null) { return null; }
			if(use_default){
				var o=obj.options;
				for(var i=0;i<o.length;i++){if(o[i].defaultSelected){return o[i].value;}}
				return o[0].value;
				}
			if (obj.selectedIndex<0){return null;}
			return(obj.options.length>0)?obj.options[obj.selectedIndex].value:null;
		case 'select-multiple': 
			if (obj.options==null) { return null; }
			var values=new Array();
			for(var i=0;i<obj.options.length;i++) {
				if((use_default&&obj.options[i].defaultSelected)||(!use_default&&obj.options[i].selected)) {
					values[values.length]=obj.options[i].value;
					}
				}
			return (values.length==0)?null:commifyArray(values,delimiter);
		}
	alert("FATAL ERROR: Field type "+obj.type+" is not supported for this function");
	return null;
	}

//-------------------------------------------------------------------
// getInputDefaultValue(input_object[,delimiter])
//   Get the default value of any form input field when it was created
//   Multiple-select fields are returned as comma-separated values, or
//   delmited by the optional second argument
//   (Doesn't support input types: button,file,password,reset,submit)

function getInputDefaultValue(obj,delimiter){return getInputValue(obj,delimiter,true);}


