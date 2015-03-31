


app.service('BIUtilitiesService', function (BITimeDimensionTypeEnum) {

    return ({
        fillDateTimeProperties: fillDateTimeProperties,
        getNextDate: getNextDate
    });

    function fillDateTimeProperties(data, timeDimensionType, fromDateString, toDateString, dontFillGroup) {
        var fromDate = new Date(fromDateString);
        var toDate = new Date(toDateString);
        if (dontFillGroup == undefined) {
            var isLongPeriod = checkIsLongPeriod(timeDimensionType, fromDate, toDate);
            if (isLongPeriod == true)
                dontFillGroup = true;
        }
        angular.forEach(data, function (itm) {
            var dateTimeValue = new Date(itm.Time);
            switch (timeDimensionType) {
                case BITimeDimensionTypeEnum.Yearly.value:
                    itm.dateTimeValue = dateTimeValue.getFullYear();
                    break;
                case BITimeDimensionTypeEnum.Monthly.value:
                    if (dontFillGroup)
                        itm.dateTimeValue = getMonthNameShort(dateTimeValue) + "-" + getShortYear(dateTimeValue);
                    else {
                        itm.dateTimeValue = getMonthNameShort(dateTimeValue);
                        itm.dateTimeGroupValue = dateTimeValue.getFullYear();
                    }
                    
                    break;
                case BITimeDimensionTypeEnum.Weekly.value:
                    itm.dateTimeValue = "Week " + itm.WeekNumber;
                    var groupName = getMonthNameShort(dateTimeValue) + "-" + getShortYear(dateTimeValue);
                    if (dontFillGroup)
                        itm.dateTimeValue = itm.dateTimeValue + "-" + groupName;
                    else 
                        itm.dateTimeGroupValue = groupName;                                        
                    break;
                case BITimeDimensionTypeEnum.Daily.value:
                    itm.dateTimeValue = dateTimeValue.getDate();
                    var groupName = getMonthNameShort(dateTimeValue) + "-" + getShortYear(dateTimeValue);
                    if (dontFillGroup)
                        itm.dateTimeValue = itm.dateTimeValue + "-" + groupName;
                    else 
                        itm.dateTimeGroupValue = groupName;
                    break;

                case BITimeDimensionTypeEnum.Hourly.value:
                    var hour = dateTimeValue.getHours().toString();
                    var minute = dateTimeValue.getMinutes().toString();
                    itm.dateTimeValue = (hour.length < 2 ? '0' + hour : hour) + ":" + (minute.length < 2 ? '0' + minute : minute);
                    var groupName = dateTimeValue.getDate() + "-" + getMonthNameShort(dateTimeValue) + "-" + getShortYear(dateTimeValue);
                    if (dontFillGroup)
                        itm.dateTimeValue = groupName + " " + itm.dateTimeValue;
                    else
                        itm.dateTimeGroupValue = groupName;
                    break;
            }
        });
        
    }

    function checkIsLongPeriod(timeDimensionType, fromDate, toDate) {
       
        switch (timeDimensionType) {
            case BITimeDimensionTypeEnum.Yearly.value:
                return false;
            case BITimeDimensionTypeEnum.Monthly.value:
                if (toDate.getFullYear() - fromDate.getFullYear() > 4)
                    return true;
                else
                    return false;
            case BITimeDimensionTypeEnum.Weekly.value:
                if (getDateDifference(fromDate, toDate) > 200)
                    return true
                else
                    return false;
            case BITimeDimensionTypeEnum.Daily.value:
                if (getDateDifference(fromDate, toDate) > 50)
                    return true;
                else
                    return false;
            case BITimeDimensionTypeEnum.Hourly.value:
                if (getDateDifference(fromDate, toDate) > 2)
                    return true;
                else
                    return false;
        }
    }

    function getNextDate(timeDimensionType, timeRecord) {
        var dateTimeValue = new Date(timeRecord.Time);
        
        switch (timeDimensionType) {
            case BITimeDimensionTypeEnum.Yearly.value:
                dateTimeValue.setFullYear(dateTimeValue.getFullYear() + 1);
                return dateTimeValue;
            case BITimeDimensionTypeEnum.Monthly.value:
                dateTimeValue.setMonth(dateTimeValue.getMonth() + 1);
                dateTimeValue.setDate(dateTimeValue.getDate() - 1);
                return dateTimeValue;
            case BITimeDimensionTypeEnum.Weekly.value:
                dateTimeValue.setDate(dateTimeValue.getDate() + 7);
                return dateTimeValue;
            case BITimeDimensionTypeEnum.Daily.value:
                dateTimeValue.setDate(dateTimeValue.getDate() + 1);
                return dateTimeValue;
        }
    }

    function getShortYear(date)
    {
        var fullYear = date.getFullYear().toString();
        if (fullYear.length == 4)
            return fullYear.substring(2, 4);
        else
            return fullYear
    }

    function getDateDifference(fromDate, toDate) {
        var timeDiff = toDate.getTime() - fromDate.getTime();
        return Math.ceil(timeDiff / (1000 * 3600 * 24));
    }

    //var locale = {
    //    en: {
    //        month_names: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
    //        month_names_short: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
    //    }
    //};


  

    //Date.prototype.getMonthName = function (lang) {
    //    lang = lang && (lang in locale) ? lang : 'en';
    //    return Date.locale[lang].month_names[this.getMonth()];
    //};

    function getMonthNameShort(date, lang) {
        var shortMonthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
         var monthIndex = date.getMonth();
         return shortMonthNames[monthIndex];
    };


});