app.constant('TimeSchedulerTypeEnum', {
    Interval: {
        name: "Interval",
        FQTN: 'Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger',
        editor:"vr-runtime-tasktrigger-interval"
    },
    Daily: {
        name: "Daily",
        FQTN: 'Vanrise.Runtime.Triggers.TimeTaskTrigger.DailyTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger',
        editor: "vr-runtime-tasktrigger-daily"

    },
    Weekly: {
        name: "Weekly",
        FQTN: 'Vanrise.Runtime.Triggers.TimeTaskTrigger.WeeklyTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger',
        editor: "vr-runtime-tasktrigger-weekly"
    },
    Monthly: {
        name: "Monthly",
        FQTN: 'Vanrise.Runtime.Triggers.TimeTaskTrigger.MonthlyTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger',
        editor: "vr-runtime-tasktrigger-monthly"
    }
});