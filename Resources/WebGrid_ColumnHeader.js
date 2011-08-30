// WebGrid resizable column
// Rev. 7, September 26, 2007
// --------------------------

var loadCookies = false;

function webGridMouseOver()
{
    document.body.style.cursor = "w-resize"; 
}
function webGridMouseOut()
{
    document.body.style.cursor = ""; 
}

function webGridMouseDown(event, gridId, columnId)
{
    if(!event)
        event = window.event;
    el = document.getElementById(gridId + 's' + columnId); 
    beginWebGridResize(el, event, gridId, columnId);
}

function beginWebGridResize(elementToDrag, e, gridId, columnId) 
{
    var cell = document.getElementById(gridId + columnId + "r0");
    var grid = document.getElementById("wgContainer_" + gridId);
    
    if(typeof(cell)=="undefined" || cell == null)
    {
        alert("Grid script error object: "+gridId + columnId + "r0 is not found.");
        return;
    }
    if(typeof(grid)=="undefined" || grid == null )
    {
        alert("Grid script error object: "+"wgContainer_" + gridId+" is not found.");
        return;
    }

    
    var minWidth = 10; 
    var startX = cell.offsetLeft + grid.offsetLeft- 3;
    var X = e.clientX;
    var Y = e.clientY;
    var width = 0;
    var columnsArray = eval(gridId + "columns");
    minWidth = GetColumnSettings(columnsArray, columnId)[1];
    endSplitter.Show(cell.offsetLeft + elementToDrag.offsetLeft + grid.offsetLeft- 3,Y);
    startSplitter.Show(cell.offsetLeft + grid.offsetLeft- 3 , Y);

    if (document.addEventListener) 
    { 
        document.addEventListener("mousemove", moveHandler, true);
        document.addEventListener("mouseup", upHandler, true);
    }
    else if (document.attachEvent) 
    {
        document.attachEvent("onmousemove", moveHandler);
        document.attachEvent("onmouseup", upHandler);
    }
    else 
    { 
        var oldmovehandler = document.onmousemove;
        var olduphandler = document.onmouseup;
        document.onmousemove = moveHandler;
        document.onmouseup = upHandler;
    }

    if (e.stopPropagation) 
        e.stopPropagation(); 
    else 
        e.cancelBubble = true; 

    if (e.preventDefault) 
        e.preventDefault(); 
    else 
        e.returnValue = false; 

    function moveHandler(e) 
    {
        if (!e) 
            e = window.event;  

        if(minWidth < e.clientX)
        {
            X = e.clientX;
            endSplitter.Splitter.MoveTo(X);
            width = e.clientX - startX;
        }
        if (e.stopPropagation) 
            e.stopPropagation(); 
        else 
            e.cancelBubble = true; 
    }

    
    function upHandler(e) 
    {
        if (!e) 
            e = window.event;  

        if (document.removeEventListener) 
        {    
            document.removeEventListener("mouseup", upHandler, true);
            document.removeEventListener("mousemove", moveHandler, true);
        }
        else if (document.detachEvent) 
        {
            document.detachEvent("onmouseup", upHandler);
            document.detachEvent("onmousemove", moveHandler);
        }
        else 
        { 
            document.onmouseup = olduphandler;
            document.onmousemove = oldmovehandler;
        }
        
        if (e.stopPropagation) 
            e.stopPropagation(); 
        else 
            e.cancelBubble = true; 
        endSplitter.Hide();
        startSplitter.Hide();
        if(minWidth < e.clientX)
        {
            X = e.clientX;
            endSplitter.Splitter.MoveTo(X);
            width = e.clientX  - startX;
        }
        if(0 != width)
            wgResizeColumns(gridId, columnId, width);
    }
}

