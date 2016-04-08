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
            email: '',
            password: '',
            confirmPassword: ''
        };

        $scope.error = null;

        $scope.loading = false;

        $scope.register = function () {
            $scope.loading = true;
            authService.register($scope.user)
            .catch(function (err) {
                $scope.loading = false;
                $scope.error = err;
            });
        };

    }]);

    OutOfRangeApp.controller('LoginCtrl', ['$scope', 'authService', function ($scope, authService) {
        $scope.user = {
            username: '',
            password: ''
        };

        $scope.error = null;
        $scope.rememberMe = false;

        $scope.loading = false;

        $scope.login = function () {
            $scope.loading = true;
            authService.login($scope.user, $scope.rememberMe)
            .catch(function (err) {
                $scope.loading = false;
                $scope.error = err.error_description;
            });
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

    OutOfRangeApp.controller('QuestionsCtrl', ['$scope', 'qaService', function ($scope, qaService) {
        (function init() {
            qaService.getQuestions().then(function (data) {
                $scope.data = data || [];
            });
        })();
    }]);

    OutOfRangeApp.controller('QuestionAddCtrl', ['$scope', 'qaService', function ($scope, qaService) {
        $scope.question = {
            title: "",
            tescription:"",
            tags:""
        };
        $scope.addQuestion = function () {
            debugger;
            var taglist = $scope.question.tags.split(/ |,/);
            $scope.question.tags = []
            for (var tag in taglist) {
                $scope.question.tags.push({ name: taglist[tag] });
            }
            qaService.addQuestion($scope.question);
        };
    }]);

    OutOfRangeApp.controller('QuestionViewCtrl', ['$scope', '$routeParams', '$timeout', 'smoothScroll', 'qaService', function ($scope, $routeParams, $timeout, smoothScroll, qaService) {
        $scope.question = {
            title: "",
            description: "",
            tags: ""
        };

        function _getQuestion() {
            qaService.viewQuestion().then(function (data) {
                $scope.question = data || [];
            });
        }

        function _scrollToElement(scrollTo) {
            scrollTo = scrollTo || $routeParams.scrollTo;

            $scope.$on('$viewContentLoaded', function () {
                scrollTo && $timeout(function () {
                    smoothScroll(document.getElementById(scrollTo), { duration: 700, easing: 'easeOutQuad' });
                }, 500);
            });
        }

        (function init() {
            _getQuestion();
            _scrollToElement();
        })();

        $scope.answer = {
            description: "",
            questionID: ""
        }

        $scope.addAnswer = function () {
            $scope.answer.questionID = $scope.question.ID;
            qaService.addAnswer($scope.answer).then(function (_data) {
                _getQuestion();

                $scope.answer.description = "";
            });
        }
    }]);

})();
