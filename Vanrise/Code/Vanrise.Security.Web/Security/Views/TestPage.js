testpage1.$inject = ['$scope'];

function testpage1($scope) {
    var widgetAPI;
    defineScope();
    load();

    function defineScope() {
        var data = [];
        for (var i = 0; i < 20; i++) {
            var obj = {
                id: "child1",
                parent:"#",
            }
        }
        $('#using_json_2').jstree({
            'core': {
                'data': [
                   { "id": "ajson1", "parent": "#", "text": "Simple root node" },
                   { "id": "ajson2", "parent": "#", "text": "Root node 2" },
                   { "id": "ajson3", "parent": "ajson2", "text": "Child 1" },
                   { "id": "ajson4", "parent": "ajson2", "text": "Child 2" },
                ]
            }
        });
    }
    function load() {

        $scope.isGettingData = false;

    }
}
appControllers.controller('testpage1', testpage1);
