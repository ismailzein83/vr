'use strict'

var TestViewController = function ($scope, $http, ValuesAPIService, $timeout) {
    
    $scope.timeObj = {
        hours: 23,
        minutes: 23
    };
    $scope.testModel = 'initial from default';
    $scope.html = '<input ng-click="click(1)" value="Click me" type="button">';
    $scope.click = function (arg) {
        alert('Clicked ' + arg);
    };

    $scope.logevent = function () {
        console.log('ng-click');
        $scope.istrue=true;
    };
    $scope.doc =  {
        fileId : 1
    };
    $scope.progress =[];
    $scope.progress.push(10);
    $scope.progress.push(20);
    $scope.progress.push(30);

    $scope.postMsg = function () {
        $http.post($scope.baseurl + "/api/routing/SaveRouteRule",
      {
          RouteRuleId: 4,
          CodeSet: {
              $type: "TOne.LCR.Entities.CodeSelectionSet, TOne.LCR.Entities",
              Code: "5345",
              ExcludedCodes: ["534", "5346"]
          },
          CarrierAccountSet: {
              $type: "TOne.LCR.Entities.CustomerSelectionSet, TOne.LCR.Entities",
              Customers: {
                  SelectionOption: "AllExceptItems",
                  SelectedValues: ["C4444", "4656"]
              }
          },
          ActionData: {
              $type: "TOne.LCR.Entities.OverrideRouteActionData, TOne.LCR.Entities",
              NoOptionAction: "SwitchToLCR",
              Options: [{
                  SupplierId: "C555fd",
                  Percentage: 55
              }, {
                  SupplierId: "D543",
                  Percentage: 5
              }, {
                  SupplierId: "G655",
                  Percentage: 34
              }]
          }
      })
  .success(function (response) {

  });
    };
    var current = 0;
    $scope.gridData = [];
    $scope.loadMoreData = function () {
        var pageInfo = gridApi.getPageInfo();
        return ValuesAPIService.Get().then(function (response) {
            for (current = pageInfo.fromRow; current <= pageInfo.toRow; current++) {
                $scope.gridData.push({
                    col1: "test " + current + "1 eeeeeeeeeeeeeeeee",
                    col2: "test " + current + "2eeeeeeeeeeeeeeeeeeeeeeeeee",
                    col3: "test " + current + "3eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee",
                });
            }


        });

       

    };
    $scope.listData = [];
    $scope.headers = ["value","name"];
    for (var i = 0 ; i < 5 ; i++) {
        $scope.listData[$scope.listData.length] = { value: i + 1, name: "test " + (i + 1) };
    }
    $scope.itemsSortable = {  animation: 150 };

    $scope.addItem = function () {
        var item = {
            col1: "test " + ++current + "1",
            col2: "test " + current + "2",
            col3: "test " + current + "3",
        };
        $scope.gridData.push(item);
        gridApi.itemAdded(item);
    }

    $scope.getCellcolor = function () {
            var letters = '0123456789ABCDEF'.split('');
            var color = '#';
            for (var i = 0; i < 6; i++) {
                color += letters[Math.floor(Math.random() * 16)];
            }
            return color ;
       
    }
    $scope.toggletime = function (e) {
            var el = angular.element(e.currentTarget);
            el.parent().data("DateTimePicker").show();

            $('.date-section').removeClass('in');
            $('.time-section').addClass('in');

            $('.btn-switcher').removeClass("glyphicon-time");
            $('.btn-switcher').addClass("glyphicon-calendar");
           
    }
    var pathArray = location.href.split('/');
    var base = pathArray[0] + '//' + pathArray[2];
    $scope.filesAdded = [];
    $('#fileUpload').fileupload({
        url: base + '/api/FileManager/UploadFile',
        formData: function (form) { return []; },
        replaceFileInput: false,
        datatype: 'json',
        add: function (e, data) {
            angular.forEach(data.files, function (file) {
                $scope.filesAdded.push(file);
            });
            data.submit();
        },
        progress: function (e, data) {
            $scope.$apply(function () {
                $scope.num = data.loaded / data.total * 100;
                $scope.num2 = data.loaded / data.total * 100 ;
            });
        },
        change: function (e, data) {
        },
        drop: function (e, data) {
        },
        done: function (e, data) {
        },
        fail: function (e, data) {
            //alert("Oups, une erreur  est survenue.");
        }
    });
    $scope.toggledate = function (e) {      
        var el = angular.element(e.currentTarget);
        el.parent().data("DateTimePicker").show();

        $('.time-section').removeClass('in');
        $('.date-section').addClass('in');

        $('.btn-switcher').addClass("glyphicon-time");
        $('.btn-switcher').removeClass("glyphicon-calendar");

    }
    var gridApi;
    $scope.gridReady = function (api) {
        gridApi = api;
        $scope.loadMoreData();
    };
    $scope.testObj = {};
    $scope.choiceSelectionChanged = function () {
        //console.log($scope.testObj);
    };
    $scope.testValueChanged = function () {
        console.log("ali")
    }
    var choicesApi;
    $scope.choicesReady = function (api) {
        choicesApi = api;
    };

    $scope.selectChoice = function () {
        //$scope.testObj.choiceSelectedIndex = 
        // choicesApi.selectChoice();
    };

    $scope.values = [
       {
           id: "1",
           Name: "child 1",
           isOpened: true,
           children: [{
               id: "2",
               Name: "child 1"
           }
           ,
           {
               id: "3",
               Name: "child 1",
               isSelected: true
           }

           ]
       },
       {
           id: "4",
           Name: "child 2",
           isDisabled: true,
           children: []
       }];
    $scope.treeReady = function (api) {
        api.refreshTree($scope.values);
    }
}
appControllers.controller('TestViewController', TestViewController);


app.directive("getStyle",function(){
    return{
        link: function (scope, element) {
          
            var dom = element[0];
            var parent = $(element).parent();
            var ph = $(parent).height();
            var pw = $(parent).width();
            var h = dom.scrollWidth;
            var w = dom.scrollHeight;
            element[0].style.top =  (100 * ((ph - 32) - ((ph - h) / 2) ) ) /ph  + "%";

            element[0].style.left = (100 * ((pw - (w/2)) / 2)) / pw + "%";

            //parent[0].style.backgroundColor = scope.getCellcolor();


            
        }        
    }      
});