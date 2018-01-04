app.constant('VR_Notification_NotificationStatusEnum', {
    New: { value: 0, description: 'New' },
    Executing: { value: 10, description: 'Executing' },
    Executed: { value: 20, description: 'Executed' },
    RolledBack: { value: 30, description: 'Rolled Back' },
    ErrorOnExecution: { value: 40, description: 'Execution Error' },
    ErrorOnRollback: { value: 50, description: 'Rollback Error' },
    Rollback: { value: 60, description: 'Rollback' }
});