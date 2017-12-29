app.constant('BPInstanceStatusEnum', {
    New: { value: 0, description: "New", isOpened: true },
    Postponed: { value: 5, description: "Postponed", isOpened: true },
    Running: { value: 10, description: "Running", isOpened: true },
    Waiting: { value: 20, description: "Waiting", isOpened: true },
    Completed: { value: 50, description: "Completed", isOpened: false },
    Aborted: { value: 60, description: "Aborted", isOpened: false },
    Suspended: { value: 70, description: "Suspended", isOpened: false },
    Terminated: { value: 80, description: "Terminated", isOpened: false }
});