function wgResizeColumns(gridId, columnId, width)
{
    var columnsArray = eval(gridId + "columns");
    var sums = GetWidthsSums(columnsArray, columnId);
    var column = document.getElementById(gridId + columnId + "r0");
    var useMinSizes = false;
    if(width > 0 && (width + sums.MinSum) > (sums.CurrentSum + parseInt(column.style.width)))
    {
        useMinSizes = true;
        width = sums.CurrentSum - sums.MinSum + parseInt(column.style.width);
    }
    if( 30 > width )
    { return; }

    wgSetColumnWidth(gridId, columnId, width);

    var deltaWidth = 0; 
    for(var i = 0; i < columnsArray.length - 1; i++)
    {
        if(columnId == columnsArray[i][3])
        {
            
            deltaWidth = columnsArray[i][6] - width;
            columnsArray[i][6] = width;
            if(useMinSizes)
            {
                for(var j = i + 1; j < columnsArray.length - 1; j++)
                {
                    if(columnsArray[j][2])
                    {
                            columnsArray[j][6] = columnsArray[j][1];
                            wgSetColumnWidth(gridId, columnsArray[j][3], columnsArray[j][1])
                    }
                }
             }
             else
             {
                wgCalculateSizes(columnsArray, i + 1, deltaWidth, sums.CurrentSum);
                for(var j = i + 1; j < columnsArray.length - 1; j++)
                {
                    if(columnsArray[j][2])
                    {
                            wgSetColumnWidth(gridId, columnsArray[j][3], columnsArray[j][6])
                    }
                }
                var crr = wgFixRoundingError(columnsArray, i + 1, sums.CurrentSum + deltaWidth);
                columnsArray[i][6] += crr;
                wgSetColumnWidth(gridId, columnsArray[i][3], columnsArray[i][6]);
             }
            break;
        }
    }
}

function wgCalculateSizes(columnsArray, startIndex, deltaWidth, currentWidth, excludeColumn)
{
    var constWidths = 0;
    var remains = 0;
    var isMinSize = false;
    // remove min size columns
    if(0 > deltaWidth)
    {
        for(var i = startIndex; i < columnsArray.length - 1; i++)
        {
            if(columnsArray[i][2] && columnsArray[i][1] == columnsArray[i][6] && columnsArray[i][3] != excludeColumn)
            {
                currentWidth -= columnsArray[i][1];
            }
        }
    }
    for(var i = startIndex; i < columnsArray.length - 1; i++)
    {
        if(columnsArray[i][2] && columnsArray[i][3] != excludeColumn)
        {
            var tmpWidth = deltaWidth*(columnsArray[i][6]/currentWidth);
            var colDelta = Math.floor(tmpWidth);
            if(0 > deltaWidth && columnsArray[i][1] == columnsArray[i][6])
                continue;
            if(columnsArray[i][1] > (columnsArray[i][6] + colDelta))
            {
                remains += columnsArray[i][6] - columnsArray[i][1] + colDelta; 
                columnsArray[i][6] = columnsArray[i][1];
                isMinSize = true;
            }
            else
            {
                columnsArray[i][6] += colDelta; 
            }
        }
    }
    if(isMinSize)
    {
        wgCalculateSizes(columnsArray, startIndex, remains, currentWidth);
    }
}

function wgFixRoundingError(columnsArray, startIndex, width)
{
        var newTotalWidth = 0;
        var correction = 0;
        for(var i = startIndex; i < columnsArray.length - 1; i++)
        {
            if(columnsArray[i][2])
            {
                newTotalWidth += columnsArray[i][6];
            }
        }
        if(width != newTotalWidth)
        {
            correction = width - newTotalWidth;
        }
        return correction;
}

