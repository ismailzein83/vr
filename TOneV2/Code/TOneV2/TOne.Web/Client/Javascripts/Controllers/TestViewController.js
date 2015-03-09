appControllers.controller('TestViewController',
    function DefaultController() {
        this.model = 'Test View model';
        this.Input = '123';
        this.alertMsg = function () {
            alert(this.Input);
        };

        this.gridOptions = {
            data:[{ name: "Moroni", age: 50 },
                { name: "Tiancum", age: 43 },
                { name: "Jacob", age: 27 },
                { name: "Nephi", age: 29 },
                { name: "Enos", age: 34 },
                { name: "Tiancum", age: 43 },
                { name: "Jacob", age: 27 },
                { name: "Nephi", age: 29 },
                { name: "Enos", age: 34 },
                { name: "Tiancum", age: 43 },
                { name: "Jacob", age: 27 },
                { name: "Nephi", age: 29 },
                { name: "Enos", age: 34 },
                { name: "Tiancum", age: 43 },
                { name: "Jacob", age: 27 },
                { name: "Nephi", age: 29 },
                { name: "Enos", age: 34 }],
            header : ["Name","Age"]
        };


        this.gridOptionsV1 = {
            data: [{ name: "Moroni", age: 50 },
                { name: "Tiancum", age: 43 },
                { name: "Jacob", age: 27 },
                { name: "Nephi", age: 29 },
                { name: "Enos", age: 34 },
                { name: "Tiancum", age: 43 },
                { name: "Jacob", age: 27 },
                { name: "Nephi", age: 29 },
                { name: "Enos", age: 34 }]
        };

    });