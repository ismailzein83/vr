'use strict';


app.directive('vrPagination', ['UtilsService', 'UISettingsService', 'MobileService', function (UtilsService, UISettingsService, MobileService) {

	var directiveDefinitionObject = {
		restrict: 'E',
		scope: {
			pagersettings: '=',
			onReady: "="
		},
		controller: function ($scope, $element, $attrs) {
			$scope.$on("$destroy", function () {
				$(window).unbind('resize', setDefaultSetting);
			});
			var pagerCtrl = this;
			var firstPageSizeChangePromiseDeferred;
			pagerCtrl.topCounts = [5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 100];
			pagerCtrl.isMobile = MobileService.isMobile();
			if (pagerCtrl.pagersettings != undefined) {
				pagerCtrl.pagersettings.itemsPerPage = parseInt(UISettingsService.getUIParameterValue('GridPageSize') || pagerCtrl.topCounts[1]);
				pagerCtrl.pagersettings.getPageInfo = function () {
					var fromRow = (pagerCtrl.pagersettings.currentPage - 1) * pagerCtrl.pagersettings.itemsPerPage + 1;
					return {
						fromRow: fromRow,
						toRow: fromRow + pagerCtrl.pagersettings.itemsPerPage - 1
					};
				};

				pagerCtrl.pageChanged = function () {
					if (pagerCtrl.pagersettings.pageChanged != undefined)
						pagerCtrl.pagersettings.pageChanged();
				};

				pagerCtrl.pageSizeChanged = function () {

					if (firstPageSizeChangePromiseDeferred != undefined) {
						firstPageSizeChangePromiseDeferred.resolve();
						firstPageSizeChangePromiseDeferred = undefined;
					}
					else {
						if (pagerCtrl.pagersettings.pageChanged != undefined)
							pagerCtrl.pagersettings.pageChanged();
					}
				};

				pagerCtrl.pagersettings.showBoundaryLinks = pagerCtrl.isMobile ? false : true;
				pagerCtrl.pagersettings.showDirectionLinks = true;
				pagerCtrl.pagersettings.maxSize = 10;

				if (pagerCtrl.onReady != null && typeof (pagerCtrl.onReady) == 'function') {
					firstPageSizeChangePromiseDeferred = UtilsService.createPromiseDeferred();
					pagerCtrl.onReady();
				}
			}
			pagerCtrl.setDefaultSetting = function () {
				setDefaultSetting();
			};
			function setDefaultSetting() {
				setTimeout(function () {
					var pagerwidth = $element.parents('.panel-body').first().width();
					pagerCtrl.pagersettings.itemsPerPage = parseInt(UISettingsService.getUIParameterValue('GridPageSize') || pagerCtrl.topCounts[1]);
					pagerCtrl.pagersettings.getPageInfo = function () {
						var fromRow = (pagerCtrl.pagersettings.currentPage - 1) * pagerCtrl.pagersettings.itemsPerPage + 1;
						return {
							fromRow: fromRow,
							toRow: fromRow + pagerCtrl.pagersettings.itemsPerPage - 1
						};
					};
					if (pagerwidth > 0) {
						if (pagerwidth - 250 < 300)
							pagerCtrl.pagersettings.maxSize = 5;
						if (pagerwidth - 250 < 200) {
							pagerCtrl.pagersettings.maxSize = 2;
						}
						if (pagerwidth - 250 < 150) {
							pagerCtrl.pagersettings.showBoundaryLinks = false;
							pagerCtrl.pagersettings.showDirectionLinks = false;
							pagerCtrl.pagersettings.maxSize = 1;
						}
					}
				}, 100);
			}
			$(window).on('resize', setDefaultSetting);
		},
		controllerAs: 'pagerCtrl',
		bindToController: true,
		compile: function (element, attrs) {
			return {
				pre: function ($scope, iElem, iAttrs, ctrl) {
					//var ctrl = $scope.ctrl;
				},
				post: function ($scope, iElem, iAttrs, ctrl) {

				}
			};
		},
		templateUrl: function (element, attrs) {
			return "/Client/Javascripts/Directives/Pagination/vr-pagination.html";
		}

	};

	return directiveDefinitionObject;



}]);

