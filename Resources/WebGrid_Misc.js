//Used to confirm UI dialogs.
var wg_isconfirmed = false;
//Used to track last triggered grid for Ajax
var wg_gridupdate = null;


function setActiveGrid(gridId) 
{
    wg_gridupdate = 'wgContent_' + gridId;
}
function fireEvent(obj,evt){

var fireOnThis = obj;
if( document.createEvent ) {
var evObj = document.createEvent('MouseEvents');
evObj.initEvent( evt, true, false );
fireOnThis.dispatchEvent(evObj);
} else if( document.createEventObject ) {
fireOnThis.fireEvent('on'+evt);
}
}

function endsWith(str, s){
	var reg = new RegExp(s + "$");
	return reg.test(str);
}

function startsWith(str, s){
	var reg = new RegExp("^" + s);
	return reg.test(str);
}

function SubmitGridOnEnter(e, argument) { 
    var activeElement = null;
    
	if (!e) return;
	if (e.target)
		activeElement = e.target;
	else if(e.srcElement) activeElement = e.srcElement;
	
    if( activeElement.type == 'textarea' || endsWith(activeElement.name, '_gridsearch')) 
    {
        return;
    }

    if(window.event) // IE
        key = event.keyCode;
    else if(e.which) // Netscape/Firefox/Opera
        key = e.which;
    
    if( key ==13 ) 
    {
        var item  = document.getElementById(argument);
        if( item != null ) 
        {
          if( e.which)  // Netscape/Firefox/Opera
          { 
             e.preventDefault();
             item.focus();
             item.click();
             fireEvent(item, 'click');
          }
          else // IE 
          {
             event.returnValue=false;
             event.cancel=true;
             item.focus();
             item.click();
          }
          return false;
        }
        else
            return false;

     }
     return true;
}

function WGCheckboxhighlight(checkbox, rowcolor) {
	if (document.getElementById) 
	{
      var tr = eval("document.getElementById('wgrh_" + checkbox.id + "')");
    } 
      else 
    {
      return;
    }
   if (tr) 
   {
	  if (checkbox.checked) 
	  {
		tr.style.accessKey = tr.className;
		tr.style.backgroundColor = rowcolor;
		tr.className = tr.style.backgroundColor;
      } 
		else 
      {
        if( tr.style.accessKey != null ) 
        {
            tr.style.backgroundColor = tr.style.accessKey;
            tr.className = tr.style.accessKey;
	    	tr.style.accessKey = null;
	    }
      }
   }
}


		
function WebGrid_Anthem_PostCallBack() {
    var loading = document.getElementById("loading");
    document.body.removeChild(loading);
}

   
      function html_entity_decode(str)
       {
      var tarea=document.createElement('textarea');
      tarea.innerHTML = str; return tarea.value;
       tarea.parentNode.removeChild(tarea);
      }

function UpdateAndClose(element) {
                    document.getElementById(element+"_headerfrom").value = document.getElementById("wgdatefrom").value;
                    document.getElementById(element+"_headerto").value = document.getElementById("wgdateto").value;
                    document.forms[0].submit();
}

function fromdatetimeChanged(calendar) {
                       if (calendar.dateClicked) {
                          var y = calendar.date.getFullYear();
                           var m = calendar.date.getMonth();     // integer, 0..11
                           m = m +1;
                           if (m < 10 ) m = '0' + m;
                           var d = calendar.date.getDate();      // integer, 1..31
                           if (d < 10 ) d = '0' + d;
                           document.getElementById("wgdatefrom").value = y + "/" + m + "/" + d;
                       }
}
function todatetimeChanged(calendar) {
                       if (calendar.dateClicked) {
                           var y = calendar.date.getFullYear();
                           var m = calendar.date.getMonth();     // integer, 0..11
                           m = m +1;
                           if (m < 10 ) m = '0' + m;
                           
                           var d = calendar.date.getDate();      // integer, 1..31
                           if (d < 10 ) d = '0' + d;
                           document.getElementById("wgdateto").value =  y + "/" + m + "/" + d;
                       }
}
function submitenter(myfield,e)
{
    var keycode;
    if (window.event) 
        keycode = window.event.keyCode;
    else if (e) 
        keycode = e.which;
    else 
        return true;

    if (keycode == 13)
    {
        myfield.form.submit();
        return false;
    }
    else
        return true;
}


