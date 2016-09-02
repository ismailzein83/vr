app.constant('Qm_CliTester_CallTestStatusEnum', {
    New: { value: 0, description: "New" },
    Initiated: { value: 10, description: "Initiated" },
    InitiationFailedWithRetry: { value: 20, description: "Initiation Failed With Retry" },
    PartiallyCompleted: { value: 30, description: "Partially Completed" },
    GetProgressFailedWithRetry: { value: 40, description: "Call Failed" },
    Completed: { value: 50, description: "Completed" },
    InitiationFailedWithNoRetry: { value: 60, description: "Initiation Failed With No Retry" },
    GetProgressFailedWithNoRetry: { value: 70, description: "Call Failed" }
});