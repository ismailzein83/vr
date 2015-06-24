var ExecuteStrategyProcessInputController = function ($scope, $http, StrategyAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {
    defineScope();
    //load();
    //loadForm();
    function defineScope() {

        $scope.customvalidateFrom = function (fromDate) {
            return validateDates(fromDate, $scope.toDate);
        };
        $scope.customvalidateTo = function (toDate) {
            return validateDates($scope.fromDate, toDate);
        };
        function validateDates(fromDate, toDate) {
            if (fromDate == undefined || toDate == undefined)
                return null;
            var from = new Date(fromDate);
            var to = new Date(toDate);
            if (from.getTime() > to.getTime())
                return "Start should be before end";
            else
                return null;
        }

        $scope.strategies = [];
        loadStrategies();
        $scope.selectedStrategies = [];


        $scope.periods = [];
        loadPeriods();
        $scope.selectedPeriod = "";


        //$scope.code = "";
        //$scope.codeList = [];
        //$scope.codeInpute = '';

        //var numberReg = /^\d+$/;
        //$scope.isNumber = function (s) {
        //    return String(s).search(numberReg) != -1
        //};

        //$scope.muteAction = function (e) {
        //    e.preventDefault();
        //    e.stopPropagation();
        //}
        //$scope.getCodes = function () {
        //    var label = '';
        //    if ($scope.codeList.length == 0)
        //        label = "Fill codes...";
        //    else if ($scope.codeList.length == 1) {
        //        label += $scope.codeList[0];
        //    }
        //    else if ($scope.codeList.length < 3) {
        //        $.each($scope.codeList, function (i, value) {
        //            if (i < $scope.codeList.length - 1)
        //                label += value + ',';
        //            else
        //                label += value;
        //        });
        //    }
        //    else
        //        label = $scope.codeList.length + " Codes selected";
        //    return label;
        //};

        //$scope.addCodeEnter = function (e) {
        //    if (e.keyCode == 13) {
        //        $scope.addCode(e);
        //    }
        //}
        //$scope.addCode = function (e) {
        //    var valid = $scope.isNumber($scope.codeInpute);
        //    if (valid) {
        //        var index = null;
        //        var index = $scope.codeList.indexOf($scope.codeInpute);
        //        if (index >= 0) {
        //            $scope.codeInpute = '';
        //            return;
        //        }
        //        else {
        //            $scope.codeList.push($scope.codeInpute);
        //            $scope.codeInpute = '';
        //        }

        //    }
        //    else {
        //        $scope.codeInpute = '';
        //    }
        //}
        //$scope.removeCode = function (e, s) {
        //    e.preventDefault();
        //    e.stopPropagation();
        //    var index = $scope.codeList.indexOf(s);
        //    $scope.codeList.splice(index, 1);
        //}


        //$scope.subViewCodeSetConnector.getData = function () {
        //    return {
        //        $type: "TOne.LCR.Entities.CodeSelectionSet, TOne.LCR.Entities",
        //        Code: $scope.code,
        //        WithSubCodes: ($scope.subCodes == true) ? true : false,
        //        ExcludedCodes: $scope.codeList
        //    };
        //};

        //$scope.subViewCodeSetConnector.setData = function (data) {
        //    $scope.subViewCodeSetConnector.data = data;
        //    loadForm();
        //}

    }


 

    function loadPeriods() {
        return StrategyAPIService.GetPeriods().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.periods.push(itm);
            });
        });
    }


    function loadStrategies() {
        return StrategyAPIService.GetAllStrategies().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.strategies.push({ id: itm.Id, name: itm.Name });
            });
        });
    }



    function getData() {
        //var fromDate = $scope.fromDate != undefined ? $scope.fromDate : '';
        //var toDate = $scope.toDate != undefined ? $scope.toDate : '';

        //var strategyId = $scope.selectedStrategies.id;
        //var suspicionLevelsList = '';

        //angular.forEach($scope.selectedSuspicionLevels, function (itm) {
        //    suspicionLevelsList = suspicionLevelsList + itm.id + ','
        //});


        //var pageInfo = mainGridAPI.getPageInfo();


        //return SuspicionAnalysisAPIService.GetFilteredSuspiciousNumbers(pageInfo.fromRow, pageInfo.toRow, fromDate, toDate, strategyId, suspicionLevelsList.slice(0, -1)).then(function (response) {
        //    angular.forEach(response, function (itm) {
        //        //var date = $filter('date')(new Date(), 'MMM dd, yyyy');
        //        //itm.FormattedDate = date;

        //        $scope.fraudResults.push(itm);
        //    });
        //});
    }


    //function loadForm() {
    //    if ($scope.subViewCodeSetConnector.data == undefined)
    //        return;
    //    var data = $scope.subViewCodeSetConnector.data;
    //    if (data != null) {
    //        $scope.codeList = data.ExcludedCodes;
    //        $scope.code = data.Code;
    //        $scope.subCodes = data.WithSubCodes;
    //    }
    //    else {
    //        $scope.zoneSelectionOption = 1;
    //        $scope.codeList = [];
    //        $scope.code = '';
    //        $scope.subCodes = false;
    //    }
    //}

    //function load() {
    //    $('#CodeListddl').on('show.bs.dropdown', function (e) {
    //        $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
    //    });

    //    //ADD SLIDEUP ANIMATION TO DROPDOWN //
    //    $('#CodeListddl').on('hide.bs.dropdown', function (e) {
    //        $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
    //    });
    //    $('div[id="CodeListddl"]').on('click', '.dropdown-toggle', function (event) {

    //        var self = $(this);
    //        var selfHeight = $(this).parent().height();
    //        var selfWidth = $(this).parent().width();
    //        var selfOffset = $(self).offset();
    //        var selfOffsetRigth = $(document).width() - selfOffset.left - selfWidth;
    //        var dropDown = self.parent().find('ul');
    //        $(dropDown).css({ position: 'fixed', top: selfOffset.top + selfHeight, left: 'auto' });
    //    });

    //    var fixDropdownPosition = function () {
    //        $('.drop-down-inside-modal').find('.dropdown-menu').hide();
    //        $('.drop-down-inside-modal').removeClass("open");

    //    };

    //    $(".modal-body").unbind("scroll");
    //    $(".modal-body").scroll(function () {
    //        fixDropdownPosition();
    //    });
    //}

}

ExecuteStrategyProcessInputController.$inject = ['$scope', '$http', 'StrategyAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
appControllers.controller('FraudAnalysis_ExecuteStrategyProcessInputController', ExecuteStrategyProcessInputController)



