var pathArray = location.href.split('/');
var protocol = pathArray[0];
var host = pathArray[2];
var baseurl = protocol + '//' + host
function decodeDate(date) {
    if (typeof (date) == "string") {
        var date = date.substring(6, date.length - 2);
        var d = new Date(parseInt(date));
    }
    else
        d = date;
    return d;
}
function TransDate2JsonAtNoon(date) {
    date.setHours(12);
    date.setMilliseconds(0);
    date.setMinutes(0);
    date.setSeconds(0);
    return '\/Date(' + date.getTime() + ')\/';
}


function decodeDateToString(date) {
    var date = date.substring(6, date.length - 2);
    var d = new Date(parseInt(date));
    return dateToString(d);
}

function changeDateJson(value) {
    if (value && value != undefined && value != '') {
        var dateString = '';
        var d = decodeDate(value);
        var day = "" + (parseInt(d.getDate()));
        if (day.length == 1)
            dateString += "0" + day;
        else
            dateString += day;
        var month = "" + (parseInt(d.getMonth()) + 1);
        if (month.length == 1)
            dateString += "/0" + month;
        else
            dateString += "/" + month;
        dateString += "/" + d.getFullYear();
        return dateString;
    }
    else
        return "";
}
function returnDateTimeJson(value) {
    if (value && value != undefined && value != '') {
        var dateString = '';
        var d = decodeDate(value);
        return dateToStringWithHoure(d);
    }
    else
        return "";

}
function dateToStringWithHoure(date) {
    var dateString = '';
    if (date) {
        var day = "" + (parseInt(date.getDate()));
        if (day.length == 1)
            dateString += "0" + day;
        else
            dateString += day;
        var month = "" + (parseInt(date.getMonth()) + 1);
        if (month.length == 1)
            dateString += "/0" + month;
        else
            dateString += "/" + month;
        dateString += "/" + date.getFullYear();
        dateString += " " + date.getHours() + ":" + date.getMinutes();

    }
    return dateString;
}
function dateToStringWithHMS(date) {
    var dateString = '';
    if (date) {
        var day = "" + (parseInt(date.getDate()));
        if (day.length == 1)
            dateString += "0" + day;
        else
            dateString += day;
        var month = "" + (parseInt(date.getMonth()) + 1);
        if (month.length == 1)
            dateString += "/0" + month;
        else
            dateString += "/" + month;
        dateString += "/" + date.getFullYear();

        dateString += " " + date.getHours() + ":" + date.getMinutes() +":"+ date.getSeconds();

    }
    return dateString;
}

function dateToStringWithHMS2(date) {
    var dateString = '';
    if (date) {
        var day = "" + (parseInt(date.getDate()));
        if (day.length == 1)
            dateString += "0" + day;
        else
            dateString += day;
        var month = "" + (parseInt(date.getMonth()) + 1);
        if (month.length == 1)
            dateString += "/0" + month;
        else
            dateString += "/" + month;
        dateString += "/" + date.getFullYear();

        var hours = date.getHours();

        var hourT = "";
        if (hours.length == 1)
            hourT = "0" + hours;
        else
            hourT = hours;

        var min = date.gdate.getMinutes();

        var minT = "";
        if (min.length == 1)
            minT = "0" + min;
        else
            minT = min;
        var minT = "";

        if (min.length == 1)
            minT = "0" + min;
        else
            minT = min;

        dateString += " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();

    }
    return dateString;
}
function timeTostring(date) {
    var timeString = "";
    if (date) {      

        timeString += date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();
    }   

    return timeString;
}

//var dateString = date.Format("d/m/y"); retourne par exemple : 24/11/10
function dateToString(date) {
    var dateString = '';
    if (date) {
        var day = "" + (parseInt(date.getDate()));
        if (day.length == 1)
            dateString += "0" + day;
        else
            dateString += day;
        var month = "" + (parseInt(date.getMonth()) + 1);
        if (month.length == 1)
            dateString += "/0" + month;
        else
            dateString += "/" + month;
        dateString += "/" + date.getFullYear();
    }
    return dateString;
}
function dateToString2(date) {
    var dateString = '';
    var day = "" + (parseInt(date.getDate()));
    if (day.length == 1)
        dateString += "0" + day;
    else
        dateString += day;

    var month = "" + (parseInt(date.getMonth()) + 1);
    if (month.length == 1)
        dateString += "/0" + month;
    else
        dateString += "/" + month;

    dateString += "/" + date.getFullYear();
    return dateString;
}
function dateToStringUnderScore(date) {
    var dateString = '';
    var day = "" + (parseInt(date.getDate()));
    if (day.length == 1)
        dateString += "0" + day;
    else
        dateString += day;

    var month = "" + (parseInt(date.getMonth()) + 1);
    if (month.length == 1)
        dateString += "/0" + month;
    else
        dateString += "/" + month;

    dateString += "/" + date.getFullYear();
    return dateString;
}
function DateDeserialize(dateStr) {
    return eval('new' + dateStr.replace(/\//g, ' '));
}

function waitAlert(msg){ 
	showAlertBootStrapMsg("attention",msg,true);
}
function hideWaitAlert(){ 
	$(".attention").hide();
	$('#notification').html('');
}
function showAlertBootStrapMsg(type,msg,wait){ 
	$('#notification').html('<div class="'+type+'" style="display: none;"> '+msg+'  <img src="images/alert/close.png" alt="" class="close" data-dismiss="alert" ></div>');
	$("."+type).show('slow');
	if(!wait){
		setTimeout(function() {
			$("."+type).delay(500).hide('slow');
			$('#notification').html('');
		}, 7000);
	}
}