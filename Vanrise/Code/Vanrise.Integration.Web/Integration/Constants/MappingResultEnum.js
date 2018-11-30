﻿app.constant('VR_Integration_MappingResultEnum', {
    Valid: { value: 1, description: "Valid" },
    Invalid: { value: 2, description: "Invalid" },
    Empty: { value: 3, description: "Empty" },
    PartialInvalid: {value: 4, description: "Partial Invalid"}
});

app.constant('VR_Integration_BatchStateEnum', {
    Normal: { value: 0, description: "Normal" },
    Missing: { value: 1, description: "Missing" },
    Delayed: { value: 2, description: "Delayed" },
    Duplicated: { value: 3, description: "Duplicated" }
});