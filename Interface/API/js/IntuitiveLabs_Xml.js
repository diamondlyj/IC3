if (IntuitiveLabs == undefined) var IntuitiveLabs = {};
if (IntuitiveLabs.Xml == undefined) IntuitiveLabs.Xml = {};

IntuitiveLabs.Xml.Load = function(str) {

    var output = null;

    if (window.DOMParser) {
        parser = new DOMParser();
        output = parser.parseFromString(str, "text/xml");
    }
    else // Internet Explorer
    {
        output = new ActiveXObject("Microsoft.XMLDOM");
        output.async = "false";
        output.loadXML(str);
    }

    return output;
}