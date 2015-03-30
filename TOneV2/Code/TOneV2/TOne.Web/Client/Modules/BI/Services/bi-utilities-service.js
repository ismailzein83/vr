
app.service('BIUtilitiesService', function () {

    return ({
        fillDateTimeProperties: fillDateTimeProperties
    });

    function fillDateTimeProperties(data, timeDimensionType, fromDate, toDate) {
        angular.forEach(data, function (itm) {
            var dateTimeValue = new Date(itm.Time);
            switch (timeDimensionType) {
                case 0:     //Yearly
                    itm.dateTimeValue = dateTimeValue.getFullYear();
                    break;
                case 1:   //Monthly
                    itm.dateTimeValue = getMonthNameShort(dateTimeValue);
                    itm.dateTimeGroupValue = dateTimeValue.getFullYear();
                    break;
                case 2:
                    itm.dateTimeValue = dateTimeValue.getDate();
                    itm.dateTimeGroupValue = getMonthNameShort(dateTimeValue) + "-" + dateTimeValue.getFullYear();
            }
        });
        
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