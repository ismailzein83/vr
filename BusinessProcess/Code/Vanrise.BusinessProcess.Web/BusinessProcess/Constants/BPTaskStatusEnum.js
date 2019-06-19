app.constant('BPTaskStatusEnum', {
    New: { value: 0, description: "New", IsClosed: false },
    Started: { value: 10, description: "Started", IsClosed: false },
    Completed: { value: 50, description: "Completed", IsClosed: true  },
    Cancelled: { value: 60, description: "Cancelled", IsClosed: true  }
});