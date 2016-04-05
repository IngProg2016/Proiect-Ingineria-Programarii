(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp');

    OutOfRangeApp.controller('MainCtrl', ['$scope', 'authService', function ($scope, authService) {
        $scope.isAuthenticated = function () {
            return authService.getAuthentificationInfo().isAuth;
        };
        $scope.userName = function () {
            return authService.getAuthentificationInfo().userName;
        };
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
            authService.login($scope.user);
        };
    }]);

    OutOfRangeApp.controller('LogoutCtrl', ['$location', 'authService', function ($location, authService) {
        (function init() {
            authService.logout();
            $location.path('/');
        })();
    }]);

    OutOfRangeApp.controller('UserCtrl', ['$location', 'authService', function ($location, authService) {
        
    }]);

})();
