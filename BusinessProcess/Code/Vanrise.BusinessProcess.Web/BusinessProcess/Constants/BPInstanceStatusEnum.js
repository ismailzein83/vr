app.constant('BPInstanceStatusEnum', {
    New: { value: 0, description: "New" },
    Running: { value: 10, description: "Running" },
    ProcessFailed: { value: 20, description: "ProcessFailed" },
    Completed: { value: 50, description: "Completed" },
    Aborted: { value: 60, description: "Aborted" },
    Suspended: { value: 70, description: "Suspended" },
    Terminated: { value: 80, description: "Terminated" },
});