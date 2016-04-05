(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp');

    OutOfRangeApp.controller('MainCtrl', ['$scope', function ($scope) {
        
    }]);

    OutOfRangeApp.controller('HomeCtrl', ['$scope', function ($scope) {

    }]);

    OutOfRangeApp.controller('RegisterCtrl', ['$scope', 'authService', function ($scope, authService) {
        $scope.user = {
            email: "",
            password: "",
            confirmPassword: ""
        };

        $scope.register = function () {
            authService.register($scope.user);
        };
    }]);

    OutOfRangeApp.controller('LoginCtrl', ['$scope', 'authService', function ($scope, authService) {
        $scope.user = {
            username: "",
            password: ""
        };

        $scope.login = function () {
            debugger;
            authService.login($scope.user);
        };
    }]);

    OutOfRangeApp.controller('LogoutCtrl', ['$location', 'authService', function ($location, authService) {
        (function init() {
            authService.logout();
            $location.path('/');
        })();
    }]);

})();
