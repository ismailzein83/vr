(function (app) {

    "use strict";
    function getCurrentYearInterval() {
        var date = new Date();
        return {
            from: new Date(date.getFullYear(), 0, 1),
            to: new Date(date.getFullYear(), date.getMonth(), date.getDate(), 23, 59, 59,998)
        };
    }

    function getCurrentWeekInterval() {
        var thisWeek = new Date(new Date().getTime() - 60 * 60 * 24 * 1000);
        var day = thisWeek.getDay();
        var lastMonday;
        if (day === 0) {
            lastMonday = new Date();
        }
        else {
            var diffToMonday = thisWeek.getDate() - day + (day === 0 ? -6 : 1);
            lastMonday = new Date(thisWeek.setDate(diffToMonday));
        }
        return {
            from: new Date(lastMonday.getFullYear(), lastMonday.getMonth(), lastMonday.getDate(), 0, 0, 0),
            to: new Date()
        };
    }

    function getLastWeekInterval() {
        var beforeOneWeek = new Date(new Date().getTime() - 60 * 60 * 24 * 7 * 1000);
        var beforeOneWeekTo = new Date(new Date().getTime() - 60 * 60 * 24 * 7 * 1000);
        var day = beforeOneWeek.getDay();
        var diffToMonday = beforeOneWeek.getDate() - day + (day === 0 ? -6 : 1);
        var beforeLastMonday = new Date(beforeOneWeek.setDate(diffToMonday));
        var lastSunday = new Date(beforeOneWeekTo.setDate(diffToMonday + 6));
        var fromDate = new Date(beforeLastMonday.getFullYear(), beforeLastMonday.getMonth(), beforeLastMonday.getDate(), 0, 0, 0);
        var toDate = new Date(lastSunday.getFullYear(), lastSunday.getMonth(), lastSunday.getDate(), 23, 59, 59,998);
        return {
            from: fromDate,
            to: toDate
        };
    }

    function getCurrentMonthInterval() {
        var date = new Date();
        return {
            from: new Date(date.getFullYear(), date.getMonth(), 1),
            to: new Date(date.getFullYear(), date.getMonth(), date.getDate(), 23, 59, 59,998)
        };
    }

    function getTodayInterval() {
        var date = new Date();
        return {
            from: new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0),
            to: new Date(date.getFullYear(), date.getMonth(), date.getDate(), 23, 59, 59,998)
        };
    }

    function getYesterdayInterval() {
        var date = new Date();
        return {
            from: new Date(date.getFullYear(), date.getMonth(), date.getDate() - 1, 0, 0, 0),
            to: new Date(date.getFullYear(), date.getMonth(), date.getDate() - 1, 23, 59, 59,998)
        };
    }

    function getLastMonthInterval() {
        var date = new Date();
        return {
            from: new Date(date.getFullYear(), date.getMonth() - 1, 1),
            to: new Date(date.getFullYear(), date.getMonth(), 0, 23, 59, 59,998)
        };
    }

    function getLastYearInterval() {
        var date = new Date();
        return {
            from: new Date(date.getFullYear() - 1, 0, 1, 0, 0, 0),
            to: new Date(date.getFullYear() - 1, 11, 31, 23, 59, 59,998)
        };
    }

    app.constant('PeriodEnum', {
        LastYear: { value: 0, description: "Last Year", getInterval: getLastYearInterval },
        LastMonth: { value: 1, description: "Last Month", getInterval: getLastMonthInterval },
        LastWeek: { value: 2, description: "Last Week", getInterval: getLastWeekInterval },
        Yesterday: { value: 3, description: "Yesterday", getInterval: getYesterdayInterval },
        Today: { value: 4, description: "Today", getInterval: getTodayInterval },
        CurrentWeek: { value: 5, description: "Current Week", getInterval: getCurrentWeekInterval },
        CurrentMonth: { value: 6, description: "Current Month", getInterval: getCurrentMonthInterval },
        CurrentYear: { value: 7, description: "Current Year", getInterval: getCurrentYearInterval }
    });

})(app);


