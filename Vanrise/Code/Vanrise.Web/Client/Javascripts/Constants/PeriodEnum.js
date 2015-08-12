(function (app) {


    function getCurrentYearInterval() {
        var date = new Date();
        return {
            from: new Date(date.getFullYear(), 0, 1),
            to: new Date()
        }
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
            from: lastMonday,
            to: new Date()
        }
    }

    function getLastWeekInterval() {
        var beforeOneWeek = new Date(new Date().getTime() - 60 * 60 * 24 * 7 * 1000);
        var day = beforeOneWeek.getDay();
        var diffToMonday = beforeOneWeek.getDate() - day + (day === 0 ? -6 : 1);
        var beforeLastMonday = new Date(beforeOneWeek.setDate(diffToMonday));
        var lastSunday = new Date(beforeOneWeek.setDate(diffToMonday + 6));
        return {
            from: beforeLastMonday,
            to: lastSunday
        }
    }

    function getCurrentMonthInterval() {
        var date = new Date();
        return {
            from: new Date(date.getFullYear(), date.getMonth(), 1),
            to: new Date()
        }
    }

    function getTodayInterval() {
        var date = new Date();
        return {
            from: date,
            to: date
        }
    }

    function getYesterdayInterval() {
        var date = new Date();
        date.setDate(date.getDate() - 1);
        return {
            from: date,
            to: date
        }
    }

    function getLastMonthInterval() {
        var date = new Date();
        return {
            from: new Date(date.getFullYear(), date.getMonth() - 1, 1),
            to: new Date(date.getFullYear(), date.getMonth(), 0)
        }
    }

    function getLastYearInterval() {
        var date = new Date();
        return {
            from: new Date(date.getFullYear() - 1, 0, 1),
            to: new Date(date.getFullYear() - 1, 11, 31)
        }
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


