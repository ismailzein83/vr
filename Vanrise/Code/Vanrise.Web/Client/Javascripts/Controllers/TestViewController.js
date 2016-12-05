"use strict";

var TestViewController = function ($scope, $http, ValuesAPIService, $timeout, UtilsService, LabelColorsEnum) {

    $scope.buttonMenuActions = [
                        {
                            name: "TOD Rule",
                            clicked: function () {
                                return ValuesAPIService.Get().then(function (response) {
                                });
                            },
                        },
                        {
                            name: "Tariff Rule",
                            clicked: function () {
                                return ValuesAPIService.Get().then(function (response) {
                                });
                            }
                        },
                        {
                            name: "Extra Charge",
                            clicked: function () {
                                return ValuesAPIService.Get().then(function (response) {
                                });
                            }
                        }
    ];
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
        $scope.istrue = true;
    };
    $scope.doc = {
        fileId: 1
    };
    $scope.colorlist = [];
    for (var prop in LabelColorsEnum) {
        $scope.colorlist.push(LabelColorsEnum[prop]);
    }
    $scope.getColor = function () {
        return LabelColorsEnum.Info.color;
    };
    $scope.progress = [];
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
          OperatorAccountSet: {
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
    $scope.effectiveOn = new Date();
    setTimeout(function () {
        $scope.$apply(function () {
            $scope.effectiveOn2 = UtilsService.cloneDateTime($scope.effectiveOn);
        })
    }, 2000);
    $scope.headers = ["value", "name"];
    for (var i = 0 ; i < 5 ; i++) {
        $scope.listData[$scope.listData.length] = { value: i + 1, name: "test " + (i + 1) };
    }
    $scope.itemsSortable = { animation: 150 };

    $scope.addItem = function () {
        var item = {
            col1: "test " + ++current + "1",
            col2: "test " + current + "2",
            col3: "test " + current + "3"
        };
        $scope.gridData.push(item);
        gridApi.itemAdded(item);
    };

    $scope.getCellcolor = function () {
        var letters = '0123456789ABCDEF'.split('');
        var color = '#';
        for (var i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;

    };
    $scope.toggletime = function (e) {
        var el = angular.element(e.currentTarget);
        el.parent().data("DateTimePicker").show();

        $('.date-section').removeClass('in');
        $('.time-section').addClass('in');

        $('.btn-switcher').removeClass("glyphicon-time");
        $('.btn-switcher').addClass("glyphicon-calendar");

    };
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
                $scope.num2 = data.loaded / data.total * 100;
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

    };
    var gridApi;
    $scope.gridReady = function (api) {
        gridApi = api;
        $scope.loadMoreData();
    };
    $scope.testObj = {};   
    $scope.groupeHeaders = [
        { lable: "test 2", type: "leaf", rotated: true, position: "pulltop pullleft " },
        { lable: "www", type: "leaf", rotated: true, position: " pullleft " },
        { lable: "wfrwfwcvscdsw", type: "leaf", rotated: true, position: "pullbottom pullleft " },
        { lable: "sfsdf", type: "leaf", rotated: true, position: "pulltop pullrigth " },
        { lable: "sddddddd", type: "leaf", rotated: true, position: "pullrigth pullbottom " },
        { lable: "Gammaracanthuskytodermogammarus loricatobaicalensis", type: "leaf", rotated: true, position: "" },
        { lable: "dddd ddd dddd ddd", type: "leaf", rotated: true, position: "pullbottom pullrigth " },
        { lable: "sdfasd asfdasf asdfasvf", type: "leaf", rotated: true, position: " pullrigth " },
        { lable: "d", type: "leaf", rotated: true, position: "pulltop pullleft " },
        { lable: "sdafasfasdfsd sfasdfsdsd", type: "leaf", rotated: true, position: "pullbottom pullleft " },
        { lable: "33 33 33cv 33", type: "leaf", rotated: true, position: "pullrigth pullbottom " },
        { lable: "eee ffff 55555 ", type: "leaf", rotated: true, position: "" }


    ];
    $scope.testValueChanged = function () {
    };
    var choicesApi;
    $scope.choicesReady = function (api) {
        choicesApi = api;
    };
    $scope.test = function () {
        alert("test")
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
    };
};
appControllers.controller('TestViewController', TestViewController);


app.directive("getStyle",function(){
    return {
        link: function (scope, element, attrs) {




            setTimeout(function () {

                var dom = element[0];
                var selw = $(element).width();
                var parent = $(element).parent();
                var ph = $(parent).height();
                var pw = $(parent).width();
                var h = dom.scrollWidth;
                var w = dom.scrollHeight;
                var pos = [];
                if (attrs.position != undefined && attrs.position != "") {
                    pos = attrs.position.split(" ");

                }
                if (pos.indexOf("pullbottom") > -1) {
                    var str1=(100 * ((ph - 33))) / ph;
                    var str2 ="%";
                    element[0].style.top = str1.concat(str2); // bottom formular
                }
                else if (pos.indexOf("pulltop") > -1) {
                    var str1 = (100 * ((ph - (ph - h)) - 33)) / ph;
                    var str2 = "%";
                    element[0].style.top = str1.concat(str2);//  top formula 
                }
                else {
                    var str1 = (100 * ((ph - 33) - ((ph - h) / 2))) / ph;
                    var str2 = "%";
                    element[0].style.top = str1.concat(str2);// center  formula
                }

                if (pos.indexOf("pullrigth") > -1) {
                    var str1 = (100 * ((pw) - (w))) / pw;
                    var str2 = "%";
                    element[0].style.left = str1.concat(str2);
                }
                else if (pos.indexOf("pullleft") > -1){
                    element[0].style.left = "0%";
                }   
                else {
                    var str1 = (100 * ((pw / 2) - (w / 3))) / pw;
                    var str2 = "%";
                    element[0].style.left = str1.concat(str2);
                }
            }, 1);

            //setTimeout(function () {

            //    var dom = element[0];
            //    var selw = $(element).width();
            //    var parent = $(element).parent();
            //    var ph = $(parent).height();
            //    var pw = $(parent).width();
            //    var h = dom.scrollWidth;
            //    var w = dom.scrollHeight;

            //    if (attrs.pullbottom != undefined)
            //        element[0].style.top = (100 * ((ph))) / ph + "%"; // bottom formular
            //    else if (attrs.pulltop != undefined)
            //        element[0].style.top = (100 * ((ph - (ph - h)) )) / ph + "%"; //  top formula 
            //    else
            //        element[0].style.top = (100 * ((ph ) - ((ph - h) / 2))) / ph + "%"; // center  formula

            //    if (attrs.pullrigth != undefined)
            //        element[0].style.left = (100 * ((pw) - (w))) / pw + "%";

            //    else if (attrs.pullleft != undefined)
            //        element[0].style.left = "0%";

            //    else
            //        element[0].style.left = (100 * ((pw / 2) - (w / 3))) / pw + "%";

            //}, 1)

            //parent[0].style.backgroundColor = scope.getCellcolor();



        }
    };
});