app.constant('BPInstanceStatusEnum', {
    New: { value: 0, description: "New", isOpened: true },
    Running: { value: 10, description: "Running", isOpened: true },
    ProcessFailed: { value: 20, description: "ProcessFailed", isOpened: true },
    Completed: { value: 50, description: "Completed", isOpened: false },
    Aborted: { value: 60, description: "Aborted", isOpened: false },
    Suspended: { value: 70, description: "Suspended", isOpened: false },
    Terminated: { value: 80, description: "Terminated", isOpened: false }
});