function wgShowColumn(gridId, columnId, show, resize)
{
    var ff = document.getElementById && !document.all;
    var columnsArray = eval(gridId + "columns");
    var columnPadding = 0;
    
    var columnIndex = 0; 
    for(var col = 0; col < columnsArray.length - 1; col++)
    {
        if(columnId == columnsArray[col][3])
        {
            columnsArray[col][2] = show;
            columnIndex = col;
            break;
        }
    } 
    
    var rows = columnsArray[columnsArray.length - 1][0];
    for(var i = 0; i < rows; i++)
    {
        var row = document.getElementById(gridId + "r" + i);
        if(show)
        {
            if(ff)
            {
                var sourceCell = document.getElementById(gridId + columnId + "r" + i);
                if(sourceCell)
                {
                    
                    var cellIndex = sourceCell.cellIndex;
                    sourceCell.style.display = "";
                    var destCell = row.insertCell(cellIndex+1);
                      for(var a = 0; a < sourceCell.attributes.length; a++)
                      {
                            var att = document.createAttribute(sourceCell.attributes.item(a).nodeName);
                            att.nodeValue = sourceCell.attributes.item(a).nodeValue;
                            if('' != att.nodeValue)
                            {
                                destCell.attributes.setNamedItem(att);
                            }
                      }
                    destCell.innerHTML = sourceCell.innerHTML;
                    row.deleteCell(sourceCell.cellIndex);
                }
                else
                {
                    for(j = columnIndex; j >=0; j--)
                    {
                        var mergedCell = document.getElementById(gridId + columnsArray[j][3] + "r" + i);
                        if(mergedCell)
                        {
                            mergedCell.colSpan += 1;
                            break;
                        }
                    }
                }
            }
            else
            {
                var cell = document.getElementById(gridId + columnId + "r" + i);
                if(cell)
                    cell.style.display = "block";
                else
                {
                    for(var j = columnIndex-1; j >=0; j--)
                    {
                        var mergedCell = document.getElementById(gridId + columnsArray[j][3] + "r" + i);
                        if(mergedCell)
                        {
                            mergedCell.colSpan += 1;
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            // hide columns
            var cell = document.getElementById(gridId + columnId + "r" + i);
            if(cell)
            {
                if(0 == i) columnPadding = cell.offsetWidth - parseInt(cell.style.width);
                if(1 == cell.colSpan)
                {
                    cell.style.display = "none";
                }
                else
                {
                    cell.colSpan -= 1;
                }
            }
            else
            {
                for(var j = columnIndex - 1; j >= 0; j--)
                {
                    var spanCell = document.getElementById(gridId + columnsArray[j][3] + "r" + i)
                    if(spanCell)
                    {
                        try {
                            if(show)
                            {
                               spanCell.colSpan += 1;
                            }else
                            {
                                spanCell.colSpan -= 1;
                            }
                        }
                        catch(e) 
                        {
                        }
                    }
                }
            } 
        }
    }
    if(resize)
    {
        var totalWidth = GetTotalWidth(columnsArray);
        var deltaWidth = 0;
        var crr = 0;
        if(show)
        {
            deltaWidth = -columnsArray[columnIndex][6];
        }
        else
        {
            deltaWidth = columnsArray[columnIndex][6];
        }
        wgCalculateSizes(columnsArray, 0, deltaWidth, totalWidth, columnId);
        var tmpW = 0;
        for(var j = 0; j < columnsArray.length - 1; j++)
        {
            if(columnsArray[j][2])
            {
                wgSetColumnWidth(gridId, columnsArray[j][3], columnsArray[j][6]);
            }
        }
        crr = wgFixRoundingError(columnsArray, 0, totalWidth + deltaWidth);
        if(show) 
        {
            var cell = document.getElementById(gridId + columnId + "r" + 0);
            columnPadding = cell.offsetWidth - parseInt(cell.style.width);
            crr -= columnPadding;
        }else
            crr += columnPadding;
        

        columnsArray[0][6] += crr;
        wgSetColumnWidth(gridId, columnsArray[0][3], columnsArray[0][6]);
    }
}

// context menu
function wgSetColumnWidth(gridId, columnId, width)
{
    var headerCell = document.getElementById(gridId + columnId + "r0");
    var headerDiv = document.getElementById(gridId + "h" + columnId);
    var headerSplitter = document.getElementById(gridId + "s" + columnId);
    var splitterWidth = 0;
    
    if(typeof(headerCell)=="undefined" || headerCell == null)
    {
        alert("Grid script error object: "+gridId + columnId + "r0 is not found.");
        return;
    }
    if(typeof(headerCell)=="undefined" || headerCell == null)
    {
        alert("Grid script error object: "+gridId +"h"+ columnId + "  is not found.");
        return;
    }
    if(typeof(headerCell)=="undefined" || headerCell == null)
    {
        alert("Grid script error object: "+gridId +"s"+ columnId + "  is not found.");
        return;
    }
    if(headerSplitter)
        splitterWidth = parseInt(headerSplitter.style.width);
    try 
    {
       headerCell.style.width = width + "px";
       headerDiv.style.width = width - splitterWidth + "px";
    }
    catch(e) 
	{ 
	  
	}
}

function wgShowContextMenu(e, gridId, ctrl)
{
    if(!e)
        e = window.event;
    if (e.stopPropagation) 
    {
            e.stopPropagation(); // Geko
            wgContextMenu.Show(e.clientX, e.clientY, gridId);
    }
        else 
    {
        e.cancelBubble = true;
        document.attachEvent("oncontextmenu", wgEventCancel);
        wgContextMenu.Show(e.screenX, e.screenY, gridId);
        document.detachEvent("oncontextmenu", wgEventCancel);
    }
    return false;
}
function wgDisableContextMenu()
{
    if (document.removeEventListener) 
    {   
        document.removeEventListener("oncontextmenu", wgEventCancel, true);
    }
    else if (document.detachEvent) 
    {
        document.detachEvent("oncontextmenu", wgEventCancel);
    }
}
function wgEventCancel(e)
{
    return false;
}

function wgOnLoad(gridId, loadCookie)
{
    if( navigator.appName == "Opera")
        return;
    var settings = GetWGSettings(gridId);
  
    if(settings && loadCookie )
    {
        var  columnsArray = eval(gridId + "columns");
        var cols = settings.split('\n');
        if(cols.length != columnsArray.length)
        {
            SetWGSettings(gridId, '', -1);
                if( wgSetUpGrid(gridId) == false )
                    wgOnLoad(gridId, false);
            return;
        }
        for(var i=0; i < cols.length - 1; i++)
        {
            var columnSetting = cols[i].split("%wg,");
            if(columnsArray[i][3] != columnSetting[3])
            {
                SetWGSettings(gridId, '', -1);
                if( wgSetUpGrid(gridId) == false )
                    wgOnLoad(gridId, false);
                return;
            }
        }
       
        for(var i=0; i < cols.length - 1; i++)
        {
            var columnSetting = cols[i].split("%wg,");
            columnsArray[i][1] = parseInt(columnSetting[1]);
            columnsArray[i][2] = ("true" == columnSetting[2]) ? true : false;
            columnsArray[i][4] = ("true" == columnSetting[4]) ? true : false;
            columnsArray[i][5] = ("true" == columnSetting[5]) ? true : false;
            columnsArray[i][6] = parseInt(columnSetting[6]);
        }
        wgSetUpGrid(gridId);
        return;
    }
    wgSetUpGrid(gridId);
}
function wgOnUnLoad(gridId)
{
     if( navigator.appName == "Opera")
        return;
   // store columns state from cookie
    var  columnsArray = eval(gridId + "columns");
    var settings = '';
    for(i = 0; i < columnsArray.length - 1; i++)
    {
        settings += (columnsArray[i].join("%wg,") + "\n"); 
    }
    settings += columnsArray[columnsArray.length - 1];
    SetWGSettings(gridId, settings, 1000);
}

function wgRegLoadEvents(onLoad, onUnload)
{
    if(document.addEventListener)
    {
        document.addEventListener("load", eval(onLoad), false);
        document.addEventListener("unload",eval(onUnload), true);
    }
    else
    {
        window.attachEvent("onload",eval(onLoad));
        window.attachEvent("onunload",eval(onUnload));
    }
}

function GetWGSettings(gridId)
{
    if(0 < document.cookie.length)
    {
        startIndex = document.cookie.indexOf(gridId + "=");
        if(-1 != startIndex)
        {
            startIndex += gridId.length + 1;
            endIndex = document.cookie.indexOf(";", startIndex);
            if(-1 == endIndex)
                endIndex = document.cookie.length;
            return unescape(document.cookie.substring(startIndex, endIndex));
        }
    }
    return null;
}

function SetWGSettings(gridId, value, expDays)
{
    var exDate = new Date();
    exDate.setDate(exDate.getDate() + expDays);
    document.cookie = gridId + "=" + escape(value) + 
    ((null == expDays) ? "" : ";expires=" + exDate.toGMTString());
}

function wgSetUpGrid(gridId)
{
    var useStyleWidth = true;
    var columnsArray = eval(gridId + "columns");
    var splitterHeight = columnsArray[columnsArray.length-1][2];
    var splitterWidth = (0 == columnsArray[columnsArray.length-1][1]) ? 2 : columnsArray[columnsArray.length-1][1];
    var wgContainer = document.getElementById("wgContainer_" + gridId);
    var wgContent = document.getElementById("wgContent_" + gridId);
    var tdHeight = 0;
    var tbl = document.getElementById(gridId);
    var headerRow = document.getElementById(gridId + "r0");
    if( tbl == null || headerRow == null )
        return;
    var rowCount = 0;
    var visibleColumnsCount = 0;
    
    var splitterHeight = 0;
    var gridStyleWidth = 0;
    var gridRenderWidth = 0
    for(var i = headerRow.rowIndex; i < tbl.rows.length; i++)
    {
        if(tbl.rows[i].id == (gridId + "r" + rowCount))
        {
           rowCount++;
           splitterHeight += tbl.rows[i].offsetHeight;
        }
        else
            break;
    }
    columnsArray[columnsArray.length - 1][0] = rowCount;

    endSplitter.height = splitterHeight;
    startSplitter.height = splitterHeight;
    
    for(var i = 0; i < columnsArray.length - 1; i++)
    {
        if(columnsArray[i][5])
        {
            var splitter = document.getElementById(gridId + "s" + columnsArray[i][3]);
            if(0 != splitter.childNodes.length)
            {
                splitterHeight = splitter.offsetHeight;
                splitterWidth = splitter.offsetWidth;
            }
            break;
        }
    }
    
    for(var i = 0; i < columnsArray.length - 1; i++)
    {
        var cell = document.getElementById(gridId + columnsArray[i][3] + "r0");
        
        if(0 == columnsArray[i][6] || isNaN(columnsArray[i][6]))
        {
            columnsArray[i][6] = parseInt(cell.style.width);
        }
        else
        {
            cell.style.width = columnsArray[i][6] + "px";
        }
        
        wgSetColumnWidth(gridId, columnsArray[i][3], columnsArray[i][6]);
        
        if (cell.offsetHeight > tdHeight) tdHeight = cell.offsetHeight;
      
    }
    
    for(var i = 0; i < columnsArray.length - 1; i++)
    {
        if(!columnsArray[i][2])
            wgShowColumn(gridId, columnsArray[i][3], columnsArray[i][2], false);
    }
}


// panel
var wgPanel = function(parentWindow)
{
    this.ff = document.getElementById && !document.all;
    this.opera = (window.opera) ? true : false;
    if(!this.ff && !this.opera)
    {
        this._window = parentWindow ? parentWindow : window;
        this._popup	= this._window.createPopup() ;
	    this.Doc	= this._popup.document ;
	    this.Div = this.Doc.body.appendChild(this.Doc.createElement('div'));
	    
    } 
    else
    {
        if (parentWindow)
        {
		    this._window = parentWindow;
		}
	    else
	    {
		    this._window = window;
		    while (window.top != this._window)
		    {
			    try
			    {
				    if ("FRAMESET" == this._window.parent.document.body.tagName)
					    break ;
			    }
			    catch(e) 
			    { 
			        break; 
			    }
			    this._window = this._window.parent;
			}
		}
		var iframe = this._iframe = this._window.document.createElement("iframe") ; 
	    iframe.frameBorder = '0';
	    iframe.scrolling = 'no' ;
	    iframe.style.position = 'absolute';
	    iframe.width = iframe.height = 0;
	    iframe.style.zIndex = "10000";
	    this._window.document.body.appendChild(iframe) ;
	    this.Doc = iframe.contentWindow.document ;
	    this.Doc.open() ;
	    this.Doc.write( '<html><head></head><body><\/body><\/html>' ) ;
	    this.Doc.close() ;

	    this.Doc.body.style.margin = this.Doc.body.style.padding = '0px' ;
	    this._iframe.contentWindow.onblur = this.Hide ;
	    iframe.contentWindow.Panel = this ;
	    this.Div = this.Doc.body.appendChild( this.Doc.createElement("div") ) ;
	    this.Div.style.cssFloat = 'left';
	}
	this.Doc.oncontextmenu = function(e){ return false; }
}

wgPanel.prototype.Show = function(x, y)
{
    if(!this.ff)
    {
        this._popup.show(x, y, 0, 0, null);
        this._popup.show( x, y, this.Div.offsetWidth, this.Div.offsetHeight, null);
    }
        else
    {
        this.Div.width = "";
        this.Div.height = "";
        this._iframe.width	= 1;
        this._iframe.height	= 1;
	    if ((x + this.Div.offsetWidth ) > this._window.document.body.clientWidth )
			x-= (x + this.Div.offsetWidth - this._window.document.body.clientWidth);
		x = (x < 0)? 0 : x;
		this._iframe.style.left	= x + "px";
	    this._iframe.style.top = y + "px";
			
		this._iframe.width	= this.Div.offsetWidth;
	    this._iframe.height = this.Div.offsetHeight;
	    this._iframe.contentWindow.focus();
	    this._IsOpened = true;
    }
}

wgPanel.prototype.Hide = function()
{
    this.ff = document.getElementById && !document.all;
    if(!this.ff)
        this._popup.hide();
    else
    {
        var wPanel = this.Panel ? this.Panel : this;
        if(!wPanel._IsOpened)
            return;
        wPanel._iframe.width = wPanel._iframe.height = 0;
    }
}

wgPanel.prototype.GetParentWindow = function()
{
    if(!this.ff)
        return this._window; 
    else
        return this._window;
}

wgPanel.prototype.ApplyStyleSheet = function(stylSheetURL)
{
    if(this.ff || this.opera)
    {
        
        var link = this.Doc.createElement("LINK");
	    link.type	= "text/css";
	    link.rel	= "stylesheet";
	    link.href	= stylSheetURL;
	    this.Doc.getElementsByTagName("HEAD")[0].appendChild(link);
	}
	else
	{
	    this.Doc.createStyleSheet(stylSheetURL);
	}
}

var wgSplitter = function(doc)
{
    this._div =  doc.body.appendChild(doc.createElement("div"));
    this._div.className = "wgSplitter";
    this._div.style.display = "none";
}

wgSplitter.prototype.Show = function(x, y, height)
{
    this._div.style.top = y + "px";
    this._div.style.left = x + "px";
    this._div.style.height = height + "px";
    this._div.style.display = "block";
}

wgSplitter.prototype.Hide = function()
{
     this._div.style.display = "none";
}

wgSplitter.prototype.MoveTo = function(x)
{
    this._div.style.left = x + "px";
}

// context menu
var wgContextMenu = new Object();
var startSplitter = new Object(); 
var endSplitter = new Object(); 


function wgCreateContextMenu()
{   
    wgContextMenu._Panel = new wgPanel((document.getElementById && !document.all) ? window.parent : window);   
    wgContextMenu._Panel.Div.className = "wgContextMenu";
 //   wgContextMenu._Panel.ApplyStyleSheet("wgContextMenu.css");
    wgContextMenu._Doc = wgContextMenu._Panel.Doc;
    startSplitter.Splitter = new wgSplitter(document);
    endSplitter.Splitter = new wgSplitter(document);
}

if(document.addEventListener)
{
     if( navigator.appName != "Opera")
        window.addEventListener("load",wgCreateContextMenu, true);
}
else
{
     if( navigator.appName != "Opera")
        window.attachEvent("onload",wgCreateContextMenu);
}

wgContextMenu.Show = function(x, y, gridId)
{
    this.gridId = gridId;
	this.Refresh();
	this._Panel.Show(x, y);
}

wgContextMenu.Refresh = function()
{
    var menuTable;
	if(this._load)
	{
	    if( this._Doc == null )
	        return;
	    menuTable = this._Doc.getElementById("wgMenuTable");
	    var rowCount = menuTable.rows.length;
	    for(i = rowCount; i >0; i--)
	    {
	        menuTable.deleteRow(i-1);
	    }
	}
	else
	{
	    // This happens typically when the page is still loading and the user is activating the context menu
	    if( wgContextMenu == null || wgContextMenu._Panel == null || wgContextMenu._Panel.Doc == null )
	        return;
	        
	    menuTable = wgContextMenu._Panel.Doc.createElement("TABLE");
	    menuTable.cellSpacing = 0;
	    menuTable.cellPadding = 0;
	    menuTable.id = "wgMenuTable";
	    this._Panel.Div.appendChild(menuTable);
	}

	var menuItems = eval(this.gridId + "columns");
	for(i = 0; i < menuItems.length - 1; i++)
	{
	    if(menuItems[i][4])
	    {
	        _Row = menuTable.insertRow(-1) ;
	        _Row.wgContextMenu = this ;
            	
	        _Row.onmouseover = wgContextMenuItem_Over;
	        _Row.onmouseout = wgContextMenuItem_Out;
	        _Row.onclick = wgContextMenuItem_OnClick ;
	        _Row.id = menuItems[i][3];
    	    
	        var oCell = _Row.insertCell(-1);
	        oCell.className = 'wgText' ;
	        if (menuItems[i][2]) 
	            oCell.innerHTML = '&radic;' ;
	        else
	            oCell.innerHTML = "&nbsp;"
    	
	        oCell = _Row.insertCell(-1) ;
	        oCell.className		= 'wgText' ;
	        oCell.noWrap		= true ;
	        oCell.innerHTML		= menuItems[i][0];
	    }
	}
	this._load = true;
	

}

function wgContextMenuItem_Over()
{
	this.className = "wgItemOver";
}
function wgContextMenuItem_Out()
{
	this.className = "wgItemOut";
}

function wgContextMenuItem_OnClick()
{
    var wnd = this.wgContextMenu._Panel.GetParentWindow();
    var columnsArray = wnd.eval(this.wgContextMenu.gridId + "columns");
    for(i = 0; i < columnsArray.length - 1; i++)
    {
        if(this.id == columnsArray[i][3])
            show = columnsArray[i][2]; 
    }
    this.wgContextMenu._Panel.Hide();
	wnd.wgShowColumn(this.wgContextMenu.gridId, this.id, !show, true);
	return false ;
}

startSplitter.Show = function(x, y)
{
    this.Splitter.Show(x,y,this.height);
}
startSplitter.Hide = function()
{
    this.Splitter.Hide();
}
endSplitter.Show = function(x, y)
{
    this.Splitter.Show(x,y,this.height);
}

endSplitter.Hide = function()
{
    this.Splitter.Hide();
}

function GetColumnSettings(columnsArray, columnId)
{
    for(var i = 0; i < columnsArray.length - 1; i++)
    {
        if(columnsArray[i][3] == columnId)
        return columnsArray[i];
    }
    return null;
}

function GetWidthsSums(columnsArray, columnId)
{
    var minSum = 0;
    var currentSum = 0;
    for(var i = 0; i < columnsArray.length - 1; i++)
    {
        if(columnId == columnsArray[i][3])
        {
            for(var j = i + 1; j < columnsArray.length - 1; j++)
            {
                if(columnsArray[j][2])
                {
                    minSum += (columnsArray[j][5])? columnsArray[j][1] : columnsArray[j][6];
                    currentSum += columnsArray[j][6];
                }
            }
            break;
        }
        
    }
    var objResult = new Object();
    objResult.MinSum = minSum;
    objResult.CurrentSum = currentSum;
    return objResult;
}

function GetTotalWidth(columnsArray)
{
    var total = 0;
    for(var i = 0; i < columnsArray.length - 1; i++)
    {
        if(columnsArray[i][2])
        {
            total += columnsArray[i][6];
        }
    }
    return total;
}