function wgrowhighlight(Wgtr, status, rowcolor) 
{
  if (document.getElementById) 
   {
      var tr = eval("document.getElementById(Wgtr.id)");
   } 
      else 
   {
      return;
   }
  if ( tr == null )
     return;
  if (status == 'over') 
   {
	     tr.className = tr.style.backgroundColor;
	     tr.style.backgroundColor = rowcolor;
   } 
   else 
   {
         tr.style.backgroundColor = tr.className;
         tr.className = null;
   }
}


function WGdisableDateTimeSearch(column) {

   if (document.getElementById == null ) 
      return;
     
   var headerfrom = document.getElementById(column+"_headerfrom");
   var headerto = document.getElementById(column+"_headerto");
   headerfrom.value = '';
   headerto.value = '';
   document.forms[0].submit();
     
}

function wgcleanclientobjects() {
    try {
        $(".wgtoolelement").removeClass("wgtooltipselected");
        $(".wgtoolelement").each(function() {
            if ($(this).qtip != null) {
                var api = $(this).qtip("api"); // Access the API via the tooltip
                if (api != null)
                    api.destroy();
            }
        });
    }
    catch (err) { }
}

function wgconfirm(message, confirmobj, title) {
    if (wg_isconfirmed == true) {
     
        wg_isconfirmed = false;
          return true;
    }
    wg_isconfirmed = false;
    try {
        $(" <div id='dialog' title='" + title + "' class='wguidialog'>" + message + "</div>").dialog({
            bgiframe: true,
            resizable: false,
            height: 160,
            width: 500,
            modal: true,
            overlay: {
                
                opacity: 0.5
            },
            buttons: {
                'Yes': function() {
                $(this).dialog('close');
          
                    wg_isconfirmed = true;
                    confirmobj.focus();
                    if (document.dispatchEvent) { // W3C
                        var oEvent = document.createEvent("MouseEvents");
                        oEvent.initMouseEvent("click", true, true, window, 1, 1, 1, 1, 1, false, false, false, false, 0, confirmobj);
                        confirmobj.dispatchEvent(oEvent);
                    }
                    else if (document.fireEvent) { // IE
                    confirmobj.fireEvent("onclick");
                    }    


                },
                Cancel: function() {
                    $(this).dialog('close');
                }
            }
        });
    }
    catch (err) {
        return confirm(message);
    }
    return wg_isconfirmed;
}

// -----------------------------------------------------------------------
// eros@recoding.it
// jqprint 0.3
//
// - 19/06/2009 - some new implementations, added Opera support
// - 11/05/2009 - first sketch
//
// Printing plug-in for jQuery, evolution of jPrintArea: http://plugins.jquery.com/project/jPrintArea
// requires jQuery 1.3.x
//------------------------------------------------------------------------

(function($) {
    var opt;

    $.fn.jqprint = function(options) {
        opt = $.extend({}, $.fn.jqprint.defaults, options);

        var $element = (this instanceof jQuery) ? this : $(this);

        if (opt.operaSupport && $.browser.opera) {
            var tab = window.open("", "jqPrint-preview");
            tab.document.open();

            var doc = tab.document;
        }
        else {
            var $iframe = $("<iframe  />");

            if (!opt.debug) { $iframe.css({ position: "absolute", width: "0px", height: "0px", left: "-600px", top: "-600px" }); }

            $iframe.appendTo("body");
            var doc = $iframe[0].contentWindow.document;
        }

        if (opt.importCSS) {
            if ($("link[media=print]").length > 0) {
                $("link[media=print]").each(function() {
                    doc.write("<link type='text/css' rel='stylesheet' href='" + $(this).attr("href") + "' media='print' />");
                });
            }
            else {
                $("link").each(function() {
                    doc.write("<link type='text/css' rel='stylesheet' href='" + $(this).attr("href") + "' />");
                });
            }
        }

        if (opt.printContainer) { doc.write($element.outer()); }
        else { $element.each(function() { doc.write($(this).html()); }); }

        doc.close();

        (opt.operaSupport && $.browser.opera ? tab : $iframe[0].contentWindow).focus();
        setTimeout(function() { (opt.operaSupport && $.browser.opera ? tab : $iframe[0].contentWindow).print(); if (tab) { tab.close(); } }, 1000);
    }

    $.fn.jqprint.defaults = {
        debug: false,
        importCSS: true,
        printContainer: true,
        operaSupport: true
    };

    // Thanks to 9__, found at http://users.livejournal.com/9__/380664.html
    jQuery.fn.outer = function() {
        return $($('<div></div>').html(this.clone())).html();
    }
})(jQuery);
