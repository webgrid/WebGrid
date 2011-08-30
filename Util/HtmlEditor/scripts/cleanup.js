/*
* class to clean up HTML code
*/


CleanUp = function()
{
}


CleanUp.prototype.CleanWordText = function(html)
{
	html = html.replace(/<o:p>\s*<\/o:p>/g, "") ;
	html = html.replace(/<o:p>.*?<\/o:p>/g, "&nbsp;") ;
	
	// Remove mso-xxx styles.
	html = html.replace( /\s*mso-[^:]+:[^;"]+;?/gi, "" ) ;

	// Remove margin styles.
	html = html.replace( /\s*MARGIN: 0cm 0cm 0pt\s*;/gi, "" ) ;
	html = html.replace( /\s*MARGIN: 0cm 0cm 0pt\s*"/gi, "\"" ) ;

	html = html.replace( /\s*TEXT-INDENT: 0cm\s*;/gi, "" ) ;
	html = html.replace( /\s*TEXT-INDENT: 0cm\s*"/gi, "\"" ) ;

	html = html.replace( /\s*TEXT-ALIGN: [^\s;]+;?"/gi, "\"" ) ;

	html = html.replace( /\s*PAGE-BREAK-BEFORE: [^\s;]+;?"/gi, "\"" ) ;

	html = html.replace( /\s*FONT-VARIANT: [^\s;]+;?"/gi, "\"" ) ;

	html = html.replace( /\s*tab-stops:[^;"]*;?/gi, "" ) ;
	html = html.replace( /\s*tab-stops:[^"]*/gi, "" ) ;

	// Remove FONT face attributes.
	//if ( bIgnoreFont )
	//{
		html = html.replace( /\s*face="[^"]*"/gi, "" ) ;
		html = html.replace( /\s*face=[^ >]*/gi, "" ) ;

		html = html.replace( /\s*FONT-FAMILY:[^;"]*;?/gi, "" ) ;
	//}
	
	// Remove Class attributes
	html = html.replace(/<(\w[^>]*) class=([^ |>]*)([^>]*)/gi, "<$1$3") ;

	// Remove styles.
	//if ( bRemoveStyles )
		html = html.replace( /<(\w[^>]*) style="([^\"]*)"([^>]*)/gi, "<$1$3" ) ;

	// Remove empty styles.
	html =  html.replace( /\s*style="\s*"/gi, '' ) ;
	
	html = html.replace( /<SPAN\s*[^>]*>\s*&nbsp;\s*<\/SPAN>/gi, '&nbsp;' ) ;
	
	html = html.replace( /<SPAN\s*[^>]*><\/SPAN>/gi, '' ) ;
	
	// Remove Lang attributes
	html = html.replace(/<(\w[^>]*) lang=([^ |>]*)([^>]*)/gi, "<$1$3") ;
	
	html = html.replace( /<SPAN\s*>(.*?)<\/SPAN>/gi, '$1' ) ;
	
	html = html.replace( /<FONT\s*>(.*?)<\/FONT>/gi, '$1' ) ;

	// Remove XML elements and declarations
	html = html.replace(/<\\?\?xml[^>]*>/gi, "") ;
	
	// Remove Tags with XML namespace declarations: <o:p></o:p>
	html = html.replace(/<\/?\w+:[^>]*>/gi, "") ;
	
	html = html.replace( /<H\d>\s*<\/H\d>/gi, '' ) ;

	html = html.replace( /<H1([^>]*)>/gi, '<div$1><b><font size="6">' ) ;
	html = html.replace( /<H2([^>]*)>/gi, '<div$1><b><font size="5">' ) ;
	html = html.replace( /<H3([^>]*)>/gi, '<div$1><b><font size="4">' ) ;
	html = html.replace( /<H4([^>]*)>/gi, '<div$1><b><font size="3">' ) ;
	html = html.replace( /<H5([^>]*)>/gi, '<div$1><b><font size="2">' ) ;
	html = html.replace( /<H6([^>]*)>/gi, '<div$1><b><font size="1">' ) ;

	html = html.replace( /<\/H\d>/gi, '</font></b></div>' ) ;
	
	html = html.replace( /<(U|I|STRIKE)>&nbsp;<\/\1>/g, '&nbsp;' ) ;

	// Remove empty tags (three times, just to be sure).
	html = html.replace( /<([^\s>]+)[^>]*>\s*<\/\1>/g, '' ) ;
	html = html.replace( /<([^\s>]+)[^>]*>\s*<\/\1>/g, '' ) ;
	html = html.replace( /<([^\s>]+)[^>]*>\s*<\/\1>/g, '' ) ;

	// replace <p>&nbsp;</p> as <br />
	//if (s.force_br_newlines)
	html = html.replace(/<P>(&nbsp;|&#160;)<\/P>/g, '<br />'); 

	// Transform <P> to <DIV>
	//var re = new RegExp("(<P)([^>]*>.*?)(<\/P>)","gi") ;	// Different because of a IE 5.0 error
	//html = html.replace( re, "<div$2</div>" ) ;

	return html ;
}

// Onother cleanup method 
//
//
CleanUp.prototype.CleanWordText2 = function(html)
{
	var bull = String.fromCharCode(8226);
	var middot = String.fromCharCode(183);


/*	var rl = tinyMCE.getParam("paste_replace_list", '\u2122,<sup>TM</sup>,\u2026,...,\u201c|\u201d,",\u2019,\',\u2013|\u2014|\u2015|\u2212,-').split(',');
	for (var i=0; i<rl.length; i+=2)
		html = html.replace(new RegExp(rl[i], 'gi'), rl[i+1]);
*/
/*	if (tinyMCE.getParam("paste_convert_headers_to_strong", false)) {
		html = html.replace(new RegExp('<p class=MsoHeading.*?>(.*?)<\/p>', 'gi'), '<p><b>$1</b></p>');
	}
*/
	html = html.replace(new RegExp('tab-stops: list [0-9]+.0pt">', 'gi'), '">' + "--list--");
	html = html.replace(new RegExp(bull + "(.*?)<BR>", "gi"), "<p>" + middot + "$1</p>");
	html = html.replace(new RegExp('<SPAN style="mso-list: Ignore">', 'gi'), "<span>" + bull); // Covert to bull list
	html = html.replace(/<o:p><\/o:p>/gi, "");
	html = html.replace(new RegExp('<br style="page-break-before: always;.*>', 'gi'), '-- page break --'); // Replace pagebreaks
	html = html.replace(new RegExp('<(!--)([^>]*)(--)>', 'g'), "");  // Word comments
	html = html.replace(/<\/?span[^>]*>/gi, "");
	html = html.replace(new RegExp('<(\\w[^>]*) style="([^"]*)"([^>]*)', 'gi'), "<$1$3");
	html = html.replace(/<\/?font[^>]*>/gi, "");

	// Strips class attributes.
	//switch (tinyMCE.getParam("paste_strip_class_attributes", "all")) {
	//	case "all":
	//		html = html.replace(/<(\w[^>]*) class=([^ |>]*)([^>]*)/gi, "<$1$3");
	//		break;

	//	case "mso":
			html = html.replace(new RegExp('<(\\w[^>]*) class="?mso([^ |>]*)([^>]*)', 'gi'), "<$1$3");
	//		break;
	//}

	//html = html.replace(new RegExp('href="?' + TinyMCE_PastePlugin._reEscape("" + document.location) + '', 'gi'), 'href="' + tinyMCE.settings['document_base_url']);
	html = html.replace(/<(\w[^>]*) lang=([^ |>]*)([^>]*)/gi, "<$1$3");
	html = html.replace(/<\\?\?xml[^>]*>/gi, "");
	html = html.replace(/<\/?\w+:[^>]*>/gi, "");
	html = html.replace(/-- page break --\s*<p>&nbsp;<\/p>/gi, ""); // Remove pagebreaks
	html = html.replace(/-- page break --/gi, ""); // Remove pagebreaks

//		html = html.replace(/\/?&nbsp;*/gi, ""); &nbsp;
//		html = html.replace(/<p>&nbsp;<\/p>/gi, '');

	/*if (!tinyMCE.settings['force_p_newlines']) {
		html = html.replace('', '' ,'gi');
		html = html.replace('</p>', '<br /><br />' ,'gi');
	}*/

	/*if (!tinyMCE.isMSIE && !tinyMCE.settings['force_p_newlines']) {
		html = html.replace(/<\/?p[^>]*>/gi, "");
	}*/

	html = html.replace(/<\/?div[^>]*>/gi, "");


	// Replace all headers with strong and fix some other issues
	//if (tinyMCE.getParam("paste_convert_headers_to_strong", false)) {
	//	html = html.replace(/<h[1-6]>&nbsp;<\/h[1-6]>/gi, '<p>&nbsp;&nbsp;</p>');
	//	html = html.replace(/<h[1-6]>/gi, '<p><b>');
	//	html = html.replace(/<\/h[1-6]>/gi, '</b></p>');
	//	html = html.replace(/<b>&nbsp;<\/b>/gi, '<b>&nbsp;&nbsp;</b>');
	//	html = html.replace(/^(&nbsp;)*/gi, '');
	//}

	html = html.replace(/--list--/gi, ""); // Remove --list--
	
	return html;
}

CleanUp.prototype.FormatHTML = function(html)
{
	alert("CleanUp.prototype.FormatHTML isn't implemented.");
}
CleanUp.prototype.ClearEditorAttributes = function(html)
{

}