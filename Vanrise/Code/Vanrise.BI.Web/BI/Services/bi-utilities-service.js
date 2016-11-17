app.service('BIUtilitiesService', function (TimeDimensionTypeEnum, VRModalService, SecurityService, UtilsService) {

    return ({
        openEntityReport: openEntityReport,
        fillDateTimeProperties: fillDateTimeProperties,
        getNextDate: getNextDate,
        checkPermissions: checkPermissions
    });

    function openEntityReport(entityType, entityId, entityName) {
        var parameters = {
            EntityType: entityType,
            EntityName: entityName,
            EntityId: entityId
        };
        var modalSettings = {
            useModalTemplate: true,
            width: "80%",
            maxHeight: "800px",
            title: entityName
        };
        VRModalService.showModal('/Client/Modules/BI/Views/Reports/EntityReport.html', parameters, modalSettings);
    }

    function checkPermissions(measures) {
        var deferred = UtilsService.createPromiseDeferred();
        var promises = [];
        var returnValue = true;

        var i = 0;
        while (returnValue && i < measures.length) {
            var requiredPermissions = measures[i].RequiredPermissions;

            if (requiredPermissions != undefined && requiredPermissions != null && requiredPermissions != '') {
                var isAllowedPromise = SecurityService.IsAllowed(requiredPermissions);
                isAllowedPromise.then(function (isAllowed) {
                    if (!isAllowed) { returnValue = false; }
                });
                promises.push(isAllowedPromise);
            }

            i++;
        }

        UtilsService.waitMultiplePromises(promises).then(function () {
            deferred.resolve(returnValue);
        }).catch(function (error) {
            deferred.reject(error);
        });

        return deferred.promise;
    }
    
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
                case TimeDimensionTypeEnum.Yearly.value:
                    itm.dateTimeValue = dateTimeValue.getFullYear();
                    break;
                case TimeDimensionTypeEnum.Monthly.value:
                    if (dontFillGroup)
                        itm.dateTimeValue = getMonthNameShort(dateTimeValue) + "-" + getShortYear(dateTimeValue);
                    else {
                        itm.dateTimeValue = getMonthNameShort(dateTimeValue);
                        itm.dateTimeGroupValue = dateTimeValue.getFullYear();
                    }
                    
                    break;
                case TimeDimensionTypeEnum.Weekly.value:
                    itm.dateTimeValue = "Week " + itm.WeekNumber;
                    var groupName = getMonthNameShort(dateTimeValue) + "-" + getShortYear(dateTimeValue);
                    if (dontFillGroup)
                        itm.dateTimeValue = itm.dateTimeValue + "-" + groupName;
                    else 
                        itm.dateTimeGroupValue = groupName;                                        
                    break;
                case TimeDimensionTypeEnum.Daily.value:
                    itm.dateTimeValue = dateTimeValue.getDate();
                    var groupName = getMonthNameShort(dateTimeValue) + "-" + getShortYear(dateTimeValue);
                    if (dontFillGroup)
                        itm.dateTimeValue = itm.dateTimeValue + "-" + groupName;
                    else 
                        itm.dateTimeGroupValue = groupName;
                    break;

                case TimeDimensionTypeEnum.Hourly.value:
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
            case TimeDimensionTypeEnum.Yearly.value:
                return false;
            case TimeDimensionTypeEnum.Monthly.value:
                if (toDate.getFullYear() - fromDate.getFullYear() > 4)
                    return true;
                else
                    return false;
            case TimeDimensionTypeEnum.Weekly.value:
                if (getDateDifference(fromDate, toDate) > 200)
                    return true;
                else
                    return false;
            case TimeDimensionTypeEnum.Daily.value:
                if (getDateDifference(fromDate, toDate) > 50)
                    return true;
                else
                    return false;
            case TimeDimensionTypeEnum.Hourly.value:
                if (getDateDifference(fromDate, toDate) > 2)
                    return true;
                else
                    return false;
        }
    }

    function getNextDate(timeDimensionType, timeRecord) {
        var dateTimeValue = new Date(timeRecord.Time);
        
        switch (timeDimensionType) {
            case TimeDimensionTypeEnum.Yearly.value:
                dateTimeValue.setFullYear(dateTimeValue.getFullYear() + 1);
                dateTimeValue.setDate(dateTimeValue.getDate() - 1);
                return dateTimeValue;
            case TimeDimensionTypeEnum.Monthly.value:
                dateTimeValue.setMonth(dateTimeValue.getMonth() + 1);
                dateTimeValue.setDate(dateTimeValue.getDate() - 1);
                return dateTimeValue;
            case TimeDimensionTypeEnum.Weekly.value:
                dateTimeValue.setDate(dateTimeValue.getDate() + 6);
                return dateTimeValue;
            case TimeDimensionTypeEnum.Daily.value:
                //dateTimeValue.setDate(dateTimeValue.getDate() + 1);
                return dateTimeValue;
        }
    }

    function getShortYear(date)
    {
        var fullYear = date.getFullYear().toString();
        if (fullYear.length == 4)
            return fullYear.substring(2, 4);
        else
            return fullYear;
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
    }
});