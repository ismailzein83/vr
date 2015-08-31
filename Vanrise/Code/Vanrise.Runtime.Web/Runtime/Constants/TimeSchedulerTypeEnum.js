app.constant('TimeSchedulerTypeEnum', {
    Interval: {
        templateURL: '/Client/Modules/Runtime/Views/TriggerTemplates/IntervalTimerTriggerTemplate.html', name: "Interval",
        FQTN: 'Vanrise.Runtime.Triggers.TimeTaskTrigger.IntervalTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger'
    },
    Daily: {
        templateURL: '/Client/Modules/Runtime/Views/TriggerTemplates/DailyTimerTriggerTemplate.html', name: "Daily",
        FQTN: 'Vanrise.Runtime.Triggers.TimeTaskTrigger.DailyTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger'
    },
    Weekly: {
        templateURL: '/Client/Modules/Runtime/Views/TriggerTemplates/WeeklyTimerTriggerTemplate.html', name: "Weekly",
        FQTN: 'Vanrise.Runtime.Triggers.TimeTaskTrigger.WeeklyTimeSchedulerTaskTrigger, Vanrise.Runtime.Triggers.TimeTaskTrigger'
    }
});