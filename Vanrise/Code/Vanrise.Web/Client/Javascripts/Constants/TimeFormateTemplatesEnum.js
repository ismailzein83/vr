app.constant('TimeFormateTemplatesEnum', {
    MMddyyy: { value: 0, description: "MM/dd/yyyy (ex: 08/22/2006)", expression: "MM/dd/yyyy" },
    ddddddMMMMyyyy: { value: 1, description: "dddd, dd MMMM yyyy (ex: Tuesday, 22 August 2006)", expression: "dddd, dd MMMM yyyy" },
    ddddddMMMMyyyyHHmm: { value: 2, description: "dddd, dd MMMM yyyy HH:mm (ex: Tuesday, 22 August 2006 06:30)", expression: "dddd, dd MMMM yyyy HH:mm" },
    ddddddMMMMyyyyhhmmtt: { value: 3, description: "dddd, dd MMMM yyyy hh:mm tt Tuesday, 22 August 2006 06:30 AM", expression: "dddd, dd MMMM yyyy hh:mm tt" },
    ddddddMMMMyyyyHmm: { value: 4, description: "dddd, dd MMMM yyyy H:mm (ex: Tuesday, 22 August 2006 6:30)", expression: "dddd, dd MMMM yyyy H:mm" },
    ddddddMMMMyyyyhmmtt: { value: 5, description: "dddd, dd MMMM yyyy h:mm tt (ex: Tuesday, 22 August 2006 6:30 AM)", expression: "dddd, dd MMMM yyyy h:mm tt" },
    ddddddMMMMyyyyHHmmss: { value: 6, description: "dddd, dd MMMM yyyy HH:mm:ss	(ex: Tuesday, 22 August 2006 06:30:07)", expression: "dddd, dd MMMM yyyy HH:mm:ss" },
    MMddyyyyHHmm: { value: 7, description: "MM/dd/yyyy HH:mm (ex: 08/22/2006 06:30)", expression: "MM/dd/yyyy HH:mm" },
    MMddyyyyhhmmtt: { value: 8, description: "MM/dd/yyyy hh:mm tt (ex: 08/22/2006 06:30 AM)", expression: "MM/dd/yyyy hh:mm tt" },
    MMddyyyyHmm: { value: 9, description: "MM/dd/yyyy H:mm	(ex: 08/22/2006 6:30)", expression: "MM/dd/yyyy H:mm" },
    MMddyyyyhmmtt: { value: 10, description: "MM/dd/yyyy h:mm tt (ex: 08/22/2006 6:30 AM)", expression: "MM/dd/yyyy h:mm tt" },
    MMddyyyyHHmmss: { value: 11, description: "MM/dd/yyyy HH:mm:ss (ex: 08/22/2006 06:30:07)", expression: "MM/dd/yyyy HH:mm:ss" },
    yyyyMMddTHHmmssfffffffK: { value: 12, description: "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK	(ex: 2006-08-22T06:30:07.7199222-04:00)", expression: "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK" },
    dddddMMMyyyyHHmmssGMT: { value: 13, description: "ddd, dd MMM yyyy HH':'mm':'ss 'GMT' (ex: Tue, 22 Aug 2006 06:30:07 GMT)", expression: "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'" },
    yyyyMMddTHHmmss: { value: 14, description: "yyyy'-'MM'-'dd'T'HH':'mm':'ss (ex: 2006-08-22T06:30:07)", expression: "yyyy'-'MM'-'dd'T'HH':'mm':'ss" },
    yyyyMMddHHmmssZ: { value: 15, description: "yyyy'-'MM'-'dd HH':'mm':'ss'Z' (ex: 2006-08-22 06:30:07Z)", expression: "yyyy'-'MM'-'dd HH':'mm':'ss'Z'" },
    ddddddMMMMyyyyHHmmssfff: { value: 16, description: "dd/MM/yyyy HH:mm:ss.fff	(ex: 22/08/2006 06:30:07.000)", expression: "dd/MM/yyyy HH:mm:ss.fff" },
    Custom: { value: 17, description: "Custom", expression: "Custom" }
});



app.constant('DateTimeFormatEnum', {
    LongDateTime: { value: 0, name: "LongDateTime", description: "Long Date Time", mask: "LongDateTime" },
    DateTime: { value: 1, name: "DateTime", description: "Date Time", mask: "DateTime" },
    Date: { value: 2, name: "Date", description: "Date", mask: "Date" }
});